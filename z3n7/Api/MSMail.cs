
using System;
using System.Collections.Generic;
using System.IO;
using ZennoLab.InterfacesLibrary.ProjectModel;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace z3nIO.Api
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

        public MSMail(IZennoPosterProjectModel project, FastDb db, bool log = false)
        {
            _project = project;
            _db = db;
            _logger = new Logger(_project, logLevel: log ? LogLevel.Debug : LogLevel.Off);
            LoadKeys();
        }
        private void LoadKeys()
        {
            var email = _project.Var("mail");
            var raw = _db.dbString($"SELECT thunderbird_client_id, graph_refresh_token FROM mail WHERE mail = '{email}';");
            var parts = raw.Split('|');
            _clientId     = parts[0];
            _refreshToken = parts[1];
            _proxy        = "";
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
            RefreshAccessToken();
            var url = $"{GRAPH_URL}/{endpoint.TrimStart('/')}";
            _logger?.Debug(url);
            return _project.GET(url, _proxy, AuthHeaders(), thrw: true);
        }

        public string Post(string endpoint, string jsonBody)
        {
            RefreshAccessToken();
            var url = $"{GRAPH_URL}/{endpoint.TrimStart('/')}";
            _logger?.Debug(url);
            return _project.POST(url, _proxy, jsonBody, AuthHeaders(), thrw: true);
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

        public void ImportFromJson(string json)
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