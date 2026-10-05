using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    /// <summary>
    /// Access token of <c>ZpServer</c>: storing, issuing, checking requests.
    /// There is one secret per machine, kept under <c>ZP_TOKEN</c> in the <c>.env</c> next to the assembly
    /// (the same file <c>Env.ReadEnv(global: true)</c> reads). The token is printed in the node line so
    /// that the line can be pasted into the DevDeck panel as a whole.
    /// </summary>
    public static class ZpAuth
    {
        /// <summary>Key of the token in <c>.env</c>.</summary>
        public const string EnvKey = "ZP_TOKEN";

        private static volatile string _token;

        /// <summary>The current token. An empty string until <c>Load</c> is called.</summary>
        public static string Token => _token ?? "";

        /// <summary>
        /// Reads the token from <c>.env</c>; when there is none, generates one (32 random bytes as hex) and
        /// tries to save it. Call it before taking the port: the configuration check is cheap, and there is no
        /// point holding the resource if something is wrong with it.
        /// A failed write does not stop the server: the node stays manageable, but the token lives only in the
        /// process memory.
        /// </summary>
        /// <param name="log">Write the path of the saved token to the log.</param>
        public static void Load(IZennoPosterProjectModel project, bool log)
        {
            var stored = SafeReadEnv(project);
            if (!string.IsNullOrEmpty(stored))
            {
                _token = stored.Trim();
                return;
            }

            _token = Generate();

            string error;
            if (TryPersist(_token, out error))
            {
                if (log) project.SendInfoToLog($"[ZpAuth] токен сгенерирован и записан в {EnvPath()}", true);
            }
            else
            {
                project.warn($"[ZpAuth] токен не сохранён ({error}) — после перезапуска ZennoPoster он сменится, узел придётся заводить в DevDeck заново");
            }
        }

        /// <summary>
        /// Checks the request token: the <c>Authorization: Bearer</c> header, otherwise the <c>?token=</c>
        /// parameter, needed for download links where a header cannot be set.
        /// </summary>
        /// <returns><c>false</c> when <c>Load</c> has not run or the token does not match.</returns>
        public static bool Authorized(HttpListenerRequest req)
        {
            var expected = _token;
            if (string.IsNullOrEmpty(expected)) return false; // Load не отработал — закрыто

            var provided = FromHeader(req) ?? req.QueryString["token"];
            return !string.IsNullOrEmpty(provided) && FixedTimeEquals(provided, expected);
        }

        // ── Внутреннее ────────────────────────────────────────────────────────

        private static string FromHeader(HttpListenerRequest req)
        {
            var raw = req.Headers["Authorization"];
            if (string.IsNullOrEmpty(raw)) return null;

            raw = raw.Trim();
            const string scheme = "Bearer ";
            if (!raw.StartsWith(scheme, StringComparison.OrdinalIgnoreCase)) return null;

            var value = raw.Substring(scheme.Length).Trim();
            return value.Length == 0 ? null : value;
        }

        /// <summary>
        /// Comparison without early exit: the time does not depend on where the values differ. It does not hide
        /// the length, but the token length is fixed and not a secret.
        /// </summary>
        private static bool FixedTimeEquals(string a, string b)
        {
            var x = Encoding.UTF8.GetBytes(a);
            var y = Encoding.UTF8.GetBytes(b);

            var diff = (uint)x.Length ^ (uint)y.Length;
            for (int i = 0, n = Math.Min(x.Length, y.Length); i < n; i++)
                diff |= (uint)(x[i] ^ y[i]);

            return diff == 0;
        }

        private static string Generate()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);

            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>Folder of the assembly: the same one <c>Env</c> uses for <c>global</c>.</summary>
        private static string EnvPath() =>
            Path.Combine(Path.GetDirectoryName(typeof(ZpAuth).Assembly.Location) ?? "", ".env");

        private static string SafeReadEnv(IZennoPosterProjectModel project)
        {
            try   { return project.ReadEnv(EnvKey, global: true); }
            catch { return null; }
        }

        private static bool TryPersist(string token, out string error)
        {
            error = "";
            try
            {
                var path = EnvPath();

                // Дописывание в конец файла без перевода строки склеило бы
                // ключ с предыдущим — проверяем последний символ.
                var prefix = "";
                if (File.Exists(path))
                {
                    var existing = File.ReadAllText(path);
                    if (existing.Length > 0 && !existing.EndsWith("\n")) prefix = Environment.NewLine;
                }

                File.AppendAllText(path, prefix + EnvKey + "=" + token + Environment.NewLine, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.GetType().Name + ": " + ex.Message;
                return false;
            }
        }
    }
}
