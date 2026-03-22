using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;
using Newtonsoft.Json.Linq;

namespace z3n7
{
    /// <summary>
    /// Работа с трафиком браузера - поиск и извлечение данных из HTTP запросов/ответов
    /// </summary>
    public partial class Traffic
    {
        #region Fields & Constructor

        private readonly IZennoPosterProjectModel _project;
        private readonly Instance _instance;
        private readonly Logger _logger;

        // Внутренний кэш (скрыт от пользователя)
        private List<TrafficElement> _cache;
        private DateTime _cacheTime;
        private const int CACHE_LIFETIME_SECONDS = 2;

        public Traffic(IZennoPosterProjectModel project, Instance instance, Logger log = null)
        {
            _project = project;
            _instance = instance;
            _logger = log;
            _instance.UseTrafficMonitoring = true;
        }

        #endregion

        #region Find Traffic Elements (Поиск элементов трафика)

        /// <summary>
        /// Найти первый элемент трафика по URL (с ожиданием)
        /// </summary>
        /// <param name="url">URL или его часть для поиска</param>
        /// <param name="strict">true = точное совпадение, false = содержит подстроку</param>
        /// <param name="timeoutSeconds">Таймаут ожидания в секундах</param>
        /// <param name="retryDelaySeconds">Задержка между попытками</param>
        public TrafficElement FindTrafficElement(string url, bool strict = false, 
            int timeoutSeconds = 15, int retryDelaySeconds = 1, bool reload = false)
        {
            if (reload) ReloadPage();
            _project.Deadline();
            _instance.UseTrafficMonitoring = true;

            var startTime = DateTime.Now;
            var timeout = TimeSpan.FromSeconds(timeoutSeconds);
            int attemptNumber = 0;

            while (DateTime.Now - startTime < timeout)
            {
                _project.Deadline(timeoutSeconds);
                attemptNumber++;

                _logger?.Send($"Attempt #{attemptNumber} searching URL: {url}");

                var element = SearchInCache(url, strict);
                if (element != null)
                {
                    _logger?.Send($"✓ Found traffic for: {url}");
                    return element;
                }

                Thread.Sleep(1000 * retryDelaySeconds);
            }

            throw new TimeoutException(
                $"Traffic element not found for URL '{url}' within {timeoutSeconds} seconds");
        }

        /// <summary>
        /// Найти все элементы трафика по URL (без ожидания, работает с текущим кэшем)
        /// </summary>
        /// <param name="url">URL или его часть для поиска</param>
        /// <param name="strict">true = точное совпадение, false = содержит подстроку</param>
        public List<TrafficElement> FindAllTrafficElements(string url, bool strict = false)
        {
            UpdateCacheIfNeeded();

            var matches = new List<TrafficElement>();

            foreach (var element in _cache)
            {
                bool isMatch = strict 
                    ? element.Url == url 
                    : element.Url.Contains(url);

                if (isMatch)
                {
                    matches.Add(element);
                }
            }

            _logger?.Send($"Found {matches.Count} traffic elements for: {url}");

            return matches;
        }

        /// <summary>
        /// Получить весь текущий трафик (все элементы)
        /// </summary>
        public List<TrafficElement> GetAllTraffic()
        {
            UpdateCacheIfNeeded();
            return new List<TrafficElement>(_cache);
        }

        #endregion

        #region Get Specific Data (Получение конкретных данных - короткие пути)

        /// <summary>
        /// Получить тело ответа (response body) по URL
        /// </summary>
        public string GetResponseBody(string url, bool strict = false, int timeoutSeconds = 15)
        {
            var element = FindTrafficElement(url, strict, timeoutSeconds);
            return element.ResponseBody;
        }

        /// <summary>
        /// Получить тело запроса (request body) по URL
        /// </summary>
        public string GetRequestBody(string url, bool strict = false, int timeoutSeconds = 15)
        {
            var element = FindTrafficElement(url, strict, timeoutSeconds);
            return element.RequestBody;
        }

        /// <summary>
        /// Получить заголовок из запроса (request header)
        /// </summary>
        public string GetRequestHeader(string url, string headerName, bool strict = false, 
            int timeoutSeconds = 15)
        {
            var element = FindTrafficElement(url, strict, timeoutSeconds);
            return element.GetRequestHeader(headerName);
        }

