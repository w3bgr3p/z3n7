using ZennoLab.InterfacesLibrary.ProjectModel;
using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace z3nIO.Web3
{
    public static partial class StringExtensions
    {
        #region CRYPTO - Address Operations
        
        public static string Seed()
        {
            return Blockchain.GenerateMnemonic("English", 12);
        }
        
        public static string NormalizeAddress(this string address)
        {
            if (string.IsNullOrEmpty(address))
                return address;
            
            if (!address.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                return "0x" + address;
            
            return address;
        }

        public static bool ChkAddress(this string shortAddress, string fullAddress)
        {
            if (string.IsNullOrEmpty(shortAddress) || string.IsNullOrEmpty(fullAddress))
                return false;

            if (!shortAddress.Contains("…") || shortAddress.Count(c => c == '…') != 1)
                return false;

            var parts = shortAddress.Split('…');
            if (parts.Length != 2)
                return false;

            string prefix = parts[0];
            string suffix = parts[1];

            if (prefix.Length < 4 || suffix.Length < 2)
                return false;

            if (fullAddress.Length < prefix.Length + suffix.Length)
                return false;

            bool prefixMatch = fullAddress.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            bool suffixMatch = fullAddress.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);

            return prefixMatch && suffixMatch;
        }

        public static string ToAdrEvm(this string key)
        {
            return ToEvmAddress(key);
        }

        public static string ToEvmAddress(this string key)
        {
            string keyType = key.DetectKeyType();
            var blockchain = new Blockchain();

            if (keyType == "seed")
            {
                var mnemonicObj = new Mnemonic(key);
                var hdRoot = mnemonicObj.DeriveExtKey();
                var derivationPath = new NBitcoin.KeyPath("m/44'/60'/0'/0/0");
                key = hdRoot.Derive(derivationPath).PrivateKey.ToHex();

            }
            return blockchain.GetAddressFromPrivateKey(key);
        }

        #endregion

        #region CRYPTO - Key Management

        public static string KeyType(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new Exception($"input isNullOrEmpty");

            input = input.Trim();
    
            if (input.StartsWith("suiprivkey1"))
                return "keySui";
    
            string cleanInput = input.StartsWith("0x") ? input.Substring(2) : input;
    
            if (Regex.IsMatch(cleanInput, @"^[0-9a-fA-F]{64}$"))
                return "keyEvm";
    
            if (Regex.IsMatch(input, @"^[123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz]{87,88}$"))
                return "keySol";
            
            var words = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 12 || words.Length == 24)
                return "seed";
            
            return "undefined";
    
            throw new Exception($"not recognized as any key or seed {input}");
        }

        private static string DetectKeyType(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (Regex.IsMatch(input, @"^[0-9a-fA-F]{64}$"))
                return "key";

            var words = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 12)
                return "seed";
            if (words.Length == 24)
                return "seed";

            return null;
        }

        public static string ToSepc256k1(this string seed, int path = 0)
        {
            var blockchain = new Blockchain();
            var mnemonicObj = new Mnemonic(seed);
            var hdRoot = mnemonicObj.DeriveExtKey();
            var derivationPath = new NBitcoin.KeyPath($"m/44'/60'/0'/0/{path}");
            var key = hdRoot.Derive(derivationPath).PrivateKey.ToHex();
            return key;
        }

        public static string ToEvmPrivateKey(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder hex = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    hex.AppendFormat("{0:x2}", b);
                }

                return hex.ToString();
            }
        }

        #endregion

        #region CRYPTO - Transaction Handling

        public static string GetTxHash(this string link)
        {
            string hash;

            if (!string.IsNullOrEmpty(link))
            {
                int lastSlashIndex = link.LastIndexOf('/');
                if (lastSlashIndex == -1) hash = link;

                else if (lastSlashIndex == link.Length - 1) hash = string.Empty;
                else hash = link.Substring(lastSlashIndex + 1);
            }
            else throw new Exception("empty Element");

            return hash;
        }

        public static string[] TxToString(this string txJson)
        {
            dynamic txData = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(txJson);

            string gas = $"{txData.gas}";
            string value = $"{txData.value}";
            string sender = $"{txData.from}";
            string recipient = $"{txData.to}";
            string data = $"{txData.data}";
           
            BigInteger gasWei = BigInteger.Parse("0" + gas.TrimStart('0', 'x'), NumberStyles.AllowHexSpecifier);
            decimal gasGwei = (decimal)gasWei / 1000000000m;
            string gwei = gasGwei.ToString().Replace(',','.');

            return new string[] { gas, value, sender, data, recipient, gwei };
        }

        #endregion

        #region CRYPTO - Value Conversion

        public static string StringToHex(this string value, string convert = "")
        {
            try
            {
                if (string.IsNullOrEmpty(value)) return "0x0";

                value = value?.Trim();
                if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
                    return "0x0";

                BigInteger result;
                switch (convert.ToLower())
                {
                    case "gwei":
                        result = (BigInteger)(number * 1000000000m);
                        break;
                    case "eth":
                        result = (BigInteger)(number * 1000000000000000000m);
                        break;
                    default:
                        result = (BigInteger)number;
                        break;
                }

                string hex = result.ToString("X").TrimStart('0');
                return string.IsNullOrEmpty(hex) ? "0x0" : "0x" + hex;
            }
            catch
            {
                return "0x0";
            }
        }

        public static string HexToString(this string hexValue, string convert = "")
        {
            try
            {
                hexValue = hexValue?.Replace("0x", "").Trim();
                if (string.IsNullOrEmpty(hexValue)) return "0";
                BigInteger number = BigInteger.Parse("0" + hexValue, NumberStyles.AllowHexSpecifier);
                switch (convert.ToLower())
                {
                    case "gwei":
                        decimal gweiValue = (decimal)number / 1000000000m;
                        return gweiValue.ToString("0.#########", CultureInfo.InvariantCulture);
                    case "eth":
                        decimal ethValue = (decimal)number / 1000000000000000000m;
                        return ethValue.ToString("0.##################", CultureInfo.InvariantCulture);
                    default:
                        return number.ToString();
                }
            }
            catch
            {
                return "0";
            }
        }

        #endregion
        
    }
    
}
