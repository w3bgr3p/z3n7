using System;
using System.Collections.Generic;
using System.Linq;
//using System.Management.Instrumentation;
using System.IO;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading;

using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.Enums.Log;

namespace z3n7.Api
{
    public class NumlexNum
    {
        
        private readonly string _baseUrl;
        private readonly string _routesPath;
        private readonly IZennoPosterProjectModel _project;
        private static readonly object RndLock = new object();
        private static readonly Random Rnd = new Random();
        private readonly NumlexRoute[] _routes;
        private readonly string _site;
        
        public NumlexNum(IZennoPosterProjectModel project, Instance instance = null, string apikey = null, bool GrafanaProxy = true)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _site = project.Var("site");

            _baseUrl = project.ReadEnv("NUMLEX_PROVIDER_BASE_URL"); 
            _routesPath = project.ReadEnv("NUMLEX_ROUTES_PATH"); 

            _routes = LoadRoutes(_routesPath);
        }

        /// <summary>
        /// Получить номер по service + country.
        /// Возвращает [id, phone].
        /// Дополнительно сохраняет:
        /// numCountry, numCountryCode, numNoCountryCode.
        /// </summary>
        /// <param name="site">Опциональный идентификатор сайта для мониторинга (не передаётся в z3n7.Nmlx)</param>
        public string[] GetNumber(string service, int country)
        {
            if (string.IsNullOrWhiteSpace(service))
                throw new ArgumentException("service is empty", nameof(service));
    

            var url = $"{_baseUrl}/number" + $"?country={country}" + $"&service={Uri.EscapeDataString(service)}" + $"&site={Uri.EscapeDataString(_site)}";
            var countryAlias = _routes.FirstOrDefault(route => route.CountryId == country)?.CountryIso;
            if (string.IsNullOrWhiteSpace(countryAlias))
                throw new Exception($"z3n7.Nmlx country alias not found in Routes: {country}");

            _project.Var("numDirection", countryAlias + "-" + service);
            var retry = 10;
            string error = null;
            while (retry-- > 0)
            {
                
                var json = _project.GET(url, log: false, thrw: true);
                var data = Newtonsoft.Json.Linq.JObject.Parse(json);
                bool success = !json.Contains("err_message");

                if (success)
                {
                    string id = data["id"].ToString();
                    string phone = data["phone"].ToString();

                    string countryName = data["country"]?.ToString() ?? country.ToString();
                    string countryCode = data["country_code"]?.ToString() ?? "";
                    string noCountryCode = data["number_without_country_code"]?.ToString() ?? "";

                    _project.Var("numId", id);
                    _project.Var("numPhone", phone);
                    _project.Var("numIssued", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
                    _project.Var("numCountry", countryName);
                    _project.Var("numCountryCode", countryCode);
                    _project.Var("numNoCountryCode", noCountryCode);
                    _project.Var("numProvider", service);
                    _project.SendInfoToLog($"using {_project.Var("numDirection")}", true);
                    return new[] { id, phone };
                }
                
                error = data["err_message"].ToString();
                Thread.Sleep(3000);
            }
            _project.SendWarningToLog(error + $" on {_project.Var("numDirection")}", true);
            _project.Var("err", error);
            throw new Exception(error);
            
        }
        
        public string[] GetNumber(string service, string countryName)
        {
            if (string.IsNullOrWhiteSpace(service))
                throw new ArgumentException("service is empty", nameof(service));

            if (string.IsNullOrWhiteSpace(countryName))
                throw new ArgumentException("countryName is empty", nameof(countryName));

            var countryKey = NormalizeKey(countryName);

            var route = _routes.FirstOrDefault(x =>
                x.Service.Equals(service, StringComparison.OrdinalIgnoreCase) &&
                x.HasCountryName(countryKey));

            if (route == null)
                throw new Exception(
                    $"z3n7.Nmlx route not found: {countryName}-{service}");

            return GetNumber(route.Service, route.CountryId);
        }
        
        public string[] GetNumber(int country)
        {
            var route = PickRouteByCountryId(country);

            if (route == null)
                throw new Exception($"z3n7.Nmlx country not found in Routes: {country}");

            return GetNumber(route.Service, route.CountryId);
        }
        
        public string[] GetNumber(string countryName)
        {
            if (string.IsNullOrWhiteSpace(countryName))
                throw new ArgumentException("countryName is empty", nameof(countryName));

            if (countryName.Contains(","))
            {
                var countries =  countryName.Split(',');
                countryName = countries[Rnd.Next(countries.Length)].Trim();
            }
            
            var route =  PickRouteByCountryName(countryName);

            if (route == null)
                throw new Exception($"z3n7.Nmlx country not found in Routes: {countryName}");

            return GetNumber(route.Service, route.CountryId);
        }
        

        public void GetNumberByProxy(string provider = null)
        {
            var rnd = _project.Int("varRnd");
            var prx = NormalizeKey(_project.Var("proxy_location"));
            var availableRoutes = new List<NumlexRoute>();

            for (int i = 0; i < _routes.Length; i++)
            {
                if (NormalizeKey(_routes[i].CountryIso) == prx)
                    availableRoutes.Add(_routes[i]);
            }

            if (availableRoutes.Count == 0)
                throw new Exception("Provider not found for proxy location: " + prx);

            var route = availableRoutes[Math.Abs(rnd) % availableRoutes.Count];

            var p = (string.IsNullOrWhiteSpace(provider)) ? route.Service : provider;
            var c = route.CountryId;
            
            GetNumber(p, c);
        }

        public string GetSms(int deadline = 180, string previous = null)
        {
            var id = _project.Var("numId");
            var d  = new Time.Deadline();

            while (true)
            {
                Thread.Sleep(5000);
                d.Check(deadline);

                var url = $"{_baseUrl}/sms/{Uri.EscapeDataString(id)}" + "?once=1";

                var age = _project.Age<string>("numIssued");

                var json = _project.GET(url, thrw: true);
                var data = Newtonsoft.Json.Linq.JObject.Parse(json);
                bool success = !json.Contains("err_message");
                
                bool received = json.Contains("sms_code");

                if (received)
                {
                    var smsCode = data["sms_code"].ToString();
                    _project.SendToLog($" received {smsCode} for {_site} on {_project.Var("numDirection")}. Took {age.Substring(3,5)}m", LogType.Info,  true , LogColor.Blue);

                    if (!string.IsNullOrWhiteSpace(previous) && smsCode == previous)
                        continue;

                    return smsCode;
                }
                
                if (success)
                {
                    _project.SendInfoToLog($" waiting sms for {_site} on {_project.Var("numDirection")}. elapsed {age.Substring(3,5)}", true);
                    continue;
                }

                
                string error = data["err_message"].ToString();
                _project.warn(error);
                throw new Exception(error);
                
                
            }
        }

        public void SmsSent()
        {
            var id = _project.Var("numId");
            var url = $"{_baseUrl}/sms/{Uri.EscapeDataString(id)}/sent";
            var json = _project.POST(url, "");
        }

        public long Elapsed()
        {
            long elapsedSeconds =
                (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - long.Parse(_project.Var("numIssued"))) / 1000;
            return elapsedSeconds;
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
            public readonly string CountryIso;

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
                CountryIso = FindCountryIso(_aliases);
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

            private static string FindCountryIso(string[] aliases)
            {
                if (aliases == null)
                    return "";

                for (int i = 0; i < aliases.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(aliases[i]) &&
                        aliases[i].Trim().Length == 2)
                    {
                        return aliases[i].Trim().ToUpperInvariant();
                    }
                }

                return "";
            }
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
