using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Api
{
    /// <summary>
    /// Client of the AnyMessage mailbox service (<c>api.anymessage.shop</c>): short-term and long-term
    /// mailboxes.
    /// State is kept in project variables: <c>anyMailId</c> for the current short-term order,
    /// <c>anyLLId</c> for the long-term one. A response whose <c>status</c> is not <c>success</c> throws
    /// with the service's message.
    /// </summary>
    public class AnyMessage
    {
        private const string BaseUrl = "https://api.anymessage.shop";
        private readonly string _apikey;
        private readonly IZennoPosterProjectModel _project;
        private readonly bool _log;
        /// <summary>Creates a client.</summary>
        /// <param name="apikey">AnyMessage API token.</param>
        /// <param name="log">Log requests and responses.</param>
        public AnyMessage(IZennoPosterProjectModel project, string apikey , bool log = false)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _apikey  = apikey  ?? throw new ArgumentNullException(nameof(apikey));
            _log = log;
        }

        // ── helpers ──────────────────────────────────────────────────────────────

        private string Get(string path) =>
            _project.GET($"{BaseUrl}{path}", log: _log, useNetHttp: false, thrw: true);

        private void Check(string tag = null)
        {
            var status = _project.Json.status?.ToString();
            if (status != "success")
                throw new Exception($"AnyMessage{(tag != null ? " " + tag : "")}: {_project.Json.value ?? _project.Json.err_message}");
        }

        // ── short-term emails ─────────────────────────────────────────────────────

        // Дословный ответ /email/order, когда на домене нет ящиков.
        private const string NoEmails = "no emails";

        /// <summary>Domain of the last ordered mailbox; may differ from the requested one after a fallback.</summary>
        public string LastDomain { get; private set; }

        /// <summary>Available domains for a site (<c>/email/quantity</c>).</summary>
        /// <param name="site">Target site, e.g. <c>instagram.com</c>.</param>
        /// <returns>Domain → (mailboxes available, price). Domains without a price are left out.</returns>
        public Dictionary<string, (int Count, double Price)> Quantity(string site)
        {
            var json = Get($"/email/quantity?token={_apikey}&site={site}");
            _project.ToJson(json);
            Check("quantity");

            var result = new Dictionary<string, (int, double)>();
            var data = JObject.Parse(json)["data"] as JObject;
            if (data == null) return result;

            foreach (var prop in data.Properties())
            {
                if (!(prop.Value is JObject info)) continue;
                try
                {
                    int count = info["count"]?.ToObject<int?>() ?? 0;
                    double? price = info["price"]?.ToObject<double?>();
                    if (price == null) continue;
                    result[prop.Name] = (count, price.Value);
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// Domains with mailboxes available, cheapest first; at equal price the one with more mailboxes first.
        /// </summary>
        /// <param name="site">Target site.</param>
        /// <param name="exclude">Domains to leave out.</param>
        public List<string> CheapestDomains(string site, params string[] exclude)
        {
            var ex = new HashSet<string>(exclude ?? new string[0], StringComparer.OrdinalIgnoreCase);
            return Quantity(site)
                .Where(kv => kv.Value.Count > 0 && !ex.Contains(kv.Key))
                .OrderBy(kv => kv.Value.Price)
                .ThenByDescending(kv => kv.Value.Count)
                .Select(kv => kv.Key)
                .ToList();
        }

        private string[] Order(string site, string domain)
        {
            var json = Get($"/email/order?token={_apikey}&site={site}&domain={domain}");
            _project.ToJson(json);
            Check("order");

            string id    = _project.Json.id.ToString();
            string email = _project.Json.email.ToString();
            _project.Var("anyMailId",    id);
            _project.Var("email", email);
            _project.Profile.Email =  email;
            LastDomain = domain;
            return new[] { id, email };
        }

        private static bool IsNoEmails(Exception ex) =>
            ex.Message.IndexOf(NoEmails, StringComparison.OrdinalIgnoreCase) >= 0;

        /// <summary>
        /// Orders a short-term mailbox. Stores the id in <c>anyMailId</c> and the address in <c>email</c> and
        /// <c>project.Profile.Email</c>.
        /// </summary>
        /// <param name="site">Target site, e.g. <c>instagram.com</c>.</param>
        /// <param name="domain">Mailbox domain.</param>
        /// <param name="fallback">
        /// When the service answers "no emails", try the cheapest available domains instead. The domain used
        /// ends up in <c>LastDomain</c>.
        /// </param>
        /// <param name="maxFallback">How many fallback domains to try.</param>
        /// <returns><c>[id, email]</c>. Throws with every attempt's error when nothing could be ordered.</returns>
        public string[] NewMail(string site, string domain = "outlook.com", bool fallback = true, int maxFallback = 5)
        {
            var errors = new List<string>();
            try
            {
                return Order(site, domain);
            }
            catch (Exception ex) when (fallback && IsNoEmails(ex))
            {
                errors.Add($"{domain}: {ex.Message}");
            }

            List<string> candidates;
            try
            {
                candidates = CheapestDomains(site, domain);
            }
            catch (Exception ex)
            {
                errors.Add($"quantity: {ex.GetType().Name}: {ex.Message}");
                throw new Exception("AnyMessage order: " + string.Join(" | ", errors), ex);
            }

            foreach (var dom in candidates.Take(maxFallback))
            {
                if (_log) _project.SendInfoToLog($"[AnyMessage] {domain}: no emails -> {dom}");
                try
                {
                    return Order(site, dom);
                }
                catch (Exception ex)
                {
                    errors.Add($"{dom}: {ex.Message}");
                    if (!IsNoEmails(ex)) break;
                }
            }
            if (candidates.Count == 0)
                errors.Add("quantity: нет доменов с count > 0");
            throw new Exception("AnyMessage order: " + string.Join(" | ", errors));
        }

        /// <summary>Waits for a message to the mailbox in <c>anyMailId</c>, polling every 5 seconds.</summary>
        /// <param name="deadline">Seconds to wait; then <c>TimeoutException</c>.</param>
        /// <returns>The message body (HTML).</returns>
        public string GetMail(int deadline = 120)
        {
            var id = _project.Var("anyMailId");
            var d  = new Time.Deadline();

            while (true)
            {
                Thread.Sleep(5000);
                d.Check(deadline);

                var json = Get($"/email/getmessage?token={_apikey}&id={id}");
                _project.ToJson(json);

                if (_project.Json.status?.ToString() == "error" &&
                    _project.Json.value?.ToString()  == "wait message")
                    continue;

                Check("getmessage");
                return _project.Json.message.ToString();
            }
        }

        /// <summary>Waits for a message and extracts a 6-digit code; stores it in <c>mailOtp</c>.</summary>
        /// <param name="matchIndex">
        /// Which 6-digit number of the message to take; -1 writes all of them to the log and returns an empty
        /// string.
        /// </param>
        /// <returns>The code. Throws when no code is found after 10 attempts.</returns>
        public string Otp(int matchIndex = 0)
        {
            var retries = 10;
            while (retries > 0)
            {
                retries--;
                var html = GetMail();
                var body = Regex.Replace(html, "<.*?>", "");
                var otp = "";
                var matches = Regex.Matches(body, @"\b\d{6}\b");

                if (matchIndex == -1)
                {
                    _project.SendInfoToLog(string.Join(Environment.NewLine, matches.Cast<Match>().Select(x => x.Value)));
                    return "";
                }
                
                if (matches.Count > matchIndex)
                {
                    otp = matches[matchIndex].Value;
                    _project.Var("mailOtp", otp);
                    return otp;
                }
                
            }

            throw new Exception("AnyMessage: OTP not found");
        }

        /// <summary>Returns one link from the message (see <c>GetHrefs</c>).</summary>
        /// <param name="hrefIndex">Index of the link; -1 writes all links to the log and returns an empty string.</param>
        /// <param name="deadline">Seconds to wait for the message.</param>
        public string Href(int hrefIndex = 0, int deadline = 60)
        {
            var hrefs = GetHrefs(deadline);
            if (hrefIndex == -1)
            {
                _project.SendInfoToLog(string.Join(Environment.NewLine, hrefs));
                return "";
            }
            return hrefs[hrefIndex];
            
        }

        /// <summary>
        /// Waits for a message and collects its unique links, skipping anchors, <c>mailto:</c>, <c>tel:</c>,
        /// <c>javascript:</c>, <c>data:</c> and links to images, styles, scripts and fonts.
        /// </summary>
        /// <param name="deadline">Seconds to wait for the message.</param>
        public List<string> GetHrefs(int deadline = 60)
        {
            var json = GetMail(deadline);
            _project.ToJson(json);

            var message = _project.Json.message?.ToString() ?? "";

            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var document = new HtmlDocument();
            document.LoadHtml(message);

            var nodes = document.DocumentNode.SelectNodes("//*[@href]");

            if (nodes == null)
                return result.ToList();

            for (int i = 0; i < nodes.Count; i++)
            {
                var href = NormalizeHref(nodes[i].GetAttributeValue("href", ""));

                if (string.IsNullOrEmpty(href))
                    continue;

                if (IsStaticHref(href))
                    continue;

                result.Add(href);
            }
            
            var list = result.ToList();
            
            if (_log)
                _project.SendInfoToLog(string.Join(Environment.NewLine, list));
            return list;
        }

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

            if (queryIndex >= 0)
                value = value.Substring(0, queryIndex);

            int hashIndex = value.IndexOf('#');

            if (hashIndex >= 0)
                value = value.Substring(0, hashIndex);

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

        /// <summary>
        /// Waits for a message and returns the first match of <c>urlPattern</c> in its HTML. Throws when there
        /// is none.
        /// </summary>
        /// <param name="urlPattern">Regular expression.</param>
        public string LinkByRegex(string urlPattern)
        {
            var html  = GetMail();
            var match = Regex.Match(html, urlPattern);
            if (!match.Success) throw new Exception("AnyMessage: link not found");
            return match.Value;
        }

        /// <summary>
        /// Orders the mailbox in <c>anyMailId</c> again under a new id; updates <c>anyMailId</c> and
        /// <c>email</c>.
        /// </summary>
        /// <returns><c>[id, email]</c>.</returns>
        public string[] Reorder()
        {
            var id   = _project.Var("anyMailId");
            var json = Get($"/email/reorder?token={_apikey}&id={id}");
            _project.ToJson(json);
            Check("reorder");

            string newId    = _project.Json.id.ToString();
            string newEmail = _project.Json.email.ToString();
            _project.Var("anyMailId",    newId);
            _project.Var("email", newEmail);
            return new[] { newId, newEmail };
        }

        /// <summary>Cancels the order in <c>anyMailId</c>.</summary>
        public void Cancel()
        {
            var id   = _project.Var("anyMailId");
            var json = Get($"/email/cancel?token={_apikey}&id={id}");
            _project.ToJson(json);
            Check("cancel");
        }

        // ── long-term emails ──────────────────────────────────────────────────────

        /// <summary>
        /// Buys a long-term mailbox. Stores the id in <c>anyLLId</c> and the address in <c>anyLLEmail</c>.
        /// </summary>
        /// <param name="site">Target site, e.g. <c>instagram.com</c>.</param>
        /// <param name="domain">Mailbox domain, e.g. <c>hotmail.com</c>.</param>
        /// <returns><c>[id, email, imapPassword, imapHost, imapPort]</c> of the first mailbox in the answer.</returns>
        public string[] OrderLongLive(string site, string domain)
        {
            var json = Get($"/longlive-email/order?token={_apikey}&site={site}&domain={domain}");
            _project.ToJson(json);
            Check("longlive order");

            var first = _project.Json.emails[0];
            string id    = first.id.ToString();
            string email = first.email.ToString();
            string pass  = first.imap.password.ToString();
            string host  = first.imap.link.ToString();
            string port  = first.imap.port.ToString();
            _project.Var("anyLLId",    id);
            _project.Var("anyLLEmail", email);
            return new[] { id, email, pass, host, port };
        }

        /// <summary>Recent messages of the long-term mailbox in <c>anyLLId</c>.</summary>
        /// <param name="subject">When set, only messages with this subject.</param>
        /// <returns>The raw JSON answer.</returns>
        public string GetLastMessages(string subject = null)
        {
            var id  = _project.Var("anyLLId");
            var qs  = string.IsNullOrEmpty(subject) ? "" : $"&subject={Uri.EscapeDataString(subject)}";
            var json = Get($"/longlive-email/getlastmessages?token={_apikey}&id={id}{qs}");
            _project.ToJson(json);
            Check("getlastmessages");
            return json;
        }

        // ── misc ──────────────────────────────────────────────────────────────────

        /// <summary>Account balance as returned by the service.</summary>
        public string Balance()
        {
            var json = Get($"/user/balance?token={_apikey}");
            _project.ToJson(json);
            Check("balance");
            return _project.Json.balance.ToString();
        }
    }
}