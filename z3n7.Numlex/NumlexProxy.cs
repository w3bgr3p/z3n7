using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;


namespace z3n7
{
    public sealed class NumlexProxy
    {
        private const string Protocol = "socks5";
        private const string Host = "unlimited-a6f433.resiproxyzone.org";
        private const string Port = "1080";
        private const string LoginPrefix = "M7Z7GICSlq";
        private const string Password = "1O10e0ozyeEQHq3";
        private const int Ttl = 120;
        private const string DefaultRoutesPath =
            @"W:\code_hard\numlex\localhost-provider\routes.json";
        private const string DefaultLocationsPath =
            @"W:\code_hard\numlex\proxy\available_locations.json";
        private const string RoutesPathEnv = "NUMLEX_ROUTES_PATH";
        private const string LocationsPathEnv = "NUMLEX_LOCATIONS_PATH";

        private readonly IZennoPosterProjectModel _project;
        private readonly Instance _instance;
        private readonly ProxyCountry[] _countries;
        private readonly Dictionary<string, ProxyCountry> _countryIndex;
        private readonly Dictionary<string, string> _fallbackIndex;

        private static readonly object RandomLock = new object();
        private static readonly Random Random = new Random();

        public NumlexProxy(
            IZennoPosterProjectModel project,
            Instance instance,
            string locationsPath = DefaultLocationsPath,
            string routesPath = DefaultRoutesPath)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            locationsPath = NumlexEnv.Resolve(project, LocationsPathEnv, locationsPath);
            routesPath = NumlexEnv.Resolve(project, RoutesPathEnv, routesPath);
            _countries = LoadLocations(locationsPath);
            _countryIndex = BuildCountryIndex(_countries);
            _fallbackIndex = BuildRouteProxyFallbacks(routesPath, locationsPath);
        }

        /// <summary>
        /// Установить прокси по ISO-коду или английскому названию страны.
        /// Примеры: PK, Pakistan, MX, Mexico.
        /// </summary>
        public string SetProxy(string country)
        {
            ProxyCountry selected = FindCountry(country);
            string requested = country;
            string fallbackIso = null;

            if (selected == null)
            {
                if (!_fallbackIndex.TryGetValue(Normalize(country), out fallbackIso))
                    throw new Exception("ProxyZone country not found: " + country);

                selected = FindCountry(fallbackIso);

                if (selected == null)
                    throw new Exception(
                        "ProxyZone fallback country not found: " +
                        country +
                        " -> " +
                        fallbackIso);
            }

            return SetProxy(selected, requested, fallbackIso);
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

        
        
        private string SetProxy(
            ProxyCountry country,
            string requestedCountry = null,
            string fallbackIso = null)
        {
            string sid = Rnd.RndString(5);

            string login =
                $"{LoginPrefix}-country-{country.IsoCode}-session-1{sid}-ttl-{Ttl}";

            string proxy =
                $"{Protocol}://{login}:{Password}@{Host}:{Port}";

            _instance.SetProxy(proxy, true, true, true, true);

            _project.Var("proxy", proxy);
            _project.Var("proxy_type", Protocol);
            _project.Var("proxy_address", Host);
            _project.Var("proxy_port", Port);
            _project.Var("proxy_login", login);
            _project.Var("proxy_password", Password);
            _project.Var("proxy_country", country.Name);
            _project.Var("proxy_country_code", country.IsoCode);
            _project.Var("proxy_requested_country", requestedCountry ?? country.IsoCode);
            _project.Var("proxy_effective_location", country.IsoCode);
            _project.Var("proxy_fallback_country_code", fallbackIso ?? "");
            _project.Var("proxy_session", sid);

            return proxy;
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

        public static Dictionary<string, string> BuildRouteProxyFallbacks(
            string routesPath = DefaultRoutesPath,
            string locationsPath = DefaultLocationsPath)
        {
            ProxyCountry[] locations = LoadLocations(locationsPath);
            RouteCountry[] routes = LoadRouteCountries(routesPath);

            var available = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < locations.Length; i++)
                available.Add(locations[i].IsoCode.ToUpperInvariant());

            var result = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < routes.Length; i++)
            {
                RouteCountry route = routes[i];
                string routeIso = route.IsoCode.ToUpperInvariant();

                if (available.Contains(routeIso))
                    continue;

                string fallbackIso = FindNearestAvailableCountry(routeIso, available);

                if (string.IsNullOrEmpty(fallbackIso))
                    continue;

                result[Normalize(route.IsoCode)] = fallbackIso;
                result[Normalize(route.CountryName)] = fallbackIso;

                for (int j = 0; j < route.Aliases.Length; j++)
                    result[Normalize(route.Aliases[j])] = fallbackIso;
            }

            return result;
        }

