using System.IO;
using ZennoLab.InterfacesLibrary.ProjectModel;
namespace z3n7.Tools
{
    public static class Extractor
    {
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

    }
    
}