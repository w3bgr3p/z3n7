using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZennoLab.InterfacesLibrary.ProjectModel;


namespace z3nIO.Api
{
    public class GmailClient
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly Logger _logger;

        private string _clientId;
        private string _clientSecret;
        private string _refreshToken;
        private string _accessToken;
        private string _proxy;

        private const string TOKEN_URL = "https://oauth2.googleapis.com/token";
        private const string GMAIL_URL = "https://gmail.googleapis.com/gmail/v1/users/me";

        public GmailClient(IZennoPosterProjectModel project, bool log = false)
        {
            _project = project;
            _logger =  new Logger(_project, logLevel: (log) ? LogLevel.Debug : LogLevel.Off);
            LoadKeys();
        }

        private void LoadKeys()
        {
            var creds = _project.DbGetColumns("client_id, client_secret, refresh_token", "_api",
                where: "id = 'gmail'");
            _clientId = creds["client_id"];
            _clientSecret = creds["client_secret"];
            _refreshToken = creds["refresh_token"];
            _proxy = "";
        }

        // ── Access token через refresh_token ─────────────────────────────────────

        private static readonly System.Net.Http.HttpClient _http = new System.Net.Http.HttpClient();


        private void RefreshAccessToken()
        {
            var form = new System.Net.Http.FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _clientId,
                ["client_secret"] = _clientSecret,
                ["refresh_token"] = _refreshToken,
                ["grant_type"] = "refresh_token"
            });
            _logger?.Debug(TOKEN_URL);
            
            var raw = _http.PostAsync(TOKEN_URL, form).Result.Content.ReadAsStringAsync().Result;
            _logger?.Debug(raw);
            var json = JObject.Parse(raw);

            _accessToken = json["access_token"]?.ToString()
                           ?? throw new Exception($"gmail token refresh failed: {raw}");

            _logger?.Send("gmail access token refreshed");
        }

        private string[] AuthHeaders() => new[]
        {
            $"Authorization: Bearer {_accessToken}",
            "Accept: application/json"
        };

        // ── Получить список ID писем ──────────────────────────────────────────────

        private List<string> GetMessageIds(string query, int maxResults = 10)
        {
            var url = $"{GMAIL_URL}/messages?q={Uri.EscapeDataString(query)}&maxResults={maxResults}";
            var raw = _project.GET(url, _proxy, AuthHeaders(), thrw: true);
            _logger?.Debug(raw);
            var json = JObject.Parse(raw);

            var ids = new List<string>();
            var messages = json["messages"] as JArray;
            if (messages == null) return ids;

            foreach (var m in messages)
                ids.Add(m["id"].ToString());

            return ids;
        }

        // ── Получить тело письма по ID ────────────────────────────────────────────

        private (string subject, string body) GetMessage(string messageId)
        {
            var url = $"{GMAIL_URL}/messages/{messageId}?format=full";
            var raw = _project.GET(url, _proxy, AuthHeaders(), thrw: true);
            _logger?.Debug(raw);
            var json = JObject.Parse(raw);

            var subject = "";
            var bodyText = "";

            // subject из headers
            var headers = json["payload"]?["headers"] as JArray;
            if (headers != null)
            {
                foreach (var h in headers)
                {
                    if (h["name"]?.ToString().Equals("Subject", StringComparison.OrdinalIgnoreCase) == true)
                        subject = h["value"]?.ToString() ?? "";
                }
            }

            // body — ищем text/plain рекурсивно
            bodyText = ExtractBody(json["payload"]);

            return (subject, bodyText);
        }

        private string ExtractBody(JToken part)
        {
            if (part == null) return "";

            var mimeType = part["mimeType"]?.ToString() ?? "";

            if (mimeType == "text/plain")
            {
                var data = part["body"]?["data"]?.ToString();
                if (!string.IsNullOrEmpty(data))
                    return Encoding.UTF8.GetString(Convert.FromBase64String(
                        data.Replace('-', '+').Replace('_', '/')));
            }

            // рекурсия по parts
            var parts = part["parts"] as JArray;
            if (parts != null)
            {
                foreach (var p in parts)
                {
                    var result = ExtractBody(p);
                    if (!string.IsNullOrEmpty(result)) return result;
                }
            }

            return "";
        }

        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>
        /// Ищет 6-значный OTP в последних письмах адресованных на targetEmail.
        /// Бросает Exception если не найден.
        /// </summary>
        public string Otp(string targetEmail, int maxResults = 10)
        {
            _logger?.Debug($"gmail: search messages for {targetEmail}");
            RefreshAccessToken();

            var query = $"to:{targetEmail} newer_than:5m";
            var ids = GetMessageIds(query, maxResults);

            _logger?.Send($"gmail: found {ids.Count} messages for {targetEmail}");

            foreach (var id in ids)
            {
                var (subject, body) = GetMessage(id);

                var match = Regex.Match(subject, @"\b\d{6}\b");
                if (match.Success) return match.Value;

                match = Regex.Match(body, @"\b\d{6}\b");
                if (match.Success) return match.Value;
            }

            throw new Exception($"Gmail: OTP not found in last {ids.Count} messages for {targetEmail}");
        }

        /// <summary>
        /// Ищет ссылку в последнем письме адресованном на targetEmail.
        /// </summary>
        public string GetLink(string targetEmail)
        {
            RefreshAccessToken();

            var ids = GetMessageIds($"to:{targetEmail} newer_than:5m", 5);
            if (ids.Count == 0)
                throw new Exception($"Gmail: no messages for {targetEmail}");

            var (_, body) = GetMessage(ids[0]);

            int start = body.IndexOf("https://");
            if (start == -1) start = body.IndexOf("http://");
            if (start == -1) throw new Exception($"Gmail: no link in message body");

            var link = body.Substring(start);
            int end = link.IndexOfAny(new[] { ' ', '\n', '\r', '\t', '"' });
            if (end != -1) link = link.Substring(0, end);

            return Uri.TryCreate(link, UriKind.Absolute, out _)
                ? link
                : throw new Exception($"Gmail: invalid link: {link}");
        }

        /// <summary>
        /// Отправляет письмо из текущего ящика.
        /// </summary>
        /// <param name="to">Email получателя</param>
        /// <param name="subject">Тема письма</param>
        /// <param name="body">Текст письма</param>
        public void SendMail(string to, string subject, string body)
        {
            _logger?.Debug($"gmail: sending email to {to}");
            RefreshAccessToken();

            // Формируем RFC 2822 сообщение
            var message = new StringBuilder();
            message.AppendLine($"To: {to}");
            message.AppendLine($"Subject: {subject}");
            message.AppendLine("Content-Type: text/plain; charset=utf-8");
            message.AppendLine();
            message.AppendLine(body);

            // Кодируем в base64url
            var raw = Convert.ToBase64String(Encoding.UTF8.GetBytes(message.ToString()))
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");

            var json = new JObject
            {
                ["raw"] = raw
            };

            var url = $"{GMAIL_URL}/messages/send";
            var response = _project.POST(url, json.ToString(), _proxy, AuthHeaders(), thrw: true);
            _logger?.Debug(response);
            _logger?.Send($"gmail: email sent to {to}");
        }
    }
}