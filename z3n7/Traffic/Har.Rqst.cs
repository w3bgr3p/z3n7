using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    public static partial class HarTraffic
    {
        /// <summary>Exports all complete saved Rqst transactions as a HAR 1.2 JSON document.</summary>
        public static string ExportRqst(IZennoPosterProjectModel project, string projectFilter = null, string taskId = null)
            => FromRqstJsonl(ZpTraffic.FilePath(project), projectFilter, taskId);

        public static string FromRqstJsonl(string path, string projectFilter = null, string taskId = null)
        {
            var page = ZpTraffic.Read(path, 0, int.MaxValue, projectFilter, taskId);
            var entries = new JArray(page.entries.Select(record => RqstEntry(
                JsonConvert.DeserializeObject<JObject>(record.GetRawText(),
                    new JsonSerializerSettings { DateParseHandling = DateParseHandling.None }))));
            return new JObject { ["log"] = new JObject {
                ["version"] = "1.2",
                ["creator"] = new JObject { ["name"] = "z3n7.Rqst", ["version"] = typeof(HarTraffic).Assembly.GetName().Version.ToString() },
                ["entries"] = entries
            }}.ToString(Formatting.None);
        }

        private static JObject RqstEntry(JObject row)
        {
            var rq = row["request"] as JObject ?? new JObject();
            var rs = row["response"] as JObject ?? new JObject();
            var headers = Headers(string.Join("\r\n", (rq["headers"] as JArray ?? new JArray()).Values<string>()));
            AddRqstHeader(headers, "Content-Type", (string)rq["contentType"]);
            AddRqstHeader(headers, "User-Agent", (string)rq["userAgent"]);
            AddRqstHeader(headers, "Cookie", (string)rq["cookies"]);
            var responseRaw = (string)rs["headers"] ?? "";
            var responseHeaders = Headers(responseRaw);
            var url = ((string)row["url"] ?? "").Split('#')[0];
            var requestBody = (string)rq["body"] ?? "";
            var responseBody = (string)rs["body"] ?? "";
            var duration = Math.Max(0, (double?)row["durationMs"] ?? 0);
            DateTimeOffset started;
            if (row["startedDateTime"] != null)
                started = DateTimeOffset.Parse((string)row["startedDateTime"], CultureInfo.InvariantCulture);
            else
                started = new DateTimeOffset(DateTime.ParseExact((string)row["timestamp"],
                    "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), TimeSpan.FromHours(-5)).AddMilliseconds(-duration);
            var request = new JObject {
                ["method"] = (string)row["method"] ?? "GET", ["url"] = url, ["httpVersion"] = "",
                ["headers"] = headers, ["cookies"] = ParseRequestCookies(RqstHeader(headers, "Cookie")),
                ["queryString"] = Query(url), ["headersSize"] = -1, ["bodySize"] = Encoding.UTF8.GetByteCount(requestBody)
            };
            if (requestBody.Length > 0)
                request["postData"] = new JObject { ["mimeType"] = RqstHeader(headers, "Content-Type"), ["text"] = requestBody };
            var cookies = ParseResponseCookies(string.Join("\r\n", responseHeaders
                .Where(h => string.Equals((string)h["name"], "Set-Cookie", StringComparison.OrdinalIgnoreCase))
                .Select(h => (string)h["value"])));
            foreach (JObject cookie in cookies)
            {
                if (cookie["expires"] == null) continue;
                if (DateTimeOffset.TryParse((string)cookie["expires"], CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal, out var expires)) cookie["expires"] = expires.ToString("o");
                else cookie.Remove("expires");
            }
            var statusLine = responseRaw.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[0].Split(new[] { ' ' }, 3);
            return new JObject {
                ["startedDateTime"] = started.ToString("o"), ["time"] = duration,
                ["request"] = request,
                ["response"] = new JObject {
                    ["status"] = (int?)row["statusCode"] ?? 0,
                    ["statusText"] = statusLine.Length == 3 ? statusLine[2] : "",
                    ["httpVersion"] = responseRaw.StartsWith("HTTP/", StringComparison.OrdinalIgnoreCase) ? HttpVersion(responseRaw) : "",
                    ["headers"] = responseHeaders, ["cookies"] = cookies,
                    ["content"] = new JObject { ["size"] = Encoding.UTF8.GetByteCount(responseBody),
                        ["mimeType"] = RqstHeader(responseHeaders, "Content-Type"), ["text"] = responseBody },
                    ["redirectURL"] = RqstHeader(responseHeaders, "Location"), ["headersSize"] = -1, ["bodySize"] = -1
                },
                ["cache"] = new JObject(),
                ["timings"] = new JObject { ["send"] = 0, ["wait"] = duration, ["receive"] = 0,
                    ["comment"] = "Rqst records only total duration; timing phases are not measured separately." },
                ["_project"] = row["project"], ["_task_id"] = row["task_id"], ["_session"] = row["session"],
                ["comment"] = "Exported from saved Rqst text. Unrecorded headers/protocols and original binary bytes cannot be reconstructed."
            };
        }

        private static string RqstHeader(JArray headers, string name)
            => headers.FirstOrDefault(h => string.Equals((string)h["name"], name, StringComparison.OrdinalIgnoreCase))?["value"]?.ToString() ?? "";

        private static void AddRqstHeader(JArray headers, string name, string value)
        {
            if (!string.IsNullOrEmpty(value) && !headers.Any(h => string.Equals((string)h["name"], name, StringComparison.OrdinalIgnoreCase)))
                headers.Add(new JObject { ["name"] = name, ["value"] = value });
        }
    }
}
