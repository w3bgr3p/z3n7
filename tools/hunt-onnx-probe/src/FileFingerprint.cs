using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace HuntOnnxProbe
{
    internal static class FileFingerprint
    {
        public static string Sha256(string path)
        {
            using (var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete))
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "");
        }

        public static string DescribeAssembly(Assembly assembly)
        {
            return assembly.FullName + " | " + assembly.Location +
                " | sha256=" + Sha256(assembly.Location);
        }

        public static string FindLoadedOnnxRuntime()
        {
            try
            {
                var module = Process.GetCurrentProcess().Modules
                    .Cast<ProcessModule>()
                    .FirstOrDefault(item => string.Equals(
                        item.ModuleName,
                        "onnxruntime.dll",
                        StringComparison.OrdinalIgnoreCase));
                return module == null ? null : module.FileName;
            }
            catch
            {
                return null;
            }
        }
    }
}
