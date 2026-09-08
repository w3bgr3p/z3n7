using System.IO;
using System.Text;
using ZennoLab.InterfacesLibrary.ProjectModel;
using System;

namespace z3n7.Tools
{
    public static class Extractor
    {
        private const string InputSettingsHtmlEntry = "InputSettings/inputSettings.html";
        
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

        public static void SaveAsXml(this IZennoPosterProjectModel project, string xmlPath = null)
        {
            var xml = ExtractXml(project.Path + project.Name);
            if (xmlPath == null)
                xmlPath = Path.Combine(project.Path, project.Name.Replace(".zp",".xml"));
            File.WriteAllText(xmlPath, xml, Encoding.Unicode);
            project.SendInfoToLog($"saved as {xmlPath}");
        }
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

        public static void SaveInputSettingsHtml(string zpPath, string html)
        {
            SaveInputSettingsHtml(zpPath, html, zpPath);
        }

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

    }
    
}
