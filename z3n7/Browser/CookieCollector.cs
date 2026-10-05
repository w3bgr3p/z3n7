using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace z3n7
{
    /// <summary>
    /// Collects cookies by visiting sites over plain HTTP (not the browser), starting from an existing
    /// cookie set, and returns them as browser-extension style JSON.
    /// </summary>
    public class CookieCollector
    {
        private readonly Random _random = new Random();
        private readonly Dictionary<string, CookieDates> _cookieDates =
            new Dictionary<string, CookieDates>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Timeout of each request, seconds.</summary>
        public int TimeoutSeconds { get; set; } = 25;
        /// <summary>Follow redirects (up to 10).</summary>
        public bool AllowRedirects { get; set; } = true;
        /// <summary>Lower bound of the made-up age of new cookies, days.</summary>
        public int MinCookieAgeDays { get; set; } = 7;
        /// <summary>Upper bound of the made-up age of new cookies, days.</summary>
        public int MaxCookieAgeDays { get; set; } = 120;
        /// <summary>Made-up last-access dates of new cookies fall within this many days before now.</summary>
        public int MaxLastAccessAgeDays { get; set; } = 14;

        /// <summary>User-Agent header of the requests.</summary>
        public string UserAgent { get; set; } =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/126.0 Safari/537.36";

        /// <summary>Accept header of the requests.</summary>
        public string Accept { get; set; } =
            "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

        /// <summary>Accept-Language header of the requests.</summary>
        public string AcceptLanguage { get; set; } =
            "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7";

        /// <summary>Receives <c>COOKIES={count}</c> at the end of <c>Run</c>; <c>null</c> for none.</summary>
        public Action<string> Log { get; set; }

        /// <summary>
        /// Loads <c>cookiesJson</c>, sends a GET to each service and saves every cookie of the involved
        /// domains.
        /// A service that times out or fails is skipped. Cookies without <c>creationDate</c> get random
        /// creation and last-access dates within the configured ages.
        /// </summary>
        /// <param name="services">URLs or host names; <c>https://</c> is added when missing.</param>
        /// <param name="cookiesJson">
        /// Starting cookies: JSON array with <c>name</c>, <c>value</c>, <c>domain</c>, <c>path</c>,
        /// <c>secure</c>, <c>httpOnly</c>, <c>expirationDate</c> and optional
        /// <c>creationDate</c>/<c>lastAccessDate</c>.
        /// </param>
        /// <param name="proxy"><c>[scheme://][user:pass@]host:port</c>; empty for none.</param>
        /// <returns>JSON array of cookies in the same format.</returns>
        public string Run(IEnumerable<string> services, string cookiesJson, string proxy = null)
        {
            _cookieDates.Clear();

            var cookieContainer = new CookieContainer();
            var domains = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            LoadCookies(cookieContainer, domains, cookiesJson);

            using (var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = cookieContainer,
                AllowAutoRedirect = AllowRedirects,
                MaxAutomaticRedirections = 10,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })
            {
                if (!string.IsNullOrWhiteSpace(proxy))
                {
                    handler.UseProxy = true;
                    handler.Proxy = CreateProxy(proxy);
                }

                using (var client = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(TimeoutSeconds)
                })
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgent);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", Accept);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", AcceptLanguage);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Pragma", "no-cache");

                    var urls = services
                        .Where(service => !string.IsNullOrWhiteSpace(service))
                        .Select(service => NormalizeUrl(service.Trim()))
                        .Distinct(StringComparer.OrdinalIgnoreCase);

                    foreach (var url in urls)
                        ProcessService(client, cookieContainer, domains, url);
                }
            }

            int cookieCount;
            var result = SaveCookies(cookieContainer, domains, out cookieCount);
            Log?.Invoke("COOKIES=" + cookieCount);
            return result;
        }

        private void ProcessService(
            HttpClient client,
            CookieContainer cookieContainer,
            HashSet<string> domains,
            string url)
        {
            var uri = new Uri(url);
            domains.Add(uri.Host);

            try
            {
                using (var response = client.GetAsync(uri).GetAwaiter().GetResult())
                    response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex) when (
                ex is System.Threading.Tasks.TaskCanceledException ||
                ex is HttpRequestException)
            {
                // Один недоступный сервис не должен останавливать сбор cookies.
            }
        }

        private static string NormalizeUrl(string value)
        {
            if (!value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                value = "https://" + value;
            }

            var builder = new UriBuilder(value);
            builder.Host = builder.Host.ToLowerInvariant();

            if ((builder.Scheme == "https" && builder.Port == 443) ||
                (builder.Scheme == "http" && builder.Port == 80))
            {
                builder.Port = -1;
            }

            return builder.Uri.ToString().TrimEnd('/');
        }

        private static IWebProxy CreateProxy(string value)
        {
            value = value.Trim();
            if (!value.Contains("://"))
                value = "http://" + value;

            var uri = new Uri(value);
            var proxy = new WebProxy(uri.Scheme + "://" + uri.Host + ":" + uri.Port);

            if (!string.IsNullOrWhiteSpace(uri.UserInfo))
            {
                var parts = uri.UserInfo.Split(new[] { ':' }, 2);
                var login = Uri.UnescapeDataString(parts[0]);
                var password = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : "";
                proxy.Credentials = new NetworkCredential(login, password);
            }

            return proxy;
        }

        private void LoadCookies(
            CookieContainer cookieContainer,
            HashSet<string> domains,
            string cookiesJson)
        {
            if (string.IsNullOrWhiteSpace(cookiesJson))
                return;

            foreach (var token in JArray.Parse(cookiesJson))
            {
                var name = token.Value<string>("name");
                var value = token.Value<string>("value") ?? "";
                var domain = token.Value<string>("domain");
                var path = token.Value<string>("path") ?? "/";

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(domain))
                    continue;

                var cookie = new Cookie(name, value, path, domain)
                {
                    Secure = token.Value<bool?>("secure") ?? false,
                    HttpOnly = token.Value<bool?>("httpOnly") ?? false
                };

                var expirationDate = token.Value<long?>("expirationDate");
                if (expirationDate.HasValue && expirationDate.Value > 0)
                {
                    cookie.Expires = DateTimeOffset
                        .FromUnixTimeSeconds(expirationDate.Value)
                        .UtcDateTime;
                }

                cookieContainer.Add(cookie);
                domains.Add(domain.TrimStart('.'));

                var creationDate = token.Value<long?>("creationDate");
                if (!creationDate.HasValue)
                    continue;

                _cookieDates[GetCookieKey(domain, path, name)] = new CookieDates
                {
                    CreationDate = creationDate.Value,
                    LastAccessDate = token.Value<long?>("lastAccessDate") ?? creationDate.Value
                };
            }
        }

        private string SaveCookies(
            CookieContainer cookieContainer,
            HashSet<string> domains,
            out int cookieCount)
        {
            var result = new JArray();
            var added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var domain in domains)
            {
                AddCookiesForUri(cookieContainer, new Uri("https://" + domain), result, added);
                AddCookiesForUri(cookieContainer, new Uri("http://" + domain), result, added);
            }

            cookieCount = result.Count;
            return JsonConvert.SerializeObject(result);
        }

        private void AddCookiesForUri(
            CookieContainer cookieContainer,
            Uri uri,
            JArray result,
            HashSet<string> added)
        {
            foreach (Cookie cookie in cookieContainer.GetCookies(uri))
            {
                var key = GetCookieKey(cookie.Domain, cookie.Path, cookie.Name);
                if (!added.Add(key))
                    continue;

                var dates = GetOrCreateCookieDates(key);
                long? expirationDate = null;

                if (cookie.Expires != DateTime.MinValue)
                {
                    expirationDate = new DateTimeOffset(cookie.Expires.ToUniversalTime())
                        .ToUnixTimeSeconds();
                }

                result.Add(new JObject
                {
                    ["domain"] = cookie.Domain,
                    ["expirationDate"] = expirationDate.HasValue
                        ? new JValue(expirationDate.Value)
                        : JValue.CreateNull(),
                    ["creationDate"] = dates.CreationDate,
                    ["lastAccessDate"] = dates.LastAccessDate,
                    ["hostOnly"] = !cookie.Domain.StartsWith("."),
                    ["httpOnly"] = cookie.HttpOnly,
                    ["name"] = cookie.Name,
                    ["path"] = cookie.Path,
                    ["sameSite"] = "Unspecified",
                    ["secure"] = cookie.Secure,
                    ["session"] = cookie.Expires == DateTime.MinValue,
                    ["storeId"] = JValue.CreateNull(),
                    ["value"] = cookie.Value
                });
            }
        }

        private CookieDates GetOrCreateCookieDates(string key)
        {
            if (_cookieDates.TryGetValue(key, out var dates))
                return dates;

            var now = DateTimeOffset.UtcNow;
            var creationDate = now
                .AddDays(-_random.Next(MinCookieAgeDays, MaxCookieAgeDays + 1))
                .AddHours(-_random.Next(0, 24))
                .AddMinutes(-_random.Next(0, 60))
                .AddSeconds(-_random.Next(0, 60));

            var earliestLastAccess = now.AddDays(-MaxLastAccessAgeDays);
            if (earliestLastAccess < creationDate)
                earliestLastAccess = creationDate;

            var availableSeconds = (long)(now - earliestLastAccess).TotalSeconds;
            var offsetSeconds = availableSeconds > 0
                ? (long)(_random.NextDouble() * availableSeconds)
                : 0;

            var lastAccessDate = earliestLastAccess.AddSeconds(offsetSeconds);
            if (lastAccessDate > now)
                lastAccessDate = now;

            dates = new CookieDates
            {
                CreationDate = creationDate.ToUnixTimeSeconds(),
                LastAccessDate = lastAccessDate.ToUnixTimeSeconds()
            };

            _cookieDates[key] = dates;
            return dates;
        }

        private static string GetCookieKey(string domain, string path, string name)
        {
            return (domain ?? "") + "|" + (path ?? "/") + "|" + (name ?? "");
        }

        private class CookieDates
        {
            public long CreationDate;
            public long LastAccessDate;
        }
    }
}
