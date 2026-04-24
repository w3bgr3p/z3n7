using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;
using Newtonsoft.Json.Linq;
using z3nIO.Tools;

namespace z3nIO.Socials
{
    /// <summary>
    /// OAuth2 авторизация Twitter для сторонних сайтов (dachain и подобных).
    /// 
    /// UI флоу (требует Instance):
    ///   Открывает страницу авторизации в браузере, логинится если нужно, кликает "Authorize app"
    /// 
    /// API флоу (без браузера):
    ///   1. GET /i/oauth2/authorize       — HTML страница
    ///   2. GET /i/api/2/oauth2/authorize — JSON, получаем auth_code
    ///   3. POST /i/api/2/oauth2/authorize — approval=true + auth_code → code
    ///   4. GET {redirectUri}?code=XXX    — callback на целевой сайт
    /// </summary>
    public class TwitterOAuth2
    {
        private const string BEARER       = "AAAAAAAAAAAAAAAAAAAAANRILgAAAAAAnNwIzUejRCOuH5E6I8xnZz4puTs%3D1Zv7ttfk8LF81IUq16cHjhLTvJu4FA33AGWWjCpTnA";
        private const string CLIENT_ID    = "aHpDd2FQVVg0UlFqTnBaNWJWaEs6MTpjaQ";
        private const string REDIRECT_URI = "https://waitlist.dachain.io/auth/x/callback/";
        private const string SCOPE        = "tweet.read users.read follows.read offline.access";

        private readonly IZennoPosterProjectModel _project;
        private readonly Instance _instance;
        private readonly Rqst _rqst;
        private readonly Logger _log;

        private string _token;
        private string _ct0;
        private string _login;
        private string _pass;
        private string _2fa;
        private string _cookies;

        /// <summary>
        /// Конструктор с Instance — доступны UI и API методы
        /// </summary>
        public TwitterOAuth2(IZennoPosterProjectModel project, Instance instance, Logger log = null)
        {
            _project  = project;
            _instance = instance;
            _log      = log;
            _rqst     = new Rqst(project, log: false);
            LoadCreds();
        }

        /// <summary>
        /// Конструктор без Instance — только API метод
        /// </summary>
        public TwitterOAuth2(IZennoPosterProjectModel project, Logger log = null)
        {
            _project  = project;
            _instance = null;
            _log      = log;
            _rqst     = new Rqst(project, log: false);
            LoadCreds();
        }

        // ─── Credentials ─────────────────────────────────────────────────────

        private void LoadCreds()
        {
            var creds = _project.DbGetColumns("login, password, otpsecret, token, ct0", "_twitter");
            _login = creds["login"];
            _pass  = creds["password"];
            _2fa   = creds.ContainsKey("otpsecret") ? creds["otpsecret"] : "";
            _token = creds["token"];
            _ct0   = creds.ContainsKey("ct0") ? creds["ct0"] : "";

            var raw  = _project.DbGet("cookies", "_instance");
            _cookies = BuildCookieString(raw);
        }

        private string BuildCookieString(string base64Cookies)
        {
            if (string.IsNullOrEmpty(base64Cookies))
                return $"auth_token={_token}; ct0={_ct0};";

            try
            {
                var json = base64Cookies.FromBase64();
                json = Cookies.ConvertCookieFormat(json, "json");
                var arr = JArray.Parse(json);

                string guest_id = "", kdt = "", twid = "";

                foreach (var c in arr)
                {
                    var name = c["name"]?.ToString();
                    var val  = c["value"]?.ToString();
                    if (name == "auth_token") _token   = val;
                    if (name == "ct0")        _ct0     = val;
                    if (name == "guest_id")   guest_id = val;
                    if (name == "kdt")        kdt      = val;
                    if (name == "twid")       twid     = val;
                }

                return $"guest_id={guest_id}; kdt={kdt}; auth_token={_token}; " +
                       $"guest_id_ads={guest_id}; guest_id_marketing={guest_id}; " +
                       $"lang=en; twid={twid}; ct0={_ct0};";
            }
            catch
            {
                return $"auth_token={_token}; ct0={_ct0};";
            }
        }

