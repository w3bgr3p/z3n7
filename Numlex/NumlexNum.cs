using System;
using System.Collections.Generic;
using System.Linq;
//using System.Management.Instrumentation;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.CommandCenter;

namespace z3nIO.Api
{
    public class NumlexNum
    {
        
        private readonly string _baseUrl;
        
        private readonly IZennoPosterProjectModel _project;
        private readonly Instance _instance;
        private static readonly object RndLock = new object();
        private static readonly Random Rnd = new Random();
        private readonly NumlexRoute[] _routes;


        public NumlexNum(IZennoPosterProjectModel project, Instance instance, string apikey = null, bool GrafanaProxy = true, 
            string routesPath = @"W:\code_hard\numlex\localhost-provider\routes.json")
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _baseUrl = "http://127.0.0.1:18581";
            _routes = LoadRoutes(routesPath);
        }

        /// <summary>
        /// Получить номер по service + country.
        /// Возвращает [id, phone].
        /// Дополнительно сохраняет:
        /// numCountry, numCountryCode, numNoCountryCode.
        /// </summary>
        /// <param name="site">Опциональный идентификатор сайта для мониторинга (не передаётся в Numlex)</param>
        public string[] GetNumber(string service, int country, string site = null)
        {
            if (string.IsNullOrWhiteSpace(service))
                throw new ArgumentException("service is empty", nameof(service));

            site = string.IsNullOrWhiteSpace(site)
                ? _instance.ActiveTab.Domain.Trim()
                : site.Trim();

            var url =
                $"{_baseUrl}/number" +
                $"?country={country}" +
                $"&service={Uri.EscapeDataString(service)}" +
                $"&site={Uri.EscapeDataString(site)}";

            var json = _project.GET(url, log: true, useNetHttp: false, thrw: true);
            
            
            _project.ToJson(json);

            string id = _project.Json.id.ToString();
            string phone = _project.Json.phone.ToString();

            string countryName = _project.Json.country?.ToString() ?? country.ToString();
            string countryCode = _project.Json.country_code?.ToString() ?? "";
            string noCountryCode = _project.Json.number_without_country_code?.ToString() ?? "";

            _project.Var("numlexId", id);
            _project.Var("numlexPhone", phone);
            _project.Var("numlexIssued", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            _project.Var("numCountry", countryName);
            _project.Var("numCountryCode", countryCode);
            _project.Var("numNoCountryCode", noCountryCode);

            return new[] { id, phone };
        }
        
        public string[] GetNumber(int country, string site = null)
        {
            var route = PickRouteByCountryId(country);

            if (route == null)
                throw new Exception($"Numlex country not found in Routes: {country}");

            return GetNumber(route.Service, route.CountryId, site);
        }
        
        public string[] GetNumber(string countryName, string site = null)
        {
            if (string.IsNullOrWhiteSpace(countryName))
                throw new ArgumentException("countryName is empty", nameof(countryName));

            var route = PickRouteByCountryName(countryName);

            if (route == null)
                throw new Exception($"Numlex country not found in Routes: {countryName}");

            return GetNumber(route.Service, route.CountryId, site);
        }
        
        public string[] GetRandomNumber(bool blacklist = false, bool whitelist = false)
        {
            var except = blacklist
                ? _project.GVar("bl_num_" + _instance.ActiveTab.Domain.Replace(".",""))
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim().ToLowerInvariant())
                    .ToHashSet()
                : null;

            var include = whitelist
                ? _project.GVar("wl_num_" + _instance.ActiveTab.Domain.Replace(".",""))
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim().ToLowerInvariant())
                    .ToHashSet()
                : null;

            var allowed = new List<NumlexRoute>();

            for (int i = 0; i < _routes.Length; i++)
            {
                var route = _routes[i];

                // Если включен whitelist — берем только то, что есть в нем
                if (include != null && !MatchesRoute(route, include))
                    continue;

                // Затем исключаем blacklist
                if (except != null && MatchesRoute(route, except))
                    continue;

                allowed.Add(route);
            }

            if (allowed.Count == 0)
                throw new Exception("Numlex: no available countries after whitelist/blacklist filtering.");

            var selected = PickRandom(allowed);