        private static RouteCountry[] LoadRouteCountries(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("routesPath is empty", nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("routes.json not found", path);

            string json = File.ReadAllText(path);

            var result = new List<RouteCountry>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (Match obj in Regex.Matches(json, @"\{[^{}]*\}", RegexOptions.Singleline))
            {
                string block = obj.Value;
                string countryName = JsonString(block, "country_name");
                var aliases = JsonArray(block, "aliases");

                if (aliases.Count == 0)
                    continue;

                string isoCode = aliases[0].ToUpperInvariant();

                if (!seen.Add(isoCode))
                    continue;

                result.Add(new RouteCountry(countryName, isoCode, aliases.ToArray()));
            }

            return result.ToArray();
        }

        private static string FindNearestAvailableCountry(
            string isoCode,
            HashSet<string> available)
        {
            CountryPoint source;

            if (!CountryCoordinates.TryGetValue(isoCode, out source))
                return null;

            string bestIso = null;
            double bestDistance = double.MaxValue;

            foreach (string candidateIso in available)
            {
                CountryPoint candidate;

                if (!CountryCoordinates.TryGetValue(candidateIso, out candidate))
                    continue;

                double distance = DistanceKm(source, candidate);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestIso = candidateIso;
                }
            }

            return bestIso;
        }

        private static double DistanceKm(CountryPoint a, CountryPoint b)
        {
            const double earthRadiusKm = 6371.0;

            double lat1 = ToRadians(a.Latitude);
            double lat2 = ToRadians(b.Latitude);
            double deltaLat = ToRadians(b.Latitude - a.Latitude);
            double deltaLon = ToRadians(b.Longitude - a.Longitude);

            double h =
                Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            return earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(h), Math.Sqrt(1 - h));
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private static string JsonString(string block, string name)
        {
            var m = Regex.Match(
                block,
                "\"" + Regex.Escape(name) + "\"\\s*:\\s*(\"(?<s>(?:\\\\.|[^\"])*)\"|(?<n>-?\\d+))",
                RegexOptions.Singleline);

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
                RegexOptions.Singleline);

            if (!m.Success)
                return result;

            foreach (Match item in Regex.Matches(m.Groups["items"].Value, "\"(?<s>(?:\\\\.|[^\"])*)\""))
                result.Add(Regex.Unescape(item.Groups["s"].Value));

            return result;
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

        private sealed class RouteCountry
        {
            public readonly string CountryName;
            public readonly string IsoCode;
            public readonly string[] Aliases;

            public RouteCountry(
                string countryName,
                string isoCode,
                string[] aliases)
            {
                CountryName = countryName;
                IsoCode = isoCode;
                Aliases = aliases ?? new string[0];
            }
        }

        private struct CountryPoint
        {
            public readonly double Latitude;
            public readonly double Longitude;

            public CountryPoint(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }
        }

        private static readonly Dictionary<string, CountryPoint> CountryCoordinates =
            new Dictionary<string, CountryPoint>(StringComparer.OrdinalIgnoreCase)
            {
                { "AE", new CountryPoint(23.4241, 53.8478) },
                { "AM", new CountryPoint(40.0691, 45.0382) },
                { "AZ", new CountryPoint(40.1431, 47.5769) },
                { "BD", new CountryPoint(23.6850, 90.3563) },
                { "BG", new CountryPoint(42.7339, 25.4858) },
                { "BJ", new CountryPoint(9.3077, 2.3158) },
                { "BY", new CountryPoint(53.7098, 27.9534) },
                { "EG", new CountryPoint(26.8206, 30.8025) },
                { "ET", new CountryPoint(9.1450, 40.4897) },
                { "GE", new CountryPoint(42.3154, 43.3569) },
                { "HN", new CountryPoint(15.2000, -86.2419) },
                { "ID", new CountryPoint(-0.7893, 113.9213) },
                { "IL", new CountryPoint(31.0461, 34.8516) },
                { "IN", new CountryPoint(20.5937, 78.9629) },
                { "KE", new CountryPoint(-0.0236, 37.9062) },
                { "KG", new CountryPoint(41.2044, 74.7661) },
                { "KZ", new CountryPoint(48.0196, 66.9237) },
                { "MX", new CountryPoint(23.6345, -102.5528) },
                { "NE", new CountryPoint(17.6078, 8.0817) },
                { "NG", new CountryPoint(9.0820, 8.6753) },
                { "OM", new CountryPoint(21.4735, 55.9754) },
                { "PH", new CountryPoint(12.8797, 121.7740) },
                { "PK", new CountryPoint(30.3753, 69.3451) },
                { "QA", new CountryPoint(25.3548, 51.1839) },
                { "RS", new CountryPoint(44.0165, 21.0059) },
                { "RU", new CountryPoint(61.5240, 105.3188) },
                { "SA", new CountryPoint(23.8859, 45.0792) },
                { "SI", new CountryPoint(46.1512, 14.9955) },
                { "TJ", new CountryPoint(38.8610, 71.2761) },
                { "TR", new CountryPoint(38.9637, 35.2433) },
                { "TZ", new CountryPoint(-6.3690, 34.8888) },
                { "UA", new CountryPoint(48.3794, 31.1656) },
                { "UZ", new CountryPoint(41.3775, 64.5853) },
                { "VE", new CountryPoint(6.4238, -66.5897) }
            };
    }
}
