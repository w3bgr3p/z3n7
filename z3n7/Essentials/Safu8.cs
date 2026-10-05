using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    /// <summary>
    /// Process-wide registry of delegates by name. SAFU registers its implementation here so that other
    /// assemblies can call it.
    /// </summary>
    public static class FunctionStorage
    {
        /// <summary>Registered delegates, keyed by name (e.g. <c>SAFU_Encode</c>).</summary>
        public static ConcurrentDictionary<string, object> Functions = new ConcurrentDictionary<string, object>();
    }

    /// <summary>Encryption used by SAFU (secure storage of account secrets).</summary>
    public interface ISAFU
    {
        /// <summary>Encrypts text with a key bound to the machine, the PIN and the account.</summary>
        string Encode(IZennoPosterProjectModel project, string toEncrypt, string pin, string acc);
        /// <summary>Decrypts text produced by <c>Encode</c> with the same machine, PIN and account.</summary>
        string Decode(IZennoPosterProjectModel project, string toDecrypt, string pin, string acc);
        /// <summary>
        /// Returns a password derived from the machine, the PIN and the account. The same inputs always give
        /// the same password.
        /// </summary>
        string HWPass(IZennoPosterProjectModel project, string pin, string acc);
        /// <summary>Encrypts text with a key bound to the machine only.</summary>
        string EncodeHWID(IZennoPosterProjectModel project, string toEncrypt);
        /// <summary>Decrypts text produced by <c>EncodeHWID</c> on the same machine.</summary>
        string DecodeHWID(IZennoPosterProjectModel project, string toDecrypt);
    }

    /// <summary>
    /// SAFU implementation: AES-256-CBC with an HMAC-SHA256 tag, keys derived with PBKDF2-SHA256 (100 000
    /// iterations).
    /// The salt comes from a 32-byte key file. The machine ID is a SHA-256 of the processor ID, the
    /// motherboard serial and the system disk serial (WMI). When the <c>jVars</c> blob contains
    /// <c>serverHwid</c>, that value is used instead of the local machine ID for <c>Encode</c>,
    /// <c>Decode</c> and <c>HWPass</c>.
    /// </summary>
    public class Z3n8SAFU : ISAFU
    {
        private readonly string _keyFilePath;
        private byte[] _fileKey;
        private readonly object _lock = new object();

        /// <summary>
        /// Creates the implementation. The key file is read on first use and must be exactly 32 bytes.
        /// </summary>
        /// <param name="keyFilePath">Path to the key file.</param>
        public Z3n8SAFU(string keyFilePath)
        {
            _keyFilePath = keyFilePath;
        }

        // ── File key ──────────────────────────────────────────────────────────

        byte[] LoadFileKey()
        {
            if (_fileKey != null) return _fileKey;
            lock (_lock)
            {
                if (_fileKey != null) return _fileKey;
                if (!File.Exists(_keyFilePath))
                    throw new FileNotFoundException("safu.key not found: " + _keyFilePath);
                _fileKey = File.ReadAllBytes(_keyFilePath);
                if (_fileKey.Length != 32)
                    throw new InvalidOperationException("safu.key must be 32 bytes, got " + _fileKey.Length);
                return _fileKey;
            }
        }

        // ── Key derivation ────────────────────────────────────────────────────

        byte[] DeriveSalt(string domain)
        {
            var fileKey = LoadFileKey();
            using (var hmac = new HMACSHA256(fileKey))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(domain));
                var salt = new byte[16];
                Array.Copy(hash, salt, 16);
                return salt;
            }
        }

        static byte[] Pbkdf2Sha256(string password, byte[] salt, int iterations, int outputBytes)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(password)))
            {
                var result = new byte[outputBytes];
                var block  = new byte[salt.Length + 4];
                Array.Copy(salt, block, salt.Length);
                block[salt.Length + 3] = 1;
                var u = hmac.ComputeHash(block);
                Array.Copy(u, result, outputBytes);
                for (int i = 1; i < iterations; i++)
                {
                    u = hmac.ComputeHash(u);
                    for (int j = 0; j < outputBytes; j++) result[j] ^= u[j];
                }
                return result;
            }
        }

        byte[] DeriveSecureKey(string pin, string hardwareId, string accountId)
        {
            if (string.IsNullOrEmpty(pin)) pin = "UNPROTECTED";
            var salt = DeriveSalt(hardwareId + ":" + accountId);
            return Pbkdf2Sha256(pin, salt, 100000, 32);
        }

        byte[] DeriveKeyFromHWID(string hardwareId)
        {
            var salt = DeriveSalt("hwid-only");
            return Pbkdf2Sha256(hardwareId, salt, 100000, 32);
        }

        // ── HWID ──────────────────────────────────────────────────────────────

        static string GetStableHWId()
        {
            var components = new List<string>();

            using (var s = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
                foreach (ManagementObject mo in s.Get())
                {
                    var id = mo["ProcessorId"] != null ? mo["ProcessorId"].ToString() : null;
                    if (!string.IsNullOrEmpty(id)) { components.Add(id); break; }
                }

            using (var s = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
                foreach (ManagementObject mo in s.Get())
                {
                    var serial = mo["SerialNumber"] != null ? mo["SerialNumber"].ToString() : null;
                    if (!string.IsNullOrEmpty(serial)) { components.Add(serial); break; }
                }

            try
            {
                string sysRoot = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
                string driveLetter = sysRoot.Replace("\\", "");
                using (var lds = new ManagementObjectSearcher("SELECT DeviceID FROM Win32_LogicalDisk WHERE DeviceID = '" + driveLetter + "'"))
                foreach (ManagementObject ld in lds.Get())
                    using (var ps = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" + ld["DeviceID"] + "'} WHERE AssocClass = Win32_LogicalDiskToPartition"))
                    foreach (ManagementObject part in ps.Get())
                        using (var ds = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + part["DeviceID"] + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition"))
                        foreach (ManagementObject drive in ds.Get())
                        {
                            var serial = drive["SerialNumber"] != null ? drive["SerialNumber"].ToString() : null;
                            if (!string.IsNullOrEmpty(serial)) { components.Add(serial); break; }
                        }
            }
            catch
            {
                using (var s = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive"))
                    foreach (ManagementObject mo in s.Get())
                    {
                        var serial = mo["SerialNumber"] != null ? mo["SerialNumber"].ToString() : null;
                        components.Add(serial);
                        break;
                    }
            }

            if (components.Count == 0) throw new Exception("HWID: no hardware components found");

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(string.Join(":", components)));
                return Convert.ToBase64String(hashBytes);
            }
        }

        // ── serverHwid из jVars ───────────────────────────────────────────────
        // Возвращает serverHwid если он есть в jVars, иначе null

        string GetServerHwid(IZennoPosterProjectModel project)
        {
            try
            {
                var blob = project.Var("jVars");
                if (string.IsNullOrEmpty(blob)) return null;
                var json = AesDecrypt(blob, DeriveKeyFromHWID(GetStableHWId()));
                if (string.IsNullOrEmpty(json)) return null;
                if (!json.TrimStart().StartsWith("{"))
                    json = Encoding.UTF8.GetString(Convert.FromBase64String(json));
                var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (dict != null && dict.TryGetValue("serverHwid", out var v) && !string.IsNullOrEmpty(v))
                    return v;
                return null;
            }
            catch { return null; }
        }

        // ── AES ───────────────────────────────────────────────────────────────

        static string AesEncrypt(string plaintext, byte[] key)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor())
                {
                    var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
                    var cipherBytes = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

                    var combined = new byte[aes.IV.Length + cipherBytes.Length];
                    Array.Copy(aes.IV, 0, combined, 0, aes.IV.Length);
                    Array.Copy(cipherBytes, 0, combined, aes.IV.Length, cipherBytes.Length);

                    using (var hmac = new HMACSHA256(key))
                    {
                        var hash = hmac.ComputeHash(combined);
                        var final = new byte[combined.Length + hash.Length];
                        Array.Copy(combined, 0, final, 0, combined.Length);
                        Array.Copy(hash, 0, final, combined.Length, hash.Length);
                        return Convert.ToBase64String(final);
                    }
                }
            }
        }

        static string AesDecrypt(string ciphertext, byte[] key)
        {
            try
            {
                var data = Convert.FromBase64String(ciphertext);
                if (data.Length < 48) return string.Empty;

                int hmacSize = 32;
                var payload      = new byte[data.Length - hmacSize];
                var receivedHmac = new byte[hmacSize];
                Array.Copy(data, 0, payload, 0, payload.Length);
                Array.Copy(data, payload.Length, receivedHmac, 0, hmacSize);

                using (var hmac = new HMACSHA256(key))
                {
                    var computed = hmac.ComputeHash(payload);
                    for (int i = 0; i < hmacSize; i++)
                        if (receivedHmac[i] != computed[i]) return string.Empty;
                }

                var iv        = new byte[16];
                var encrypted = new byte[payload.Length - 16];
                Array.Copy(payload, 0, iv, 0, 16);
                Array.Copy(payload, 16, encrypted, 0, encrypted.Length);

                using (var aes = Aes.Create())
                {
                    aes.Key     = key;
                    aes.IV      = iv;
                    aes.Mode    = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    using (var decryptor = aes.CreateDecryptor())
                        return Encoding.UTF8.GetString(decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length));
                }
            }
            catch { return string.Empty; }
        }

        // ── ISAFU implementation ──────────────────────────────────────────────

        /// <summary>Encrypts <c>toEncrypt</c>. An empty PIN is replaced by a fixed placeholder.</summary>
        /// <returns>Base64 of IV + ciphertext + HMAC, or an empty string for empty input.</returns>
        public string Encode(IZennoPosterProjectModel project, string toEncrypt, string pin, string acc)
        {
            if (string.IsNullOrEmpty(toEncrypt)) return string.Empty;
            var hwid = GetServerHwid(project) ?? GetStableHWId();
            var key = DeriveSecureKey(pin, hwid, acc);
            return AesEncrypt(toEncrypt, key);
        }

        /// <summary>Decrypts <c>toDecrypt</c>.</summary>
        /// <returns>
        /// The plaintext, or an empty string when the input is empty, malformed, or the HMAC does not match.
        /// </returns>
        public string Decode(IZennoPosterProjectModel project, string toDecrypt, string pin, string acc)
        {
            if (string.IsNullOrEmpty(toDecrypt)) return string.Empty;
            var hwid = GetServerHwid(project) ?? GetStableHWId();
            var key = DeriveSecureKey(pin, hwid, acc);
            return AesDecrypt(toDecrypt, key);
        }

        /// <summary>
        /// Returns a deterministic 24-character password with at least one lowercase letter, uppercase letter,
        /// digit and symbol.
        /// </summary>
        public string HWPass(IZennoPosterProjectModel project, string pin, string acc)
        {
            var hwid = GetServerHwid(project) ?? GetStableHWId();
            var secureKey = DeriveSecureKey(pin, hwid, acc);
            using (var hmac = new HMACSHA256(secureKey))
            {
                var seedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes("PASSWORD_SEED"));

                var sb      = new StringBuilder();
                string lo   = "abcdefghijklmnopqrstuvwxyz";
                string up   = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                string dg   = "0123456789";
                string sp   = "!@#$%^&*()_+-=[]{}|;:,.<>?";
                var sets    = new string[] { lo, up, dg, sp };

                for (int i = 0; i < 4; i++)
                    sb.Append(sets[i][seedBytes[i] % sets[i].Length]);

                string all = lo + up + dg + sp;
                for (int i = 4; i < 24; i++)
                {
                    int si  = (i * 2) % seedBytes.Length;
                    int idx = Math.Abs((seedBytes[si] << 8) | seedBytes[(si + 1) % seedBytes.Length]) % all.Length;
                    sb.Append(all[idx]);
                }

                var chars = sb.ToString().ToCharArray();
                for (int i = chars.Length - 1; i > 0; i--)
                {
                    int j = Math.Abs(BitConverter.ToInt32(seedBytes, (i * 4) % (seedBytes.Length - 3))) % (i + 1);
                    char tmp = chars[i]; chars[i] = chars[j]; chars[j] = tmp;
                }

                return new string(chars);
            }
        }

        /// <summary>Encrypts with a key derived from the local machine ID only.</summary>
        /// <returns>Base64 of IV + ciphertext + HMAC, or an empty string for empty input.</returns>
        public string EncodeHWID(IZennoPosterProjectModel project, string toEncrypt)
        {
            if (string.IsNullOrEmpty(toEncrypt)) return string.Empty;
            var key = DeriveKeyFromHWID(GetStableHWId());
            return AesEncrypt(toEncrypt, key);
        }

        /// <summary>Decrypts with a key derived from the local machine ID only.</summary>
        /// <returns>The plaintext, or an empty string when decryption fails.</returns>
        public string DecodeHWID(IZennoPosterProjectModel project, string toDecrypt)
        {
            if (string.IsNullOrEmpty(toDecrypt)) return string.Empty;
            var key = DeriveKeyFromHWID(GetStableHWId());
            return AesDecrypt(toDecrypt, key);
        }
    }

    // ── Точка входа ───────────────────────────────────────────────────────────

    /// <summary>
    /// Entry point to secure storage. Call <c>InitZ3n8</c> once (done by <c>InitVariables</c>) before the
    /// other methods.
    /// </summary>
    public static partial class SAFU
    {
        /// <summary>Registers <c>Z3n8SAFU</c> in <c>FunctionStorage</c> and writes the key path to the log.</summary>
        /// <param name="keyFilePath">Path to the 32-byte key file.</param>
        public static void InitZ3n8(IZennoPosterProjectModel project, string keyFilePath)
        {
            var impl = new Z3n8SAFU(keyFilePath);

            FunctionStorage.Functions["SAFU_Encode"] =
                (Func<IZennoPosterProjectModel, string, string, string, string>)impl.Encode;
            FunctionStorage.Functions["SAFU_Decode"] =
                (Func<IZennoPosterProjectModel, string, string, string, string>)impl.Decode;
            FunctionStorage.Functions["SAFU_HWPass"] =
                (Func<IZennoPosterProjectModel, string, string, string>)impl.HWPass;
            FunctionStorage.Functions["SAFU_EncryptHWID"] =
                (Func<IZennoPosterProjectModel, string, string>)impl.EncodeHWID;
            FunctionStorage.Functions["SAFU_DecryptHWID"] =
                (Func<IZennoPosterProjectModel, string, string>)impl.DecodeHWID;

            project.SendInfoToLog("[SAFU] Z3n8SAFU initialized, key=" + keyFilePath, true);
        }

        /// <summary>Decrypts with the machine-bound key. Returns an empty string for empty input.</summary>
        public static string DecryptHWID(IZennoPosterProjectModel project, string toDecrypt)
        {
            if (string.IsNullOrEmpty(toDecrypt)) return string.Empty;
            var func = (Func<IZennoPosterProjectModel, string, string>)
                FunctionStorage.Functions["SAFU_DecryptHWID"];
            return func(project, toDecrypt);
        }

        /// <summary>Encrypts with the machine-bound key. Returns an empty string for empty input.</summary>
        public static string EncryptHWID(IZennoPosterProjectModel project, string toEncrypt)
        {
            if (string.IsNullOrEmpty(toEncrypt)) return string.Empty;
            var func = (Func<IZennoPosterProjectModel, string, string>)
                FunctionStorage.Functions["SAFU_EncryptHWID"];
            return func(project, toEncrypt);
        }

        /// <summary>
        /// Decrypts with the key of the current account (<c>acc0</c>) and the PIN from the <c>cfgPin</c> secure
        /// variable.
        /// </summary>
        public static string Decode(IZennoPosterProjectModel project, string toDecrypt)
        {
            if (string.IsNullOrEmpty(toDecrypt)) return string.Empty;
            string pin = project.SecureVar("cfgPin");
            string acc = project.Var("acc0");
            var func = (Func<IZennoPosterProjectModel, string, string, string, string>)
                FunctionStorage.Functions["SAFU_Decode"];
            return func(project, toDecrypt, pin, acc);
        }

        /// <summary>
        /// Encrypts with the key of the current account (<c>acc0</c>) and the PIN from the <c>cfgPin</c> secure
        /// variable.
        /// </summary>
        public static string Encode(IZennoPosterProjectModel project, string toEncrypt)
        {
            if (string.IsNullOrEmpty(toEncrypt)) return string.Empty;
            string pin = project.SecureVar("cfgPin");
            string acc = project.Var("acc0");
            var func = (Func<IZennoPosterProjectModel, string, string, string, string>)
                FunctionStorage.Functions["SAFU_Encode"];
            return func(project, toEncrypt, pin, acc);
        }

        /// <summary>
        /// Returns the deterministic password of the current account (<c>acc0</c>), using the PIN from the
        /// <c>cfgPin</c> secure variable.
        /// </summary>
        public static string HWPass(this IZennoPosterProjectModel project)
        {
            string pin = project.SecureVar("cfgPin");
            string acc = project.Var("acc0");
            var func = (Func<IZennoPosterProjectModel, string, string, string>)
                FunctionStorage.Functions["SAFU_HWPass"];
            return func(project, pin, acc);
        }
    }
}