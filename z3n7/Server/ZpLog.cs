using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    /// <summary>
    /// Чтение файловых логов ZennoPoster.
    ///
    /// Каталог берётся из project.LogOptions.LogFile, но само имя оттуда брать
    /// нельзя: свойство говорит "executionLog.txt", а на диске лежат
    /// executionLog-ZennoPoster.txt и executionLog-ProjectMaker.txt — Зенка
    /// дописывает имя процесса, и это отдельный механизм от SplitLogByThread.
    ///
    /// Благодаря этому лог ZennoPoster читается и из ProjectMaker, и наоборот.
    /// Файлы в UTF-8 без BOM, ZennoPoster держит их открытыми на запись.
    /// </summary>
    internal static class ZpLog
    {
        /// <summary>Короткое имя вида → префикс файла.</summary>
        private static readonly Dictionary<string, string> Kinds =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "execution", "executionLog"      },
                { "errors",    "nonCriticalErrors" },
                { "critical",  "criticalErrors"    },
            };

        /// <summary>Сколько байт с конца файла читать. Записи длиннее просто обрежутся.</summary>
        private const int TailBytes = 512 * 1024;

        private static readonly Regex RecordStart =
            new Regex(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}", RegexOptions.Compiled);

        private static readonly Regex ModuleTail =
            new Regex("\"([^\"]*)\"\\s*$", RegexOptions.Compiled);

        /// <summary>
        /// В executionLog шесть колонок — пятая это имя проекта. В остальных
        /// пять, проекта нет. Проверено на живых файлах 7.9.1.0.
        /// </summary>
        public static bool HasProjectColumn(string kind) =>
            string.Equals(kind, "execution", StringComparison.OrdinalIgnoreCase);

        /// <summary>Каталог логов, или null если LogOptions недоступен.</summary>
        public static string Dir(IZennoPosterProjectModel project)
        {
            try
            {
                var configured = project.LogOptions.LogFile;
                if (string.IsNullOrEmpty(configured)) return null;
                return Path.GetDirectoryName(configured);
            }
            catch { return null; }
        }

        /// <summary>Файлы логов, лежащие в каталоге — для подсказки, когда запрошенного нет.</summary>
        public static string[] Available(IZennoPosterProjectModel project)
        {
            var dir = Dir(project);
            if (dir == null) return new string[0];
            try   { return Directory.GetFiles(dir, "*.txt").Select(Path.GetFileName).ToArray(); }
            catch { return new string[0]; }
        }

        /// <summary>
        /// Полный путь к файлу лога. Возвращает null и заполняет error, если
        /// вид неизвестен, каталог не определён или файла нет.
        /// </summary>
        public static string Resolve(IZennoPosterProjectModel project, string kind, string process, out string error)
        {
            error = null;

            string prefix;
            if (!Kinds.TryGetValue(kind ?? "", out prefix))
            {
                error = "unknown kind, expected: " + string.Join(", ", Kinds.Keys);
                return null;
            }

            var dir = Dir(project);
            if (dir == null) { error = "LogOptions.LogFile is empty"; return null; }

            var path = Path.Combine(dir, prefix + "-" + process + ".txt");
            if (!File.Exists(path)) { error = "no such log file: " + path; return null; }

            return path;
        }

        public sealed class Entry
        {
            public string ts      { get; set; }
            public string level   { get; set; }
            public string thread  { get; set; }
            public string logger  { get; set; }
            public string project { get; set; }
            public string module  { get; set; }
            public string text    { get; set; }

            /// <summary>Пустые поля в JSON не выводим — они только шумят.</summary>
            public Dictionary<string, object> ToJson()
            {
                var d = new Dictionary<string, object>
                {
                    { "ts",     ts     },
                    { "level",  level  },
                    { "thread", thread },
                };
                if (!string.IsNullOrEmpty(logger))  d["logger"]  = logger;
                if (!string.IsNullOrEmpty(project)) d["project"] = project;
                if (!string.IsNullOrEmpty(module))  d["module"]  = module;
                d["text"] = text;
                return d;
            }
        }

        /// <summary>
        /// Последние max записей. Читается только хвост файла, поэтому первая
        /// запись в окне может оказаться обрезанной — её отбрасываем вместе с
        /// куском строки, на котором начали.
        /// </summary>
        public static List<Entry> Tail(string path, int max, string projectFilter, bool hasProjectColumn)
        {
            var entries = new List<Entry>();

            string text;
            try
            {
                // ReadWrite — ZennoPoster держит файл открытым на запись.
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fs.Length > TailBytes) fs.Seek(-TailBytes, SeekOrigin.End);

                    using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    {
                        // Начали с середины строки — дочитываем её и выбрасываем.
                        if (fs.Position > 0) sr.ReadLine();
                        text = sr.ReadToEnd();
                    }
                }
            }
            catch { return entries; }

            Entry current = null;
            var tail = new StringBuilder();

            foreach (var raw in text.Replace("\r\n", "\n").Split('\n'))
            {
                if (RecordStart.IsMatch(raw))
                {
                    Flush(entries, current, tail, hasProjectColumn);
                    current = Parse(raw, hasProjectColumn, tail);
                }
                else if (current != null && raw.Trim().Length > 0)
                {
                    if (tail.Length > 0) tail.Append('\n');
                    tail.Append(raw.Trim());
                }
            }
            Flush(entries, current, tail, hasProjectColumn);

            if (!string.IsNullOrEmpty(projectFilter))
                entries = entries.Where(e => string.Equals(e.project, projectFilter,
                                             StringComparison.OrdinalIgnoreCase)).ToList();

            return entries.Count > max ? entries.GetRange(entries.Count - max, max) : entries;
        }

        private static void Flush(List<Entry> into, Entry entry, StringBuilder tail, bool clean)
        {
            if (entry == null) return;

            // text из Parse — запасной вариант на случай записи без продолжения.
            if (tail.Length > 0)
            {
                var body = tail.ToString();
                entry.text = clean ? Unwrap(body) : body;
            }
            tail.Length = 0;
            into.Add(entry);
        }

        private static Entry Parse(string line, bool hasProjectColumn, StringBuilder tail)
        {
            var f = line.Split('|');
            var entry = new Entry
            {
                ts      = f.Length > 0 ? f[0] : "",
                level   = f.Length > 1 ? f[1] : "",
                thread  = f.Length > 2 ? f[2] : "",
                logger  = f.Length > 3 ? f[3] : "",
                project = "",
                module  = "",
                text    = "",
            };

            var rest = 4;
            if (hasProjectColumn && f.Length > 4) { entry.project = f[4]; rest = 5; }

            var head = f.Length > rest ? string.Join("|", f.Skip(rest)).Trim() : "";
            tail.Length = 0;

            if (hasProjectColumn)
            {
                // Заголовок здесь — только обёртка вида «Событие в модуле "X"»
                // (814 из 842 записей) либо «Ошибка в модуле "X"» (28). Уровень
                // уже есть в level, так что от неё нужно лишь имя модуля.
                // Колонка logger в этом файле всегда ZennoLab.LogLibrary.InternalError
                // и не несёт ничего — даже у INFO.
                entry.logger = "";
                entry.module = ModuleName(head);
                entry.text   = head;   // запасной вариант, если продолжения не будет
            }
            else
            {
                tail.Append(head);
            }

            return entry;
        }

        /// <summary>Имя модуля из хвоста заголовка: ... "Имя". Пустое, если его нет.</summary>
        private static string ModuleName(string head)
        {
            var m = ModuleTail.Match(head);
            return m.Success ? m.Groups[1].Value : "";
        }

        /// <summary>
        /// Снимает обёртку «Сообщение: "…"». Кавычки берутся крайние: внутри
        /// нередко лежит JSON со своими, и жадный захват их сохраняет.
        /// </summary>
        private static string Unwrap(string s)
        {
            var t     = s.Trim();
            var open  = t.IndexOf('"');
            var close = t.LastIndexOf('"');
            return (open < 0 || close <= open) ? t : t.Substring(open + 1, close - open - 1);
        }
    }
}