        // ─── UI METHOD ────────────────────────────────────────────────────────

        /// <summary>
        /// Авторизация через браузер (Instance обязателен).
        /// Открывает страницу авторизации, логинится если нужно, кликает "Authorize app".
        /// Порт из старого XAuth().
        /// </summary>
        public void AuthorizeUI()
        {
            if (_instance == null)
                throw new InvalidOperationException("Instance required for UI authorization");

            DateTime deadline = DateTime.Now.AddSeconds(120);

            check:
            if (DateTime.Now > deadline) throw new Exception("[OAuth2 UI] timeout");

            _instance.HeClick(("button", "innertext", "Accept\\ all\\ cookies", "regexp", 0), deadline: 0, thr0w: false);
            _instance.HeClick(("button", "data-testid", "xMigrationBottomBar", "regexp", 0), deadline: 0, thr0w: false);

            Thread.Sleep(2000);

            string state = null;

            // Ошибки
            if (!_instance.ActiveTab.FindElementByXPath("//*[contains(text(), 'Sorry, we could not find your account')]", 0).IsVoid)
                state = "NotFound";
            else if (!_instance.ActiveTab.FindElementByXPath("//*[contains(text(), 'Your account is suspended')]", 0).IsVoid)
                state = "Suspended";
            else if (!_instance.ActiveTab.FindElementByXPath("//*[contains(text(), 'Wrong password!')]", 0).IsVoid)
                state = "WrongPass";
            else if (!_instance.ActiveTab.FindElementByAttribute("span", "innertext", "Oops,\\ something\\ went\\ wrong.\\ Please\\ try\\ again\\ later.", "regexp", 0).IsVoid)
                state = "SomethingWentWrong";
            else if (!_instance.ActiveTab.FindElementByAttribute("*", "innertext", "Suspicious\\ login\\ prevented", "regexp", 0).IsVoid)
                state = "SuspiciousLogin";

            // Шаги логина
            else if (!_instance.ActiveTab.FindElementByAttribute("a", "data-testid", "login", "regexp", 0).IsVoid)
                state = "ClickLogin";
            else if (!_instance.ActiveTab.FindElementByAttribute("input:text", "autocomplete", "username", "text", 0).IsVoid)
                state = "InputLogin";
            else if (!_instance.ActiveTab.FindElementByAttribute("input:password", "autocomplete", "current-password", "text", 0).IsVoid)
                state = "InputPass";
            else if (!_instance.ActiveTab.FindElementByAttribute("input:text", "data-testid", "ocfEnterTextTextInput", "text", 0).IsVoid)
                state = "InputOTP";

            // Страница подтверждения OAuth
            else if (!_instance.ActiveTab.FindElementByAttribute("li", "data-testid", "UserCell", "regexp", 0).IsVoid)
                state = "CheckUser";

            // Уже авторизовано — редирект на целевой сайт
            else if (!_instance.ActiveTab.URL.Contains("twitter.com") && !_instance.ActiveTab.URL.Contains("x.com"))
                state = "Done";

            _log?.Send($"[OAuth2 UI] state={state} url={_instance.ActiveTab.URL.Substring(0, Math.Min(60, _instance.ActiveTab.URL.Length))}");

            switch (state)
            {
                case "NotFound":
                case "Suspended":
                case "SuspiciousLogin":
                case "WrongPass":
                case "SomethingWentWrong":
                    throw new Exception($"[OAuth2 UI] {state}");

                case "ClickLogin":
                    _instance.HeClick(("a", "data-testid", "login", "regexp", 0));
                    goto check;

                case "InputLogin":
                    _instance.JsSet("[autocomplete='username']", _login);
                    _instance.HeClick(("span", "innertext", "Next", "regexp", 1), "clickOut");
                    goto check;

                case "InputPass":
                    _instance.JsSet("[name='password']", _pass);
                    _instance.HeClick(("button", "data-testid", "LoginForm_Login_Button", "regexp", 0), "clickOut");
                    goto check;

                case "InputOTP":
                    _instance.JsSet("[name='text']", Otp.Offline(_2fa));
                    _instance.HeClick(("span", "innertext", "Next", "regexp", 1), "clickOut");
                    goto check;

                case "CheckUser":
                    var userdata = _instance.HeGet(("li", "data-testid", "UserCell", "regexp", 0));
                    if (userdata.Contains(_login))
                    {
                        _instance.HeClick(("button", "data-testid", "OAuth_Consent_Button", "regexp", 0));
                        goto check;
                    }
                    throw new Exception("[OAuth2 UI] wrong account on consent page");

                case "Done":
                    _log?.Send($"[OAuth2 UI] Done. Final URL: {_instance.ActiveTab.URL}");
                    return;

                default:
                    goto check;
            }
        }

