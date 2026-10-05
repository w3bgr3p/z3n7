using System;
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

namespace z3n7
{
    /// <summary>
    /// Message severity. A logger drops messages below its minimum level; <c>Off</c> drops everything
    /// except forced messages.
    /// </summary>
    public enum LogLevel { Debug = 0, Info = 1, Warning = 2, Error = 3, Off = 99 }

    /// <summary>
    /// Writes messages to the ZennoPoster log and, optionally, as JSON to an HTTP log collector.
    /// Header fields are switched on by substrings of the <c>cfgLog</c> project variable: <c>acc</c>
    /// (account), <c>time</c> (project age), <c>port</c> (instance port), <c>caller</c> (calling member),
    /// <c>wrap</c> (print the header at all), <c>http</c> (send to the collector), <c>force</c> (ignore the
    /// level filter).
    /// </summary>
    public class Logger
    {
        /// <summary>Creates a logger for the project with default settings.</summary>
        public static Logger Get(IZennoPosterProjectModel project, Instance instance = null)
            => new Logger(project, instance);

        /// <summary>Does nothing. Kept for compatibility: the logger no longer caches instances.</summary>
        public static void ClearCache(IZennoPosterProjectModel project)
        {
            // Kept for compatibility. Logger no longer has a cache.
        }

        /// <summary>
        /// Returns a copy of this logger bound to <c>instance</c> (its port and PID are sent to the collector).
        /// </summary>
        public Logger WithInstance(Instance instance)
            => new Logger(_project, instance, _minLevel, _logHost, _http, _timezone, Emoji);

        // ── Config ────────────────────────────────────────────────────────────
        private readonly IZennoPosterProjectModel _project;
        private readonly LogLevel  _minLevel;
        private readonly string    _logHost;
        private readonly bool      _http;
        private readonly int       _timezone;
        private readonly string    _port;
        private readonly string    _pid;

        /// <summary>Prefix shown in brackets before every message, e.g. the class marker.</summary>
        public string Emoji { get; set; }

        // cfgLog flags
        private readonly bool _fAcc, _fPort, _fTime, _fCaller, _fWrap, _fForce;

        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

        // ── Constructor ───────────────────────────────────────────────────────
        /// <summary>
        /// Creates a logger bound to a ZennoPoster project.
        /// The minimum level is taken from the <c>logLevel</c> project variable when it parses as
        /// <c>LogLevel</c>; otherwise <c>Debug</c> when <c>debug</c> is <c>True</c>; otherwise the
        /// <c>logLevel</c> argument. The collector address is <c>logHost</c>, then the <c>logHost</c> global
        /// variable, then <c>http://localhost:10993/log</c>.
        /// </summary>
        /// <param name="instance">
        /// Optional browser instance; its port and PID are read from the window title and sent to the
        /// collector.
        /// </param>
        /// <param name="logLevel">Minimum level when the project variables do not set one.</param>
        /// <param name="logHost">URL of the HTTP log collector.</param>
        /// <param name="http">
        /// Allow sending to the collector. It is sent only when <c>cfgLog</c> also contains <c>http</c>.
        /// </param>
        /// <param name="timezoneOffset">Hours added to UTC for the collector timestamp.</param>
        /// <param name="classEmoji">Value of <c>Emoji</c>.</param>
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

            if (instance != null)
            {
                var m = Regex.Match(instance.FormTitle ?? "", @"Port:(\d+); Pid:(\d+)");
                _port = m.Groups[1].Value;
                _pid  = m.Groups[2].Value;
            }
        }