        /// <summary>
        /// Получить заголовок из ответа (response header)
        /// </summary>
        public string GetResponseHeader(string url, string headerName, bool strict = false, 
            int timeoutSeconds = 15)
        {
            var element = FindTrafficElement(url, strict, timeoutSeconds);
            return element.GetResponseHeader(headerName);
        }

        /// <summary>
        /// Получить все заголовки запроса в виде словаря
        /// </summary>
        public Dictionary<string, string> GetAllRequestHeaders(string url, bool strict = false, 
            int timeoutSeconds = 15)
        {
            var element = FindTrafficElement(url, strict, timeoutSeconds);
            return element.GetAllRequestHeaders();
        }

        /// <summary>
        /// Получить все заголовки ответа в виде словаря
        /// </summary>
        public Dictionary<string, string> GetAllResponseHeaders(string url, bool strict = false, 
            int timeoutSeconds = 15)
        {
            var element = FindTrafficElement(url, strict, timeoutSeconds);
            return element.GetAllResponseHeaders();
        }

        
        public enum Filter
        {
            NoFilter,
            /// <summary>Всё включено, файлы исключены</summary>
            Debug,

            /// <summary>URL + Body + Headers + Cookies</summary>
            ApiPrivate,

            /// <summary>URL + Body только</summary>
            Api
        }
        
        public string GetApiStructure(
            string urlFilter = "api",
            Filter preset = Filter.Api)
        {
            switch (preset)
            {
                case Filter.NoFilter:
                    return GetApiStructure(
                        urlFilter,
                        includeStatus:  true,
                        includeHeaders: true,
                        includeBodies:  true,
                        includeCookies: true,
                        excludeFiles:   false);
                
                case Filter.Debug:
                    return GetApiStructure(
                        urlFilter,
                        includeStatus:  true,
                        includeHeaders: true,
                        includeBodies:  true,
                        includeCookies: true,
                        excludeFiles:   true);

                case Filter.ApiPrivate:
                    return GetApiStructure(
                        urlFilter,
                        includeStatus:  false,
                        includeHeaders: true,
                        includeBodies:  true,
                        includeCookies: true,
                        excludeFiles:   true);

                case Filter.Api:
                    return GetApiStructure(
                        urlFilter,
                        includeStatus:  false,
                        includeHeaders: false,
                        includeBodies:  true,
                        includeCookies: false,
                        excludeFiles:   true);

                default:
                    throw new ArgumentOutOfRangeException("preset", preset, null);
            }
        }
        
        public string GetApiStructure(
            string urlFilter = "api",
            bool includeStatus = true,   // 0
            bool includeHeaders = true,  // 1
            bool includeBodies = true,   // 2
            bool includeCookies = true,  // 3
            bool excludeFiles = true     // 4
        )
        {
            var all = FindAllTrafficElements(urlFilter);
            var uniqueEndpoints = new Dictionary<string, JObject>();

            foreach (var el in all)
            {
                // --- Фильтрация файлов (Пункт 4) ---
                if (excludeFiles)
                {
                    // Отсекаем query-параметры для чистоты проверки
                    string urlPath = el.Url.Split('?')[0];
                    bool hasSlashAtEnd = urlPath.EndsWith("/");
                    string lastSegment = urlPath.Substring(urlPath.LastIndexOf('/') + 1);
                    
                    // Если нет слэша в конце и в сегменте есть точка — это файл
                    if (!hasSlashAtEnd && lastSegment.Contains(".")) 
                        continue;
                }

                string key = $"{el.Method}:{el.Url}";
                if (uniqueEndpoints.ContainsKey(key))
                    continue;

                var item = new JObject();
                item["method"] = el.Method;
                item["url"] = el.Url;

                // --- Включение данных по фильтрам (Пункты 0-3) ---
                if (includeStatus) 
                    item["statusCode"] = el.StatusCode?.ToString();

                if (includeHeaders)
                {
                    item["requestHeaders"] = ParseHeaders(el.RequestHeaders);
                    item["responseHeaders"] = ParseHeaders(el.ResponseHeaders);
                }

                if (includeCookies)
                {
                    item["requestCookies"] = el.RequestCookies;
                    item["responseCookies"] = el.ResponseCookies;
                }

                if (includeBodies)
                {
                    if (!string.IsNullOrEmpty(el.RequestBody))
                    {
                        try { item["requestBody"] = JToken.Parse(el.RequestBody); }
                        catch { item["requestBody"] = el.RequestBody; }
                    }

                    if (!string.IsNullOrEmpty(el.ResponseBody))
                    {
                        try { item["responseBody"] = JToken.Parse(el.ResponseBody); }
                        catch { item["responseBody"] = el.ResponseBody; }
                    }
                }

                uniqueEndpoints[key] = item;
            }

            // --- Исправление PUT и динамическая группировка ---
            var snapshot = new JObject();
            snapshot["totalEndpoints"] = uniqueEndpoints.Count;

            // Группируем все найденные методы (GET, POST, PUT, DELETE...) автоматически
            var groups = uniqueEndpoints.Values
                .GroupBy(e => e["method"]?.ToString().ToUpper())
                .ToDictionary(g => $"{g.Key.ToLower()}Endpoints", g => new JArray(g.ToList()));

            foreach (var group in groups)
            {
                snapshot[group.Key] = group.Value;
            }

            string json = JsonConvert.SerializeObject(snapshot, Formatting.Indented);
            
            // Используем Try-Catch для буфера обмена (на случай запуска в потоках без STA)
            try { System.Windows.Forms.Clipboard.SetText(json); } catch { }
            
            return json;

            // Вспомогательный метод парсинга (без изменений)
            JObject ParseHeaders(string headersString)
            {
                var result = new JObject();
                if (string.IsNullOrEmpty(headersString)) return result;
                var matches = Regex.Matches(headersString, @"([\w\-\:]+):\s(.+?)(?=[\w\-\:]+:\s|$)", RegexOptions.Singleline);
                foreach (Match match in matches)
                {
                    string k = match.Groups[1].Value.Trim();
                    string v = match.Groups[2].Value.Trim();
                    result[k] = v;
                }
                return result;
            }
        }
        #endregion