        // ─── API METHOD ───────────────────────────────────────────────────────

        /// <summary>
        /// Авторизация через HTTP запросы без браузера.
        /// Возвращает ответ callback сайта или пустую строку при ошибке.
        /// </summary>
        public string AuthorizeAPI(
            string clientId    = CLIENT_ID,
            string redirectUri = REDIRECT_URI,
            string scope       = SCOPE)
        {
            var verifier  = GenerateCodeVerifier();
            var challenge = DeriveCodeChallenge(verifier);
            var state     = RandomState();

            _log?.Send($"[OAuth2 API] state={state}");

            // ШАГ 1: GET HTML страница — без API заголовков
            var htmlUrl = BuildHtmlPageUrl(clientId, redirectUri, scope, state, challenge);
            _rqst.GET(htmlUrl, proxy: "+", headers: BuildHeadersForPage(), cookies: _cookies);

            // ШАГ 2: GET /i/api/2/oauth2/authorize — получаем auth_code
            var apiUrl  = BuildApiUrl(clientId, redirectUri, scope, state, challenge);
            var apiResp = _rqst.GET(apiUrl, proxy: "+", headers: BuildHeadersForApi(), cookies: _cookies);

            _log?.Send($"[OAuth2 API] Step2: {apiResp.Substring(0, Math.Min(200, apiResp.Length))}");

            string authCode = "";
            try
            {
                authCode = JObject.Parse(apiResp)["auth_code"]?.ToString() ?? "";
            }
            catch
            {
                _log?.Send("[OAuth2 API] !W Failed to parse auth_code");
                return "";
            }

            if (string.IsNullOrEmpty(authCode))
            {
                _log?.Send("[OAuth2 API] !W auth_code is empty");
                return "";
            }

            _log?.Send($"[OAuth2 API] auth_code ok");

            // ШАГ 3: POST approval — получаем code
            var code = PostApproval(authCode);
            if (string.IsNullOrEmpty(code))
            {
                _log?.Send("[OAuth2 API] !W code extraction failed");
                return "";
            }

            _log?.Send($"[OAuth2 API] code ok");

            // ШАГ 4: GET callback на целевой сайт
            var callbackUrl  = $"{redirectUri}?state={state}&code={code}";
            var callbackResp = _rqst.GET(callbackUrl, proxy: "+", returnSuccessWithStatus: true);

            _log?.Send($"[OAuth2 API] Done: {callbackResp.Substring(0, Math.Min(100, callbackResp.Length))}");
            return callbackResp;
        }

        // ─── POST approval ────────────────────────────────────────────────────

        private string PostApproval(string authCode)
        {
            var body = BuildFormBody(new Dictionary<string, string>
            {
                ["approval"]  = "true",
                ["code"]      = authCode,
            });

            var resp = _rqst.POST(
                "https://twitter.com/i/api/2/oauth2/authorize",
                body,
                proxy:   "+",
                headers: BuildHeadersForApi(),
                cookies: _cookies,
                returnSuccessWithStatus: true
            );

            _log?.Send($"[OAuth2 API] Step3: {resp.Substring(0, Math.Min(200, resp.Length))}");
            return ExtractCode(resp);
        }

