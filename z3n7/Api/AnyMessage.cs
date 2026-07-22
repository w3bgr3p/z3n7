using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;
using z3nIO;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Api
{
    public class AnyMessage
    {
        private const string BaseUrl = "https://api.anymessage.shop";
        private readonly string _apikey;
        private readonly IZennoPosterProjectModel _project;
        public AnyMessage(IZennoPosterProjectModel project, string apikey )
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _apikey  = apikey  ?? throw new ArgumentNullException(nameof(apikey));
        }

        // ── helpers ──────────────────────────────────────────────────────────────

        private string Get(string path) =>
            _project.GET($"{BaseUrl}{path}", log: true, useNetHttp: false, thrw: true);

        private void Check(string tag = null)
        {
            var status = _project.Json.status?.ToString();
            if (status != "success")
                throw new Exception($"AnyMessage{(tag != null ? " " + tag : "")}: {_project.Json.value ?? _project.Json.err_message}");
        }

        // ── short-term emails ─────────────────────────────────────────────────────

        /// <summary>Заказать временный email. Возвращает [id, email].</summary>
        /// <param name="site">Сайт, например "instagram.com"</param>
        /// <param name="domain">Домен: "mailcom", "gmx", "hotmail", "outlook" (или через запятую)</param>
        public string[] NewMail(string site, string domain = "outlook.com")
        {
            var json = Get($"/email/order?token={_apikey}&site={site}&domain={domain}");
            _project.ToJson(json);
            Check("order");

            string id    = _project.Json.id.ToString();
            string email = _project.Json.email.ToString();
            _project.Var("anyMailId",    id);
            _project.Var("email", email);
            return new[] { id, email };
        }

        /// <summary>Ждать письмо. Возвращает HTML тела.</summary>
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

        /// <summary>Получить OTP (6 цифр) из письма.</summary>
        public string Otp()
        {
            var html = GetMail();
            var body = Regex.Replace(html, "<.*?>", "");

            var match = Regex.Match(body, @"\b\d{6}\b");
            if (match.Success) return match.Value;

            throw new Exception("AnyMessage: OTP not found");
        }
        
        public HashSet<string> GetHrefs(int deadline = 60)
        {
            var json = GetMail(deadline);
            _project.ToJson(json);

            var message = _project.Json.message?.ToString() ?? "";

            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var document = new HtmlDocument();
            document.LoadHtml(message);

            var nodes = document.DocumentNode.SelectNodes("//*[@href]");

            if (nodes == null)
                return result;

            for (int i = 0; i < nodes.Count; i++)
            {
                var href = NormalizeHref(nodes[i].GetAttributeValue("href", ""));

                if (string.IsNullOrEmpty(href))
                    continue;

                if (IsStaticHref(href))
                    continue;

                result.Add(href);
            }

            return result;
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

        /// <summary>Извлечь ссылку из письма по паттерну.</summary>
        public string Link(string urlPattern)
        {
            var html  = GetMail();
            var match = Regex.Match(html, urlPattern);
            if (!match.Success) throw new Exception("AnyMessage: link not found");
            return match.Value;
        }

        /// <summary>Перезаказать тот же email (новый id).</summary>
        public string[] Reorder()
        {
            var id   = _project.Var("anyMailId");
            var json = Get($"/email/reorder?token={_apikey}&id={id}");
            _project.ToJson(json);
            Check("reorder");

            string newId    = _project.Json.id.ToString();
            string newEmail = _project.Json.email.ToString();
            _project.Var("anyMailId",    newId);
            _project.Var("anyMailEmail", newEmail);
            return new[] { newId, newEmail };
        }

        /// <summary>Отменить активацию.</summary>
        public void Cancel()
        {
            var id   = _project.Var("anyMailId");
            var json = Get($"/email/cancel?token={_apikey}&id={id}");
            _project.ToJson(json);
            Check("cancel");
        }

        // ── long-term emails ──────────────────────────────────────────────────────

        /// <summary>
        /// Купить долгосрочный почтовый ящик.
        /// Возвращает первый email из списка: [id, email, imapPass, imapHost, imapPort].
        /// </summary>
        /// <param name="site">Сайт, например "instagram.com"</param>
        /// <param name="domain">Домен, например "hotmail.com"</param>
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

        /// <summary>Получить последние сообщения (за 40 мин) для долгосрочного ящика.</summary>
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

        public string Balance()
        {
            var json = Get($"/user/balance?token={_apikey}");
            _project.ToJson(json);
            Check("balance");
            return _project.Json.balance.ToString();
        }
    }
}