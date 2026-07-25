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

        public static void SaveAsXml(this IZennoPosterProjectModel project, string xmlPath)
        {
            var xml = ExtractXml(project.Path + project.Name);
            xml =xml.Replace("utf-16","utf-8");
            File.WriteAllText(Path.Combine(xmlPath, project.Name.Replace(".zp",".xml")),xml);
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

        public static void BuildZpFromXml(string xml, string zpPath)
        {
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name != "ProjectMaker") continue;
	            
                var loaderType  = asm.GetType("ZennoLab.TemplateManipulator.V4.ProjectLoaderV4");
                var archiveType = asm.GetType("ZennoLab.TemplateManipulator.V4.ProjectArchiveV4");
	            
                string tmpPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zp");
				        
                byte[] zpBytes = Convert.FromBase64String(ZpToCsx.Template());
                File.WriteAllBytes(tmpPath, zpBytes);
                var archive = Activator.CreateInstance(archiveType, tmpPath);
                var loader  = Activator.CreateInstance(loaderType);
                
                byte[] bytes = (byte[])loaderType.GetMethod("ToByteArray")
                    .Invoke(loader, new object[] { xml });
	        
                archiveType.GetMethod("RemoveEntries")
                    .Invoke(archive, new object[] {(Func<string, bool>)(name => name.EndsWith(".xml"))});
	        
                archiveType.GetMethod("SaveProject")
                    .Invoke(archive, new object[] { "Template.xml", bytes });
		        
                archiveType.GetMethod("SaveToFile")
                    .Invoke(archive, new object[] { zpPath });
	            
                File.Delete(tmpPath);
                break;
            }
        }

    }
    
}