        #region Page Actions (Действия со страницей)

        /// <summary>
        /// Перезагрузить страницу и обновить кэш трафика
        /// </summary>
        public Traffic ReloadPage(int delaySeconds = 1)
        {
            _project.Deadline();

            _instance.ActiveTab.MainDocument.EvaluateScript("location.reload(true)");
            if (_instance.ActiveTab.IsBusy) _instance.ActiveTab.WaitDownloading();
            
            Thread.Sleep(1000 * delaySeconds);
            
            ForceRefreshCache();

            return this;
        }

        /// <summary>
        /// Явно обновить кэш трафика (обычно не требуется - обновляется автоматически)
        /// </summary>
        public Traffic RefreshTrafficCache()
        {
            ForceRefreshCache();
            return this;
        }

        #endregion

        #region Internal Cache Management (Внутреннее управление кэшем - скрыто от API)

        private void UpdateCacheIfNeeded()
        {
            bool cacheExpired = _cache == null || 
                                (DateTime.Now - _cacheTime).TotalSeconds > CACHE_LIFETIME_SECONDS;

            if (cacheExpired)
            {
                ForceRefreshCache();
            }
        }

        private void ForceRefreshCache()
        {
            var rawTraffic = _instance.ActiveTab.GetTraffic();
            _cache = new List<TrafficElement>();

            foreach (var item in rawTraffic)
            {
                // Пропускаем OPTIONS запросы
                if (item.Method == "OPTIONS") continue;

                _cache.Add(ConvertToTrafficElement(item));
            }

            _cacheTime = DateTime.Now;

            _logger?.Send($"Cache refreshed: {_cache.Count} elements");
        }

        private TrafficElement SearchInCache(string url, bool strict)
        {
            UpdateCacheIfNeeded();

            foreach (var element in _cache)
            {
                bool isMatch = strict 
                    ? element.Url == url 
                    : element.Url.Contains(url);

                if (isMatch)
                {
                    return element;
                }
            }

            // Не нашли - принудительно обновляем кэш и ищем еще раз
            ForceRefreshCache();

            foreach (var element in _cache)
            {
                bool isMatch = strict 
                    ? element.Url == url 
                    : element.Url.Contains(url);

                if (isMatch)
                {
                    return element;
                }
            }

            return null;
        }