        /// <summary>
        /// Creates a logger without a ZennoPoster project: messages go only to the HTTP collector. The caller
        /// name is always included.
        /// </summary>
        /// <param name="logLevel">Minimum level.</param>
        /// <param name="logHost">URL of the HTTP log collector; default <c>http://localhost:10993/log</c>.</param>
        /// <param name="http">Send messages to the collector.</param>
        /// <param name="timezoneOffset">Hours added to UTC for the collector timestamp.</param>
        /// <param name="classEmoji">Value of <c>Emoji</c>.</param>
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
        /// <summary>
        /// Writes a message. Messages below the minimum level are dropped unless <c>show</c> is true or
        /// <c>cfgLog</c> contains <c>force</c>.
        /// The ZennoPoster log type follows the level; text containing <c>!W</c> or <c>!E</c> is logged as a
        /// warning or an error.
        /// </summary>
        /// <param name="toLog">Message; <c>ToString()</c> is used, <c>null</c> is written as "null".</param>
        /// <param name="caller">Filled in by the compiler with the calling member name.</param>
        /// <param name="show">Write even if below the minimum level.</param>
        /// <param name="thrw">
        /// After writing to the ZennoPoster log, throw an <c>Exception</c> with the message. Only when the
        /// logger has a project and <c>toZp</c> is true.
        /// </param>
        /// <param name="toZp">Write to the ZennoPoster log.</param>
        /// <param name="cut">
        /// When the message has more than this many line breaks, join it into one line. 0 keeps it as is.
        /// </param>
        /// <param name="level">Severity used for filtering and for the collector.</param>
        /// <param name="type">
        /// ZennoPoster log type; overridden by <c>level</c> Warning/Error and by the <c>!W</c>/<c>!E</c>
        /// markers.
        /// </param>
        /// <param name="color">ZennoPoster log color.</param>
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

        /// <summary>Writes a message at <c>Debug</c> level.</summary>
        public void Debug(object msg, [CallerMemberName] string caller = "")
            => Send(msg, caller, level: LogLevel.Debug);

        /// <summary>Writes a message at <c>Info</c> level.</summary>
        public void Info(object msg, [CallerMemberName] string caller = "")
            => Send(msg, caller, level: LogLevel.Info);

        /// <summary>Writes a warning.</summary>
        /// <param name="show">Write even if below the minimum level.</param>
        /// <param name="thrw">Throw an <c>Exception</c> with the message after writing.</param>
        public void Warn(object msg, [CallerMemberName] string caller = "", bool show = false, bool thrw = false)
            => Send(msg, caller, show, thrw, level: LogLevel.Warning, type: LogType.Warning);

        /// <summary>Writes an error. Errors are always written regardless of the minimum level.</summary>
        /// <param name="thrw">Throw an <c>Exception</c> with the message after writing.</param>
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
namespace z3n7
{
    public static partial class ProjectExtensions
    {
        /// <summary>
        /// Writes a message to the project log through a default <c>Logger</c>. When called directly from a C#
        /// action, the generated action name is replaced by the project name.
        /// </summary>
        /// <param name="toLog">Message.</param>
        /// <param name="show">Write even if below the minimum level.</param>
        /// <param name="toZp">Write to the ZennoPoster log.</param>
        public static void log(
            this IZennoPosterProjectModel project,
            object toLog,
            [CallerMemberName] string caller = "",
            bool show = true,
            bool toZp = true)
        {
            if (Regex.IsMatch(caller, @"^M[a-f0-9]{32}$"))
                caller = project.Name;
            Logger.Get(project).Send(toLog, caller, show: show,  toZp: toZp);
        }

        /// <summary>Writes a warning to the project log.</summary>
        /// <param name="msg">Message.</param>
        /// <param name="thrw">Also store the message in the <c>err</c> variable and throw an <c>Exception</c>.</param>
        /// <param name="show">Write even if below the minimum level.</param>
        public static void warn(
            this IZennoPosterProjectModel project,
            string msg,
            bool thrw = false,
            bool show = true,
            [CallerMemberName] string caller = "")
        {
            if (thrw)
                project.Var("err", msg);
            Logger.Get(project).Warn(msg, caller, show: show, thrw: thrw);
            
        }
        

        /// <summary>Writes an exception message as a warning and stores it in the <c>err</c> variable.</summary>
        /// <param name="ex">Exception to report.</param>
        /// <param name="thrw">Throw an <c>Exception</c> with the message after writing.</param>
        /// <param name="withStack">Append the stack trace.</param>
        /// <param name="toZp">Not used.</param>
        public static void warn(
            this IZennoPosterProjectModel project,
            Exception ex,
            bool thrw      = false,
            bool withStack = false,
            bool toZp      = true,
            [CallerMemberName] string caller = "")
        {
            var msg = withStack ? ex.Message + "\n" + ex.StackTrace : ex.Message;
            project.Var("err", msg);
            Logger.Get(project).Warn(msg, caller, show: true, thrw: thrw);
        }
    }
}
