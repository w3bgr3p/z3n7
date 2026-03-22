using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ZennoLab.InterfacesLibrary.Enums.Log;
using ZennoLab.InterfacesLibrary.ProjectModel;
using System.Text;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

using ZennoLab.CommandCenter;

namespace z3n7
{
    public enum LogLevel
    {
        Debug   = 0,
        Info    = 1,
        Warning = 2,
        Error   = 3,
        Off     = 99
    }

    public class Logger
    {
        
        [ThreadStatic]
        private static Logger _threadLogger;

        public static Logger Current => _threadLogger;

        public static Logger Init(IZennoPosterProjectModel project, Instance instance = null)
        {
            _threadLogger = Get(project, instance);
            return _threadLogger;
        }

        public static void log(string msg,  [CallerMemberName] string caller = "")
            => _threadLogger?.Info(msg, caller);
        public static void warn(string msg, [CallerMemberName] string caller = "")
            => _threadLogger?.Warn(msg, caller);
        public static void err(string msg,  [CallerMemberName] string caller = "")
            => _threadLogger?.Error(msg, caller);
        
        private static readonly ConcurrentDictionary<string, Logger> _loggerCache 
            = new ConcurrentDictionary<string, Logger>();
        
        public static Logger Get(IZennoPosterProjectModel project, Instance instance = null)
        {
            string key = project.Var("acc0");
    
            // Если инстанс передан — обновляем (мог смениться)
            if (instance != null && _loggerCache.TryGetValue(key, out var existing))
            {
                // пересоздаём только если инстанс другой
                if (existing._instance != instance)
                    return _loggerCache[key] = new Logger(project, instance:instance);
                return existing;
            }
    
            return _loggerCache.GetOrAdd(key, _ => new Logger(project, instance:instance));
        }
        
        public static void ClearCache(IZennoPosterProjectModel project)
        {
            _loggerCache.TryRemove(project.Var("acc0"), out _);
            _threadLogger = null; // добавить
        }
        
        public Logger WithInstance(Instance instance)
        {
            string key = _project?.Var("acc0") ?? "";
            var updated = new Logger(_project, instance, 
                logLevel: _minLevel, logHost: _logHost, http: _http);
            if (!string.IsNullOrEmpty(key))
                Logger._loggerCache[key] = updated;
            return updated;
        }
        
        // ── Config ────────────────────────────────────────────────────────────
        private readonly IZennoPosterProjectModel _project;
        private Instance _instance;
        private readonly bool     _persistent;
        private readonly Stopwatch _stopwatch;
        private readonly string   _logHost;
        private readonly int      _timezone;
        private readonly bool     _http;
        private readonly LogLevel _minLevel;

        private string Port { set; get; } = "";
        private string Pid { set; get; } = "";
        public string Emoji { set; get; }
        
        
        // cfgLog flags
        private readonly bool _fAcc, _fPort, _fTime, _fCaller, _fWrap, _fForce;

        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

        // ── Constructors ──────────────────────────────────────────────────────
        
        /// <summary>Full constructor — with ZennoPoster project context.</summary>
        public Logger(
            IZennoPosterProjectModel project,
            Instance instance = null,
            string     classEmoji     = null,
            bool       persistent     = true,
            LogLevel   logLevel       = LogLevel.Info,
            string     logHost        = null,
            bool       http           = true,
            int        timezoneOffset = -5)
        {
            _project    = project;
            _instance    = instance;
            Emoji      = classEmoji;
            _persistent = persistent;
            _stopwatch  = persistent ? Stopwatch.StartNew() : null;
            _http       = http;
            _timezone   = timezoneOffset;
            _minLevel   = logLevel;
            _logHost = !string.IsNullOrEmpty(logHost)                        ? logHost
                     : !string.IsNullOrEmpty(_project?.GVar("logHost"))      ? _project.GVar("logHost")
                     : "http://localhost:10993/log";

            bool debugVar = _project?.Var("debug") == "True";
            if (debugVar) _minLevel = LogLevel.Debug;
            _minLevel = (LogLevel)Math.Min((int)_minLevel, (int)LogLevel.Info);

            string cfg = _project?.Var("cfgLog") ?? "";
            _fAcc    = cfg.Contains("acc");
            _fPort   = cfg.Contains("port");
            _fTime   = cfg.Contains("time");
            _fCaller = cfg.Contains("caller");
            _fWrap   = cfg.Contains("wrap");
            _fForce  = cfg.Contains("force");

            if (_instance != null)
            {
                var m = Regex.Match(_instance.FormTitle ?? "", @"Port:(\d+); Pid:(\d+)");
                Port = m.Groups[1].Value;
                Pid  = m.Groups[2].Value;
            }
        }

