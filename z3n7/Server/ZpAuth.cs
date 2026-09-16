using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    /// <summary>
    /// Токен доступа к ZpServer: хранение, выдача, проверка запроса.
    ///
    /// Секрет один на машину и лежит в ключе ZP_TOKEN файла .env рядом со
    /// сборкой (тот же файл, что читает Env.ReadEnv(global: true)). Токен
    /// печатается в строке узла, чтобы её можно было целиком вставить в панель
    /// DevDeck.
    /// </summary>
    public static class ZpAuth
    {
        public const string EnvKey = "ZP_TOKEN";

        private static volatile string _token;

        /// <summary>Действующий токен. Пустая строка, пока не вызван Load.</summary>
        public static string Token => _token ?? "";

        /// <summary>
        /// Достаёт токен из .env, а при отсутствии — генерирует и пытается
        /// сохранить. Вызывать до захвата порта: проверка конфигурации дешёвая,
        /// и незачем занимать ресурс, если с ней что-то не так.
        ///
        /// Неудачная запись файла сервер не останавливает: узел остаётся
        /// управляемым, но токен живёт только в памяти процесса.
        /// </summary>
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
        /// Токен запроса: заголовок Authorization: Bearer, иначе параметр
        /// ?token= — он нужен для ссылок на скачивание, куда заголовок
        /// не подставить.
        /// </summary>
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
        /// Сравнение без раннего выхода: время не зависит от того, на каком
        /// байте значения разошлись. Длину такое сравнение не скрывает — но
        /// длина токена фиксированная и секретом не является.
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

        /// <summary>Каталог сборки — тот же, что использует Env для global.</summary>
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