        private TrafficElement ConvertToTrafficElement(dynamic rawItem)
        {
            var responseBody = rawItem.ResponseBody == null
                ? string.Empty
                : Encoding.UTF8.GetString(rawItem.ResponseBody, 0, rawItem.ResponseBody.Length);

            return new TrafficElement(_project)
            {
                Method = rawItem.Method ?? string.Empty,
                StatusCode = rawItem.ResultCode.ToString(),
                Url = rawItem.Url ?? string.Empty,
                ResponseContentType = rawItem.ResponseContentType ?? string.Empty,
                RequestHeaders = rawItem.RequestHeaders ?? string.Empty,
                RequestCookies = rawItem.RequestCookies ?? string.Empty,
                RequestBody = rawItem.RequestBody ?? string.Empty,
                ResponseHeaders = rawItem.ResponseHeaders ?? string.Empty,
                ResponseCookies = rawItem.ResponseCookies ?? string.Empty,
                ResponseBody = responseBody
            };
        }

        #endregion

        #region Nested Class - TrafficElement

        /// <summary>
        /// Один элемент трафика (HTTP запрос + ответ)
        /// </summary>
        public class TrafficElement
        {
            private readonly IZennoPosterProjectModel _project;

            internal TrafficElement(IZennoPosterProjectModel project)
            {
                _project = project;
            }

            // HTTP Request
            public string Method { get; internal set; }
            public string Url { get; internal set; }
            public string RequestHeaders { get; internal set; }
            public string RequestCookies { get; internal set; }
            public string RequestBody { get; internal set; }

            // HTTP Response
            public string StatusCode { get; internal set; }
            public string ResponseContentType { get; internal set; }
            public string ResponseHeaders { get; internal set; }
            public string ResponseCookies { get; internal set; }
            public string ResponseBody { get; internal set; }

            /// <summary>
            /// Распарсить ResponseBody как JSON в project.Json
            /// </summary>
            public TrafficElement ParseResponseBodyAsJson()
            {
                if (!string.IsNullOrEmpty(ResponseBody))
                {
                    _project.Json.FromString(ResponseBody);
                }
                return this;
            }

            /// <summary>
            /// Распарсить RequestBody как JSON в project.Json
            /// </summary>
            public TrafficElement ParseRequestBodyAsJson()
            {
                if (!string.IsNullOrEmpty(RequestBody))
                {
                    _project.Json.FromString(RequestBody);
                }
                return this;
            }

            /// <summary>
            /// Получить конкретный заголовок из запроса
            /// </summary>
            public string GetRequestHeader(string headerName)
            {
                var headers = ParseHeaders(RequestHeaders);
                var key = headerName.ToLower();
                return headers.ContainsKey(key) ? headers[key] : null;
            }

            /// <summary>
            /// Получить конкретный заголовок из ответа
            /// </summary>
            public string GetResponseHeader(string headerName)
            {
                var headers = ParseHeaders(ResponseHeaders);
                var key = headerName.ToLower();
                return headers.ContainsKey(key) ? headers[key] : null;
            }

            /// <summary>
            /// Получить все заголовки запроса в виде словаря
            /// </summary>
            public Dictionary<string, string> GetAllRequestHeaders()
            {
                return ParseHeaders(RequestHeaders);
            }

            /// <summary>
            /// Получить все заголовки ответа в виде словаря
            /// </summary>
            public Dictionary<string, string> GetAllResponseHeaders()
            {
                return ParseHeaders(ResponseHeaders);
            }

            private Dictionary<string, string> ParseHeaders(string headersString)
            {
                var headers = new Dictionary<string, string>();
                if (string.IsNullOrWhiteSpace(headersString)) return headers;

                foreach (var line in headersString.Split('\n'))
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed)) continue;

                    var colonIndex = trimmed.IndexOf(':');
                    if (colonIndex <= 0) continue;

                    var key = trimmed.Substring(0, colonIndex).Trim().ToLower();
                    var value = trimmed.Substring(colonIndex + 1).Trim();
                    headers[key] = value;
                }