        /// <summary>Standalone constructor — no ZennoPoster project.</summary>
        public Logger(
            string     classEmoji     = null,
            bool       persistent     = true,
            LogLevel   logLevel       = LogLevel.Info,
            string     logHost        = null,
            bool       http           = true,
            int        timezoneOffset = -5)
        {
            _project    = null;
            Emoji      = classEmoji;
            _persistent = persistent;
            _stopwatch  = persistent ? Stopwatch.StartNew() : null;
            _http       = http;
            _timezone   = timezoneOffset;
            _minLevel   =  logLevel;

            _logHost = logHost ?? "http://localhost:10993/log";

            _fCaller = true;
            _fWrap   = true;
            // rest are false
        }

        // ── Public API ────────────────────────────────────────────────────────

        public void Send(
            object   toLog,
            [CallerMemberName] string callerName = "",
            bool     show   = false,
            bool     thrw   = false,
            bool     toZp   = true,
            int      cut    = 0,
            LogLevel level  = LogLevel.Info,
            LogType  type   = LogType.Info,
            LogColor color  = LogColor.Default)
        {
            if (_fForce) show = true;
            if (!show && level < _minLevel) return;

            string body = BuildBody(toLog?.ToString() ?? "null", callerName, cut);

            // Map LogLevel → LogType if not overridden
            if (level == LogLevel.Warning) type = LogType.Warning;
            if (level == LogLevel.Error)   type = LogType.Error;

            // Override by message markers (legacy support)
            if (body.Contains("!W")) type = LogType.Warning;
            if (body.Contains("!E")) type = LogType.Error;

            string header = _fWrap ? BuildHeader(callerName) : string.Empty;
            string full   = header + body;

            if (_project != null && toZp)
            {
                _project.SendToLog(full, type, toZp, color);
                if (thrw) throw new Exception(full);
            }

            if (_http)
                SendHttp(body, type, callerName, level);
        }

        public void Debug(object toLog, [CallerMemberName] string callerName = "")
            => Send(toLog, callerName, level: LogLevel.Debug, type: LogType.Info);

        public void Info(object toLog, [CallerMemberName] string callerName = "")
            => Send(toLog, callerName, level: LogLevel.Info);

        public void Warn(
            object   toLog,
            [CallerMemberName] string callerName = "",
            bool     show  = false,
            bool     thrw  = false,
            bool     toZp  = true,
            int      cut   = 0,
            LogColor color = LogColor.Default)
            => Send(toLog, callerName, show, thrw, toZp, cut, LogLevel.Warning, LogType.Warning, color);

        public void Error(
            object   toLog,
            [CallerMemberName] string callerName = "",
            bool     thrw  = false)
            => Send(toLog, callerName, show: true, thrw: thrw, level: LogLevel.Error, type: LogType.Error);

        // ── Private helpers ───────────────────────────────────────────────────

        private string BuildHeader(string callerName)
        {
            var sb = new StringBuilder();
            if (_project != null)
            {
                if (_fAcc)  sb.Append($"  🤖 [{_project.Var("acc0")}]");
                if (_fTime) sb.Append($"  ⏱️ [{_project.Age<string>()}]");
                if (_fPort) sb.Append($"  🔌 [{_project.Var("instancePort")}]");
            }
            if (_fCaller) sb.Append($"  🔲 [{callerName}]");
            return sb.ToString();
        }

