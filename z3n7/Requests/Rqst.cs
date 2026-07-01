using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.Enums.Http;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3nIO
{
    public class Rqst
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly Logger _logger;
        private readonly Request _rqst;
        private readonly bool _mask;
        private readonly string _logHost;
        private static readonly HttpClient _httpLogClient = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
        private static readonly object LockObject = new object();

        public Rqst(IZennoPosterProjectModel project, bool log = false, bool mask = false)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
           

            _logger =  new Logger(project,null,classEmoji: "↑↓", logLevel: (log) ? LogLevel.Info : LogLevel.Off);
            _rqst = new Request();
            _logHost = !string.IsNullOrEmpty(_project.GVar("logHost")) 
                ? _project.GVar("logHost").Replace("/log", "/http-log")
                : "http://localhost:10993/http-log";
            _mask = mask;
        }

        #region Nested Request Class
        private class Request
        {
            public string Method { get; set; }
            public string Url { get; set; }
            public string Body { get; set; }
            public string[] Headers { get; set; }
            public string Cookies { get; set; }
            public string CookieSource { get; set; }
            public string Proxy { get; set; }
            public string Response { get; set; }
            public string ResponseBody { get; set; }
            public int Deadline { get; set; }
            public int StatusCode { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public ResponceType ResponceType { get; set; }
            public bool UseContainer { get; set; }
            public string UserAgent { get; set; }
            public string ContentType { get; set; }
            
            public string Source { get; set; } = "z3n7";
        }
        #endregion

        #region Public API Methods
        public string GET(string url, string proxy = "", string[] headers = null, string cookies = null,
            bool log = false, bool parse = false, int deadline = 30, bool thrw = false, 
            bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
        {
            return Execute("GET", url, "", proxy, headers, cookies, log, parse, deadline, 
                thrw, useNetHttp, returnSuccessWithStatus, bodyOnly);
        }

        public string POST(string url, string body, string proxy = "", string[] headers = null, 
            string cookies = null, bool log = false, bool parse = false, int deadline = 30, 
            bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, 
            bool bodyOnly = false)
        {
            return Execute("POST", url, body, proxy, headers, cookies, log, parse, deadline, 
                thrw, useNetHttp, returnSuccessWithStatus, bodyOnly);
        }

        public string PUT(string url, string body, string proxy = "", string[] headers = null, 
            string cookies = null, bool log = false, bool parse = false, int deadline = 30, 
            bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, 
            bool bodyOnly = false)
        {
            return Execute("PUT", url, body, proxy, headers, cookies, log, parse, deadline, 
                thrw, useNetHttp, returnSuccessWithStatus, bodyOnly);
        }

        public string DELETE(string url, string proxy = "", string[] headers = null, 
            string cookies = null, bool log = false, int deadline = 30, bool thrw = false, 
            bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
        {
            return Execute("DELETE", url, "", proxy, headers, cookies, log, false, deadline, 
                thrw, useNetHttp, returnSuccessWithStatus, bodyOnly);
        }
        #endregion

        #region Core Execute Method
        private string Execute(string method, string url, string body, string proxy, 
            string[] headers, string cookies, bool log, bool parse, int deadline, 
            bool thrw, bool useNetHttp, bool returnSuccessWithStatus, bool bodyOnly)
        {
            PrepareRequest(method, url, body, proxy, headers, cookies, deadline, bodyOnly);
            
            _rqst.StartTime = DateTime.UtcNow;
            
            try
            {
                string responseBody = useNetHttp 
                    ? ExecuteViaNetHttp() 
                    : ExecuteViaZennoPoster();

                _rqst.EndTime = DateTime.UtcNow;
                _rqst.ResponseBody = responseBody;

                LogHttpTransaction(_rqst);

                if (log)
                {
                    LogStatus();
                    _logger?.Send($"response: [{responseBody}]");
                }

                if (_rqst.StatusCode < 200 || _rqst.StatusCode >= 300)
                {
                    string errorMessage = FormatErrorMessage(_rqst.StatusCode, responseBody);
                    _logger?.Send($"!W HTTP Error: [{errorMessage}] url:[{url}] proxy:[{proxy}]");

                    if (thrw) throw new Exception(errorMessage);
                    return errorMessage;
                }

                if (returnSuccessWithStatus)
                {
                    return $"{_rqst.StatusCode}\r\n\r\n{responseBody.Trim()}";
                }

                if (parse)
                {
                    ParseJson(responseBody);
                }

                return responseBody.Trim();
            }
            catch (Exception e)
            {
                string errorMessage = $"Error: {e.Message}";
                _logger?.Send($"!W RequestErr: [{e.Message}] url:[{url}] proxy:[{proxy}]");
                if (thrw) throw;
                return errorMessage;
            }
        }
        #endregion

        #region Request Preparation
        private void PrepareRequest(string method, string url, string body, string proxy, 
            string[] headers, string cookies, int deadline, bool bodyOnly)
        {
            _rqst.Method = method;
            _rqst.Url = url;
            _rqst.Body = body; // Используем Body, как в вашем классе
            _rqst.Deadline = deadline;
            _rqst.Proxy = ParseProxy(proxy);
            _rqst.ResponceType = bodyOnly ? ResponceType.BodyOnly : ResponceType.HeaderAndBody; // Фиксируем тип ответа

            string tempUserAgent;
            string tempContentType;
            _rqst.Headers = PrepareHeaders(headers, out tempUserAgent, out tempContentType);
    
            // Вот эти две строки критичны для логгера и POST!
            _rqst.UserAgent = tempUserAgent;
            _rqst.ContentType = tempContentType;

            bool isAccount = !string.IsNullOrEmpty(_project.Var("acc0"));
    
            if (cookies == "-" || !isAccount)
            {
                _rqst.Cookies = "";
                _rqst.UseContainer = false;
                _rqst.CookieSource = "null";
            }
            else
            {
                _rqst.Cookies = string.IsNullOrEmpty(cookies) 
                    ? GetCookiesForRequest(url) 
                    : cookies;
                _rqst.UseContainer = string.IsNullOrEmpty(_rqst.Cookies);
        
                if (_rqst.UseContainer)
                    _rqst.CookieSource = "container";
            }
        }
        #endregion

        #region Execute via ZennoPoster
        private string ExecuteViaZennoPoster()
        {
            string fullResponse;

            lock (LockObject)
            {
                var zennoMethod = (ZennoLab.InterfacesLibrary.Enums.Http.HttpMethod)Enum.Parse(
                    typeof(ZennoLab.InterfacesLibrary.Enums.Http.HttpMethod),
                    _rqst.Method,
                    true);

                fullResponse = ZennoPoster.HTTP.Request(
                    zennoMethod,
                    _rqst.Url,
                    _rqst.Body,
                    _rqst.ContentType,
                    _rqst.Proxy,
                    "UTF-8",
                    _rqst.ResponceType,
                    _rqst.Deadline * 1000,
                    _rqst.Cookies ?? "",
                    _rqst.UserAgent,
                    true,
                    5,
                    _rqst.Headers,
                    "",
                    false,
                    _rqst.Method != "GET",
                    _rqst.UseContainer ? _project.Profile.CookieContainer : null);
            }

            ParseResponse(fullResponse, out int statusCode, out string responseBody);
            _rqst.StatusCode = statusCode;
            _rqst.Response = fullResponse;
            return responseBody;
        }
        #endregion

        #region Execute via NetHttp
        private string ExecuteViaNetHttp()
        {
            var netHttp = new NetHttp(_project, log: null);

            var headersDic = ConvertHeadersToDictionary(_rqst.Headers);
            
            if (!headersDic.ContainsKey("Cookie") && !string.IsNullOrEmpty(_rqst.Cookies))
            {
                headersDic["Cookie"] = _rqst.Cookies.TrimEnd(';', ' ');
            }

            string response;
            
            switch (_rqst.Method.ToUpper())
            {
                case "GET":
                    response = netHttp.GET(_rqst.Url, _rqst.Proxy, headersDic, 
                        parse: false, deadline: _rqst.Deadline, throwOnFail: false);
                    break;
                case "POST":
                    response = netHttp.POST(_rqst.Url, _rqst.Body, _rqst.Proxy, headersDic, 
                        parse: false, deadline: _rqst.Deadline, throwOnFail: false);
                    break;
                case "PUT":
                    response = netHttp.PUT(_rqst.Url, _rqst.Body, _rqst.Proxy, headersDic, 
                        parse: false, deadline: _rqst.Deadline, throwOnFail: false);
                    break;
                case "DELETE":
                    response = netHttp.DELETE(_rqst.Url, _rqst.Proxy, headersDic);
                    break;
                default:
                    throw new NotSupportedException($"HTTP method {_rqst.Method} not supported");
            }

            _rqst.StatusCode = TryParseStatusFromNetHttpResponse(response);
            return response;
        }
        #endregion

        #region Cookies Management
        private string GetCookiesForRequest(string url)
        {
            string cookiesJson = _project.Var("cookies");
            _rqst.CookieSource = "var";

            if (string.IsNullOrEmpty(cookiesJson))
            {
                string cookiesBase64 = _project.DbGet("cookies", "_instance");
                if (!string.IsNullOrEmpty(cookiesBase64))
                {
                    cookiesJson = cookiesBase64.FromBase64();
                    _project.Var("cookies", cookiesJson);
                    _rqst.CookieSource = "db";
                }
            }

            if (string.IsNullOrEmpty(cookiesJson))
            {
                _rqst.CookieSource = "null";
                return null;
            }

            string domain = ExtractDomain(url);
            if (string.IsNullOrEmpty(domain)) return null;

            var cookiePairs = new List<string>();

            try
            {
                JArray cookiesArray = JArray.Parse(cookiesJson);

                foreach (var cookie in cookiesArray)
                {
                    string cookieDomain = cookie["domain"]?.ToString() ?? "";

                    if (IsDomainMatch(domain, cookieDomain))
                    {
                        string name = cookie["name"]?.ToString() ?? "";
                        string value = cookie["value"]?.ToString() ?? "";

                        if (!string.IsNullOrEmpty(name))
                        {
                            cookiePairs.Add($"{name}={value}");
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return cookiePairs.Count == 0 ? null : string.Join("; ", cookiePairs) + ";";
        }

        private static bool IsDomainMatch(string requestDomain, string cookieDomain)
        {
            if (string.IsNullOrEmpty(requestDomain) || string.IsNullOrEmpty(cookieDomain))
                return false;

            if (cookieDomain.StartsWith("."))
            {
                return requestDomain.EndsWith(cookieDomain.Substring(1)) ||
                       requestDomain == cookieDomain.Substring(1);
            }

            return requestDomain == cookieDomain;
        }

        private static string ExtractDomain(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                return uri.Host;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Headers Management
        private string[] PrepareHeaders(string[] headers, out string userAgent, out string contentType)
        {
            userAgent = _project.Profile.UserAgent;
            contentType = "application/json";

            if (headers == null || headers.Length == 0)
            {
                try
                {
                    headers = _project.Var("headers").Split('\n');
                }
                catch
                {
                    headers = new string[0];
                }
            }

            if (headers.Length == 0) return headers;

            var normalizedHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var header in headers)
            {
                if (string.IsNullOrWhiteSpace(header)) continue;
                if (header.TrimStart().StartsWith(":")) continue;

                var colonIndex = header.IndexOf(':');
                if (colonIndex == -1) continue;

                var key = header.Substring(0, colonIndex).Trim();
                var value = header.Substring(colonIndex + 1).Trim();

                if (AUTOMATIC_HEADERS.Contains(key)) continue;

                if (key.Equals("user-agent", StringComparison.OrdinalIgnoreCase))
                {
                    userAgent = value;
                    continue;
                }

                if (key.Equals("content-type", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = value;
                    continue;
                }

                normalizedHeaders[key] = value;
            }

            return normalizedHeaders.Select(kvp => $"{kvp.Key}: {kvp.Value}").ToArray();
        }

        private static readonly HashSet<string> AUTOMATIC_HEADERS = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "host", "connection", "proxy-connection", "content-length",
            "transfer-encoding", "expect", "upgrade", "te"
        };

        private static Dictionary<string, string> ConvertHeadersToDictionary(string[] headersArray)
        {
            var forbiddenHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "authority", "method", "path", "scheme", "host", "content-length",
                "connection", "upgrade", "proxy-connection", "transfer-encoding"
            };

            var headersDic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (headersArray == null) return headersDic;

            foreach (var header in headersArray)
            {
                if (string.IsNullOrWhiteSpace(header) || header.StartsWith(":")) continue;

                var colonIndex = header.IndexOf(':');
                if (colonIndex == -1) continue;

                var key = header.Substring(0, colonIndex).Trim();
                var value = header.Substring(colonIndex + 1).Trim();

                if (!forbiddenHeaders.Contains(key))
                {
                    headersDic[key] = value;
                }
            }

            return headersDic;
        }
        #endregion

        #region Proxy Management
        private string ParseProxy(string proxyString)
        {
            if (string.IsNullOrEmpty(proxyString)) return "";

            if (proxyString == "+")
            {
                string projectProxy = _project.Var("proxy");
                proxyString = !string.IsNullOrEmpty(projectProxy)
                    ? projectProxy
                    : _project.DbGet("proxy", "_instance");
            }
            else if (proxyString == "z")
            {
                proxyString = _project.DbGet("z_proxy", "_instance");
            }

            try
            {
                string scheme = "http";

                if (proxyString.Contains("//"))
                {
                    var schemeParts = proxyString.Split(new[] { "//" }, StringSplitOptions.None);
                    scheme = schemeParts[0].TrimEnd(':');
                    proxyString = schemeParts[1];
                }

                if (proxyString.Contains("@"))
                {
                    var parts = proxyString.Split('@');
                    var credentials = parts[0];
                    var proxyHost = parts[1];
                    var creds = credentials.Split(':');
                    return $"{scheme}://{creds[0]}:{creds[1]}@{proxyHost}";
                }

                return $"{scheme}://{proxyString}";
            }
            catch (Exception e)
            {
                _logger?.Send($"!W Proxy parsing error: [{e.Message}] [{proxyString}]");
                return "";
            }
        }
        #endregion

        #region Response Parsing
        private static void ParseResponse(string fullResponse, out int statusCode, out string body)
        {
            statusCode = 200;
            body = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(fullResponse))
                {
                    statusCode = 0;
                    return;
                }

                int firstLineEnd = fullResponse.IndexOf("\r\n");
                if (firstLineEnd == -1)
                {
                    body = fullResponse.Trim();
                    return;
                }

                string statusLine = fullResponse.Substring(0, firstLineEnd);
                string[] parts = statusLine.Split(' ');
                
                if (parts.Length >= 2)
                {
                    int.TryParse(parts[1], out statusCode);
                }

                int bodyStart = fullResponse.IndexOf("\r\n\r\n");
                if (bodyStart != -1)
                {
                    body = fullResponse.Substring(bodyStart + 4).Trim();
                }
            }
            catch
            {
                statusCode = 200;
                body = fullResponse.Trim();
            }
        }

        private static int TryParseStatusFromNetHttpResponse(string response)
        {
            if (string.IsNullOrEmpty(response)) return 0;

            if (response.Contains("!!!"))
            {
                var parts = response.Split(new[] { "!!!" }, StringSplitOptions.None);
                if (parts.Length > 0 && int.TryParse(parts[0].Trim(), out int code))
                {
                    return code;
                }
            }

            if (response.StartsWith("Error:") || response.StartsWith("Ошибка:"))
            {
                return 0;
            }

            return 200;
        }
        #endregion

        #region Logging
        private void LogHttpTransaction(Request req)
        {
            Task.Run(async () =>
            {
                try
                {
                    await SendLogAsync(req);
                }
                catch { }
            });
        }

        private async Task SendLogAsync(Request req)
        {
            try
            {
                int durationMs = (int)(req.EndTime - req.StartTime).TotalMilliseconds;

                var httpLog = new
                {
                    timestamp = DateTime.UtcNow.AddHours(-5).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    method = req.Method,
                    url = req.Url,
                    statusCode = req.StatusCode,
                    durationMs = durationMs,
                    proxy = MaskProxyCredentials(req.Proxy),
                    request = new
                    {
                        headers = req.Headers,
                        cookies = req.Cookies,
                        cookiesSource = req.CookieSource,
                        body = req.Body,
                    },
                    response = new
                    {
                        body = req.ResponseBody
                    },
                    machine = Environment.MachineName,
                    project = _project.ProjectName(),
                    account = _project.Var("acc0") ?? "",
                    session = _project.Var("varSessionId") ?? "",
                    port = _project.Var("port") ?? "",
                    pid = _project.Var("pid") ?? "",
                    task_id  = _project?.TaskId  ?? "",
                    source = req.Source
                };

                string json = JsonConvert.SerializeObject(httpLog);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    await _httpLogClient.PostAsync(_logHost, content);
                    
                    string trafficHost = _logHost.Replace("/http-log", "/traffic");
                    if (trafficHost != _logHost)
                        await _httpLogClient.PostAsync(trafficHost, content);
                }
            }
            catch { }
        }

        private static string MaskProxyCredentials(string proxy)
        {
            if (string.IsNullOrEmpty(proxy)) return "";

            try
            {
                if (proxy.Contains("@"))
                {
                    var parts = proxy.Split('@');
                    if (parts.Length == 2)
                    {
                        return $"***:***@{parts[1]}";
                    }
                }
            }
            catch { }

            return proxy;
        }

        private void LogStatus()
        {
            if (_rqst.StatusCode >= 200 && _rqst.StatusCode < 300)
            {
                _logger?.Send($"✓ HTTP {_rqst.StatusCode}");
            }
            else if (_rqst.StatusCode == 429)
            {
                _logger?.Send($"!W HTTP 429 Rate Limited | url:[{_rqst.Url}] proxy:[{_rqst.Proxy}]");
            }
            else if (_rqst.StatusCode >= 400 && _rqst.StatusCode < 500)
            {
                _logger?.Send($"!W HTTP {_rqst.StatusCode} Client Error | url:[{_rqst.Url}] proxy:[{_rqst.Proxy}]");
            }
            else if (_rqst.StatusCode >= 500)
            {
                _logger?.Send($"!W HTTP {_rqst.StatusCode} Server Error | url:[{_rqst.Url}] proxy:[{_rqst.Proxy}]");
            }
            else if (_rqst.StatusCode == 0)
            {
                _logger?.Send($"!W HTTP Request Failed | url:[{_rqst.Url}] proxy:[{_rqst.Proxy}]");
            }
        }
        #endregion

        #region Helpers
        private string FormatErrorMessage(int statusCode, string body)
        {
            string statusText = GetStatusText(statusCode);
            string bodyPreview = body.Length > 100 ? body.Substring(0, 100) + "..." : body;

            return string.IsNullOrWhiteSpace(body)
                ? $"{statusCode} {statusText}"
                : $"{statusCode} {statusText}: {bodyPreview}";
        }

        private static string GetStatusText(int statusCode)
        {
            switch (statusCode)
            {
                case 0: return "Connection Failed";
                case 400: return "Bad Request";
                case 401: return "Unauthorized";
                case 403: return "Forbidden";
                case 404: return "Not Found";
                case 405: return "Method Not Allowed";
                case 408: return "Request Timeout";
                case 429: return "Too Many Requests";
                case 500: return "Internal Server Error";
                case 502: return "Bad Gateway";
                case 503: return "Service Unavailable";
                case 504: return "Gateway Timeout";
                default:
                    if (statusCode >= 400 && statusCode < 500) return "Client Error";
                    if (statusCode >= 500) return "Server Error";
                    return "Unknown Error";
            }
        }

        private void ParseJson(string json)
        {
            try
            {
                _project.Json.FromString(json);
            }
            catch (Exception ex)
            {
                _logger?.Send($"!W JSON parsing error: {ex.Message}] [{json}]");
            }
        }
        #endregion
    }

    #region Extension Methods for Backward Compatibility
    public static class RqstExtensions
    {
        public static string GET(this IZennoPosterProjectModel project, string url, 
            string proxy = "", string[] headers = null, string cookies = null,
            bool log = false, bool parse = false, int deadline = 30, bool thrw = false,
            bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
        {
            var rqst = new Rqst(project, log);
            return rqst.GET(url, proxy, headers, cookies, log, parse, deadline, 
                thrw, useNetHttp, returnSuccessWithStatus, bodyOnly);
        }

        public static string POST(this IZennoPosterProjectModel project, string url, string body,
            string proxy = "", string[] headers = null, string cookies = null,
            bool log = false, bool parse = false, int deadline = 30, bool thrw = false,
            bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
        {
            var rqst = new Rqst(project, log);
            return rqst.POST(url, body, proxy, headers, cookies, log, parse, deadline,
                thrw, useNetHttp, returnSuccessWithStatus, bodyOnly);
        }

        public static string PUT(this IZennoPosterProjectModel project, string url, string body,
            string proxy = "", string[] headers = null, string cookies = null,
            bool log = false, bool parse = false, int deadline = 30, bool thrw = false,
            bool useNetHttp = false, bool returnSuccessWithStatus = false)
        {
            var rqst = new Rqst(project, log);
            return rqst.PUT(url, body, proxy, headers, cookies, log, parse, deadline,
                thrw, useNetHttp, returnSuccessWithStatus);
        }

        public static string DELETE(this IZennoPosterProjectModel project, string url,
            string proxy = "", string[] headers = null, string cookies = null,
            bool log = false, int deadline = 30, bool thrw = false,
            bool useNetHttp = false, bool returnSuccessWithStatus = false)
        {
            var rqst = new Rqst(project, log);
            return rqst.DELETE(url, proxy, headers, cookies, log, deadline,
                thrw, useNetHttp, returnSuccessWithStatus);
        }
    }
    #endregion
}