using System;
using System.Collections.Generic;

using System.IO;

using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;


namespace z3n7
{
    public sealed class HttpProxy
    {
        private const string Protocol = "http";
        private const string ProviderBaseUrl = "http://127.0.0.1:18581";
        private const int Ttl = 120;
        private const string DefaultLocationsPath =
            @"W:\code_hard\numlex\proxy\available_locations.json";

        private readonly IZennoPosterProjectModel _project;
        private readonly Instance _instance;
        private readonly ProxyCountry[] _countries;
        private readonly Dictionary<string, ProxyCountry> _countryIndex;

        private static readonly object RandomLock = new object();
        private static readonly Random Random = new Random();

        public HttpProxy(
            IZennoPosterProjectModel project,
            Instance instance,
            string locationsPath = DefaultLocationsPath)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            locationsPath = locationsPath = project.ReadEnv("NUMLEX_LOCATIONS_PATH");
            _countries = LoadLocations(locationsPath);
            _countryIndex = BuildCountryIndex(_countries);
        }

        /// <summary>
        /// Установить прокси по ISO-коду или английскому названию страны.
        /// Примеры: PK, Pakistan, MX, Mexico.
        /// </summary>
        public string SetProxy(string country)
        {
            ProxyCountry selected = FindCountry(country);

            if (selected == null)
                throw new Exception("ProxyZone country not found: " + country);

            return SetProxy(selected);
        }

        public bool ReleaseProxy()
        {
            string leaseId = _project.Var("proxy_lease_id");

            if (string.IsNullOrWhiteSpace(leaseId))
                return true;

            string body = JsonConvert.SerializeObject(new
            {
                proxyLeaseId = leaseId
            });

            string response = _project.POST(
                ProviderBaseUrl + "/proxy/release",
                body,
                deadline: 15,
                thrw: true);

            JObject root = JObject.Parse(response);
            bool released = root["released"] == null ||
                            root["released"].Value<bool>();

            if (released)
                _project.Var("proxy_lease_id", string.Empty);

            return released;
        }

        /// <summary>
        /// Выбрать случайную страну.
        ///
        /// blacklist=true:
        /// читает глобальную переменную bl + текущий домен.
        ///
        /// whitelist=true:
        /// читает глобальную переменную wl + текущий домен.
        ///
        /// В фильтрах можно указывать ISO-коды или английские названия:
        /// PK
        /// Pakistan
        /// MX
        /// Mexico
        /// </summary>
        public string SetRandomProxy(
            bool blacklist = false,
            bool whitelist = false)
        {
            HashSet<string> blocked = blacklist
                ? ReadFilter("bl_proxy_" + _instance.ActiveTab.Domain)
                : null;

            HashSet<string> included = whitelist
                ? ReadFilter("wl_proxy_" + _instance.ActiveTab.Domain)
                : null;

            var allowed = new List<ProxyCountry>();

            for (int i = 0; i < _countries.Length; i++)
            {
                ProxyCountry country = _countries[i];

                if (included != null &&
                    !MatchesFilter(country, included))
                {
                    continue;
                }

                if (blocked != null &&
                    MatchesFilter(country, blocked))
                {
                    continue;
                }

                allowed.Add(country);
            }

            if (allowed.Count == 0)
            {
                throw new Exception(
                    "ProxyZone: no countries available after whitelist/blacklist filtering.");
            }

            ProxyCountry selected;

            lock (RandomLock)
            {
                selected = allowed[Random.Next(allowed.Count)];
            }

            return SetProxy(selected);
        }

        
        