        // ─── PKCE ─────────────────────────────────────────────────────────────

        private static string GenerateCodeVerifier()
        {
            var bytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(bytes);
            return Base64UrlEncode(bytes);
        }

        private static string DeriveCodeChallenge(string verifier)
        {
            using (var sha = SHA256.Create())
                return Base64UrlEncode(sha.ComputeHash(Encoding.ASCII.GetBytes(verifier)));
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static string RandomState()
        {
            var bytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        // ─── Headers ─────────────────────────────────────────────────────────

        private string[] BuildHeadersForPage()
        {
            return new[]
            {
                $"User-Agent: {_project.Profile.UserAgent}",
                "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8",
                "Accept-Language: en-US,en;q=0.9",
                "sec-fetch-dest: document",
                "sec-fetch-mode: navigate",
                "sec-fetch-site: cross-site",
                "upgrade-insecure-requests: 1",
            };
        }

        private string[] BuildHeadersForApi()
        {
            return new[]
            {
                $"User-Agent: {_project.Profile.UserAgent}",
                "Accept: */*",
                "Accept-Language: en-US,en;q=0.9",
                $"authorization: Bearer {BEARER}",
                "content-type: application/x-www-form-urlencoded",
                $"x-csrf-token: {_ct0}",
                "x-twitter-active-user: yes",
                "x-twitter-auth-type: OAuth2Session",
                "x-twitter-client-language: en",
                "sec-ch-ua: \"Chromium\";v=\"112\", \"Google Chrome\";v=\"112\", \";Not A Brand\";v=\"99\"",
                "sec-ch-ua-mobile: ?0",
                "sec-ch-ua-platform: \"Windows\"",
                "sec-fetch-dest: empty",
                "sec-fetch-mode: cors",
                "sec-fetch-site: same-origin",
            };
        }

        // ─── URL builders ─────────────────────────────────────────────────────

        private static string BuildHtmlPageUrl(
            string clientId, string redirectUri, string scope, string state, string challenge)
        {
            return "https://twitter.com/i/oauth2/authorize" +
                   $"?response_type=code" +
                   $"&client_id={Uri.EscapeDataString(clientId)}" +
                   $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                   $"&scope={scope.Replace(" ", "+")}" +
                   $"&state={state}" +
                   $"&code_challenge={challenge}" +
                   $"&code_challenge_method=S256";
        }

        private static string BuildApiUrl(
            string clientId, string redirectUri, string scope, string state, string challenge)
        {
            return "https://twitter.com/i/api/2/oauth2/authorize" +
                   $"?client_id={Uri.EscapeDataString(clientId)}" +
                   $"&code_challenge={challenge}" +
                   $"&code_challenge_method=S256" +
                   $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                   $"&response_type=code" +
                   $"&scope={Uri.EscapeDataString(scope)}" +
                   $"&state={state}";
        }

        // ─── Helpers ─────────────────────────────────────────────────────────

        private static string BuildFormBody(Dictionary<string, string> fields)
        {
            var parts = new List<string>();
            foreach (var kv in fields)
                parts.Add($"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}");
            return string.Join("&", parts);
        }

        private static string ExtractCode(string response)
        {
            if (string.IsNullOrEmpty(response)) return "";

            // JSON { "redirect_uri": "...?code=XXX" }
            var jsonMatch = Regex.Match(response, @"""redirect_uri""\s*:\s*""([^""]+)""");
            if (jsonMatch.Success)
            {
                var m = Regex.Match(jsonMatch.Groups[1].Value, @"[?&]code=([^&\s""]+)");
                if (m.Success) return m.Groups[1].Value;
            }

            // Location header
            var locMatch = Regex.Match(response, @"[Ll]ocation:\s*\S*[?&]code=([^&\s\r\n]+)");
            if (locMatch.Success) return locMatch.Groups[1].Value;

            // code в теле
            var bodyMatch = Regex.Match(response, @"[?&]code=([^&\s\r\n""]+)");
            if (bodyMatch.Success) return bodyMatch.Groups[1].Value;

            return "";
        }
    }
}