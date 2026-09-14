using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using z3n7.DbUtils;
using z3n7.Tools;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    /// <summary>
    /// HTTP-сервер внутри ZennoPoster.
    /// Принимает команды от оркестратора напрямую, минуя БД.
    /// 
    /// Запуск: project.StartZpServer()
    /// Остановка: project.StopZpServer()
    /// 
    /// Endpoints:
    ///   GET  /state         — tasks + processes текущей машины
    ///   GET  /traffic       — JSONL traffic: tail=N либо страницы по байтовому оффсету
    ///   POST /command       — { action, task_id, payload } → исполняет немедленно
    /// </summary>
    public static class ZpServer
    {
        

        private static HttpListener  _listener;
        private static Thread        _thread;
        private static volatile bool _running;
        private static int           _port;

        /// <summary>Сколько портов подряд перебрать, начиная с запрошенного.</summary>
        private const int PortRange = 20;

        // ── Start / Stop ──────────────────────────────────────────────────────

        public static void StartZpServer(this IZennoPosterProjectModel project, int port = 22222, bool log = false, bool openFirewall = false)
        {
            // Сервер переживает запуск проекта, поэтому строку узла печатаем
            // на каждый вызов — и когда подняли сейчас, и когда он уже висел.
            if (_running)
            {
                if (log) LogNode(project, _port);
                return;
            }

            if (!Bind(project, port)) return;
            if (openFirewall) EnsureFirewall(project, _port, log);

            _running = true;
            _thread  = new Thread(() => Loop(project, log)) { IsBackground = true };
            _thread.Start();

            if (log) LogNode(project, _port);
        }

        public static void StopZpServer(this IZennoPosterProjectModel project, bool log = false)
        {
            if (!_running) return;
            _running = false;
            _listener?.Stop();
            if (log) project.SendInfoToLog("[ZpServer] Stopped", false);
        }

        /// <summary>
        /// Занимает первый свободный порт начиная с <paramref name="port"/>.
        /// Занятый порт — не ошибка: на машине может уже висеть чужой слушатель.
        /// </summary>
        private static bool Bind(IZennoPosterProjectModel project, int port)
        {
            for (var p = port; p < port + PortRange; p++)
            {
                if (IsPortBusy(p)) continue;

                var listener = new HttpListener();
                listener.Prefixes.Add($"http://+:{p}/");
                try
                {
                    listener.Start();
                }
                catch (HttpListenerException ex)
                {
                    // 5 = ERROR_ACCESS_DENIED: нет URL ACL на префикс. Перебор портов
                    // не поможет — упрёмся в то же самое на каждом.
                    if (ex.ErrorCode == 5)
                    {
                        project.warn($"[ZpServer] {ex.Message} — нужен netsh http add urlacl url=http://+:{p}/ user=Everyone");
                        return false;
                    }
                    continue; // порт перехватили между проверкой и Start
                }

                _listener = listener;
                _port     = p;
                return true;
            }

            project.warn($"[ZpServer] нет свободного порта в диапазоне {port}..{port + PortRange - 1}");
            return false;
        }

        // ── Loop ──────────────────────────────────────────────────────────────

        private static void Loop(IZennoPosterProjectModel project, bool log)
        {
            while (_running)
            {
                HttpListenerContext ctx;
                try   { ctx = _listener.GetContext(); }
                catch { break; }

                Task.Run(() => Handle(ctx, project, log));
            }
        }

        private static async Task Handle(HttpListenerContext ctx, IZennoPosterProjectModel project, bool log)
        {
            var path   = ctx.Request.Url?.AbsolutePath.ToLower() ?? "";
            var method = ctx.Request.HttpMethod;

            // CORS headers
            ctx.Response.AddHeader("Access-Control-Allow-Origin", "*");
            ctx.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            ctx.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

            // Handle preflight
            if (method == "OPTIONS")
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.Close();
                return;
            }

            try
            {
                if (path == "/state"   && method == "GET")  { await ServeState(ctx, project);   return; }
                if (path == "/command" && method == "POST") { await ServeCommand(ctx, project, log); return; }
                if (path == "/task/xml"    && method == "GET")  { await ServeTaskXml(ctx);            return; }
                if (path == "/task/xml"    && method == "POST") { await ReceiveTaskXml(ctx, log);     return; }
                if (path == "/task/settings" && method == "GET") { await ServeTaskSettings(ctx); return; }
                if (path == "/log"     && method == "GET")  { await ServeLog(ctx, project);     return; }
                if (path == "/traffic" && method == "GET")  { await ServeTraffic(ctx, project); return; }
                if (path == "/traffic/har" && method == "GET") { await ServeTrafficHar(ctx, project); return; }
                if (path == "/debug/assemblies" && method == "GET") { await ServeDebugAssemblies(ctx); return; }

                ctx.Response.StatusCode = 404;
                ctx.Response.Close();
            }
            catch (Exception ex)
            {
                if (log) project.SendInfoToLog($"[ZpServer] Handle error: {ex.Message}", false);
                try { ctx.Response.StatusCode = 500; ctx.Response.Close(); } catch { }
            }
        }

        // ── Handlers ──────────────────────────────────────────────────────────

        private static async Task ServeState(HttpListenerContext ctx, IZennoPosterProjectModel project)
        {
            var tasks = ZennoPoster.TasksList
                .Select(xml => Convert.ToBase64String(Encoding.UTF8.GetBytes(xml)))
                .ToList();

            var procs = ProcessManager.ZennoProcesses().Select(arr => new
            {
                name   = arr[0],
                ram    = arr[1],
                uptime = arr[2],
                pid    = arr[3],
            });

            await WriteJson(ctx.Response, new
            {
                machine   = Environment.MachineName,
                tasks_b64 = tasks,
                processes = procs,
            });
        }

        private static async Task ServeCommand(HttpListenerContext ctx, IZennoPosterProjectModel project, bool log)
        {
            JsonElement? json = await ReadJson(ctx.Request);
            if (json == null) { await WriteError(ctx.Response, 400, "Invalid JSON"); return; }

            var action  = json.Value.TryGetProperty("action",  out var a) ? a.GetString() ?? "" : "";
            var taskId  = json.Value.TryGetProperty("task_id", out var t) ? t.GetString() ?? "" : "";
            var payload = json.Value.TryGetProperty("payload", out var p) ? p.GetString() ?? "" : "";

            if (string.IsNullOrEmpty(action)) { await WriteError(ctx.Response, 400, "action required"); return; }

            try
            {
                ExecAction(project, action, taskId, payload);
                await WriteJson(ctx.Response, new { ok = true });
            }
            catch (Exception ex)
            {
                await WriteError(ctx.Response, 500, ex.Message);
            }
        }
        
        // GET /task/xml?path=C:\tasks\mytask.zp
        private static async Task ServeTaskXml(HttpListenerContext ctx)
        {
            var path = ctx.Request.QueryString["path"] ?? "";
            if (string.IsNullOrEmpty(path)) { await WriteError(ctx.Response, 400, "path required"); return; }

            try
            {
                // Загружаем ProjectMaker.dll если ещё не загружена
                EnsureProjectMakerLoaded();

                var xml = ZpToCsx.ExtractXml(path);
                if (xml == null)
                {
                    await WriteError(ctx.Response, 404, "ExtractXml returned null");
                    return;
                }

                // Проверяем, что это действительно ошибка, а не просто XML с текстом "Exception"
                if (xml.StartsWith("System.") && xml.Contains("Exception"))
                {
                    await WriteError(ctx.Response, 500, xml);
                    return;
                }

                await WriteJson(ctx.Response, new { xml });
            }
            catch (Exception ex)
            {
                await WriteError(ctx.Response, 500, ex.Message);
            }
        }

        private static void EnsureProjectMakerLoaded()
        {
            // Проверяем, загружена ли ProjectMaker
            var loaded = System.AppDomain.CurrentDomain.GetAssemblies()
                .Any(asm => asm.GetName().Name == "ProjectMaker");

            if (!loaded)
            {
                // Ищем ProjectMaker.dll в папке ZennoPoster
                var zpDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var pmPath = Path.Combine(zpDir, "ProjectMaker.dll");

                if (File.Exists(pmPath))
                {
                    System.Reflection.Assembly.LoadFrom(pmPath);
                }
                else
                {
                    throw new FileNotFoundException("ProjectMaker.dll not found in ZennoPoster directory");
                }
            }
        }

        // POST /task/xml  body: { path, xml }
        private static async Task ReceiveTaskXml(HttpListenerContext ctx, bool log)
        {
            var json = await ReadJson(ctx.Request);
            if (json == null) { await WriteError(ctx.Response, 400, "Invalid JSON"); return; }

            var path = json.Value.TryGetProperty("path", out var p) ? p.GetString() ?? "" : "";
            var xml  = json.Value.TryGetProperty("xml",  out var x) ? x.GetString() ?? "" : "";

            // Декодируем из base64
            try
            {
                var bytes = Convert.FromBase64String(xml);
                xml = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                // Если не base64, используем как есть
            }

            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(xml))
            { await WriteError(ctx.Response, 400, "path and xml required"); return; }

            try
            {
                EnsureProjectMakerLoaded();
                ZpToCsx.XmlToZp(xml, path);
                await WriteJson(ctx.Response, new { ok = true });
            }
            catch (Exception ex)
            {
                await WriteError(ctx.Response, 500, ex.Message);
            }
        }

        // GET /task/settings?task_id=<guid> — input settings задачи плоским словарём
        private static async Task ServeTaskSettings(HttpListenerContext ctx)
        {
            Guid guid;
            if (!Guid.TryParse(ctx.Request.QueryString["task_id"] ?? "", out guid))
            { await WriteError(ctx.Response, 400, "valid task_id required"); return; }

            var xml = ZennoPoster.ExportInputSettings(guid);
            if (string.IsNullOrEmpty(xml))
            { await WriteError(ctx.Response, 404, $"no input settings for {guid}"); return; }

            var payload = TaskManager.XmlToPayload(xml);
            await WriteJson(ctx.Response, new
            {
                task_id  = guid.ToString(),
                xml_b64  = payload.xmlB64,
                json_b64 = payload.jsonB64,
            });
        }

        /// <summary>
        /// payload — {"xml_b64":"...","json_b64":"..."}, те же два блоба, что лежали
        /// в БД колонками _xml_b64 и _json_b64. Наложение делает TaskManager.PayloadToXml,
        /// то есть ровно тот же код, что и путь через базу.
        /// </summary>
        private static void SetInputSettings(Guid guid, string payload)
        {
            JsonElement json;
            try   { json = JsonSerializer.Deserialize<JsonElement>(payload); }
            catch { throw new Exception("payload must be JSON with xml_b64 and json_b64"); }

            JsonElement x, j;
            var xmlB64  = json.TryGetProperty("xml_b64",  out x) ? x.GetString() ?? "" : "";
            var jsonB64 = json.TryGetProperty("json_b64", out j) ? j.GetString() ?? "" : "";

            if (string.IsNullOrEmpty(xmlB64)) throw new Exception("xml_b64 required");

            var xml = TaskManager.PayloadToXml(xmlB64, jsonB64);
            if (string.IsNullOrEmpty(xml)) throw new Exception("PayloadToXml produced nothing");

            ZennoPoster.ImportInputSettings(guid, xml);
        }

        // GET /log?kind=execution&process=ZennoPoster&n=200&project=Simroute.uber
        //
        // kind: execution (лог проектов) | errors (nonCriticalErrors, со стектрейсами
        // движка) | critical. process по умолчанию ZennoPoster — его лог интересен и
        // тогда, когда сам код крутится в ProjectMaker.
        private static async Task ServeLog(HttpListenerContext ctx, IZennoPosterProjectModel project)
        {
            var q       = ctx.Request.QueryString;
            var kind    = q["kind"]    ?? "execution";
            var process = q["process"] ?? "ZennoPoster";
            var filter  = q["project"] ?? "";

            int n;
            if (!int.TryParse(q["n"], out n) || n <= 0) n = 200;
            if (n > 2000) n = 2000;

            string error;
            var file = ZpLog.Resolve(project, kind, process, out error);
            if (file == null)
            {
                await WriteJson(ctx.Response, new { error, available = ZpLog.Available(project) });
                return;
            }

            var entries = ZpLog.Tail(file, n, filter, ZpLog.HasProjectColumn(kind));
            await WriteJson(ctx.Response, new
            {
                file,
                count   = entries.Count,
                entries = entries.Select(e => e.ToJson()),
            });
        }

        // GET /traffic?offset=0&n=200&project=Simroute.uber&task_id=...
        // Continue with next_offset, retaining the same filters. When has_more
        // is false, retain next_offset for the next poll to collect new traffic.
        //
        // GET /traffic?tail=200&... — last N records instead, read backwards from
        // the end of the file. next_offset then points at the end of the current
        // file, so forward polling can continue from there.
        private static async Task ServeTraffic(HttpListenerContext ctx, IZennoPosterProjectModel project)
        {
            var q = ctx.Request.QueryString;

            if (q["tail"] != null)
            {
                int tail;
                if (!int.TryParse(q["tail"], out tail) || tail <= 0) tail = 200;
                tail = Math.Min(tail, 2000);
                try
                {
                    var last = ZpTraffic.Tail(ZpTraffic.FilePath(project), tail, q["project"], q["task_id"]);
                    await WriteJson(ctx.Response, last);
                }
                catch (Exception ex) { await WriteError(ctx.Response, 500, ex.Message); }
                return;
            }

            long offset = 0;
            if (q["offset"] != null && (!long.TryParse(q["offset"], out offset) || offset < 0))
            {
                await WriteError(ctx.Response, 400, "offset must be a non-negative byte offset");
                return;
            }
            int n;
            if (!int.TryParse(q["n"], out n) || n <= 0) n = 200;
            n = Math.Min(n, 2000);
            try
            {
                var page = ZpTraffic.Read(ZpTraffic.FilePath(project), offset, n, q["project"], q["task_id"]);
                await WriteJson(ctx.Response, page);
            }
            catch (ArgumentException ex) { await WriteError(ctx.Response, 400, ex.Message); }
            catch (Exception ex) { await WriteError(ctx.Response, 500, ex.Message); }
        }

        // GET /traffic/har?project=Simroute.uber&task_id=... — complete HAR 1.2, no pagination envelope.
        private static async Task ServeTrafficHar(HttpListenerContext ctx, IZennoPosterProjectModel project)
        {
            var q = ctx.Request.QueryString;
            var har = HarTraffic.ExportRqst(project, q["project"], q["task_id"]);
            var bytes = Encoding.UTF8.GetBytes(har);
            ctx.Response.ContentType = "application/json; charset=utf-8";
            ctx.Response.Headers["Content-Disposition"] = "attachment; filename=traffic.har";
            ctx.Response.ContentLength64 = bytes.Length;
            await ctx.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            ctx.Response.Close();
        }

        // GET /debug/assemblies — список загруженных сборок
        private static async Task ServeDebugAssemblies(HttpListenerContext ctx)
        {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies()
                .Select(asm => new
                {
                    name = asm.GetName().Name,
                    version = asm.GetName().Version?.ToString() ?? "",
                    location = asm.IsDynamic ? "(dynamic)" : (asm.Location ?? "(no location)")
                })
                .OrderBy(a => a.name)
                .ToList();

            await WriteJson(ctx.Response, new { assemblies, count = assemblies.Count });
        }

        // ── Exec ──────────────────────────────────────────────────────────────

        private static void ExecAction(IZennoPosterProjectModel project, string action, string taskId, string payload)
        {
            if (!string.IsNullOrEmpty(taskId) && !Guid.TryParse(taskId, out _))
                throw new Exception($"Invalid task_id: {taskId}");

            var guid = string.IsNullOrEmpty(taskId) ? Guid.Empty : new Guid(taskId);

            switch (action.ToLower())
            {
                case "start":                  ZennoPoster.StartTask(guid);                                                              break;
                case "stop":                   ZennoPoster.StopTask(guid);                                                               break;
                case "interrupt":              ZennoPoster.InterruptTask(guid);                                                           break;
                case "add_tries":              if (int.TryParse(payload, out var add))     ZennoPoster.AddTries(guid, add);              break;
                case "set_tries":              if (int.TryParse(payload, out var set))     ZennoPoster.SetTries(guid, set);              break;
                case "set_threads":            if (int.TryParse(payload, out var thr))     ZennoPoster.SetMaxThreads(guid, thr);         break;
                case "clear_success":          ZennoPoster.ClearSuccess(guid);                                                            break;
                case "clear_fails":            ZennoPoster.ClearFails(guid);                                                             break;
                case "kill_by_uptime":         if (int.TryParse(payload, out var min))     project.KillByUptime(min);                   break;
                case "set_input_settings":     SetInputSettings(guid, payload);                                                          break;
                case "set_execution_settings": ZennoPoster.SetExecutionSettings(guid, TaskManager.JsonToXml(payload));                  break;
                case "set_scheduler_settings": ZennoPoster.SetSchedulerSettings(guid, TaskManager.JsonToXml(payload));                  break;
                default: throw new Exception($"Unknown action: {action}");
            }
        }

        // ── Node info ──────────────────────────────────────────────────────────

        /// <summary>
        /// Печатает строку регистрации узла: сначала подсказку, затем сам JSON
        /// отдельной строкой, чтобы её можно было скопировать целиком.
        /// </summary>
        private static void LogNode(IZennoPosterProjectModel project, int port)
        {
            var firewall = Firewall.Enabled();
            var portRule = Firewall.HasPortRule(port);
            var external = GetExternalIp(project);

            project.SendInfoToLog("Скопируйте следующую строку и вставьте в панели управления узлами DevDeck", true);
            project.SendInfoToLog(NodeJson(port, external, firewall, portRule), true);

            if (firewall == true && portRule == false)
                project.SendInfoToLog($"[ZpServer] файрвол включён, правила на порт {port} не видно. Открыть: netsh advfirewall firewall add rule name={Firewall.RuleName(port)} dir=in action=allow protocol=TCP localport={port}", true);
        }

        /// <summary>
        /// Создаёт правило файрвола, если подходящего не нашлось. Неудача не мешает
        /// серверу работать: HasPortRule не учитывает block-правила и привязку к
        /// профилю, так что порт может оказаться открыт и без нашего правила.
        /// </summary>
        private static void EnsureFirewall(IZennoPosterProjectModel project, int port, bool log)
        {
            if (Firewall.HasPortRule(port) == true) return;

            string error;
            if (Firewall.TryOpen(port, out error))
            {
                if (log) project.SendInfoToLog($"[ZpServer] правило файрвола создано: {Firewall.RuleName(port)}", true);
            }
            else
            {
                project.warn($"[ZpServer] не удалось открыть порт {port}: {error}");
            }
        }

        private static string NodeJson(int port, string external, bool? firewall, bool? portRule) =>
            JsonSerializer.Serialize(new
            {
                machine  = Environment.MachineName,
                host     = GetLocalIp(),
                external = external,
                port     = port,
                firewall = State(firewall, "on",  "off"),
                portRule = State(portRule, "yes", "no"),
            });

        /// <summary>
        /// Сервисы-эхо, отдающие адрес, с которого к ним пришло соединение.
        /// Хосты выбраны IPv4-only намеренно: универсальные (api.ipify.org,
        /// icanhazip.com) на машине с IPv6-связностью возвращают v6-адрес,
        /// и в строке узла оказывалась бы то одна семья, то другая.
        /// </summary>
        private static readonly string[] IpEcho =
        {
            "https://api4.ipify.org",
            "https://ipv4.icanhazip.com",
            "https://checkip.amazonaws.com",
        };

        /// <summary>
        /// Внешний IPv4 узла. Пустая строка, если ни один сервис не ответил —
        /// пустое поле честнее выдуманного адреса.
        /// </summary>
        private static string GetExternalIp(IZennoPosterProjectModel project)
        {
            foreach (var url in IpEcho)
            {
                try
                {
                    var body = project.GET(url, deadline: 5, bodyOnly: true);
                    if (string.IsNullOrEmpty(body)) continue;

                    var ip = body.Trim();
                    if (IPAddress.TryParse(ip, out var parsed) &&
                        parsed.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return ip;
                }
                catch { }
            }

            return "";
        }

        private static string State(bool? value, string yes, string no) =>
            value == null ? "unknown" : (value.Value ? yes : no);

        /// <summary>
        /// Адрес адаптера, через который система реально ходит наружу.
        ///
        /// Dns.GetHostEntry отдаёт IPv4 в произвольном порядке, и на машине с
        /// VirtualBox или WSL первым оказывается виртуальный адаптер, недостижимый
        /// из сети. UDP-connect пакетов не шлёт — только заставляет ОС выбрать
        /// маршрут и назначить сокету локальный адрес.
        /// </summary>
        private static string GetLocalIp()
        {
            try
            {
                using (var s = new System.Net.Sockets.Socket(
                           System.Net.Sockets.AddressFamily.InterNetwork,
                           System.Net.Sockets.SocketType.Dgram,
                           System.Net.Sockets.ProtocolType.Udp))
                {
                    s.Connect("8.8.8.8", 65530);
                    var ip = (s.LocalEndPoint as IPEndPoint)?.Address;
                    if (ip != null) return ip.ToString();
                }
            }
            catch { }

            // Нет маршрута по умолчанию — падаем обратно на список адресов хоста.
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return ip.ToString();
            }
            catch { }

            return "127.0.0.1";
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        
        private static bool IsPortBusy(int port)
        {
            try
            {
                var t = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, port);
                t.Start();
                t.Stop();
                return false;
            }
            catch { return true; }
        }

        private static async Task<JsonElement?> ReadJson(HttpListenerRequest req)
        {
            using var reader = new System.IO.StreamReader(req.InputStream);
            var body = await reader.ReadToEndAsync();
            try   { return JsonSerializer.Deserialize<JsonElement>(body); }
            catch { return null; }
        }

        private static async Task WriteJson(HttpListenerResponse res, object data)
        {
            res.ContentType = "application/json";
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
            res.ContentLength64 = bytes.Length;
            await res.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            res.Close();
        }

        private static async Task WriteError(HttpListenerResponse res, int code, string message)
        {
            res.StatusCode = code;
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { error = message }));
            res.ContentType = "application/json";
            res.ContentLength64 = bytes.Length;
            await res.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            res.Close();
        }
        
    }
}
