using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;

using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Api
{
    /// <summary>
    /// Client of the BestMailBox temporary mailbox service (default <c>https://mail.autoz3n.xyz</c>).
    /// A response whose <c>success</c> is not true throws with the service's error.
    /// </summary>
    public class BestMailBox
    {
        private const string DefaultBaseUrl = "https://mail.autoz3n.xyz";
        private readonly string _baseUrl;
        private readonly string _apikey;
        private readonly IZennoPosterProjectModel _project;
        private readonly bool _useNetHttp;
        private readonly bool _log;

        /// <summary>Creates a client.</summary>
        /// <param name="apikey">
        /// API key; default <c>BESTMAILBOX_API_KEY</c> from the project's <c>.env</c>. Throws when neither is
        /// set.
        /// </param>
        /// <param name="baseUrl">
        /// Service URL; default <c>BESTMAILBOX_BASE_URL</c> from the project's <c>.env</c>, else the built-in
        /// one.
        /// </param>
        /// <param name="useNetHttp">Send requests through <c>NetHttp</c> instead of ZennoPoster's HTTP client.</param>
        /// <param name="log">Log requests and responses.</param>
        public BestMailBox(
            IZennoPosterProjectModel project,
            string apikey = null,
            string baseUrl = null,
            bool useNetHttp = false,
            bool log = false)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _useNetHttp = useNetHttp;
            _log = log;

            _apikey = !string.IsNullOrWhiteSpace(apikey)
                ? apikey
                : project.ReadEnv("BESTMAILBOX_API_KEY");

            if (string.IsNullOrWhiteSpace(_apikey))
                throw new ArgumentException("BestMailBox: API key not provided and BESTMAILBOX_API_KEY is not set in environment.");

            _baseUrl = (!string.IsNullOrWhiteSpace(baseUrl)
                ? baseUrl
                : (project.ReadEnv("BESTMAILBOX_BASE_URL") ?? DefaultBaseUrl)).TrimEnd('/');
        }

        // ── HTTP helpers ──

        private string Get(string path) =>
            _project.GET($"{_baseUrl}{path}", log: _log, useNetHttp: _useNetHttp, thrw: true);

        private string Post(string path, string body = "{}") =>
            _project.POST($"{_baseUrl}{path}", body: body, log: _log, useNetHttp: _useNetHttp, thrw: true);

        private string Delete(string path) =>
            _project.DELETE($"{_baseUrl}{path}", log: _log, useNetHttp: _useNetHttp, thrw: true);

        private void Check(string tag = null)
        {
            var success = _project.Json.success?.ToString()?.ToLower();
            if (success != "true")
            {
                var err = _project.Json.error?.ToString() ?? _project.Json.message?.ToString() ?? "Unknown error";
                throw new Exception($"BestMailBox{(tag != null ? " " + tag : "")}: {err}");
            }
        }

        // ── Mailbox Operations ──

        /// <summary>
        /// Creates a mailbox. Stores the id in <c>mailId</c> and <c>bestMailId</c>, the address in <c>email</c>
        /// and <c>project.Profile.Email</c>.
        /// </summary>
        /// <param name="domain">Mailbox domain; random when <c>null</c>.</param>
        /// <param name="prefix">Local part of the address; generated when <c>null</c>.</param>
        /// <param name="ttl">Mailbox lifetime in seconds.</param>
        /// <returns><c>[id, email]</c>.</returns>
        public string[] NewMail(string domain = null, string prefix = null, int ttl = 1200)
        {
            var query = $"?api_key={Uri.EscapeDataString(_apikey)}&ttl={ttl}";
            if (!string.IsNullOrWhiteSpace(domain)) query += $"&domain={Uri.EscapeDataString(domain)}";
            if (!string.IsNullOrWhiteSpace(prefix)) query += $"&prefix={Uri.EscapeDataString(prefix)}";

            var json = Post($"/api/mailbox{query}");
            _project.ToJson(json);
            Check("order");

            string id = _project.Json.data.id.ToString();
            string email = _project.Json.data.email.ToString();

            _project.Var("mailId", id);
            _project.Var("bestMailId", id);
            _project.Var("email", email);
            _project.Profile.Email = email;

            return new[] { id, email };
        }

        /// <summary>
        /// Polls the service every 2.5 seconds for a one-time code found by the service in the mailbox.
        /// </summary>
        /// <param name="deadline">Seconds to wait; then <c>TimeoutException</c>.</param>
        /// <param name="id">
        /// Mailbox id or address; default is the <c>mailId</c> variable, then <c>bestMailId</c>, then
        /// <c>email</c>.
        /// </param>
        /// <returns>The code.</returns>
        public string Otp(int deadline = 60, string id = null)
        {
            var mailId = id ?? _project.Var("mailId");
            if (string.IsNullOrWhiteSpace(mailId)) mailId = _project.Var("bestMailId");
            if (string.IsNullOrWhiteSpace(mailId)) mailId = _project.Var("email");

            var d = new Time.Deadline();

            while (true)
            {
                Thread.Sleep(2500);
                d.Check(deadline);

                var json = Get($"/api/mailbox/{mailId}/otp?silent=true&api_key={Uri.EscapeDataString(_apikey)}");
                _project.ToJson(json);

                if (_project.Json.success?.ToString()?.ToLower() == "true" && _project.Json.data != null)
                {
                    string otp = _project.Json.data.otp?.ToString();
                    if (!string.IsNullOrWhiteSpace(otp))
                    {
                        return otp;
                    }
                }
            }
        }

        /// <summary>Polls every 2.5 seconds for the latest message.</summary>
        /// <param name="deadline">Seconds to wait; then <c>TimeoutException</c>.</param>
        /// <param name="id">
        /// Mailbox id or address; default is the <c>mailId</c> variable, then <c>bestMailId</c>, then
        /// <c>email</c>.
        /// </param>
        /// <returns>The HTML body, or the text body when there is no HTML.</returns>
        public string GetMail(int deadline = 60, string id = null)
        {
            var mailId = id ?? _project.Var("mailId");
            if (string.IsNullOrWhiteSpace(mailId)) mailId = _project.Var("bestMailId");
            if (string.IsNullOrWhiteSpace(mailId)) mailId = _project.Var("email");

            var d = new Time.Deadline();

            while (true)
            {
                Thread.Sleep(2500);
                d.Check(deadline);

                var json = Get($"/api/mailbox/{mailId}/latest?silent=true&api_key={Uri.EscapeDataString(_apikey)}");
                _project.ToJson(json);

                if (_project.Json.success?.ToString()?.ToLower() == "true" && _project.Json.data != null)
                {
                    var html = _project.Json.data.html?.ToString();
                    if (!string.IsNullOrWhiteSpace(html)) return html;

                    var text = _project.Json.data.text?.ToString();
                    if (!string.IsNullOrWhiteSpace(text)) return text;
                }
            }
        }

        /// <summary>
        /// Polls every 2.5 seconds for the latest message and returns its links: the service's verification
        /// links, else all links of the HTML body. Anchors, <c>mailto:</c>, <c>tel:</c>, <c>javascript:</c>,
        /// <c>data:</c> and links to images, styles, scripts and fonts are skipped.
        /// </summary>
        /// <param name="deadline">Seconds to wait; then <c>TimeoutException</c>.</param>
        /// <param name="id">
        /// Mailbox id or address; default is the <c>mailId</c> variable, then <c>bestMailId</c>, then
        /// <c>email</c>.
        /// </param>
        public HashSet<string> GetHrefs(int deadline = 60, string id = null)
        {
            var mailId = id ?? _project.Var("mailId");
            if (string.IsNullOrWhiteSpace(mailId)) mailId = _project.Var("bestMailId");
            if (string.IsNullOrWhiteSpace(mailId)) mailId = _project.Var("email");

            var d = new Time.Deadline();
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            while (true)
            {
                Thread.Sleep(2500);
                d.Check(deadline);

                var json = Get($"/api/mailbox/{mailId}/latest?silent=true&api_key={Uri.EscapeDataString(_apikey)}");
                _project.ToJson(json);

                if (_project.Json.success?.ToString()?.ToLower() == "true" && _project.Json.data != null)
                {
                    if (_project.Json.data.verificationLinks != null)
                    {
                        foreach (var link in _project.Json.data.verificationLinks)
                        {
                            var url = NormalizeHref(link?.ToString());
                            if (!string.IsNullOrEmpty(url) && !IsStaticHref(url))
                            {
                                result.Add(url);
                            }
                        }
                    }

                    if (result.Count == 0)
                    {
                        var html = _project.Json.data.html?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(html))
                        {
                            var doc = new HtmlDocument();
                            doc.LoadHtml(html);
                            var nodes = doc.DocumentNode.SelectNodes("//*[@href]");
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    var href = NormalizeHref(node.GetAttributeValue("href", ""));
                                    if (!string.IsNullOrEmpty(href) && !IsStaticHref(href))
                                    {
                                        result.Add(href);
                                    }
                                }
                            }
                        }
                    }

                    if (result.Count > 0) return result;
                }
            }
        }

        /// <summary>Deletes the mailbox and its messages.</summary>
        /// <param name="id">
        /// Mailbox id or address; default is the <c>mailId</c> variable, then <c>bestMailId</c>, then
        /// <c>email</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> when the service confirmed; <c>false</c> on any error or when there is no id.
        /// </returns>
        public bool DeleteMail(string id = null)
        {
            var mailId = id ?? _project.Var("mailId");
            if (string.IsNullOrWhiteSpace(mailId)) mailId = _project.Var("bestMailId");
            if (string.IsNullOrWhiteSpace(mailId)) mailId = _project.Var("email");

            if (string.IsNullOrWhiteSpace(mailId)) return false;

            try
            {
                var json = Delete($"/api/mailbox/{mailId}?api_key={Uri.EscapeDataString(_apikey)}");
                _project.ToJson(json);
                return _project.Json.success?.ToString()?.ToLower() == "true";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Domains the service offers.</summary>
        public List<string> GetDomains()
        {
            var json = Get($"/api/domains?api_key={Uri.EscapeDataString(_apikey)}");
            _project.ToJson(json);
            Check("domains");

            var list = new List<string>();
            if (_project.Json.domains != null)
            {
                foreach (var d in _project.Json.domains)
                {
                    list.Add(d.ToString());
                }
            }
            return list;
        }

        // ── Static URL helpers ──

        private static string NormalizeHref(string href)
        {
            if (string.IsNullOrWhiteSpace(href))
                return "";

            href = WebUtility.HtmlDecode(href).Trim();

            if (href.Length == 0)
                return "";

            if (href.StartsWith("#") ||
                href.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ||
                href.StartsWith("tel:", StringComparison.OrdinalIgnoreCase) ||
                href.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase) ||
                href.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                return "";
            }

            return href;
        }

        private static bool IsStaticHref(string href)
        {
            string value = href;
            int queryIndex = value.IndexOf('?');
            if (queryIndex >= 0) value = value.Substring(0, queryIndex);

            int hashIndex = value.IndexOf('#');
            if (hashIndex >= 0) value = value.Substring(0, hashIndex);

            return
                value.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".js", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".woff", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".eot", StringComparison.OrdinalIgnoreCase);
        }
    }
}