        private string BuildBody(string text, string callerName, int cut)
        {
            if (cut > 0 && text.Count(c => c == '\n') > cut)
                text = text.Replace("\r\n", " ").Replace('\n', ' ');

            string prefix = !string.IsNullOrEmpty(Emoji) ? $"[ {Emoji} ] " : "";
            return $"\n          {prefix}{text.Trim()}";
        }
        
        private void SendHttp(string body, LogType type, string caller, LogLevel level)
        {


            // Присвоение остальных переменных
            string prj     = _project?.Name.Replace(".zp", "") ?? "";
            string acc     = _project?.Var("acc0")             ?? "";
            string session = _project?.Var("varSessionId")     ?? "";
            string taskId  = _project?.TaskId                  ?? "";

            _ = Task.Run(async () =>
            {
                try
                {
                    var payload = new
                    {
                        machine   = Environment.MachineName,
                        project   = prj,
                        timestamp = DateTime.UtcNow.AddHours(_timezone).ToString("yyyy-MM-dd HH:mm:ss"),
                        level     = level.ToString().ToUpper(),
                        account   = acc,
                        session   = session,
                        port      = Port,
                        pid       = Pid,
                        task_id   = taskId,
                        caller    = caller,
                        message   = body.Trim(),
                        origin = "z3n7"
                    };

                    string json = JsonConvert.SerializeObject(payload);
                    using (var cts = new System.Threading.CancellationTokenSource(1000))
                    using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        await _httpClient.PostAsync(_logHost, content, cts.Token);
                    }
                }
                catch { }
            });
        }
    }
}

// ── Extension methods ─────────────────────────────────────────────────────────
namespace z3n7
{
    public static partial class ProjectExtensions
    {
        public static void log(
            this IZennoPosterProjectModel project,
            object toLog,
            [CallerMemberName] string callerName = "",
            bool show = true,
            bool thrw = false,
            bool toZp = true)
        {
            if (Regex.IsMatch(callerName, @"^M[a-f0-9]{32}$"))
                callerName = project.Name;
            Logger.Get(project).Send(toLog, callerName, show: show, thrw: thrw, toZp: toZp);
        }

        public static void warn(
            this IZennoPosterProjectModel project,
            string toLog,
            bool thrw = false,
            bool show = true,
            bool toZp = true,
            [CallerMemberName] string callerName = "")
            => Logger.Get(project).Warn(toLog, callerName, show: show, thrw: thrw, toZp: toZp);

        public static void warn(
            this IZennoPosterProjectModel project,
            Exception ex,
            bool thrw = false,
            bool withStack = false,
            bool toZp = true,
            [CallerMemberName] string callerName = "")
        {
            var msg = withStack ? ex.Message + "\n" + ex.StackTrace : ex.Message;
            Logger.Get(project).Warn(msg, callerName, show: true, thrw: thrw, toZp: toZp);
        }

        internal static void ObsoleteCode(this IZennoPosterProjectModel project, string newName = "unknown")
        {
            try
            {
                var sb = new StringBuilder();
                var trace = new StackTrace(1, true);
                string oldName = "", callerName = "";

                for (int i = 0; i < trace.FrameCount; i++)
                {
                    var frame = trace.GetFrame(i);
                    var method = frame?.GetMethod();
                    if (method?.DeclaringType == null) continue;

                    string typeName = method.DeclaringType.FullName ?? "";
                    if (typeName.StartsWith("System.") || typeName.StartsWith("ZennoLab.")) continue;

                    string methodName = $"{typeName}.{method.Name}";
                    if (i == 0) oldName = methodName;
                    else
                    {
                        callerName = methodName;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(callerName) || callerName == "z3nCore.Init.RunProject")
                    callerName = System.IO.Path.Combine(project.Path, project.Name);

                sb.Append($"![OBSOLETE CODE]. Method: [{oldName}] called from: [{callerName}]");
                if (!string.IsNullOrEmpty(newName)) sb.Append($". Use: [{newName}] instead");

                project.SendWarningToLog(sb.ToString().Trim(), true);
            }
            catch
            {
            }
        }
    }
}