                return headers;
            }
        }

        #endregion
        
        #region z3n8
        
        private static readonly HttpClient _httpClient = new HttpClient 
        { 
            Timeout = TimeSpan.FromSeconds(5) 
        };
        
        public void SendToController(string urlFilter = "api", Filter preset = Filter.Api)
        {
            var all = FindAllTrafficElements(urlFilter);
            if (all.Count == 0) return;

            string prj     = _project?.Name.Replace(".zp", "") ?? "";
            string acc     = _project?.Var("acc0")             ?? "";
            string session = _project?.Var("varSessionId")     ?? "";
            string taskId  = _project?.TaskId                  ?? "";
            string logHost = _project?.GVar("logHost") ?? "http://localhost:10993/log";
            string host    = logHost.Replace("/log", "/traffic");

            foreach (var el in all)
            {
                var payload = new
                {
                    machine    = Environment.MachineName,
                    project    = prj,
                    account    = acc,
                    session    = session,
                    task_id    = taskId,
                    timestamp  = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    method     = el.Method,
                    url        = el.Url,
                    statusCode = el.StatusCode,
                    request    = new { body = el.RequestBody,  headers = el.RequestHeaders  },
                    response   = new { body = el.ResponseBody, headers = el.ResponseHeaders }
                };

                string json = JsonConvert.SerializeObject(payload);
        
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var cts = new System.Threading.CancellationTokenSource(3000);
                        try
                        {
                            var content = new StringContent(json, Encoding.UTF8, "application/json");
                            try
                            {
                                await _httpClient.PostAsync(host, content, cts.Token);
                            }
                            finally { content.Dispose(); }
                        }
                        finally { cts.Dispose(); }
                    }
                    catch { }
                });
            }
        }
        #endregion
        
    }


    #region Extension Methods

    public static partial class ProjectExtensions
    {
        
        /// <summary>
        /// Получить заголовки запроса и сохранить в переменную проекта
        /// </summary>
        public static void SaveRequestHeadersToVariable(this IZennoPosterProjectModel project, 
            Instance instance, string url, bool strict = false, Logger log = null)
        {
            var traffic = new Traffic(project, instance, log: log);
            var element = traffic.FindTrafficElement(url, strict);
            
            var cleanHeaders = new StringBuilder();
            foreach (string header in element.RequestHeaders.Split('\n'))
            {
                // Пропускаем псевдо-заголовки HTTP/2
                if (header.StartsWith(":")) continue;
                if (string.IsNullOrWhiteSpace(header)) continue;
                
                cleanHeaders.AppendLine(header.Trim());
            }
            
            project.Var("headers", cleanHeaders.ToString());
            
            log?.Send($"Headers saved to variable 'headers':\n{cleanHeaders}");
        }

        /// <summary>
        /// Получить заголовки и сохранить в переменную проекта и/или БД
        /// </summary>
        public static void CollectRequestHeaders(this IZennoPosterProjectModel project, 
            Instance instance, string url, bool strict = false, 
            bool saveToVariable = true, bool saveToDatabase = true, Logger log = null)
        {
            var traffic = new Traffic(project, instance, log: log);
            var element = traffic.FindTrafficElement(url, strict);
            
            var cleanHeaders = new StringBuilder();
            int headerCount = 0;
            
            foreach (string header in element.RequestHeaders.Split('\n'))
            {
                // Пропускаем псевдо-заголовки HTTP/2
                if (header.StartsWith(":")) continue;
                if (string.IsNullOrWhiteSpace(header)) continue;
                
                cleanHeaders.AppendLine(header.Trim());
                headerCount++;
            }
            
            var headersText = cleanHeaders.ToString();
            
            log?.Send($"[SUCCESS]: collected={headerCount}, length={headersText.Length}\n{headersText}");
            
            if (saveToVariable) 
                project.Var("headers", headersText);
            
            if (saveToDatabase) 
                project.DbUpd($"headers = '{headersText}'");
        }

        public static string GrabApi(
            this IZennoPosterProjectModel project, Instance instance, string url, 
            bool includeStatus = true,   // 0
            bool includeHeaders = true,  // 1
            bool includeBodies = true,   // 2
            bool includeCookies = true,  // 3
            bool excludeFiles = true,     // 4
            string  toVariable = null, bool clipboard = true, Logger log = null)
        {
            var structure  = new Traffic(project, instance, log: log).GetApiStructure(url,includeStatus,includeHeaders,includeBodies,includeCookies);
            if (!string.IsNullOrEmpty(toVariable)) project.Var(toVariable,structure);
            if (clipboard)  System.Windows.Forms.Clipboard.SetText(structure);
            return structure;
        }

        public static void ForwardTraffic(this IZennoPosterProjectModel project,
            Instance instance, string urlFilter = "api", Traffic.Filter preset = Traffic.Filter.Api)
        {
            new Traffic(project, instance).SendToController(urlFilter, preset);
        }



    }

    #endregion
    
    
    
    
    
    
    
}