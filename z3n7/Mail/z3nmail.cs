using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;

using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Api
{
    public class z3nmail
    {
        private readonly string _baseUrl= "https://mail.autoz3n.xyz";
        private readonly string _apikey;
        private readonly IZennoPosterProjectModel _project;
        private readonly bool _useNetHttp;
        private readonly bool _log;

        public z3nmail(
            IZennoPosterProjectModel project,
            string apikey = null,
            string baseUrl = null,
            bool useNetHttp = false,
            bool log = false)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _useNetHttp = useNetHttp;
            _log = log;
            _apikey = project.ReadEnv("Z3NMAIL_API_KEY"); 
            
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

        /// <summary>Создает временный почтовый ящик. Возвращает [id, email].</summary>
        /// <param name="domain">Желаемый домен (например "autoz3n.xyz" или "z3nd3v.xyz"), если null — выбирается случайно.</param>
        /// <param name="prefix">Желаемый префикс (например "alex.miller"), если null — генерируется автоматически.</param>
        /// <param name="ttl">Время жизни ящика в секундах (по умолчанию 1200 = 20 минут).</param>
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

        /// <summary>Быстрое получение OTP-кода (4-8 знаков). Опрашивает API до получения или таймаута.</summary>
        /// <param name="deadline">Таймаут ожидания в секундах (по умолчанию 60 сек).</param>
        /// <param name="id">ID ящика или email (если null, берется из переменной mailId).</param>
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

        /// <summary>Ожидает письмо и возвращает полное тело последнего письма (HTML / текст).</summary>
        /// <param name="deadline">Таймаут ожидания в секундах (по умолчанию 60 сек).</param>
        /// <param name="id">ID ящика или email (если null, берется из переменной mailId).</param>
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

        /// <summary>Извлекает ссылки активации/подтверждения из полученного письма.</summary>
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
                    // 1. Проверяем готовый массив verificationLinks от бэкенда
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

                    // 2. Если массив пуст, парсим HTML
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

        /// <summary>Досрочно уничтожает ящик и все его письма.</summary>
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

        /// <summary>Получает список доступных доменов.</summary>
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
