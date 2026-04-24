using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;
using Newtonsoft.Json.Linq;
using z3nIO.Tools;


namespace z3nIO.Socials
{
    public class GitHubAuth
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly Logger _logger;

        public GitHubAuth(IZennoPosterProjectModel project, bool log = false)
        {
            _project = project;
            _logger  = log ? new Logger(project) : null;
        }

        public System.Net.Http.HttpClient Run()
        {
            var creds     = _project.DbGetColumns("login, email, otpsecret", "_github", true);
            var login     = creds["login"];
            var password  = creds["email"];
            var otpSecret = creds["otpsecret"];

            var cookieContainer = new System.Net.CookieContainer();
            var handler = new System.Net.Http.HttpClientHandler
            {
                CookieContainer   = cookieContainer,
                AllowAutoRedirect = false,
                UseCookies        = true,
            };

            var http = new System.Net.Http.HttpClient(handler);
            try
            {
                http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/143.0.0.0 Safari/537.36");
                http.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                http.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

                // 1. GET /login
                var loginResp = http.GetAsync("https://github.com/login").Result;
                var loginHtml = loginResp.Content.ReadAsStringAsync().Result;

                var csrf = ExtractCsrf(loginHtml);
                if (string.IsNullOrEmpty(csrf))
                    throw new Exception("login page: no csrf. status=" + loginResp.StatusCode);

                _logger?.Send("csrf ok, cookies=" + cookieContainer.Count);

                // 2. POST /session
                var loginParams = new System.Net.Http.FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("login",              login),
                    new KeyValuePair<string,string>("password",           password),
                    new KeyValuePair<string,string>("authenticity_token", csrf),
                    new KeyValuePair<string,string>("commit",             "Sign in"),
                });

                var sessionResp = http.PostAsync("https://github.com/session", loginParams).Result;
                _logger?.Send("session post status=" + sessionResp.StatusCode + ", cookies=" + cookieContainer.Count);

                var location = sessionResp.Headers.Location != null ? sessionResp.Headers.Location.ToString() : "";
                _logger?.Send("redirect to=" + location);

                if (location.Contains("sessions/two-factor"))
                {
                    // 3. GET 2FA page
                    var twoFaResp = http.GetAsync("https://github.com/sessions/two-factor/app").Result;
                    var twoFaHtml = twoFaResp.Content.ReadAsStringAsync().Result;

                    var csrf2fa = ExtractCsrf(twoFaHtml);
                    if (string.IsNullOrEmpty(csrf2fa))
                        throw new Exception("2fa page: no csrf. status=" + twoFaResp.StatusCode);

                    // 4. POST 2FA
                    var otp = _project.OtpCode(otpSecret);
                    _logger?.Send("otp=" + otp);

                    var twoFaParams = new System.Net.Http.FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string,string>("authenticity_token", csrf2fa),
                        new KeyValuePair<string,string>("app_otp",            otp),
                    });

                    var twoFaPostResp = http.PostAsync("https://github.com/sessions/two-factor", twoFaParams).Result;
                    _logger?.Send("2fa post status=" + twoFaPostResp.StatusCode);

                    if (twoFaPostResp.StatusCode != System.Net.HttpStatusCode.Found &&
                        twoFaPostResp.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new Exception("2fa failed: " + twoFaPostResp.StatusCode);
                }

                _logger?.Send("github auth ok, cookies=" + cookieContainer.Count);
                SaveCookies(cookieContainer);
                return http;
            }
            finally
            {
                //http.Dispose();
                //handler.Dispose();
            }
        }

        private void SaveCookies(System.Net.CookieContainer container)
        {
            var cookies = container.GetCookies(new Uri("https://github.com"));
            var list = new Newtonsoft.Json.Linq.JArray();
            foreach (System.Net.Cookie c in cookies)
            {
                list.Add(new Newtonsoft.Json.Linq.JObject
                {
                    ["name"]   = c.Name,
                    ["value"]  = c.Value,
                    ["domain"] = c.Domain,
                    ["path"]   = c.Path,
                });
            }
            _project.Var("cookies", list.ToString());
            _logger?.Send($"cookies saved: {cookies.Count}");
        }

        private static string ExtractCsrf(string html)
        {
            var marker = "name=\"authenticity_token\" value=\"";
            var start  = html.IndexOf(marker);
            if (start < 0) return null;
            start += marker.Length;
            var end = html.IndexOf("\"", start);
            return end > start ? html.Substring(start, end - start) : null;
        }
    }
}


namespace z3nIO
{
    // BluesmindsAuth.cs
    public class BluesmindsAuth
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly Logger _logger;

        public BluesmindsAuth(IZennoPosterProjectModel project, bool log = false)
        {
            _project = project;
            _logger  = log ? new Logger(project) : null;
        }

