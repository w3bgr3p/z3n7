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
    public sealed class NumlexJsonRoute
    {
        public string Site { get; internal set; }
        public string ProxyLocation { get; internal set; }
        public string NumberLocation { get; internal set; }
        public string NumberProvider { get; internal set; }
    }

    public static class NumDb
    {
        private static readonly object JsonRandomLock = new object();
        private static readonly Random JsonRandom = new Random();

        public static NumlexJsonRoute FromJson(IZennoPosterProjectModel project)
        {
            
            var path = project.ReadEnv("NUMLEX_SITES_JSON");
            if (string.IsNullOrWhiteSpace(path))
                throw new Exception("NUMLEX_SITES_JSON is empty");
            if (!File.Exists(path))
                throw new FileNotFoundException("Numlex instance config not found", path);

            var configs = JsonConvert.DeserializeObject<List<NumlexJsonConfig>>(File.ReadAllText(path));
            if (configs == null || configs.Count == 0)
                throw new Exception("Numlex instance config has no sites");

            var requestedSite = GetSiteSafeName(project.Var("url"));
            var config = configs.FirstOrDefault(x =>
                !string.IsNullOrWhiteSpace(x.Site) &&
                GetSiteSafeName(x.Site) == requestedSite);

            if (config == null)
                throw new Exception($"Numlex instance config has no site: {project.Var("url")}");

            var numberLocations = (config.NumberLocations ?? new List<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().ToUpperInvariant())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var numberProviders = (config.NumberProviders ?? new Dictionary<string, List<string>>())
                .ToDictionary(
                    x => x.Key.Trim().ToUpperInvariant(),
                    x => (x.Value ?? new List<string>())
                        .Where(provider => !string.IsNullOrWhiteSpace(provider))
                        .Select(provider => provider.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList(),
                    StringComparer.OrdinalIgnoreCase);

            var availableNumberRoutes = new List<KeyValuePair<string, string>>();
            foreach (var numberLocation in numberLocations)
            {
                if (!numberProviders.TryGetValue(numberLocation, out var providers) || providers.Count == 0)
                    throw new Exception($"Numlex instance config has no providers for {numberLocation}: {config.Site}");

                foreach (var provider in providers)
                {
                    var direction = $"{requestedSite}-{numberLocation}-{provider}".Replace("'", "''");
                    var activeCooldownWhere =
                        $@"""direction"" = '{direction}' " +
                        "AND NULLIF(\"limit\", '')::timestamptz > NOW()";

                    var cooldownActive = project
                        .DbGetLines("direction", where: activeCooldownWhere)
                        .Any(x => !string.IsNullOrWhiteSpace(x));

                    if (!cooldownActive)
                        availableNumberRoutes.Add(new KeyValuePair<string, string>(numberLocation, provider));
                }
            }

            if (availableNumberRoutes.Count == 0)
                throw new Exception($"Cooldown active for all number routes: {config.Site}");

            var proxyLocations = (config.ProxyLocations ?? new List<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToList();

            lock (JsonRandomLock)
            {
                var numberRoute = availableNumberRoutes[JsonRandom.Next(availableNumberRoutes.Count)];

                return new NumlexJsonRoute
                {
                    Site = config.Site,
                    NumberLocation = numberRoute.Key,
                    ProxyLocation = proxyLocations.Count == 0
                        ? numberRoute.Key
                        : proxyLocations[JsonRandom.Next(proxyLocations.Count)],
                    NumberProvider = numberRoute.Value
                };
            }
        }

        private sealed class NumlexJsonConfig
        {
            [JsonProperty("site")]
            public string Site { get; set; }

            [JsonProperty("proxy_locations")]
            public List<string> ProxyLocations { get; set; }

            [JsonProperty("number_locations")]
            public List<string> NumberLocations { get; set; }

            [JsonProperty("number_providers")]
            public Dictionary<string, List<string>> NumberProviders { get; set; }
        }
        
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
        public static string GetSiteSafeName_(string url)
        {
            if (!url.Contains("://"))
                url = "https://" + url;

            string host = new Uri(url).Host;

            if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                host = host.Substring(4);

            var safeName = "nmlx-" + host.Replace(".", "_").ToLower();
            return safeName;
            
        }
        
        public static string GetSiteSafeName(string url)
        {
            if (!url.Contains("://"))
                url = "https://" + url;

            string host = new Uri(url).Host;

            if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                host = host.Substring(4);

            var safeName = "nmlx-" + host.Replace(".", "_").ToLower();
            return host;
            
        }
        
        public static bool CanTakeNumber(IZennoPosterProjectModel project, string country, string provider)
            {
                var site = GetSiteSafeName(project.Var("url"));
                var direction = $"{site}-{country}-{provider}"
                    .Replace("'", "''");

                var where = $@"
            ""direction"" = '{direction}'
            AND (
                NULLIF(""limit"", '') IS NULL
                OR ""limit""::timestamptz <= NOW()
            )";

            return project
                .DbGetLines(
                    "direction",
                    project.Var("projectTable"),
                    where: where)
                .Any(x => !string.IsNullOrWhiteSpace(x));
        }

    }
}
