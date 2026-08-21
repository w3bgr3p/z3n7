using System;
using System.IO;
using System.Threading;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    public class NumlexCaptcha
    {
        
        
        
        
        public static object LockObject = new object();

        public static bool SolveWithCapGuru(IZennoPosterProjectModel _project, string key = "b0a9dced568bc5467b80a02fbc00fc53")
        {
            _project.Context["capguru_key"] = key;
            var extHashFile = Path.Combine(_project.Path,".internal", "CapGuru.txt");
            if (!File.Exists(extHashFile)) 
                throw new FileNotFoundException("CapGuru.txt file not found", extHashFile);

            byte[] fileBytes = Convert.FromBase64String(File.ReadAllText(extHashFile));

            string tempFilePath = Path.Combine(Path.GetTempPath(), "Cap.Guru.24.zp");

            lock (LockObject) 
            {
                File.WriteAllBytes(tempFilePath, fileBytes);
                bool res = _project.ExecuteProject(tempFilePath, null, true, true, true);
                if (File.Exists(tempFilePath)) { File.Delete(tempFilePath); }
                return res;
            }
        }
        
    }
}