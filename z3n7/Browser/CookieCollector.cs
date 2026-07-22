using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace z3n7
{
    public class CookieCollector
    {
        private readonly Random _random = new Random();
        private readonly Dictionary<string, CookieDates> _cookieDates =
            new Dictionary<string, CookieDates>(StringComparer.OrdinalIgnoreCase);

        public int TimeoutSeconds { get; set; } = 25;
        public bool AllowRedirects { get; set; } = true;
        public int MinCookieAgeDays { get; set; } = 7;
        public int MaxCookieAgeDays { get; set; } = 120;
        public int MaxLastAccessAgeDays { get; set; } = 14;

        public string UserAgent { get; set; } =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/126.0 Safari/537.36";

        public string Accept { get; set; } =
            "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

        public string AcceptLanguage { get; set; } =
            "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7";

        public Action<string> Log { get; set; }

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

            return SaveCookies(cookieContainer, domains);
        }

        private void ProcessService(
            HttpClient client,
            CookieContainer cookieContainer,
            HashSet<string> domains,
            string url)
        {
            var uri = new Uri(url);
            domains.Add(uri.Host);

            var beforeCount = cookieContainer.GetCookies(uri).Count;
            var statusCode = 0;
            var status = "OK";
            var stopwatch = Stopwatch.StartNew();

            try
            {
                using (var response = client.GetAsync(uri).GetAwaiter().GetResult())
                {
                    statusCode = (int)response.StatusCode;
                    response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

                    if (statusCode >= 300 && statusCode <= 399)
                        status = "REDIRECT";
                    else if (statusCode >= 400)
                        status = "HTTP_" + statusCode;
                }
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                status = "TIMEOUT";
            }
            catch (HttpRequestException ex)
            {
                status = "ERROR";
                Log?.Invoke(url + " | " + ex.Message);
            }

            stopwatch.Stop();
            var afterCount = cookieContainer.GetCookies(uri).Count;

            Log?.Invoke(
                url +
                " | HTTP=" + statusCode +
                " | STATUS=" + status +
                " | TIME=" + stopwatch.ElapsedMilliseconds + " ms" +
                " | COOKIES=" + beforeCount + " -> " + afterCount);
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

        private string SaveCookies(CookieContainer cookieContainer, HashSet<string> domains)
        {
            var result = new JArray();
            var added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var domain in domains)
            {
                AddCookiesForUri(cookieContainer, new Uri("https://" + domain), result, added);
                AddCookiesForUri(cookieContainer, new Uri("http://" + domain), result, added);
            }

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
