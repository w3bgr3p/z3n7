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
        public static void CreateTable(IZennoPosterProjectModel project)
        {
            var jsonPath = @"W:\code_hard\numlex\localhost-provider\routes.json";
            var routes = JsonConvert.DeserializeObject<List<JObject>>(
                File.ReadAllText(jsonPath)
            );
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

            var tableName = $"numlex_sites.{project.Name.ToLower().Split('.')[1]}";
            project.Var("projectTable", tableName);
            var tableStructure = new Dictionary<string, string>
            {
                { "id",               "INTEGER PRIMARY KEY AUTOINCREMENT" },
                { "direction",        "TEXT NOT NULL UNIQUE" },
                { "captcha_type",     "TEXT DEFAULT ''" },
                { "success",          "INTEGER DEFAULT 0" },
                { "failed",           "INTEGER DEFAULT 0" },
                { "rate",             "INTEGER DEFAULT 0" },
                { "error",            "TEXT DEFAULT ''" },
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
    }
}