            return GetNumber(selected.Service, selected.CountryId, _instance.ActiveTab.Domain.Trim());
        }

        public void GetNumberByProxy(string provider = null)
        {
            var rnd = _project.Int("varRnd");
            var prx = _project.Var("proxy_location").Trim().ToLower();

            var providers = new Dictionary<string, string[]>
            {
                { "ru", new[] { "oth 0", "mt 0", "drp 0" } },

                { "kg", new[] { "oth 11", "sdek 11" } },
                { "tj", new[] { "oth 143" } },
                { "kz", new[] { "oth 2", "who 2" } },
                { "uz", new[] { "oth 40", "who 40" } },

                { "pk", new[] { "oth 66", "am 66", "aor 66" } },
                { "hn", new[] { "oth 88" } },

                { "bd", new[] { "fb 60", "am 60", "oth 60" } },
                { "tz", new[] { "oth 9" } },

                { "mx", new[] { "oth 54" } },
                { "ph", new[] { "oth 4", "who 4" } },
                { "bg", new[] { "oth 83" } },
                { "rs", new[] { "oth 29" } },
                { "il", new[] { "oth 13" } },
                { "ve", new[] { "oth 70" } },

                { "ae", new[] { "who 95" } },
                { "az", new[] { "who 35" } },
                { "et", new[] { "who 71" } },
                { "id", new[] { "who 6" } },
                { "by", new[] { "who 51" } },
                { "ua", new[] { "who 1" } },
                { "ke", new[] { "who 8" } },
                { "om", new[] { "who 107" } },
                { "am", new[] { "who 148" } },
                { "ge", new[] { "who 128" } },
                { "ne", new[] { "who 139" } },
                { "ng", new[] { "who 19" } },
                { "bj", new[] { "who 120" } },
                { "eg", new[] { "who 21" } },
                { "si", new[] { "who 59" } },
                
            };

            string[] availableProviders;

            if (!providers.TryGetValue(prx, out availableProviders))
                throw new Exception("Provider not found for proxy location: " + prx);
            
            if (string.IsNullOrEmpty(provider))
                provider = availableProviders[Math.Abs(rnd) % availableProviders.Length];

            var parts = provider.Split(' ');

            var p = parts[0];
            var c = int.Parse(parts[1]);

            _project.Var("numProvider", p);
            _project.Var("numDirection", _project.Var("proxy_location")+"-" + p);
            GetNumber(p, c);
        }

        public string GetSms(int deadline = 120)
        {
            var id = _project.Var("numlexId");
            var d  = new Time.Deadline();

            while (true)
            {
                Thread.Sleep(5000);
                d.Check(deadline);

                var url =
                    $"{_baseUrl}/sms/{Uri.EscapeDataString(id)}" +
                    "?once=1";

                var age = _project.Age<string>("numlexIssued");

                var json = _project.GET(url, useNetHttp: true, thrw: true);
                _project.ToJson(json);
                _project.SendInfoToLog($"{age} {_project.Var("numCountry")} {_instance.ActiveTab.Domain} {json}", true);

                string waiting = "";

                try
                {
                    waiting = _project.Json.waiting.ToString();
                }
                catch { }

                if (waiting.Equals("True", StringComparison.OrdinalIgnoreCase) ||
                    waiting.Equals("true", StringComparison.OrdinalIgnoreCase))
                    continue;

                string code = "";

                try
                {
                    code = _project.Json.sms_code.ToString();
                }
                catch { }

                if (!string.IsNullOrEmpty(code))
                    return code;

                throw new Exception($"Localhost provider getSms returned unexpected response: {json}");
            }
        }
        
        private static string OnlyDigits(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            var chars = new List<char>();

            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsDigit(value[i]))
                    chars.Add(value[i]);
            }

            return new string(chars.ToArray());
        }

        private NumlexRoute PickRouteByCountryId(int countryId)
        {
            var list = new List<NumlexRoute>();

            for (int i = 0; i < _routes.Length; i++)
            {
                if (_routes[i].CountryId == countryId)
                    list.Add(_routes[i]);
            }

            return PickRandom(list);
        }

        private NumlexRoute PickRouteByCountryName(string countryName)
        {
            var list = new List<NumlexRoute>();
            string key = NormalizeKey(countryName);

            for (int i = 0; i < _routes.Length; i++)
            {
                if (_routes[i].HasCountryName(key))
                    list.Add(_routes[i]);
            }

            return PickRandom(list);
        }

        private static NumlexRoute PickRandom(List<NumlexRoute> list)
        {
            if (list == null || list.Count == 0)
                return null;

            lock (RndLock)
            {
                return list[Rnd.Next(list.Count)];
            }
        }
        
        private static string NormalizeKey(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            value = value.Trim().ToLowerInvariant();

            var chars = new List<char>();

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                if (c == ' ' || c == '-' || c == '_' || c == '.')
                    continue;

                chars.Add(c);
            }

            return new string(chars.ToArray());
        }

        private class NumlexRoute
        {
            public readonly string CountryName;
            public readonly int CountryId;
            public readonly string Service;
            public readonly string CountryCode;

            private readonly string[] _aliases;

            public NumlexRoute(
                string countryName,
                int countryId,
                string service,
                string countryCode,
                params string[] aliases)
            {
                CountryName = countryName;
                CountryId = countryId;
                Service = service;
                CountryCode = countryCode;

                var list = new List<string>();
                list.Add(countryName);

                if (aliases != null)
                {
                    for (int i = 0; i < aliases.Length; i++)
                        list.Add(aliases[i]);
                }

                _aliases = list.ToArray();
            }

            public bool HasCountryName(string normalizedName)
            {
                for (int i = 0; i < _aliases.Length; i++)
                {
                    if (NormalizeKey(_aliases[i]) == normalizedName)
                        return true;
                }

                return false;
            }
        }

        private class PhoneSplit
        {
            public readonly string CountryCode;
            public readonly string NoCountryCode;

            public PhoneSplit(string countryCode, string noCountryCode)
            {
                CountryCode = countryCode;
                NoCountryCode = noCountryCode;
            }
        }
        private static bool MatchesRoute(NumlexRoute route, HashSet<string> bad)
        {
            if (route == null || bad == null || bad.Count == 0)
                return false;

            if (bad.Contains(NormalizeKey(route.CountryName)))
                return true;

            if (bad.Contains(route.CountryId.ToString()))
                return true;

            if (bad.Contains(OnlyDigits(route.CountryCode)))
                return true;

            if (bad.Contains("+" + OnlyDigits(route.CountryCode)))
                return true;

            return false;
        }
        
        private static NumlexRoute[] LoadRoutes(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("routesPath is empty", nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("routes.json not found", path);

            string json = File.ReadAllText(path);

            var result = new List<NumlexRoute>();

            foreach (Match obj in Regex.Matches(json, @"\{[^{}]*\}", RegexOptions.Singleline))
            {
                string block = obj.Value;

                string countryName = JsonString(block, "country_name");
                int countryId = int.Parse(JsonString(block, "country_id"));
                string service = JsonString(block, "service");
                string countryCode = JsonString(block, "country_code");

                var aliases = JsonArray(block, "aliases");

                result.Add(new NumlexRoute(
                    countryName,
                    countryId,
                    service,
                    countryCode,
                    aliases.ToArray()
                ));
            }

            if (result.Count == 0)
                throw new Exception("routes.json has no routes");

            return result.ToArray();
        }

        private static string JsonString(string block, string name)
        {
            var m = Regex.Match(
                block,
                "\"" + Regex.Escape(name) + "\"\\s*:\\s*(\"(?<s>(?:\\\\.|[^\"])*)\"|(?<n>-?\\d+))",
                RegexOptions.Singleline
            );

            if (!m.Success)
                throw new Exception("routes.json field not found: " + name);

            if (m.Groups["n"].Success)
                return m.Groups["n"].Value;

            return Regex.Unescape(m.Groups["s"].Value);
        }

        private static List<string> JsonArray(string block, string name)
        {
            var result = new List<string>();

            var m = Regex.Match(
                block,
                "\"" + Regex.Escape(name) + "\"\\s*:\\s*\\[(?<items>.*?)\\]",
                RegexOptions.Singleline
            );

            if (!m.Success)
                return result;

            foreach (Match item in Regex.Matches(m.Groups["items"].Value, "\"(?<s>(?:\\\\.|[^\"])*)\""))
                result.Add(Regex.Unescape(item.Groups["s"].Value));

            return result;
        }
    }
}
