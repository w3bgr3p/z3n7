using System;
using System.IO;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    public static class Env
    {
        private const string EnvFileName = ".env";

        public static string ReadEnv(this IZennoPosterProjectModel project, string key, bool global =  false)
        {
            
            string envPath = (global) ? 
                Path.Combine(Path.GetDirectoryName( typeof(Env).Assembly.Location), EnvFileName): 
                Path.Combine(project.Path, EnvFileName);

            if (!File.Exists(envPath))
                return null;

            foreach (string rawLine in File.ReadAllLines(envPath))
            {
                string line = rawLine.Trim();

                if (line.Length == 0 || line.StartsWith("#"))
                    continue;

                int eq = line.IndexOf('=');

                if (eq <= 0)
                    continue;

                string name = line.Substring(0, eq).Trim();

                if (!string.Equals(name, key, StringComparison.OrdinalIgnoreCase))
                    continue;

                string value = line.Substring(eq + 1).Trim();

                if (value.Length >= 2 &&
                    ((value[0] == '"' && value[value.Length - 1] == '"') ||
                     (value[0] == '\'' && value[value.Length - 1] == '\'')))
                {
                    value = value.Substring(1, value.Length - 2);
                }

                return value;
            }

            return null;
        }
        
    }
}