        public string Run(System.Net.Http.HttpClient githubHttp)
        {
            var clientId = "Ov23li6dIXgEIlgaZfwe";
            var scope    = "user:email";

            // 0. Получить временную сессию bluesminds
            var bCookies = new System.Net.CookieContainer();
            var bHandler = new System.Net.Http.HttpClientHandler
            {
                CookieContainer   = bCookies,
                AllowAutoRedirect = true,
                UseCookies        = true,
            };
            var bHttp = new System.Net.Http.HttpClient(bHandler);
            try
            {
                bHttp.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                bHttp.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
                bHttp.DefaultRequestHeaders.Add("cache-control", "no-store");

                // Получить временную сессию
                var statusResp = bHttp.GetAsync("https://api.bluesminds.com/api/status").Result;
                var statusBody = statusResp.Content.ReadAsStringAsync().Result;
                _logger?.Send("status=" + statusResp.StatusCode + " cookies=" + bCookies.Count);

                var tempSession = bCookies.GetCookies(new Uri("https://api.bluesminds.com"))["session"];
                _logger?.Send("temp session=" + (tempSession != null ? tempSession.Value.Substring(0, Math.Min(20, tempSession.Value.Length)) : "NULL"));

                // 1. Получить authorize URL от bluesminds — он генерирует state и сохраняет в сессии
                // Судя по трафику — клиент сам формирует URL с client_id от bluesminds
                // State генерирует frontend и передаёт в /api/oauth/github
                // Значит нам нужно имитировать что делает frontend

                // Сгенерировать state и сохранить его в bluesminds сессии через правильный endpoint
                // Но проще — взять state из реального authorize redirect

                var state = GenerateState();
                var authorizeUrl = string.Format(
                    "https://github.com/login/oauth/authorize?client_id={0}&state={1}&scope={2}",
                    clientId, state, scope);

                var authResp = githubHttp.GetAsync(authorizeUrl).Result;
                _logger?.Send("authorize status=" + authResp.StatusCode);

                string code;
                string stateForCallback;

                if (authResp.StatusCode == System.Net.HttpStatusCode.Found)
                {
                    var loc = authResp.Headers.Location.ToString();
                    _logger?.Send("auto-redirect loc=" + loc);
                    code = ExtractQueryParam(loc, "code");
                    stateForCallback = ExtractQueryParam(loc, "state");
                }
                else
                {
                    var authHtml = authResp.Content.ReadAsStringAsync().Result;
                    var csrf = ExtractCsrf(authHtml);
                    if (string.IsNullOrEmpty(csrf))
                        throw new Exception("authorize page: no csrf. status=" + authResp.StatusCode);

                    var postParams = new System.Net.Http.FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string,string>("authenticity_token", csrf),
                        new KeyValuePair<string,string>("authorize",          "1"),
                        new KeyValuePair<string,string>("client_id",          clientId),
                        new KeyValuePair<string,string>("state",              state),
                        new KeyValuePair<string,string>("scope",              scope),
                    });

                    var postResp = githubHttp.PostAsync("https://github.com/login/oauth/authorize", postParams).Result;
                    var loc = postResp.Headers.Location != null ? postResp.Headers.Location.ToString() : "";
                    _logger?.Send("callback url=" + loc);

                    code = ExtractQueryParam(loc, "code");
                    stateForCallback = ExtractQueryParam(loc, "state");
                }

                _logger?.Send("code=" + (code ?? "NULL") + " state=" + (stateForCallback ?? "NULL"));

                if (string.IsNullOrEmpty(code))
                    throw new Exception("no code in callback");

                // 2. Передать code+state в bluesminds с его сессией
                bHttp.DefaultRequestHeaders.Remove("new-api-user");
                bHttp.DefaultRequestHeaders.Add("new-api-user", "-1");

                var callbackUrl = string.Format(
                    "https://api.bluesminds.com/api/oauth/github?code={0}&state={1}",
                    code, stateForCallback);

                var callbackResp = bHttp.GetAsync(callbackUrl).Result;
                var callbackBody = callbackResp.Content.ReadAsStringAsync().Result;
                _logger?.Send("callback status=" + callbackResp.StatusCode);
                _logger?.Send("callback body=" + callbackBody);

                var parsed = JObject.Parse(callbackBody);
                if (!(bool)parsed["success"])
                    throw new Exception("oauth failed: " + callbackBody);

                var uid = parsed["data"]["id"].ToString();
                _project.Var("bluesminds_user_id", uid);

                var sessionCookie = bCookies.GetCookies(new Uri("https://api.bluesminds.com"))["session"];
                if (sessionCookie == null)
                    throw new Exception("no session cookie");

                var session = sessionCookie.Value;
                _project.Var("bluesminds_session", session);
                _logger?.Send("session ok uid=" + uid);

