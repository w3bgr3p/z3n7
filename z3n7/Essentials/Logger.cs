using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.Enums.Log;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3nIO
{
    public enum LogLevel { Debug = 0, Info = 1, Warning = 2, Error = 3, Off = 99 }

    public class Logger
    {
        // ── Static cache ──────────────────────────────────────────────────────
        private static readonly ConcurrentDictionary<string, Logger> _cache
            = new ConcurrentDictionary<string, Logger>();

        public static Logger Get(IZennoPosterProjectModel project, Instance instance = null)
        {
            string key = project.Var("acc0");

            if (instance != null && _cache.TryGetValue(key, out var existing) && existing._instance != instance)
                return _cache[key] = new Logger(project, instance);

            return _cache.GetOrAdd(key, _ => new Logger(project, instance));
        }

        public static void ClearCache(IZennoPosterProjectModel project)
            => _cache.TryRemove(project.Var("acc0"), out _);

        public Logger WithInstance(Instance instance)
        {
            var updated = new Logger(_project, instance, _minLevel, _logHost, _http);
            string key = _project?.Var("acc0") ?? "";
            if (!string.IsNullOrEmpty(key)) _cache[key] = updated;
            return updated;
        }

        // ── Config ────────────────────────────────────────────────────────────
        private readonly IZennoPosterProjectModel _project;
        private readonly Instance  _instance;
        private readonly LogLevel  _minLevel;
        private readonly string    _logHost;
        private readonly bool      _http;
        private readonly int       _timezone;
        private readonly string    _port;
        private readonly string    _pid;

        public string Emoji { get; set; }

        // cfgLog flags
        private readonly bool _fAcc, _fPort, _fTime, _fCaller, _fWrap, _fForce;

        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

        // ── Constructor ───────────────────────────────────────────────────────
        public Logger(
            IZennoPosterProjectModel project,
            Instance  instance       = null,
            LogLevel  logLevel       = LogLevel.Info,
            string    logHost        = null,
            bool      http           = true,
            int       timezoneOffset = -5,
            string    classEmoji     = null)
        {
            _project  = project;
            _instance = instance;
            _timezone = timezoneOffset;
            Emoji     = classEmoji;

            string levelVar = _project?.Var("logLevel");
            _minLevel = !string.IsNullOrEmpty(levelVar) && Enum.TryParse(levelVar, true, out LogLevel parsed)
                ? parsed
                : (_project?.Var("debug") == "True" ? LogLevel.Debug : logLevel);

            _logHost = !string.IsNullOrEmpty(logHost)                   ? logHost
                     : !string.IsNullOrEmpty(_project?.GVar("logHost")) ? _project.GVar("logHost")
                     : "http://localhost:10993/log";

            string cfg = _project?.Var("cfgLog") ?? "";
            _http    = http && cfg.Contains("http");
            _fAcc    = cfg.Contains("acc");
            _fPort   = cfg.Contains("port");
            _fTime   = cfg.Contains("time");
            _fCaller = cfg.Contains("caller");
            _fWrap   = cfg.Contains("wrap");
            _fForce  = cfg.Contains("force");

            if (_instance != null)
            {
                var m = Regex.Match(_instance.FormTitle ?? "", @"Port:(\d+); Pid:(\d+)");
                _port = m.Groups[1].Value;
                _pid  = m.Groups[2].Value;
            }
        }

        /// <summary>Standalone — без ZennoPoster контекста.</summary>
        public Logger(
            LogLevel logLevel       = LogLevel.Info,
            string   logHost        = null,
            bool     http           = true,
            int      timezoneOffset = -5,
            string   classEmoji     = null)
        {
            _minLevel = logLevel;
            _logHost  = logHost ?? "http://localhost:10993/log";
            _http     = http;
            _timezone = timezoneOffset;
            Emoji     = classEmoji;
            _fCaller  = true;
            _fWrap    = true;
        }

        // ── Public API ────────────────────────────────────────────────────────
        public void Send(
            object   toLog,
            [CallerMemberName] string caller = "",
            bool     show  = false,
            bool     thrw  = false,
            bool     toZp  = true,
            int      cut   = 0,
            LogLevel level = LogLevel.Info,
            LogType  type  = LogType.Info,
            LogColor color = LogColor.Default)
        {
            if (_fForce) show = true;
            if (!show && level < _minLevel) return;

            string body = BuildBody(toLog?.ToString() ?? "null", cut);

            if (level == LogLevel.Warning) type = LogType.Warning;
            if (level == LogLevel.Error)   type = LogType.Error;
            if (body.Contains("!W"))       type = LogType.Warning;
            if (body.Contains("!E"))       type = LogType.Error;

            string full = (_fWrap ? BuildHeader(caller) : "") + body;

            if (_project != null && toZp)
            {
                _project.SendToLog(full, type, toZp, color);
                if (thrw) throw new Exception(full);
            }

            if (_http) SendHttp(body, type, caller, level);
        }

        public void Debug(object msg, [CallerMemberName] string caller = "")
            => Send(msg, caller, level: LogLevel.Debug);

        public void Info(object msg, [CallerMemberName] string caller = "")
            => Send(msg, caller, level: LogLevel.Info);

        public void Warn(object msg, [CallerMemberName] string caller = "", bool show = false, bool thrw = false)
            => Send(msg, caller, show, thrw, level: LogLevel.Warning, type: LogType.Warning);

        public void Error(object msg, [CallerMemberName] string caller = "", bool thrw = false)
            => Send(msg, caller, show: true, thrw: thrw, level: LogLevel.Error, type: LogType.Error);

        // ── Private ───────────────────────────────────────────────────────────
        private string BuildHeader(string caller)
        {
            var sb = new StringBuilder();
            if (_project != null)
            {
                if (_fAcc)  sb.Append($"  🤖 [{_project.Var("acc0")}]");
                if (_fTime) sb.Append($"  ⏱️ [{_project.Age<string>()}]");
                if (_fPort) sb.Append($"  🔌 [{_project.Var("instancePort")}]");
            }
            if (_fCaller) sb.Append($"  🔲 [{caller}]");
            return sb.ToString();
        }

        private string BuildBody(string text, int cut)
        {
            if (cut > 0 && text.Count(c => c == '\n') > cut)
                text = text.Replace("\r\n", " ").Replace('\n', ' ');

            string prefix = !string.IsNullOrEmpty(Emoji) ? $"[ {Emoji} ] " : "";
            return $"\n          {prefix}{text.Trim()}";
        }

        private void SendHttp(string body, LogType type, string caller, LogLevel level)
        {
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
                        machine    = Environment.MachineName,
                        project    = prj,
                        timestamp  = DateTime.UtcNow.AddHours(_timezone).ToString("yyyy-MM-dd HH:mm:ss"),
                        level      = level.ToString().ToUpper(),
                        account    = acc,
                        session    = session,
                        port       = _port,
                        pid        = _pid,
                        task_id    = taskId,
                        caller     = caller,
                        message    = body.Trim(),
                        origin     = "z3n7",
                        elapsed_ms = _project.Age<long>(),
                    };

                    string json = JsonConvert.SerializeObject(payload);
                    using var cts     = new System.Threading.CancellationTokenSource(1000);
                    using var content = new StringContent(json, Encoding.UTF8, "application/json");
                    await _httpClient.PostAsync(_logHost, content, cts.Token);
                }
                catch { }
            });
        }
    }
}

// ── Extensions ────────────────────────────────────────────────────────────────
namespace z3nIO
{
    public static partial class ProjectExtensions
    {
        public static void log(
            this IZennoPosterProjectModel project,
            object toLog,
            [CallerMemberName] string caller = "",
            bool show = true,
            bool thrw = false,
            bool toZp = true)
        {
            if (Regex.IsMatch(caller, @"^M[a-f0-9]{32}$"))
                caller = project.Name;
            Logger.Get(project).Send(toLog, caller, show: show, thrw: thrw, toZp: toZp);
        }

        public static void warn(
            this IZennoPosterProjectModel project,
            string toLog,
            bool thrw = false,
            bool show = true,
            bool toZp = true,
            [CallerMemberName] string caller = "")
            => Logger.Get(project).Warn(toLog, caller, show: show, thrw: thrw);

        public static void warn(
            this IZennoPosterProjectModel project,
            Exception ex,
            bool thrw      = false,
            bool withStack = false,
            bool toZp      = true,
            [CallerMemberName] string caller = "")
        {
            var msg = withStack ? ex.Message + "\n" + ex.StackTrace : ex.Message;
            Logger.Get(project).Warn(msg, caller, show: true, thrw: thrw);
        }
    }
}