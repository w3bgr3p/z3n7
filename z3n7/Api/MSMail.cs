
using System;
using System.Collections.Generic;
using System.IO;
using ZennoLab.InterfacesLibrary.ProjectModel;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace z3n7.Api
{
    public class MSMail
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly Logger _logger;

        private string _clientId;
        private string _refreshToken;
        private string _accessToken;
        private string _proxy;
        private readonly FastDb _db;

        private const string TOKEN_URL = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
        private const string GRAPH_URL = "https://graph.microsoft.com/v1.0";

        private static readonly HttpClient _http = new HttpClient();

        public MSMail(IZennoPosterProjectModel project, FastDb db, string proxy = "", bool log = false)
        {
            _project = project;
            _db = db;
            _logger = new Logger(_project, logLevel: log ? LogLevel.Debug : LogLevel.Off);
            _proxy = proxy;
            EnsureTable();
            LoadKeys();
            if (!string.IsNullOrEmpty(_project.Var("mail")))
                RefreshAccessToken();
        }
        private void LoadKeys()
        {
            var email = _project.Var("mail");
            if (!string.IsNullOrWhiteSpace(email))
            {
                var raw = _db.dbString($"SELECT thunderbird_client_id, graph_refresh_token FROM mail WHERE mail = '{email}';");
                var parts = raw.Split('|');
                if (parts.Length < 2) throw new Exception($"MSMail: no credentials for {email}");

                _clientId     = parts[0];
                _refreshToken = parts[1];
            }
        }
        // ── Token ─────────────────────────────────────────────────────────────
        private void RefreshAccessToken()
        {
            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"]     = _clientId,
                ["refresh_token"] = _refreshToken,
                ["grant_type"]    = "refresh_token",
                ["scope"]         = "https://graph.microsoft.com/.default offline_access"
            });

            _logger?.Debug(TOKEN_URL);

            var raw = _http.PostAsync(TOKEN_URL, form).Result.Content.ReadAsStringAsync().Result;
            _logger?.Debug(raw);

            var json = JObject.Parse(raw);
            _accessToken = json["access_token"]?.ToString()
                ?? throw new Exception($"graph token refresh failed: {raw}");

            _logger?.Send("graph access token refreshed");
        }

        private string[] AuthHeaders() => new[]
        {
            $"Authorization: Bearer {_accessToken}",
            "Accept: application/json"
        };

        // ── Public API ────────────────────────────────────────────────────────

        public string Get(string endpoint)
        {
            //RefreshAccessToken();
            var url = $"{GRAPH_URL}/{endpoint.TrimStart('/')}";
            _logger?.Debug(url);
            return _project.GET(url, _proxy, AuthHeaders(), thrw: true);
        }

        public string Post(string endpoint, string jsonBody)
        {
            //RefreshAccessToken();
            var url = $"{GRAPH_URL}/{endpoint.TrimStart('/')}";
            _logger?.Debug(url);

            return _project.POST(url,  jsonBody, _proxy, AuthHeaders(), thrw: true);
        }

        public string Delete(string endpoint)
        {
            //RefreshAccessToken();
            var url = $"{GRAPH_URL}/{endpoint.TrimStart('/')}";
            _logger?.Debug(url);
            return _project.DELETE(url, _proxy, AuthHeaders(), thrw: true);
        }

        /// <summary>Inbox messages, newest first.</summary>
        public JArray GetMessages(int top = 10)
        {
            var raw = Get($"me/messages?$top={top}&$orderby=receivedDateTime desc");
            _project.ToJson(raw);
            return JObject.Parse(raw)["value"] as JArray ?? new JArray();
        }

        /// <summary>Send message.</summary>
        public void SendMail(string toEmail, string subject, string body)
        {
            var payload = new JObject
            {
                ["message"] = new JObject
                {
                    ["subject"] = subject,
                    ["body"]    = new JObject { ["contentType"] = "Text", ["content"] = body },
                    ["toRecipients"] = new JArray
                    {
                        new JObject
                        {
                            ["emailAddress"] = new JObject { ["address"] = toEmail }
                        }
                    }
                }
            };
            Post("me/sendMail", payload.ToString());
        }

        /// <summary>
        /// Отправляет письмо самому себе и проверяет его получение.
        /// Возвращает true если письмо успешно отправлено и получено.
        /// </summary>
        public bool SelfCheck(int timeoutSeconds = 30, int checkIntervalSeconds = 3)
        {
            _logger?.Send("MSMail: starting self-check");

            // Получаем свой email
            var profileRaw = Get("me");
            var profile = JObject.Parse(profileRaw);
            var myEmail = profile["mail"]?.ToString() ?? profile["userPrincipalName"]?.ToString();

            if (string.IsNullOrEmpty(myEmail))
                throw new Exception("MSMail: cannot determine own email address");

            _logger?.Debug($"MSMail: own email is {myEmail}");

            // Генерируем уникальный идентификатор для письма
            var checkId = Guid.NewGuid().ToString("N");
            var subject = $"SelfCheck {checkId}";
            var body = $"This is a self-check message sent at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";

            _logger?.Debug($"MSMail: sending self-check email with ID {checkId}");

            // Отправляем письмо самому себе
            SendMail(myEmail, subject, body);

            _logger?.Send("MSMail: self-check email sent, waiting for delivery");

            // Ждем получения письма
            var startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < timeoutSeconds)
            {
                System.Threading.Thread.Sleep(checkIntervalSeconds * 1000);

                _logger?.Debug("MSMail: checking inbox for self-check message");

                var messages = GetMessages(20);
                foreach (var msg in messages)
                {
                    var msgSubject = msg["subject"]?.ToString() ?? "";
                    if (msgSubject.Contains(checkId))
                    {
                        _logger?.Send($"MSMail: self-check PASSED - message received in {(DateTime.UtcNow - startTime).TotalSeconds:F1}s");
                        return true;
                    }
                }
            }

            _logger?.Send($"MSMail: self-check FAILED - message not received within {timeoutSeconds}s");
            return false;
        }

        /// <summary>
        /// Удаляет последнее письмо из inbox.
        /// </summary>
        public void DelLast()
        {
            _logger?.Debug("MSMail: deleting last message");

            var messages = GetMessages(1);
            if (messages.Count == 0)
            {
                _logger?.Send("MSMail: no messages to delete");
                return;
            }

            var messageId = messages[0]["id"]?.ToString();
            if (string.IsNullOrEmpty(messageId))
                throw new Exception("MSMail: cannot get message ID");

            Delete($"me/messages/{messageId}");
            _logger?.Send($"MSMail: deleted message {messageId}");
        }

        /// <summary>
        /// Удаляет все письма из inbox.
        /// </summary>
        public void CleanAll(int batchSize = 50)
        {
            _logger?.Send("MSMail: cleaning all messages");

            int totalDeleted = 0;
            while (true)
            {
                var messages = GetMessages(batchSize);
                if (messages.Count == 0)
                    break;

                _logger?.Debug($"MSMail: deleting batch of {messages.Count} messages");

                foreach (var msg in messages)
                {
                    var messageId = msg["id"]?.ToString();
                    if (!string.IsNullOrEmpty(messageId))
                    {
                        Delete($"me/messages/{messageId}");
                        totalDeleted++;
                    }
                }

                _logger?.Debug($"MSMail: deleted {totalDeleted} messages so far");
            }

            _logger?.Send($"MSMail: cleanup complete - deleted {totalDeleted} messages");
        }

        private void EnsureTable()
        {
            var createTable = @"CREATE TABLE IF NOT EXISTS mail (
                    mail                 TEXT PRIMARY KEY,
                    password              TEXT DEFAULT '',
                    access_token          TEXT DEFAULT '',
                    refresh_token         TEXT DEFAULT '',
                    thunderbird_client_id TEXT DEFAULT '',
                    graph_access_token    TEXT DEFAULT '',
                    graph_refresh_token   TEXT DEFAULT '');";
            _db.dbString(createTable);
        }
        
        public void ImportFromJson(string json)
        {
            var arr  = Newtonsoft.Json.Linq.JArray.Parse(json);
            foreach (var item in arr)
            {
                string Esc(string s) => (s ?? "").Replace("'", "''");
                _db.dbString($@" INSERT OR IGNORE INTO mail 
                    (mail, password, access_token, refresh_token,thunderbird_client_id, graph_access_token, graph_refresh_token) VALUES
                    ('{Esc(item["email"]?.ToString())}',
                     '{Esc(item["password"]?.ToString())}',
                     '{Esc(item["access_token"]?.ToString())}',
                     '{Esc(item["refresh_token"]?.ToString())}',
                     '{Esc(item["thunderbird_client_id"]?.ToString())}',
                     '{Esc(item["graph_access_token"]?.ToString())}',
                     '{Esc(item["graph_refresh_token"]?.ToString())}');
                ");
            }
        }
    }
}