                return session;
            }
            finally
            {
                //bHttp.Dispose();
                //bHandler.Dispose();
            }
        }

        private static string GenerateState()
        {
            var chars  = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var rng    = new Random();
            var sb     = new System.Text.StringBuilder(12);
            for (int i = 0; i < 12; i++)
                sb.Append(chars[rng.Next(chars.Length)]);
            return sb.ToString();
        }

        private static string ExtractQueryParam(string url, string param)
        {
            if (string.IsNullOrEmpty(url)) return null;
            var marker = param + "=";
            var start  = url.IndexOf(marker);
            if (start < 0) return null;
            start += marker.Length;
            var end = url.IndexOf("&", start);
            return end > start ? url.Substring(start, end - start) : url.Substring(start);
        }

        private static string ExtractCsrf(string html)
        {
            var marker = "name=\"authenticity_token\" value=\"";
            var start  = html.IndexOf(marker);
            if (start < 0) return null;
            start += marker.Length;
            var end = html.IndexOf("\"", start);
            return end > start ? html.Substring(start, end - start) : null;
        }
    }
    
    public class BluesmindsToken
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly Logger _logger;

        public BluesmindsToken(IZennoPosterProjectModel project, bool log = false)
        {
            _project = project;
            _logger  = log ? new Logger(project) : null;
        }

        public string Run(string session, string userId)
        {
            var bCookies = new System.Net.CookieContainer();
            bCookies.Add(new Uri("https://api.bluesminds.com"), new System.Net.Cookie("session", session));

            var bHandler = new System.Net.Http.HttpClientHandler
            {
                CookieContainer   = bCookies,
                AllowAutoRedirect = true,
                UseCookies        = true,
            };

            var bHttp = new System.Net.Http.HttpClient(bHandler);
            try
            {
                bHttp.DefaultRequestHeaders.Add("User-Agent",    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                bHttp.DefaultRequestHeaders.Add("Accept",        "application/json, text/plain, */*");
                bHttp.DefaultRequestHeaders.Add("cache-control", "no-store");
                bHttp.DefaultRequestHeaders.Add("new-api-user",  userId);
                bHttp.DefaultRequestHeaders.Add("Referer",       "https://api.bluesminds.com/console/token");

                // 1. Создать токен
                var tokenName = "key_" + DateTime.UtcNow.Ticks.ToString().Substring(10);
                var createBody = new System.Net.Http.StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        remain_quota        = 0,
                        expired_time        = -1,
                        unlimited_quota     = true,
                        model_limits_enabled = false,
                        model_limits        = "",
                        cross_group_retry   = false,
                        group               = "",
                        allow_ips           = "",
                        name                = tokenName
                    }),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var createResp = bHttp.PostAsync("https://api.bluesminds.com/api/token/", createBody).Result;
                var createJson = createResp.Content.ReadAsStringAsync().Result;
                _logger?.Send("create token status=" + createResp.StatusCode + " body=" + createJson);

                var createResult = Newtonsoft.Json.Linq.JObject.Parse(createJson);
                if (createResult["success"] == null || !(bool)createResult["success"])
                    throw new Exception("create token failed: " + createJson);

                // 2. Получить список — взять id созданного
                var listResp = bHttp.GetAsync("https://api.bluesminds.com/api/token/?p=1&size=10").Result;
                var listJson = listResp.Content.ReadAsStringAsync().Result;
                _logger?.Send("list status=" + listResp.StatusCode);

                var listResult = Newtonsoft.Json.Linq.JObject.Parse(listJson);
                var items      = listResult["data"]["items"] as Newtonsoft.Json.Linq.JArray;

                if (items == null || items.Count == 0)
                    throw new Exception("token list empty");

                // Найти по имени
                int tokenId = -1;
                foreach (var item in items)
                {
                    if (item["name"] != null && item["name"].ToString() == tokenName)
                    {
                        tokenId = (int)item["id"];
                        break;
                    }
                }

                if (tokenId < 0)
                    tokenId = (int)items[0]["id"]; // fallback — первый

                _logger?.Send("token id=" + tokenId);

                // 3. Получить ключ
                var keyResp = bHttp.PostAsync(
                    "https://api.bluesminds.com/api/token/" + tokenId + "/key",
                    new System.Net.Http.StringContent("")
                ).Result;
                var keyJson = keyResp.Content.ReadAsStringAsync().Result;
                _logger?.Send("key status=" + keyResp.StatusCode);

                var keyResult = Newtonsoft.Json.Linq.JObject.Parse(keyJson);
                var key       = keyResult["data"]["key"].ToString();

                if (string.IsNullOrEmpty(key))
                    throw new Exception("no key in response: " + keyJson);

                _project.Var("bluesminds_key", key);
                _logger?.Send("key=" + key.Substring(0, Math.Min(10, key.Length)) + "...");

                return key;
            }
            finally
            {
                bHttp.Dispose();
                bHandler.Dispose();
            }
        }
    }
}