        private string SetProxy(ProxyCountry country)
        {
            string threadId = GetThreadId();
            string accountId = _project.Var("acc0");
            string body = JsonConvert.SerializeObject(new
            {
                country = country.IsoCode,
                ttl = Ttl,
                threadId,
                accountId
            });

            string response = _project.POST(
                ProviderBaseUrl + "/proxy/lease",
                body,
                deadline: 30,
                thrw: true);

            JObject root = JObject.Parse(response);
            string proxy = root["proxy"]?.ToString();
            string leaseId = root["proxyLeaseId"]?.ToString();

            if (string.IsNullOrWhiteSpace(proxy))
                throw new Exception("Proxy gateway response has empty proxy: " + response);

            if (string.IsNullOrWhiteSpace(leaseId))
                throw new Exception("Proxy gateway response has empty proxyLeaseId: " + response);

            var proxyUri = new Uri(proxy);

            _instance.SetProxy(proxy, true, true, true, true);

            _project.Var("proxy", proxy);
            _project.Var("proxy_type", Protocol);
            _project.Var("proxy_address", proxyUri.Host);
            _project.Var("proxy_port", proxyUri.Port.ToString());
            _project.Var("proxy_login", string.Empty);
            _project.Var("proxy_password", string.Empty);
            _project.Var("proxy_country", root["country"]?.ToString() ?? country.Name);
            _project.Var("proxy_country_code", root["countryCode"]?.ToString() ?? country.IsoCode);
            _project.Var("proxy_session", leaseId);
            _project.Var("proxy_lease_id", leaseId);
            _project.Var("proxy_expires_unix_ms", root["expiresUnixMs"]?.ToString() ?? string.Empty);
            _project.Var("proxy_thread_id", threadId);
            _project.Var("proxy_account_id", accountId);

            return proxy;
        }

        private static string GetThreadId()
        {
            return "zenno-" + Thread.CurrentThread.ManagedThreadId;
        }

        private ProxyCountry FindCountry(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            ProxyCountry country;

            return _countryIndex.TryGetValue(
                Normalize(value),
                out country)
                ? country
                : null;
        }

        private HashSet<string> ReadFilter(string variableName)
        {
            var result = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            string raw;

            try
            {
                raw = _project.GVar(variableName);
            }
            catch
            {
                return result;
            }

            if (string.IsNullOrWhiteSpace(raw))
                return result;

            string[] items = raw.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < items.Length; i++)
            {
                string normalized = Normalize(items[i]);

                if (!string.IsNullOrEmpty(normalized))
                    result.Add(normalized);
            }

            return result;
        }

        private static bool MatchesFilter(
            ProxyCountry country,
            HashSet<string> filter)
        {
            if (country == null ||
                filter == null ||
                filter.Count == 0)
            {
                return false;
            }

            return
                filter.Contains(Normalize(country.IsoCode)) ||
                filter.Contains(Normalize(country.Name));
        }

        private static Dictionary<string, ProxyCountry>
            BuildCountryIndex(ProxyCountry[] countries)
        {
            var index = new Dictionary<string, ProxyCountry>(
                StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < countries.Length; i++)
            {
                ProxyCountry country = countries[i];

                index[Normalize(country.IsoCode)] = country;
                index[Normalize(country.Name)] = country;
            }

            AddAlias(index, "Russian Federation", "RU");
            AddAlias(index, "United States", "US");
            AddAlias(index, "United States of America", "US");
            AddAlias(index, "America", "US");
            AddAlias(index, "United Arab Emirates", "AE");

            return index;
        }

        private static void AddAlias(
            Dictionary<string, ProxyCountry> index,
            string alias,
            string isoCode)
        {
            ProxyCountry country;

            if (index.TryGetValue(
                    Normalize(isoCode),
                    out country))
            {
                index[Normalize(alias)] = country;
            }
        }

        private static ProxyCountry[] LoadLocations(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("locationsPath is empty", nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("available_locations.json not found", path);

            string json = File.ReadAllText(path);

            var result = new List<ProxyCountry>();

            foreach (Match item in Regex.Matches(
                json,
                "\"(?<iso>[A-Z]{2})\"\\s*:\\s*\"(?<name>(?:\\\\.|[^\"])*)\"",
                RegexOptions.Singleline))
            {
                result.Add(new ProxyCountry(
                    item.Groups["iso"].Value,
                    Regex.Unescape(item.Groups["name"].Value)));
            }

            if (result.Count == 0)
                throw new Exception("available_locations.json has no locations");

            return result.ToArray();
        }

        private static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            value = value.Trim().ToLowerInvariant();

            var chars = new List<char>(value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                if (char.IsWhiteSpace(c) ||
                    c == '-' ||
                    c == '_' ||
                    c == '.')
                {
                    continue;
                }

                chars.Add(c);
            }

            return new string(chars.ToArray());
        }

        private sealed class ProxyCountry
        {
            public readonly string IsoCode;
            public readonly string Name;

            public ProxyCountry(
                string isoCode,
                string name)
            {
                IsoCode = isoCode;
                Name = name;
            }
        }
    }
}
