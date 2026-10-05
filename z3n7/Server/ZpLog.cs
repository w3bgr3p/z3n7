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
    /// Reads ZennoPoster's log files.
    /// The folder comes from <c>project.LogOptions.LogFile</c>, but the file name must not: the property
    /// says <c>executionLog.txt</c>, while the files on disk are <c>executionLog-ZennoPoster.txt</c> and
    /// <c>executionLog-ProjectMaker.txt</c> — ZennoPoster appends the process name, a mechanism separate
    /// from <c>SplitLogByThread</c>. Thanks to that, the ZennoPoster log can be read from ProjectMaker and
    /// vice versa. The files are UTF-8 without BOM and ZennoPoster keeps them open for writing.
    /// </summary>
    internal static class ZpLog
    {
        /// <summary>Short kind name → file prefix.</summary>
        private static readonly Dictionary<string, string> Kinds =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "execution", "executionLog"      },
                { "errors",    "nonCriticalErrors" },
                { "critical",  "criticalErrors"    },
            };

        /// <summary>How many bytes to read from the end of a file. Longer records are cut.</summary>
        private const int TailBytes = 512 * 1024;

        private static readonly Regex RecordStart =
            new Regex(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}", RegexOptions.Compiled);

        private static readonly Regex ModuleTail =
            new Regex("\"([^\"]*)\"\\s*$", RegexOptions.Compiled);

        /// <summary>
        /// The execution log has six columns, the fifth being the project name; the others have five and no
        /// project. Observed on 7.9.1.0 log files.
        /// </summary>
        public static bool HasProjectColumn(string kind) =>
            string.Equals(kind, "execution", StringComparison.OrdinalIgnoreCase);

        /// <summary>Log folder, or <c>null</c> when <c>LogOptions</c> is not available.</summary>
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

        /// <summary>Log files in the folder, as a hint when the requested one is missing.</summary>
        public static string[] Available(IZennoPosterProjectModel project)
        {
            var dir = Dir(project);
            if (dir == null) return new string[0];
            try   { return Directory.GetFiles(dir, "*.txt").Select(Path.GetFileName).ToArray(); }
            catch { return new string[0]; }
        }

        /// <summary>
        /// Full path of a log file. Returns <c>null</c> and sets <c>error</c> when the kind is unknown, the
        /// folder is not known or the file does not exist.
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

        /// <summary>One log record.</summary>
        public sealed class Entry
        {
            /// <summary>Timestamp as written in the file.</summary>
            public string ts      { get; set; }
            /// <summary>Level.</summary>
            public string level   { get; set; }
            /// <summary>Thread.</summary>
            public string thread  { get; set; }
            /// <summary>Logger name; empty for the execution log, where it carries nothing.</summary>
            public string logger  { get; set; }
            /// <summary>Project name; execution log only.</summary>
            public string project { get; set; }
            /// <summary>Action (module) name taken from the record header; execution log only.</summary>
            public string module  { get; set; }
            /// <summary>Message text.</summary>
            public string text    { get; set; }

            /// <summary>Fields for JSON; empty optional fields are left out because they only add noise.</summary>
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
        /// The last <c>max</c> records. Only the tail of the file is read, so the first record in the window
        /// may be cut; it is dropped together with the partial line the read started in.
        /// </summary>
        /// <param name="path">Log file.</param>
        /// <param name="max">Most records to return.</param>
        /// <param name="projectFilter">Only records of this project; empty for all.</param>
        /// <param name="hasProjectColumn">Whether the file has the project column (see <c>HasProjectColumn</c>).</param>
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

        /// <summary>Module name from the end of the header: … <c>"Name"</c>. Empty when there is none.</summary>
        private static string ModuleName(string head)
        {
            var m = ModuleTail.Match(head);
            return m.Success ? m.Groups[1].Value : "";
        }

        /// <summary>
        /// Removes the <c>Message: "…"</c> wrapper. The outermost quotes are used: the text often holds JSON
        /// with its own quotes, and a greedy match keeps them.
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
