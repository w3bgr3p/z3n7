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
    internal static class ZpTraffic
    {
        public static string FilePath(IZennoPosterProjectModel project)
        {
            var directory = ZpLog.Dir(project);
            if (string.IsNullOrEmpty(directory))
                throw new InvalidOperationException("LogOptions.LogFile is empty");
            return Path.GetFullPath(Path.Combine(directory, "trafficLog.jsonl"));
        }

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
                    var bytes = Encoding.UTF8.GetBytes(json + "\n");
                    using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read))
                        stream.Write(bytes, 0, bytes.Length);
                }
                finally { if (acquired) mutex.ReleaseMutex(); }
            }
        }

        // Byte offsets refer to complete UTF-8 JSONL records; an unfinished last
        // line is left for the next poll. No tail window or response-body truncation.
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

        private static bool Matches(JsonElement entry, string field, string filter)
        {
            return string.IsNullOrEmpty(filter) || (entry.TryGetProperty(field, out var value)
                && string.Equals(value.GetString(), filter, StringComparison.OrdinalIgnoreCase));
        }

        public sealed class Page
        {
            public string file { get; set; }
            public int count => entries.Count;
            public List<JsonElement> entries { get; } = new List<JsonElement>();
            public long next_offset { get; set; }
            public bool has_more { get; set; }
        }
    }
}
