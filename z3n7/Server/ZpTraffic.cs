using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    /// <summary>
    /// The JSONL traffic file <c>trafficLog.jsonl</c> in ZennoPoster's log folder: one line per <c>Rqst</c>
    /// request, with rotation and readers for the embedded server.
    /// </summary>
    internal static class ZpTraffic
    {
        /// <summary>File size after which it is moved to <c>trafficLog_{yyyyMMdd_HHmmss}.jsonl</c>.</summary>
        private const long RotateBytes = 50L * 1024 * 1024;

        /// <summary>How many rotated files are kept; older ones are deleted.</summary>
        private const int KeepRotated = 5;

        /// <summary>Chunk size when reading from the end. Longer records are not lost.</summary>
        private const int ChunkBytes = 64 * 1024;

        /// <summary>
        /// Scan limit of <c>Tail</c>; without it a filter with no matches would read the whole file.
        /// </summary>
        private const long MaxScanBytes = 64L * 1024 * 1024;

        /// <summary>Full path of the traffic file. Throws when <c>LogOptions.LogFile</c> is empty.</summary>
        public static string FilePath(IZennoPosterProjectModel project)
        {
            var directory = ZpLog.Dir(project);
            if (string.IsNullOrEmpty(directory))
                throw new InvalidOperationException("LogOptions.LogFile is empty");
            return Path.GetFullPath(Path.Combine(directory, "trafficLog.jsonl"));
        }

        /// <summary>
        /// Appends one JSON line. Writers in different ZennoPoster processes and AppDomains are serialised by a
        /// named mutex (5 s wait, then <c>IOException</c>); the file is rotated first when it is too large.
        /// </summary>
        public static void Append(IZennoPosterProjectModel project, string json)
        {
            var path = FilePath(project);
            string mutexName;
            using (var hash = SHA256.Create())
                mutexName = "Local\\z3n7.Traffic." + BitConverter.ToString(
                    hash.ComputeHash(Encoding.UTF8.GetBytes(path.ToUpperInvariant()))).Replace("-", "");

            // Coordinate writers in different ZennoPoster processes and AppDomains.
            using (var mutex = new Mutex(false, mutexName))
            {
                var acquired = false;
                try
                {
                    try { acquired = mutex.WaitOne(TimeSpan.FromSeconds(5)); }
                    catch (AbandonedMutexException) { acquired = true; }
                    if (!acquired) throw new IOException("Traffic file is busy: " + path);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    Rotate(path);
                    var bytes = Encoding.UTF8.GetBytes(json + "\n");
                    using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read))
                        stream.Write(bytes, 0, bytes.Length);
                }
                finally { if (acquired) mutex.ReleaseMutex(); }
            }
        }

        // ── Ротация ───────────────────────────────────────────────────────────

        /// <summary>
        /// Moves an oversized file to <c>trafficLog_{yyyyMMdd_HHmmss}.jsonl</c>. Called only under the
        /// <c>Append</c> mutex. A rotation failure must not break traffic writing: the current file is simply
        /// written further.
        /// </summary>
        private static void Rotate(string path)
        {
            try
            {
                var info = new FileInfo(path);
                if (!info.Exists || info.Length <= RotateBytes) return;

                var target = Path.Combine(Path.GetDirectoryName(path),
                    Path.GetFileNameWithoutExtension(path) + "_" +
                    DateTime.Now.ToString("yyyyMMdd_HHmmss") + Path.GetExtension(path));

                // Вторая ротация в ту же секунду — оставляем как есть, дорастёт.
                if (File.Exists(target)) return;

                File.Move(path, target);

                var rotated = Rotated(path);
                for (var i = KeepRotated; i < rotated.Count; i++)
                    try { File.Delete(rotated[i]); } catch { }
            }
            catch { }
        }

        /// <summary>
        /// Rotated files from newest to oldest. The name holds the date, so sorting by name is sorting by time.
        /// </summary>
        private static List<string> Rotated(string path)
        {
            try
            {
                var files = Directory.GetFiles(
                    Path.GetDirectoryName(path),
                    Path.GetFileNameWithoutExtension(path) + "_*" + Path.GetExtension(path));
                Array.Sort(files, StringComparer.OrdinalIgnoreCase);
                Array.Reverse(files);
                return new List<string>(files);
            }
            catch { return new List<string>(); }
        }

        // Byte offsets refer to complete UTF-8 JSONL records; an unfinished last
        // line is left for the next poll. No tail window or response-body truncation.
        /// <summary>
        /// Reads complete records forward from a byte offset; an unfinished last line is left for the next
        /// call.
        /// </summary>
        /// <param name="path">Traffic file.</param>
        /// <param name="offset">Byte offset just after a complete line; 0 for the start.</param>
        /// <param name="max">Most records to return.</param>
        /// <param name="project">Keep only records of this project; empty for all.</param>
        /// <param name="taskId">Keep only records of this task; empty for all.</param>
        /// <returns>
        /// The page; <c>next_offset</c> is where to continue. Throws when the offset does not follow a complete
        /// line or the file is gone.
        /// </returns>
        public static Page Read(string path, long offset, int max, string project, string taskId)
        {
            if (offset < 0 || max < 1) throw new ArgumentOutOfRangeException();
            var page = new Page { file = path, next_offset = offset };
            if (!File.Exists(path))
            {
                if (offset != 0) throw new ArgumentException("Traffic file no longer exists; restart with offset=0");
                return page;
            }
            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                long end = file.Length;
                if (offset > end) throw new ArgumentException("offset exceeds file length; restart with offset=0");
                if (offset > 0)
                {
                    file.Position = offset - 1;
                    if (file.ReadByte() != '\n') throw new ArgumentException("offset must follow a complete JSONL line");
                }
                file.Position = offset;
                using (var input = new BufferedStream(file, 65536))
                using (var line = new MemoryStream())
                {
                    for (long position = offset; position < end; position++)
                    {
                        int value = input.ReadByte();
                        if (value < 0) break;
                        if (value != '\n') { line.WriteByte((byte)value); continue; }
                        using (var document = JsonDocument.Parse(Encoding.UTF8.GetString(line.ToArray())))
                        {
                            var entry = document.RootElement;
                            if (Matches(entry, "project", project) && Matches(entry, "task_id", taskId))
                                page.entries.Add(entry.Clone());
                        }
                        line.SetLength(0);
                        page.next_offset = position + 1;
                        if (page.count >= max) { page.has_more = page.next_offset < end; return page; }
                    }
                }
            }
            return page;
        }

        // ── Tail ──────────────────────────────────────────────────────────────

        /// <summary>
        /// The last <c>max</c> records matching the filters. Files are read from the end in chunks and line
        /// boundaries are searched backwards, so a record of any size (a response body can be megabytes)
        /// arrives whole. Rotated files are read too.
        /// <c>next_offset</c> is set to the length of the current file, so forward polling with <c>Read</c> can
        /// continue from there.
        /// </summary>
        /// <param name="path">Traffic file.</param>
        /// <param name="max">Most records to return.</param>
        /// <param name="project">Keep only records of this project; empty for all.</param>
        /// <param name="taskId">Keep only records of this task; empty for all.</param>
        public static Page Tail(string path, int max, string project, string taskId)
        {
            if (max < 1) throw new ArgumentOutOfRangeException("max");

            var page = new Page { file = path, next_offset = 0 };
            if (!File.Exists(path)) return page;

            try { page.next_offset = new FileInfo(path).Length; } catch { }

            var found = new List<JsonElement>();   // от новых к старым
            long scanned = 0;

            var files = new List<string> { path };
            files.AddRange(Rotated(path));

            foreach (var file in files)
            {
                if (found.Count >= max || scanned >= MaxScanBytes) break;
                ScanBackwards(file, max, project, taskId, found, ref scanned);
            }

            page.truncated = found.Count < max && scanned >= MaxScanBytes;
            found.Reverse();
            page.entries.AddRange(found);
            return page;
        }

        /// <summary>
        /// Reads one file from the end. Adds to <c>found</c> until <c>max</c> records are collected or the scan
        /// budget is spent.
        /// </summary>
        private static void ScanBackwards(string file, int max, string project, string taskId,
                                          List<JsonElement> found, ref long scanned)
        {
            try
            {
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var position = stream.Length;
                    var buffer = new byte[ChunkBytes];

                    // Начало строки, уехавшее в уже прочитанный (более поздний) кусок.
                    var pending = new List<byte>();

                    while (position > 0)
                    {
                        if (found.Count >= max || scanned >= MaxScanBytes) return;

                        var size = (int)Math.Min(ChunkBytes, position);
                        position -= size;
                        stream.Position = position;
                        ReadExactly(stream, buffer, size);
                        scanned += size;

                        var end = size;
                        for (var i = size - 1; i >= 0; i--)
                        {
                            if (buffer[i] != (byte)'\n') continue;
                            Collect(Line(buffer, i + 1, end - i - 1, pending), project, taskId, found);
                            pending.Clear();
                            end = i;
                            if (found.Count >= max) return;
                        }

                        if (end > 0)
                        {
                            var head = new byte[end];
                            Buffer.BlockCopy(buffer, 0, head, 0, end);
                            pending.InsertRange(0, head);
                        }
                    }

                    // Первая строка файла — перед ней нет '\n'.
                    if (pending.Count > 0 && found.Count < max)
                        Collect(pending.ToArray(), project, taskId, found);
                }
            }
            catch { }
        }

        private static byte[] Line(byte[] buffer, int start, int length, List<byte> pending)
        {
            var line = new byte[length + pending.Count];
            if (length > 0) Buffer.BlockCopy(buffer, start, line, 0, length);
            pending.CopyTo(line, length);
            return line;
        }

        /// <summary>
        /// A broken line is skipped silently instead of failing the whole result. (<c>Read</c> does not skip: a
        /// broken line makes it throw.)
        /// </summary>
        private static void Collect(byte[] line, string project, string taskId, List<JsonElement> found)
        {
            if (line.Length == 0) return;
            try
            {
                using (var document = JsonDocument.Parse(Encoding.UTF8.GetString(line)))
                {
                    var entry = document.RootElement;
                    if (Matches(entry, "project", project) && Matches(entry, "task_id", taskId))
                        found.Add(entry.Clone());
                }
            }
            catch { }
        }

        private static void ReadExactly(Stream stream, byte[] buffer, int count)
        {
            var read = 0;
            while (read < count)
            {
                var chunk = stream.Read(buffer, read, count - read);
                if (chunk <= 0) throw new IOException("Unexpected end of traffic file");
                read += chunk;
            }
        }

        private static bool Matches(JsonElement entry, string field, string filter)
        {
            return string.IsNullOrEmpty(filter) || (entry.TryGetProperty(field, out var value)
                && string.Equals(value.GetString(), filter, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>A batch of traffic records.</summary>
        public sealed class Page
        {
            /// <summary>File that was read.</summary>
            public string file { get; set; }
            /// <summary>Number of records.</summary>
            public int count => entries.Count;
            /// <summary>The records, oldest first.</summary>
            public List<JsonElement> entries { get; } = new List<JsonElement>();
            /// <summary>Offset to continue reading from.</summary>
            public long next_offset { get; set; }
            /// <summary>More complete records follow <c>next_offset</c>.</summary>
            public bool has_more { get; set; }

            /// <summary>
            /// <c>Tail</c> hit the scan limit: there are fewer records than requested, not because there are no
            /// more.
            /// </summary>
            public bool truncated { get; set; }
        }
    }
}
