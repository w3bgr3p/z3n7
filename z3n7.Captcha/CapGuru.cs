using System;
using System.IO;
using System.Threading;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Captcha
{
    public static partial class CaptchaExtensions
    {
        private static readonly object LockObject = new object();
        public static bool CapGuru(this IZennoPosterProjectModel project)
        {
            string key = project.ReadEnv("CAPGURU_KEY");
            project.Context["capguru_key"] = key;
            
            var extHashFile = project.ReadEnv("CAPGURU_HASH_FILE");
            if (!File.Exists(extHashFile)) 
                throw new FileNotFoundException("CapGuru.txt file not found", extHashFile);
            
            byte[] fileBytes = Convert.FromBase64String(File.ReadAllText(extHashFile));

            string tempFilePath = Path.Combine(Path.GetTempPath(), "Cap.Guru.24.zp");

            lock (LockObject) 
            {
                File.WriteAllBytes(tempFilePath, fileBytes);
                bool res = project.ExecuteProject(tempFilePath, null, true, true, true);
                if (File.Exists(tempFilePath)) { File.Delete(tempFilePath); }
                return res;
            }

        }
        
    }
}
