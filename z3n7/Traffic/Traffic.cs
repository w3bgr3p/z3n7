using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
namespace z3n7

{
    public class Traffic
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly Instance                 _instance;
        private string _defaultFilter;

        public Traffic(IZennoPosterProjectModel project, Instance instance, string defaultFilter = null)
        {
            _project  = project;
            _instance = instance;
            _instance.UseTrafficMonitoring = true;
            _defaultFilter = defaultFilter;
        }

        // ─── Snapshot ──────────────────────────────────────────────────────────

        private List<TrafficElement> Grab()
        {
            var filter = _defaultFilter ?? _instance.ActiveTab.Domain;
            var raw = _instance.ActiveTab.GetTraffic(new[] { filter }).ToList(); // материализуем сразу
            return raw
                .Where(r => r.Method != "OPTIONS")
                .Select(r => ToElement(r))
                .ToList();
        }

        // ─── Public API ────────────────────────────────────────────────────────

        public TrafficElement Find(string url, bool strict = false, int timeoutSec = 15)
        {
            var deadline = DateTime.Now.AddSeconds(timeoutSec);
            while (DateTime.Now < deadline)
            {
                var el = Grab().FirstOrDefault(e => Matches(e.Url, url, strict));
                if (el != null) return el;
                Thread.Sleep(1000);
            }
            throw new TimeoutException($"Traffic not found: '{url}' in {timeoutSec}s");
        }

        public List<TrafficElement> FindAll(string url, bool strict = false)
        {
            return Grab().Where(e => Matches(e.Url, url, strict)).ToList();
        }

        public string GetApiStructure(string urlFilter = "api", bool includeHeaders = false, bool excludeFiles = true)
        {
            var unique = new Dictionary<string, JObject>();

            foreach (var el in FindAll(urlFilter))
            {
                if (excludeFiles && IsFile(el.Url)) continue;

                string key = $"{el.Method}:{el.Url}";
                if (unique.ContainsKey(key)) continue;

                var obj = new JObject();
                obj["method"] = el.Method;
                obj["url"]    = el.Url;

                if (includeHeaders)
                {
                    obj["requestHeaders"]  = ParseHeaders(el.RequestHeaders);
                    obj["responseHeaders"] = ParseHeaders(el.ResponseHeaders);
                }

                if (!string.IsNullOrEmpty(el.RequestBody))  obj["requestBody"]  = TryJson(el.RequestBody);
                if (!string.IsNullOrEmpty(el.ResponseBody)) obj["responseBody"] = TryJson(el.ResponseBody);

                unique[key] = obj;
            }

            var result = new JObject();
            result["total"] = unique.Count;
            foreach (var g in unique.Values.GroupBy(e => e["method"]?.ToString().ToUpper()))
                result[$"{g.Key.ToLower()}Endpoints"] = new JArray(g.ToList());

            string json = JsonConvert.SerializeObject(result, Formatting.Indented);
            
            return json;
        }

        public void SaveHeadersToVar(string url, string varName = "headers", bool strict = false)
        {
            var el  = Find(url, strict);
            var sb  = new StringBuilder();
            foreach (var line in el.RequestHeaders.Split('\n'))
            {
                var t = line.Trim();
                if (string.IsNullOrEmpty(t) || t.StartsWith(":")) continue;
                sb.AppendLine(t);
            }
            _project.Var(varName, sb.ToString());
        }

        // ─── TrafficElement ────────────────────────────────────────────────────

        public class TrafficElement
        {
            public string Method          { get; internal set; }
            public string Url             { get; internal set; }
            public string StatusCode      { get; internal set; }
            public string RequestHeaders  { get; internal set; }
            public string RequestCookies  { get; internal set; }
            public string RequestBody     { get; internal set; }
            public string ResponseHeaders { get; internal set; }
            public string ResponseCookies { get; internal set; }
            public string ResponseBody    { get; internal set; }
        }

        // ─── Helpers ───────────────────────────────────────────────────────────

        private static bool Matches(string url, string filter, bool strict) =>
            strict ? url == filter : url.Contains(filter);

        private static bool IsFile(string url)
        {
            string seg = url.Split('?')[0];
            seg = seg.Substring(seg.LastIndexOf('/') + 1);
            return !url.Split('?')[0].EndsWith("/") && seg.Contains(".");
        }

        private static JToken TryJson(string s)
        {
            try { return JToken.Parse(s); } catch { return s; }
        }

        private static JObject ParseHeaders(string raw)
        {
            var obj = new JObject();
            if (string.IsNullOrEmpty(raw)) return obj;
            foreach (Match m in Regex.Matches(raw, @"([\w\-]+):\s(.+?)(?=[\w\-]+:\s|$)", RegexOptions.Singleline))
                obj[m.Groups[1].Value.Trim()] = m.Groups[2].Value.Trim();
            return obj;
        }

        private TrafficElement ToElement(dynamic r)
        {
            byte[] raw  = r.ResponseBody as byte[] ?? Array.Empty<byte>();
            string body = raw.Length == 0 ? "" : Decompress(raw, (r.ResponseHeaders ?? "").ToString());

            return new TrafficElement
            {
                Method          = r.Method              ?? "",
                StatusCode      = r.ResultCode?.ToString() ?? "",
                Url             = r.Url                  ?? "",
                RequestHeaders  = r.RequestHeaders       ?? "",
                RequestCookies  = r.RequestCookies       ?? "",
                RequestBody     = r.RequestBody          ?? "",
                ResponseHeaders = r.ResponseHeaders      ?? "",
                ResponseCookies = r.ResponseCookies      ?? "",
                ResponseBody    = body
            };
        }

        private static string Decompress(byte[] data, string headers)
        {
            if (headers.Contains("content-encoding: gzip") || headers.Contains("content-encoding: deflate"))
            {
                try
                {
                    using (var ms  = new System.IO.MemoryStream(data))
                    using (var gz  = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress))
                    using (var dst = new System.IO.MemoryStream())
                    {
                        gz.CopyTo(dst);
                        return Encoding.UTF8.GetString(dst.ToArray());
                    }
                }
                catch { }
            }
            try { return Encoding.UTF8.GetString(data); } catch { return ""; }
        }
    }
}