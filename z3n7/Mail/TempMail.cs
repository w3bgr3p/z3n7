using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Api
{
    public class TempMail
    {
        private const string BaseUrl = "https://privatix-temp-mail-v1.p.rapidapi.com";
        private const string RapidApiHost = "privatix-temp-mail-v1.p.rapidapi.com";

        private readonly IZennoPosterProjectModel _project;
        private readonly string[] _headers;
        private readonly bool _log;
        private readonly bool _useNetHttp;
        private readonly string _proxy;

        public TempMail(
            IZennoPosterProjectModel project,
            string apikey,
            bool log = false,
            bool useNetHttp = false,
            string proxy = "")
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            if (string.IsNullOrWhiteSpace(apikey))
                throw new ArgumentException("RapidAPI key is required", nameof(apikey));

            _log = log;
            _useNetHttp = useNetHttp;
            _proxy = proxy ?? "";
            _headers = new[]
            {
                $"X-RapidAPI-Key: {apikey}",
                $"X-RapidAPI-Host: {RapidApiHost}",
                "Accept: application/json"
            };
        }

        private string Get(string path) =>
            _project.GET(
                $"{BaseUrl}{path}",
                _proxy,
                _headers,
                log: _log,
                useNetHttp: _useNetHttp,
                thrw: true);

        public string[] GetDomains()
        {
            var json = Get("/request/domains/format/json/");
            var domains = JsonConvert.DeserializeObject<string[]>(json);

            if (domains == null || domains.Length == 0)
                throw new Exception($"TempMail: domains not found: {json}");

            return domains;
        }

        /// <summary>Создать временный email. Возвращает [md5, email].</summary>
        public string[] NewMail(string login = null, string domain = null)
        {
            if (string.IsNullOrWhiteSpace(login))
                login = Guid.NewGuid().ToString("N").Substring(0, 10);

            if (string.IsNullOrWhiteSpace(domain))
            {
                var domains = GetDomains();
                var index = (int)((uint)Guid.NewGuid().GetHashCode() % (uint)domains.Length);
                domain = domains[index];
            }

            var email = CreateAddress(login, domain);
            var id = HashEmail(email);

            _project.Var("tempMailId", id);
            _project.Var("mailId", id);
            _project.Var("email", email);
            _project.Profile.Email = email;

            return new[] { id, email };
        }

        /// <summary>Получить текущий список сообщений без ожидания.</summary>
        public string GetMessages()
        {
            var id = _project.Var("tempMailId");
            if (string.IsNullOrWhiteSpace(id))
                id = _project.Var("mailId");

            if (string.IsNullOrWhiteSpace(id))
                throw new Exception("TempMail: call NewMail first");

            return Get($"/request/mail/id/{id}/format/json/");
        }

        /// <summary>Ждать письмо. Возвращает JSON первого сообщения.</summary>
        public string GetMail(int deadline = 120)
        {
            var d = new Time.Deadline();

            while (true)
            {
                var json = GetMessages();
                var token = ParseResponse(json);

                if (token is JArray messages && messages.Count > 0)
                    return messages[0].ToString(Formatting.None);

                if (token is JObject message && message["mail_id"] != null)
                    return message.ToString(Formatting.None);

                Thread.Sleep(5000);
                d.Check(deadline);
            }
        }

        public string Otp(int deadline = 120)
        {
            var message = JObject.Parse(GetMail(deadline));
            var subject = message["mail_subject"]?.ToString() ?? "";
            var body = message["mail_text"]?.ToString()
                       ?? message["mail_text_only"]?.ToString()
                       ?? message["mail_html"]?.ToString()
                       ?? "";

            var match = Regex.Match(subject, @"\b\d{6}\b");
            if (match.Success) return match.Value;

            match = Regex.Match(Regex.Replace(body, "<.*?>", ""), @"\b\d{6}\b");
            if (match.Success) return match.Value;

            throw new Exception("TempMail: OTP not found");
        }

        public HashSet<string> GetHrefs(int deadline = 120)
        {
            var message = JObject.Parse(GetMail(deadline));
            var html = message["mail_html"]?.ToString()
                       ?? message["mail_text"]?.ToString()
                       ?? "";
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var nodes = document.DocumentNode.SelectNodes("//*[@href]");
            if (nodes == null) return result;

            foreach (var node in nodes)
            {
                var href = NormalizeHref(node.GetAttributeValue("href", ""));
                if (!string.IsNullOrEmpty(href) && !IsStaticHref(href))
                    result.Add(href);
            }

            return result;
        }

        public string Link(string urlPattern, int deadline = 120)
        {
            var json = GetMail(deadline);
            var match = Regex.Match(json, urlPattern);
            if (match.Success) return WebUtility.HtmlDecode(match.Value);

            throw new Exception("TempMail: link not found");
        }

        public static string CreateAddress(string login, string domain)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Login is required", nameof(login));
            if (string.IsNullOrWhiteSpace(domain))
                throw new ArgumentException("Domain is required", nameof(domain));

            return login.Trim() + "@" + domain.Trim().TrimStart('@');
        }

        public static string HashEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));

            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLowerInvariant()));
                var result = new StringBuilder(bytes.Length * 2);
                foreach (var value in bytes)
                    result.Append(value.ToString("x2"));
                return result.ToString();
            }
        }

        private static JToken ParseResponse(string json)
        {
            try
            {
                return JToken.Parse(json);
            }
            catch (JsonReaderException)
            {
                throw new Exception($"TempMail: invalid API response: {json}");
            }
        }

        private static string NormalizeHref(string href)
        {
            if (string.IsNullOrWhiteSpace(href)) return "";

            href = WebUtility.HtmlDecode(href).Trim();
            if (href.StartsWith("#") ||
                href.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ||
                href.StartsWith("tel:", StringComparison.OrdinalIgnoreCase) ||
                href.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase) ||
                href.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                return "";

            return href;
        }

        private static bool IsStaticHref(string href)
        {
            var value = href.Split('?', '#')[0];
            return Regex.IsMatch(
                value,
                @"\.(png|jpe?g|gif|webp|svg|ico|css|js|woff2?|ttf|eot)$",
                RegexOptions.IgnoreCase);
        }
    }
}
