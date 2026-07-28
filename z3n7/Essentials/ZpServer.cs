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
    ///   POST /command       — { action, task_id, payload } → исполняет немедленно
    /// </summary>
    public static class ZpServer
    {
        

        private static HttpListener  _listener;
        private static Thread        _thread;
        private static volatile bool _running;

        // ── Start / Stop ──────────────────────────────────────────────────────

        public static void StartZpServer(this IZennoPosterProjectModel project, int port = 22222, bool log = false)
        {
            if (_running) return;
            if (IsPortBusy(port))  return;

            RegisterNode(project, port, log);

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://+:{port}/");
            _listener.Start();
            _running = true;

            _thread = new Thread(() => Loop(project, log)) { IsBackground = true };
            _thread.Start();

            if (log) project.SendInfoToLog($"[ZpServer] Listening on port {port}", false);
        }

        public static void StopZpServer(this IZennoPosterProjectModel project, bool log = false)
        {
            if (!_running) return;
            _running = false;
            _listener?.Stop();
            UnregisterNode(project);
            if (log) project.SendInfoToLog("[ZpServer] Stopped", false);
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
                case "update_settings":        project.Pull(guid);                                                                       break;
                case "kill_by_uptime":         if (int.TryParse(payload, out var min))     project.KillByUptime(min);                   break;
                default: throw new Exception($"Unknown action: {action}");
            }
        }

        // ── Node registration ─────────────────────────────────────────────────

        private static void RegisterNode(IZennoPosterProjectModel project, int port, bool log)
        {
            var host    = GetLocalIp();
            var machine = Environment.MachineName;
            var now     = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            project.TblAdd(DbSchema.ZpNodes.Columns, DbSchema.ZpNodes.Name);

            var isPg = project.Var("dbSource").StartsWith("Host=");
            string q = isPg
                ? $"INSERT INTO \"{DbSchema.ZpNodes.Name}\" (machine, host, port, updated_at) " +
                  $"VALUES ('{machine}', '{host}', '{port}', '{now}') " +
                  $"ON CONFLICT (machine) DO UPDATE SET host = EXCLUDED.host, port = EXCLUDED.port, updated_at = EXCLUDED.updated_at"
                
                : $"INSERT OR REPLACE INTO \"{DbSchema.ZpNodes.Name}\" (machine, host, port, updated_at) " +
                  $"VALUES ('{machine}', '{host}', '{port}', '{now}')";

            project.DbQ(q, log);
        }

        private static void UnregisterNode(IZennoPosterProjectModel project)
        {
            project.DbQ($"DELETE FROM \"{DbSchema.ZpNodes.Name}\" WHERE \"machine\" = '{Environment.MachineName}'");
        }

        private static string GetLocalIp()
        {
            try
            {
                var host  = Dns.GetHostEntry(Dns.GetHostName());
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