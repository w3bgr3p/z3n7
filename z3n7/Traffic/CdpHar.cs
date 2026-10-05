using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    /// <summary>
    /// HAR recorder that talks to the instance browser over its own DevTools endpoint
    /// (the browser writes the port to &lt;user-data-dir&gt;\DevToolsActivePort).
    /// Does not depend on Tab.GetTraffic. Records only what happens after Start().
    /// </summary>
    public sealed class CdpHar : IDisposable
    {
        /// <summary>Most entries kept; the oldest are dropped beyond it.</summary>
        public int MaxEntries = 5000;
        /// <summary>Response bodies larger than this (encoded size) are not fetched.</summary>
        public long MaxBodyBytes = 10 * 1024 * 1024;
        /// <summary>Timeout for connecting and for each DevTools command, ms.</summary>
        public int CommandTimeoutMs = 10000;

        private static readonly Dictionary<int, CdpHar> Active = new Dictionary<int, CdpHar>();
        private static readonly object ActiveLock = new object();

        private readonly object _sync = new object();
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
        private readonly Dictionary<int, TaskCompletionSource<JObject>> _pending = new Dictionary<int, TaskCompletionSource<JObject>>();
        private readonly List<Entry> _entries = new List<Entry>();
        private readonly Dictionary<string, Entry> _byRequest = new Dictionary<string, Entry>();
        private readonly HashSet<string> _attachedTargets = new HashSet<string>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private ClientWebSocket _ws;
        private int _nextId;
        private int _bodyFetchesInFlight;

        /// <summary>DevTools port of the browser.</summary>
        public int DevToolsPort { get; private set; }
        /// <summary>Last error of the background receive loop, or <c>null</c>.</summary>
        public string LastError { get; private set; }
        /// <summary>Whether the DevTools connection is open.</summary>
        public bool IsAlive { get { return _ws != null && _ws.State == WebSocketState.Open; } }

        private CdpHar() { }

        // ─── Registry ───────────────────────────────────────────────────────

        /// <summary>Starts (or returns the running) recorder for this instance.</summary>
        public static CdpHar Start(Instance instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            lock (ActiveLock)
            {
                CdpHar existing;
                if (Active.TryGetValue(instance.Port, out existing))
                {
                    if (existing.IsAlive) return existing;
                    existing.Dispose();
                    Active.Remove(instance.Port);
                }

                var port = ResolveDevToolsPort(instance);
                var rec = Connect(port);
                Active[instance.Port] = rec;
                return rec;
            }
        }

        /// <summary>Recorder bound to an explicit DevTools port (not registered per instance).</summary>
        public static CdpHar Connect(int devToolsPort)
        {
            var rec = new CdpHar { DevToolsPort = devToolsPort };
            try { rec.Connect(); }
            catch { rec.Dispose(); throw; }
            return rec;
        }

        /// <summary>Running recorder for this instance or null.</summary>
        public static CdpHar For(Instance instance)
        {
            if (instance == null) return null;
            lock (ActiveLock)
            {
                CdpHar rec;
                return Active.TryGetValue(instance.Port, out rec) && rec.IsAlive ? rec : null;
            }
        }

        /// <summary>Stops and forgets the recorder of this instance, if any.</summary>
        public static void Stop(Instance instance)
        {
            if (instance == null) return;
            lock (ActiveLock)
            {
                CdpHar rec;
                if (!Active.TryGetValue(instance.Port, out rec)) return;
                Active.Remove(instance.Port);
                rec.Dispose();
            }
        }

        // ─── DevTools port discovery ────────────────────────────────────────

        /// <summary>
        /// Finds the DevTools port of the instance's browser from <c>DevToolsActivePort</c> files: the instance
        /// profile folder, the ProjectMaker browser folder and ZennoPoster's <c>Trash\Profiles\*</c>. The
        /// profile's own port wins when it is live; otherwise the only live candidate; otherwise the one whose
        /// page URL equals <c>ActiveTab.URL</c>.
        /// </summary>
        /// <returns>
        /// The port. Throws <c>InvalidOperationException</c> when no live browser or several matching ones are
        /// found.
        /// </returns>
        public static int ResolveDevToolsPort(Instance instance)
        {
            var files = new List<string>();
            Action<string> addDir = dir =>
            {
                try
                {
                    if (string.IsNullOrEmpty(dir)) return;
                    var f = Path.Combine(dir, "DevToolsActivePort");
                    if (File.Exists(f) && !files.Contains(f, StringComparer.OrdinalIgnoreCase)) files.Add(f);
                }
                catch { }
            };

            string profilePath = null;
            try { profilePath = instance.ProfilePath; } catch { }
            addDir(profilePath);
            addDir(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"ZennoLab\ZennoPoster\7\ProjectMaker\Browser"));
            try
            {
                var trash = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Trash\Profiles");
                if (Directory.Exists(trash))
                    foreach (var d in Directory.GetDirectories(trash)) addDir(d);
            }
            catch { }

            var live = new List<KeyValuePair<int, JArray>>();
            var seen = new List<string>();
            foreach (var f in files)
            {
                int port;
                try { port = int.Parse(File.ReadAllLines(f)[0].Trim()); }
                catch { continue; }
                var list = HttpJson("http://localhost:" + port + "/json/list") as JArray;
                seen.Add(f + " -> " + port + (list == null ? " (dead)" : ""));
                if (list != null) live.Add(new KeyValuePair<int, JArray>(port, list));
            }

            if (live.Count == 0)
                throw new InvalidOperationException("step=cdp_resolve | no live DevToolsActivePort. profilePath="
                    + profilePath + " | checked: " + string.Join("; ", seen));

            if (profilePath != null)
            {
                var own = Path.Combine(profilePath, "DevToolsActivePort");
                if (File.Exists(own))
                {
                    int ownPort;
                    if (int.TryParse(File.ReadAllLines(own)[0].Trim(), out ownPort) && live.Any(l => l.Key == ownPort))
                        return ownPort;
                }
            }
            if (live.Count == 1) return live[0].Key;

            string url = null;
            try { url = instance.ActiveTab.URL; } catch { }
            var matched = live.Where(l => l.Value.Any(t =>
                (string)t["type"] == "page" && string.Equals((string)t["url"], url, StringComparison.Ordinal))).ToList();
            if (matched.Count == 1) return matched[0].Key;

            throw new InvalidOperationException("step=cdp_resolve | " + matched.Count
                + " browsers match ActiveTab.URL=" + url + " | checked: " + string.Join("; ", seen));
        }

        private static JToken HttpJson(string url)
        {
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = 3000;
                req.ReadWriteTimeout = 3000;
                req.Proxy = null;
                using (var resp = req.GetResponse())
                using (var sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                    return JToken.Parse(sr.ReadToEnd());
            }
            catch { return null; }
        }

        // ─── Transport ──────────────────────────────────────────────────────

        private void Connect()
        {
            var ver = HttpJson("http://localhost:" + DevToolsPort + "/json/version");
            var wsUrl = ver == null ? null : (string)ver["webSocketDebuggerUrl"];
            if (string.IsNullOrEmpty(wsUrl))
                throw new InvalidOperationException("step=cdp_connect | no webSocketDebuggerUrl on port " + DevToolsPort);

            _ws = new ClientWebSocket();
            _ws.Options.Proxy = null;
            if (!_ws.ConnectAsync(new Uri(wsUrl), _cts.Token).Wait(CommandTimeoutMs))
                throw new TimeoutException("step=cdp_connect | websocket connect timeout: " + wsUrl);

            var loop = new Thread(ReceiveLoop) { IsBackground = true, Name = "CdpHar:" + DevToolsPort };
            loop.Start();

            Call(null, "Target.setDiscoverTargets", new JObject { ["discover"] = true });
            var targets = Call(null, "Target.getTargets", new JObject());
            foreach (var t in (JArray)targets["targetInfos"] ?? new JArray())
                if ((string)t["type"] == "page") Attach((string)t["targetId"]);
        }

        private void Attach(string targetId)
        {
            lock (_sync) { if (!_attachedTargets.Add(targetId)) return; }
            SendNoWait(null, "Target.attachToTarget", new JObject { ["targetId"] = targetId, ["flatten"] = true });
        }

        private JObject Call(string sessionId, string method, JObject prms)
        {
            var tcs = Send(sessionId, method, prms);
            if (!tcs.Task.Wait(CommandTimeoutMs))
                throw new TimeoutException("step=cdp_call | " + method + " timeout " + CommandTimeoutMs + "ms");
            var msg = tcs.Task.Result;
            if (msg["error"] != null)
                throw new InvalidOperationException("step=cdp_call | " + method + " | server: " + msg["error"].ToString(Formatting.None));
            return (JObject)msg["result"] ?? new JObject();
        }

        private void SendNoWait(string sessionId, string method, JObject prms, Action<JObject> onReply = null)
        {
            var tcs = Send(sessionId, method, prms);
            if (onReply != null)
                tcs.Task.ContinueWith(t => { try { onReply(t.Result); } catch { } }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private TaskCompletionSource<JObject> Send(string sessionId, string method, JObject prms)
        {
            var tcs = new TaskCompletionSource<JObject>();
            int id;
            lock (_sync) { id = ++_nextId; _pending[id] = tcs; }
            var msg = new JObject { ["id"] = id, ["method"] = method, ["params"] = prms ?? new JObject() };
            if (sessionId != null) msg["sessionId"] = sessionId;
            var bytes = Encoding.UTF8.GetBytes(msg.ToString(Formatting.None));

            _sendLock.Wait();
            try
            {
                if (!_ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts.Token).Wait(CommandTimeoutMs))
                    throw new TimeoutException("step=cdp_send | " + method);
            }
            catch (Exception ex)
            {
                lock (_sync) _pending.Remove(id);
                tcs.TrySetException(ex);
            }
            finally { _sendLock.Release(); }
            return tcs;
        }

        private void ReceiveLoop()
        {
            var buf = new byte[1 << 16];
            var ms = new MemoryStream();
            try
            {
                while (!_cts.IsCancellationRequested && _ws.State == WebSocketState.Open)
                {
                    var r = _ws.ReceiveAsync(new ArraySegment<byte>(buf), _cts.Token).Result;
                    if (r.MessageType == WebSocketMessageType.Close) break;
                    ms.Write(buf, 0, r.Count);
                    if (!r.EndOfMessage) continue;
                    var text = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                    ms.SetLength(0);
                    try { Dispatch(JObject.Parse(text)); }
                    catch (Exception ex) { LastError = "dispatch: " + ex.GetType().Name + ": " + ex.Message; }
                }
            }
            catch (Exception ex)
            {
                if (!_cts.IsCancellationRequested) LastError = "receive: " + ex.GetBaseException().Message;
            }
            finally
            {
                lock (_sync)
                {
                    foreach (var p in _pending.Values) p.TrySetCanceled();
                    _pending.Clear();
                }
            }
        }

        // ─── Events ─────────────────────────────────────────────────────────

        private sealed class Entry
        {
            public string Key, SessionId, RequestId, Url, Method, PostData, ResourceType;
            public JObject RequestHeaders, ResponseHeaders, ExtraRequestHeaders, ExtraResponseHeaders;
            public double WallTime, StartTs, EndTs, ResponseTs;
            public int Status;
            public string StatusText, Protocol, MimeType, RemoteIp, Failed, Body;
            public bool BodyBase64, FromCache, Finished;
            public long EncodedLength = -1;
        }

        private void Dispatch(JObject m)
        {
            var idTok = m["id"];
            if (idTok != null)
            {
                TaskCompletionSource<JObject> tcs;
                lock (_sync)
                {
                    var id = (int)idTok;
                    if (!_pending.TryGetValue(id, out tcs)) return;
                    _pending.Remove(id);
                }
                tcs.TrySetResult(m);
                return;
            }

            var method = (string)m["method"];
            var p = (JObject)m["params"] ?? new JObject();
            var sid = (string)m["sessionId"];

            switch (method)
            {
                case "Target.targetCreated":
                    if ((string)p["targetInfo"]["type"] == "page") Attach((string)p["targetInfo"]["targetId"]);
                    return;
                case "Target.targetDestroyed":
                    lock (_sync) _attachedTargets.Remove((string)p["targetId"]);
                    return;
                case "Target.attachedToTarget":
                    SendNoWait((string)p["sessionId"], "Network.enable", new JObject
                    {
                        ["maxTotalBufferSize"] = 200 * 1024 * 1024,
                        ["maxResourceBufferSize"] = 20 * 1024 * 1024
                    });
                    return;
            }

            if (sid == null || method == null || !method.StartsWith("Network.")) return;
            var rid = (string)p["requestId"];
            if (rid == null) return;
            var key = sid + "|" + rid;

            lock (_sync)
            {
                Entry e;
                _byRequest.TryGetValue(key, out e);
                switch (method)
                {
                    case "Network.requestWillBeSent":
                        if (e != null && p["redirectResponse"] != null)
                        {
                            FillResponse(e, (JObject)p["redirectResponse"]);
                            e.EndTs = (double)p["timestamp"];
                            e.Finished = true;
                        }
                        var req = (JObject)p["request"];
                        var ne = new Entry
                        {
                            Key = key, SessionId = sid, RequestId = rid,
                            Url = (string)req["url"] + ((string)req["urlFragment"] ?? ""),
                            Method = (string)req["method"],
                            RequestHeaders = (JObject)req["headers"] ?? new JObject(),
                            PostData = (string)req["postData"],
                            ResourceType = (string)p["type"],
                            WallTime = (double?)p["wallTime"] ?? 0,
                            StartTs = (double)p["timestamp"],
                        };
                        if (e != null && e.ExtraRequestHeaders != null && p["redirectResponse"] == null)
                            ne.ExtraRequestHeaders = e.ExtraRequestHeaders;
                        if (e != null && p["redirectResponse"] == null) _entries.Remove(e);
                        _entries.Add(ne);
                        _byRequest[key] = ne;
                        if (ne.PostData == null && (bool?)req["hasPostData"] == true)
                            SendNoWait(sid, "Network.getRequestPostData", new JObject { ["requestId"] = rid },
                                r => { if (r["result"] != null) lock (_sync) ne.PostData = (string)r["result"]["postData"]; });
                        Trim();
                        return;
                    case "Network.requestWillBeSentExtraInfo":
                        if (e == null) { e = new Entry { Key = key, SessionId = sid, RequestId = rid }; _byRequest[key] = e; }
                        e.ExtraRequestHeaders = (JObject)p["headers"];
                        return;
                }

                if (e == null) return;
                switch (method)
                {
                    case "Network.responseReceived":
                        FillResponse(e, (JObject)p["response"]);
                        e.ResponseTs = (double)p["timestamp"];
                        break;
                    case "Network.responseReceivedExtraInfo":
                        e.ExtraResponseHeaders = (JObject)p["headers"];
                        break;
                    case "Network.requestServedFromCache":
                        e.FromCache = true;
                        break;
                    case "Network.loadingFailed":
                        e.Failed = ((string)p["errorText"] ?? "") + (p["blockedReason"] != null ? " blocked=" + p["blockedReason"] : "");
                        e.EndTs = (double)p["timestamp"];
                        e.Finished = true;
                        break;
                    case "Network.loadingFinished":
                        e.EndTs = (double)p["timestamp"];
                        e.EncodedLength = (long?)p["encodedDataLength"] ?? -1;
                        e.Finished = true;
                        if (e.EncodedLength <= MaxBodyBytes) FetchBody(e);
                        break;
                }
            }
        }

        private void FetchBody(Entry e)
        {
            Interlocked.Increment(ref _bodyFetchesInFlight);
            var tcs = Send(e.SessionId, "Network.getResponseBody", new JObject { ["requestId"] = e.RequestId });
            tcs.Task.ContinueWith(t =>
            {
                try
                {
                    if (t.Status == TaskStatus.RanToCompletion && t.Result["result"] != null)
                        lock (_sync)
                        {
                            e.Body = (string)t.Result["result"]["body"];
                            e.BodyBase64 = (bool?)t.Result["result"]["base64Encoded"] == true;
                        }
                }
                finally { Interlocked.Decrement(ref _bodyFetchesInFlight); }
            });
        }

        private static void FillResponse(Entry e, JObject r)
        {
            e.Status = (int?)r["status"] ?? 0;
            e.StatusText = (string)r["statusText"] ?? "";
            e.Protocol = (string)r["protocol"];
            e.MimeType = (string)r["mimeType"];
            e.RemoteIp = (string)r["remoteIPAddress"];
            e.ResponseHeaders = (JObject)r["headers"] ?? new JObject();
            if ((bool?)r["fromDiskCache"] == true) e.FromCache = true;
        }

        private void Trim()
        {
            while (_entries.Count > MaxEntries)
            {
                _byRequest.Remove(_entries[0].Key);
                _entries.RemoveAt(0);
            }
        }

        // ─── Export ─────────────────────────────────────────────────────────

        /// <summary>Number of recorded entries.</summary>
        public int Count { get { lock (_sync) return _entries.Count; } }

        /// <summary>Forgets all recorded entries.</summary>
        public void Clear()
        {
            lock (_sync)
            {
                _entries.Clear();
                _byRequest.Clear();
            }
        }

        /// <summary>
        /// Writes the recorded traffic to a HAR 1.2 file (missing folders are created, an existing file is
        /// replaced).
        /// </summary>
        /// <param name="path">Target file.</param>
        /// <param name="urlRegex">Case-insensitive regex the URL must match; <c>null</c> keeps everything.</param>
        /// <param name="waitBodiesMs">How long to wait for response bodies still being fetched.</param>
        /// <returns>Number of entries written.</returns>
        public int Save(string path, string urlRegex = null, int waitBodiesMs = 3000)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException("path");

            var until = DateTime.UtcNow.AddMilliseconds(waitBodiesMs);
            while (Volatile.Read(ref _bodyFetchesInFlight) > 0 && DateTime.UtcNow < until) Thread.Sleep(50);

            var rx = string.IsNullOrEmpty(urlRegex) ? null : new Regex(urlRegex, RegexOptions.IgnoreCase);
            var entries = new JArray();
            lock (_sync)
            {
                foreach (var e in _entries)
                    if (rx == null || rx.IsMatch(e.Url ?? "")) entries.Add(ToHar(e));
            }

            var har = new JObject
            {
                ["log"] = new JObject
                {
                    ["version"] = "1.2",
                    ["creator"] = new JObject { ["name"] = "z3n7.CdpHar", ["version"] = "1.0" },
                    ["pages"] = new JArray(),
                    ["entries"] = entries
                }
            };

            var full = Path.GetFullPath(path);
            var dir = Path.GetDirectoryName(full);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(full, har.ToString(Formatting.Indented), new UTF8Encoding(false));
            return entries.Count;
        }

        private static JObject ToHar(Entry e)
        {
            var reqHeaders = e.ExtraRequestHeaders ?? e.RequestHeaders ?? new JObject();
            var respHeaders = e.ExtraResponseHeaders ?? e.ResponseHeaders ?? new JObject();
            var total = e.EndTs > 0 && e.StartTs > 0 ? Math.Max(0, (e.EndTs - e.StartTs) * 1000) : -1;
            var wait = e.ResponseTs > 0 && e.StartTs > 0 ? Math.Max(0, (e.ResponseTs - e.StartTs) * 1000) : -1;

            var request = new JObject
            {
                ["method"] = e.Method ?? "GET",
                ["url"] = e.Url ?? "",
                ["httpVersion"] = e.Protocol ?? "",
                ["cookies"] = Cookies(Header(reqHeaders, "cookie"), ';'),
                ["headers"] = HeaderList(reqHeaders),
                ["queryString"] = Query(e.Url ?? ""),
                ["headersSize"] = -1,
                ["bodySize"] = e.PostData == null ? 0 : Encoding.UTF8.GetByteCount(e.PostData)
            };
            if (e.PostData != null)
                request["postData"] = new JObject
                {
                    ["mimeType"] = Header(reqHeaders, "content-type") ?? "",
                    ["text"] = e.PostData
                };

            var content = new JObject
            {
                ["size"] = e.Body == null ? 0 : (e.BodyBase64 ? e.Body.Length * 3 / 4 : Encoding.UTF8.GetByteCount(e.Body)),
                ["mimeType"] = e.MimeType ?? Header(respHeaders, "content-type") ?? ""
            };
            if (e.Body != null)
            {
                content["text"] = e.Body;
                if (e.BodyBase64) content["encoding"] = "base64";
            }

            var setCookie = Header(respHeaders, "set-cookie");
            return new JObject
            {
                ["startedDateTime"] = e.WallTime > 0
                    ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(e.WallTime * 1000).ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'")
                    : DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"),
                ["time"] = total,
                ["request"] = request,
                ["response"] = new JObject
                {
                    ["status"] = e.Status,
                    ["statusText"] = e.StatusText ?? "",
                    ["httpVersion"] = e.Protocol ?? "",
                    ["cookies"] = setCookie == null ? new JArray() : new JArray(setCookie.Split('\n')
                        .Select(c => Cookies(c.Split(';')[0], ';')).SelectMany(a => a)),
                    ["headers"] = HeaderList(respHeaders),
                    ["content"] = content,
                    ["redirectURL"] = Header(respHeaders, "location") ?? "",
                    ["headersSize"] = -1,
                    ["bodySize"] = e.EncodedLength
                },
                ["cache"] = new JObject(),
                ["timings"] = new JObject { ["send"] = 0, ["wait"] = wait, ["receive"] = total >= 0 && wait >= 0 ? total - wait : -1 },
                ["serverIPAddress"] = e.RemoteIp ?? "",
                ["_resourceType"] = e.ResourceType ?? "",
                ["_fromCache"] = e.FromCache,
                ["_finished"] = e.Finished,
                ["_error"] = e.Failed
            };
        }

        private static string Header(JObject headers, string name)
        {
            foreach (var p in headers.Properties())
                if (string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)) return (string)p.Value;
            return null;
        }

        private static JArray HeaderList(JObject headers)
        {
            var a = new JArray();
            foreach (var p in headers.Properties())
                foreach (var v in ((string)p.Value ?? "").Split('\n'))
                    a.Add(new JObject { ["name"] = p.Name, ["value"] = v });
            return a;
        }

        private static JArray Cookies(string raw, char sep)
        {
            var a = new JArray();
            if (string.IsNullOrWhiteSpace(raw)) return a;
            foreach (var part in raw.Split(sep))
            {
                var s = part.Trim();
                var eq = s.IndexOf('=');
                if (eq <= 0) continue;
                a.Add(new JObject { ["name"] = s.Substring(0, eq).Trim(), ["value"] = s.Substring(eq + 1).Trim() });
            }
            return a;
        }

        private static JArray Query(string url)
        {
            var a = new JArray();
            var q = url.IndexOf('?');
            if (q < 0) return a;
            var query = url.Substring(q + 1);
            var h = query.IndexOf('#');
            if (h >= 0) query = query.Substring(0, h);
            foreach (var pair in query.Split('&'))
            {
                if (pair.Length == 0) continue;
                var eq = pair.IndexOf('=');
                a.Add(new JObject
                {
                    ["name"] = Unescape(eq < 0 ? pair : pair.Substring(0, eq)),
                    ["value"] = eq < 0 ? "" : Unescape(pair.Substring(eq + 1))
                });
            }
            return a;
        }

        private static string Unescape(string s)
        {
            try { return Uri.UnescapeDataString(s.Replace("+", " ")); } catch { return s; }
        }

        /// <summary>Closes the DevTools connection.</summary>
        public void Dispose()
        {
            try { _cts.Cancel(); } catch { }
            try
            {
                if (_ws != null && _ws.State == WebSocketState.Open)
                    _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).Wait(2000);
            }
            catch { }
            try { if (_ws != null) _ws.Dispose(); } catch { }
        }
    }

    /// <summary>Extension methods on <c>Instance</c>: HAR recording over DevTools.</summary>
    public static partial class InstanceExtensions
    {
        /// <summary>Starts HAR recording over DevTools for this instance. Call BEFORE the traffic you need.</summary>
        public static CdpHar StartHar(this Instance instance)
        {
            return CdpHar.Start(instance);
        }

        /// <summary>Writes the traffic recorded since <c>StartHar</c> to a HAR file. See <c>CdpHar.Save</c>.</summary>
        /// <param name="path">Target file.</param>
        /// <param name="urlRegex">Case-insensitive regex the URL must match; <c>null</c> keeps everything.</param>
        /// <returns>Number of entries written. Throws when the recorder was not started.</returns>
        public static int SaveHar(this Instance instance, string path, string urlRegex = null)
        {
            var rec = CdpHar.For(instance);
            if (rec == null) throw new InvalidOperationException("step=har_save | recorder not started: call instance.StartHar() first");
            return rec.Save(path, urlRegex);
        }

        /// <summary>Stops HAR recording for this instance.</summary>
        public static void StopHar(this Instance instance)
        {
            CdpHar.Stop(instance);
        }
    }
}
