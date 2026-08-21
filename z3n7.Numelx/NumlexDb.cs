using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ZennoLab.InterfacesLibrary.Enums.Log;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    public static class NumlexDb
    {
        
        
        public static void CreateTable(IZennoPosterProjectModel project, string site = null)
        {
            var envDbSource = project.ReadEnv("NUMLEX_DB");
            project.Var("dbSource", envDbSource);

            var jsonPath = project.ReadEnv("NUMLEX_ROUTES_PATH");
            
            var routes = JsonConvert.DeserializeObject<List<JObject>>(File.ReadAllText(jsonPath));
            var directions = routes
                .SelectMany(route =>
                {
                    var service = route["service"]?.ToString()?.Trim();

                    return (route["aliases"]?.Values<string>() ?? Enumerable.Empty<string>())
                        .Where(alias => !string.IsNullOrWhiteSpace(alias))
                        .Select(alias => $"{alias.Trim()}-{service}");
                })
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (string.IsNullOrEmpty(site) && project.Var("site") == "")
            {
                project.Var("site", project.Name.Split('.')[1]);
            }

            var table = !string.IsNullOrEmpty(site) ? site : project.Var("site");
            
            
            var tableName = $"numlex_sites.{table}";
            project.Var("projectTable", tableName);
            var tableStructure = new Dictionary<string, string>
            {
                { "id",               "INTEGER PRIMARY KEY AUTOINCREMENT" },
                { "direction",        "TEXT NOT NULL UNIQUE" },
                { "sent",             "INTEGER DEFAULT 0" },
                { "success",          "INTEGER DEFAULT 0" },
                { "failed",           "INTEGER DEFAULT 0" },
                { "skip",             "BOOLEAN DEFAULT False" },
                { "captcha_type",     "TEXT DEFAULT ''" },
                { "error",            "TEXT DEFAULT ''" },
                { "limit",            "TEXT DEFAULT ''" },
            };

            project.PrepareProjectTable(tableStructure, tableName);

            foreach (var direction in directions)
            {
                project.DbQ($@"INSERT INTO {tableName} (""direction"", ""success"", ""failed"") VALUES ('{direction.Replace("'", "''")}', 0, 0) ON CONFLICT (""direction"") DO NOTHING;");
            }
        }

        public static void IncreaseSuccess(IZennoPosterProjectModel project)
        {
            var q =
                $@"UPDATE {project.Var("projectTable")} SET ""success"" = ""success"" + 1 WHERE ""direction"" = '{project.Var("numDirection")}';";
            project.DbQ( q);
            project.SendToLog(q,LogType.Info, true, LogColor.Blue );

        }
        public static void IncreaseFailed(IZennoPosterProjectModel project)
        {
            var q =
                $@"UPDATE {project.Var("projectTable")} SET ""failed"" = ""failed"" + 1 WHERE ""direction"" = '{project.Var("numDirection")}';";
            project.DbQ(q);
            project.SendToLog(q,LogType.Info, true, LogColor.Orange );
        }

        public static void Increase(IZennoPosterProjectModel project, string column)
        {
            var q =
                $@"UPDATE {project.Var("projectTable")} SET ""{column}"" = ""{column}"" + 1 WHERE ""direction"" = '{project.Var("numDirection")}';";
            project.DbQ(q, true);
        }
        
        public static void ErrToDb(IZennoPosterProjectModel project)
        {
            var err = project.Var("err").Replace("'", "");
            project.DbUpd($"error = '{project.Var("err")}'", log:true, where:$"direction = '{project.Var("numDirection")}'"  );
        }

        public static void AddDir(IZennoPosterProjectModel project )
        {
            var site = GetSiteSafeName(project.Var("url"));
            var direction = site + "-" + project.Var("numDirection");
            project.DbQ($@"INSERT INTO {project.Var("projectTable")} (""direction"", ""success"", ""failed"") VALUES ('{direction.Replace("'", "''")}', 0, 0) ON CONFLICT (""direction"") DO NOTHING;");
        }
        
        public static void AdOnDir(IZennoPosterProjectModel project, string column )
        {
            var site = GetSiteSafeName(project.Var("url"));
            var direction = site + "-" + project.Var("numDirection");
            
            if (column == "error")
            {
                var err = project.Var("err").Replace("'", "");
                project.DbUpd($"error = '{project.Var("err")}'", project.Var("projectTable"), log:true, where:$"direction = '{direction}'"  );
                return;
            }
            if (column == "limit")
            {
                var limit  = Time.Cd("nextH");
                project.DbUpd($"limit = '{limit}'", project.Var("projectTable"), log:true, where:$"direction = '{direction}'"  );
                return;
            }
            
            var q = $@"UPDATE {project.Var("projectTable")} SET ""{column}"" = ""{column}"" + 1 WHERE ""direction"" = '{direction}';";
            project.DbQ(q, true);
        }
        public static string GetSiteSafeName(string url)
        {
            if (!url.Contains("://"))
                url = "https://" + url;

            string host = new Uri(url).Host;

            if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                host = host.Substring(4);

            var safeName = "nmlx" + host.Replace(".", "_").ToLower();
            return safeName;
            
        }
        

    }
}
