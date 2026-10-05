using System.IO;
using System.Text;
using ZennoLab.InterfacesLibrary.ProjectModel;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace z3n7.Tools
{
    /// <summary>
    /// Reading and writing ZennoPoster project files (.zp) through ProjectMaker's own loader, and searching
    /// their actions. Works only inside ProjectMaker: it uses the ProjectMaker assembly loaded in the
    /// process and does nothing elsewhere.
    /// </summary>
    public static class Extractor
    {
        private const string InputSettingsHtmlEntry = "InputSettings/inputSettings.html";
        private static readonly Regex RxXmlDecl = new Regex(@"<\?xml[^?]*\?>", RegexOptions.Compiled);
        
        /// <summary>Unpacks the project XML of a .zp file.</summary>
        /// <param name="zpPath">Project file.</param>
        /// <returns>The XML; the loader's exception text when it fails; <c>null</c> outside ProjectMaker.</returns>
        public static string ExtractXml(string zpPath)
        {
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name != "ProjectMaker") continue;
                try
                {
                    var loaderType = asm.GetType("ZennoLab.TemplateManipulator.V4.ProjectLoaderV4");
                    var archiveType = asm.GetType("ZennoLab.TemplateManipulator.V4.ProjectArchiveV4");
                    var archive = System.Activator.CreateInstance(archiveType, zpPath);
                    var loader = System.Activator.CreateInstance(loaderType);
                    string xml = (string)loaderType.GetMethod("LoadFromBytesArray").Invoke(loader, new object[] { archive });
                    return xml;
                }
                catch(System.Reflection.TargetInvocationException ex)
                {
                    return (ex.InnerException != null ? ex.InnerException.ToString() : ex.ToString());
                }
            }
            return null;
        }

        /// <summary>Unpacks the current project to XML (UTF-16).</summary>
        /// <param name="xmlPath">Target file; default is the project file name with <c>.xml</c>.</param>
        public static void SaveAsXml(this IZennoPosterProjectModel project, string xmlPath = null)
        {
            var xml = ExtractXml(project.Path + project.Name);
            if (xmlPath == null)
                xmlPath = Path.Combine(project.Path, project.Name.Replace(".zp",".xml"));
            File.WriteAllText(xmlPath, xml, Encoding.Unicode);
            project.SendInfoToLog($"saved as {xmlPath}");
        }
        /// <summary>Reads the input settings HTML (<c>InputSettings/inputSettings.html</c>) of a .zp file.</summary>
        /// <returns>The HTML without BOM; <c>null</c> outside ProjectMaker.</returns>
        public static string ExtractInputSettingsHtml(string zpPath)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name != "ProjectMaker") continue;

                var archiveType = asm.GetType("ZennoLab.TemplateManipulator.V4.ProjectArchiveV4");
                var archive = Activator.CreateInstance(archiveType, zpPath);
                try
                {
                    var bytes = (byte[])archiveType.GetMethod("GetBytes")
                        .Invoke(archive, new object[] { InputSettingsHtmlEntry });
                    return Encoding.UTF8.GetString(bytes).TrimStart('\uFEFF');
                }
                finally
                {
                    archiveType.GetMethod("Dispose", Type.EmptyTypes)?.Invoke(archive, null);
                }
            }

            return null;
        }

        /// <summary>Replaces the input settings HTML inside the .zp file itself.</summary>
        public static void SaveInputSettingsHtml(string zpPath, string html)
        {
            SaveInputSettingsHtml(zpPath, html, zpPath);
        }

        /// <summary>
        /// Writes a copy of the .zp file with the input settings HTML replaced. When the output is the source
        /// file, it is written through a temporary file.
        /// </summary>
        /// <param name="zpPath">Source project.</param>
        /// <param name="html">New input settings HTML.</param>
        /// <param name="outputZpPath">Target project file.</param>
        public static void SaveInputSettingsHtml(string zpPath, string html, string outputZpPath)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name != "ProjectMaker") continue;

                var archiveType = asm.GetType("ZennoLab.TemplateManipulator.V4.ProjectArchiveV4");
                var archive = Activator.CreateInstance(archiveType, zpPath);
                var overwriteSource = string.Equals(
                    Path.GetFullPath(zpPath),
                    Path.GetFullPath(outputZpPath),
                    StringComparison.OrdinalIgnoreCase);
                var savePath = overwriteSource
                    ? Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".zp")
                    : outputZpPath;

                try
                {
                    var bytes = Encoding.UTF8.GetBytes("\uFEFF" + html.TrimStart('\uFEFF'));
                    archiveType.GetMethod("RemoveEntry")
                        .Invoke(archive, new object[] { InputSettingsHtmlEntry });
                    archiveType.GetMethod("AddEntry", new[] { typeof(string), typeof(byte[]) })
                        .Invoke(archive, new object[] { InputSettingsHtmlEntry, bytes });
                    archiveType.GetMethod("SaveToFile")
                        .Invoke(archive, new object[] { savePath });
                }
                finally
                {
                    archiveType.GetMethod("Dispose", Type.EmptyTypes)?.Invoke(archive, null);
                }

                if (overwriteSource)
                {
                    try
                    {
                        File.Copy(savePath, outputZpPath, true);
                    }
                    finally
                    {
                        File.Delete(savePath);
                    }
                }

                return;
            }
        }

        /// <summary>Builds a .zp file from project XML, using the current project file as the container.</summary>
        /// <param name="xml">Project XML; default is the current project's <c>.xml</c> next to it.</param>
        /// <param name="zpPath">
        /// Target file; default is the project name with a Unix-ms suffix, e.g. <c>name.1730000000000.zp</c>.
        /// </param>
        public static void BuildZpFromXml(this IZennoPosterProjectModel project,string xml = null, string zpPath= null)
        {
            var sourceZpPath = Path.Combine(project.Path, project.Name);
            if (xml == null)
            {
                var xmlPath = Path.Combine(project.Path, project.Name.Replace(".zp", ".xml"));
                xml = File.ReadAllText(xmlPath);
            }

            zpPath = zpPath ?? Path.Combine(
                project.Path,
                project.Name.Replace(".zp", $".{((long)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds)).ToString()}.zp"));
            
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name != "ProjectMaker") continue;
	            
                var loaderType  = asm.GetType("ZennoLab.TemplateManipulator.V4.ProjectLoaderV4");
                var archiveType = asm.GetType("ZennoLab.TemplateManipulator.V4.ProjectArchiveV4");
                var tmpPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".zp");
                object archive = null;

                try
                {
                    File.Copy(sourceZpPath, tmpPath, true);
                    archive = Activator.CreateInstance(archiveType, tmpPath);
                    var loader = Activator.CreateInstance(loaderType);

                    byte[] bytes = (byte[])loaderType.GetMethod("ToByteArray")
                        .Invoke(loader, new object[] { xml });

                    archiveType.GetMethod("RemoveEntries")
                        .Invoke(archive, new object[]
                        {
                            (Func<string, bool>)(name => string.Equals(
                                name,
                                "Template.xml",
                                StringComparison.OrdinalIgnoreCase))
                        });

                    archiveType.GetMethod("SaveProject")
                        .Invoke(archive, new object[] { "Template.xml", bytes });

                    archiveType.GetMethod("SaveToFile")
                        .Invoke(archive, new object[] { zpPath });
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    if (ex.InnerException != null)
                        System.Runtime.ExceptionServices.ExceptionDispatchInfo
                            .Capture(ex.InnerException)
                            .Throw();
                    throw;
                }
                finally
                {
                    if (archive != null)
                        archiveType.GetMethod("Dispose", Type.EmptyTypes)?.Invoke(archive, null);
                    File.Delete(tmpPath);
                }

                break;
            }
        }


        // ── Search ────────────────────────────────────────────────────────────

        /// <summary>One match of <c>SearchInZp</c>.</summary>
        public class SearchHit
        {
            /// <summary>Project file.</summary>
            public string ZpPath;
            /// <summary>Id of the action (step) containing the match.</summary>
            public string StepId;
            /// <summary>The match with surrounding text; line breaks collapsed to spaces.</summary>
            public string Context;
            /// <summary><c>ZpPath</c>, <c>StepId</c> and <c>Context</c>, tab-separated.</summary>
            public override string ToString() => $"{ZpPath}\t{StepId}\t{Context}";
        }

        /// <summary>
        /// Finds text in the actions of every .zp file in a folder (case-insensitive, in attributes and values;
        /// XML entities are decoded for matching). One hit per action; each hit is also written to the log.
        /// Unpacking takes about a second per file, so unpacked XML can be cached in a hidden <c>.xml</c>
        /// folder; the cache file name holds the project's modification time and size, so a changed project is
        /// re-read automatically.
        /// </summary>
        /// <param name="text">Text to find.</param>
        /// <param name="folder">Folder; default is the project folder.</param>
        /// <param name="recursive">Include subfolders.</param>
        /// <param name="cache">Use the XML cache.</param>
        /// <param name="padding">Characters of context on each side.</param>
        /// <remarks>
        /// Works only inside ProjectMaker: it uses the ProjectMaker assembly loaded in the process and does
        /// nothing elsewhere.
        /// </remarks>
        public static List<SearchHit> SearchInZp(this IZennoPosterProjectModel project, string text, string folder = null, bool recursive = true, bool cache = true, int padding = 60)
        {
            folder = folder ?? project.Path;
            var hits = new List<SearchHit>();
            if (string.IsNullOrEmpty(text) || !Directory.Exists(folder)) return hits;

            var cacheDir = cache ? PrepareCacheDir(folder) : null;
            var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            foreach (var zpPath in Directory.GetFiles(folder, "*.zp", option))
                SearchInZp(project, zpPath, text, hits, folder, cacheDir, padding);

            return hits;
        }

        static void SearchInZp(IZennoPosterProjectModel project, string zpPath, string text, List<SearchHit> hits, string folder, string cacheDir, int padding)
        {
            XDocument doc;
            try
            {
                var xml = cacheDir == null ? ExtractXml(zpPath) : CachedXml(zpPath, folder, cacheDir);
                if (string.IsNullOrEmpty(xml)) return;
                doc = XDocument.Parse(RxXmlDecl.Replace(xml, ""));
            }
            catch
            {
                return;
            }

            foreach (var step in doc.Descendants("Step"))
            {
                var stepId = step.Attribute("ID")?.Value;
                if (string.IsNullOrEmpty(stepId)) continue;
                string context;
                if (!StepContains(step, text, padding, out context)) continue;

                var hit = new SearchHit { ZpPath = zpPath, StepId = stepId, Context = context };
                project.SendInfoToLog(hit.ToString());
                hits.Add(hit);
            }
        }

        // ── Кэш распакованного XML ────────────────────────────────────────────
        //
        // ExtractXml стоит ~1 секунду на файл (замерено: всё время внутри
        // ZennoLab.LoadFromBytesArray, наш парсинг и поиск — ~2 мс), и эта секунда
        // не параллелится. Поэтому распакованный XML кладём рядом с проектами.
        //
        // Валидность определяет само имя файла кэша: в него зашиты время модификации
        // и размер .zp. Изменился проект — имя другое, файла нет, перечитываем только его.
        // Отдельной инвалидации не требуется.

        private const string CacheDirName = ".xml";

        static string PrepareCacheDir(string folder)
        {
            try
            {
                var dir = Path.Combine(folder, CacheDirName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    File.SetAttributes(dir, File.GetAttributes(dir) | FileAttributes.Hidden);
                }
                return dir;
            }
            catch
            {
                return null;   // кэш — удобство, а не условие работы
            }
        }

        static string CachedXml(string zpPath, string folder, string cacheDir)
        {
            string prefix = null, cachePath = null;
            try
            {
                var fi = new FileInfo(zpPath);
                prefix = CacheKeyPrefix(zpPath, folder);
                cachePath = Path.Combine(cacheDir, $"{prefix}.{fi.LastWriteTimeUtc.Ticks}.{fi.Length}.xml");
                if (File.Exists(cachePath)) return File.ReadAllText(cachePath, Encoding.UTF8);
            }
            catch
            {
                return ExtractXml(zpPath);
            }

            var xml = ExtractXml(zpPath);
            if (string.IsNullOrEmpty(xml)) return xml;

            try
            {
                foreach (var stale in Directory.GetFiles(cacheDir, prefix + ".*.xml"))
                    File.Delete(stale);
                File.WriteAllText(cachePath, xml, Encoding.UTF8);
            }
            catch
            {
                // не записалось — не беда, в следующий раз распакуем заново
            }

            return xml;
        }

        // Имя проекта + короткий хеш его папки: читаемо и не конфликтует между подпапками.
        static string CacheKeyPrefix(string zpPath, string folder)
        {
            var dir = Path.GetDirectoryName(Path.GetFullPath(zpPath)) ?? "";
            var hash = 2166136261u;
            foreach (var c in dir.ToLowerInvariant())
                hash = (hash ^ c) * 16777619u;

            var name = Path.GetFileNameWithoutExtension(zpPath);
            foreach (var bad in Path.GetInvalidFileNameChars())
                name = name.Replace(bad, '_');

            return $"{name}.{hash:x8}";
        }

        // Первое совпадение в шаге; context — найденное вместе с padding символов слева и справа.
        static bool StepContains(XElement step, string text, int padding, out string context)
        {
            foreach (var el in step.DescendantsAndSelf())
            {
                foreach (var attr in el.Attributes())
                    if (Contains(attr.Value, text, padding, out context)) return true;

                if (!el.HasElements && Contains(el.Value, text, padding, out context)) return true;
            }
            context = null;
            return false;
        }

        static bool Contains(string haystack, string needle, int padding, out string context)
        {
            context = null;
            if (string.IsNullOrEmpty(haystack)) return false;

            // Контекст строим по раскодированному тексту: иначе вырезка приезжает
            // засыпанной &#xD;&#xA; и &quot;, и читать её невозможно.
            // Декодирование затрагивает только строки с '&' — остальные не трогаем.
            if (haystack.IndexOf('&') >= 0)
            {
                var decoded = DecodeEntities(haystack);
                var d = decoded.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
                if (d >= 0)
                {
                    context = Snippet(decoded, d, needle.Length, padding);
                    return true;
                }
            }

            // Откат на сырой текст: нужен, когда ищут сами сущности (&quot; и т.п.).
            var i = haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
            if (i < 0) return false;

            context = Snippet(haystack, i, needle.Length, padding);
            return true;
        }

        // Вырезка вокруг совпадения. Переводы строк и табы схлопываются в пробелы,
        // иначе одна находка разъезжается на несколько строк лога.
        static string Snippet(string haystack, int index, int length, int padding)
        {
            if (padding < 0) padding = 0;

            var from = Math.Max(0, index - padding);
            var to   = Math.Min(haystack.Length, index + length + padding);

            var sb = new StringBuilder(to - from + 2);
            if (from > 0) sb.Append('…');

            var space = false;
            for (var i = from; i < to; i++)
            {
                var c = haystack[i];
                if (c == '\r' || c == '\n' || c == '\t' || c == ' ')
                {
                    if (!space && sb.Length > 0) sb.Append(' ');
                    space = true;
                    continue;
                }
                sb.Append(c);
                space = false;
            }

            if (to < haystack.Length) sb.Append('…');
            return sb.ToString();
        }

        static string DecodeEntities(string s) =>
            s.Replace("&#xD;&#xA;", "\n").Replace("&#xD;", "\r").Replace("&#xA;", "\n")
             .Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"");

    }
    
}
