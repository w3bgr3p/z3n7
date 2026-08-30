using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    /// <summary>
    /// Standalone HAR exporter for a ZennoPoster C# action.
    /// </summary>
    public static class HarTraffic
    {
        // ZennoPoster 7.9 does not return traffic with an empty filter set and
        // does not reliably handle catch-all patterns. Filters are OR-ed, and
        // every valid HTTP(S) host contains at least one ASCII letter or digit.
        private static readonly string[] AllUrlFilters =
            "abcdefghijklmnopqrstuvwxyz0123456789"
                .Select(character => character.ToString())
                .ToArray();

        public static int Save(
            Instance instance,
            string path)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException("path");
            if (instance.ActiveTab == null) throw new InvalidOperationException("ActiveTab is null");

            var traffic = instance.ActiveTab.GetTraffic(
                new ZennoLab.CommandCenter.Classes.GetTrafficSettings
                {
                    GatherAllTraffic = true,
                    UrlFilters = AllUrlFilters
                }).ToList();
            var entries = new JArray();
            var skipped = new List<string>();

            for (var i = 0; i < traffic.Count; i++)
            {
                try
                {
                    entries.Add(ToEntry(traffic[i]));
                }
                catch (Exception ex)
                {
                    skipped.Add("#" + i + ": " + ex.GetType().Name + ": " + ex.Message);
                }
            }

            var har = new JObject(
                new JProperty("log", new JObject(
                    new JProperty("version", "1.2"),
                    new JProperty("creator", new JObject(
                        new JProperty("name", "ZennoPoster HarTraffic"),
                        new JProperty("version", "1.0"))),
                    new JProperty("pages", new JArray()),
                    new JProperty("entries", entries))));

            var fullPath = Path.GetFullPath(path);
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(
                fullPath,
                JsonConvert.SerializeObject(har, Formatting.Indented),
                new UTF8Encoding(false));
            

            return entries.Count;
        }

        private static JObject ToEntry(TrafficItem item)
        {
            var requestHeadersRaw = item.RequestHeaders ?? "";
            var responseHeadersRaw = item.ResponseHeaders ?? "";
            var requestBody = item.RequestBody ?? "";
            var responseBody = DecodeBody(item.ResponseBody ?? new byte[0], responseHeadersRaw);
            var mime = HeaderValue(responseHeadersRaw, "content-type")
                       ?? item.ResponseContentType
                       ?? "application/octet-stream";
            var elapsed = item.Time < 0 ? 0 : item.Time;
            var status = (int)item.ResultCode;

            var request = new JObject(
                new JProperty("method", item.Method ?? "GET"),
                new JProperty("url", item.Url ?? ""),
                new JProperty("httpVersion", HttpVersion(requestHeadersRaw)),
                new JProperty("cookies", ParseRequestCookies(item.RequestCookies ?? "")),
                new JProperty("headers", Headers(requestHeadersRaw)),
                new JProperty("queryString", Query(item.Url ?? "")),
                new JProperty("headersSize", ByteCountOrMinusOne(requestHeadersRaw)),
                new JProperty("bodySize", Encoding.UTF8.GetByteCount(requestBody)));

            if (!string.IsNullOrEmpty(requestBody))
            {
                var requestMime = HeaderValue(requestHeadersRaw, "content-type") ?? "";
                request["postData"] = new JObject(
                    new JProperty("mimeType", requestMime),
                    new JProperty("text", requestBody),
                    new JProperty("params", requestMime.IndexOf("x-www-form-urlencoded",
                        StringComparison.OrdinalIgnoreCase) >= 0
                        ? FormParams(requestBody)
                        : new JArray()));
            }

            var content = new JObject(
                new JProperty("size", responseBody.Length),
                new JProperty("mimeType", mime));

            if (responseBody.Length > 0)
            {
                if (IsText(mime))
                    content["text"] = DecodeText(
                        responseBody,
                        mime,
                        item.ResponseContentCharset ?? "");
                else
                {
                    content["text"] = Convert.ToBase64String(responseBody);
                    content["encoding"] = "base64";
                }
            }

            var response = new JObject(
                new JProperty("status", status),
                new JProperty("statusText", StatusText(status)),
                new JProperty("httpVersion", HttpVersion(responseHeadersRaw)),
                new JProperty("cookies", ParseResponseCookies(item.ResponseCookies ?? "")),
                new JProperty("headers", Headers(responseHeadersRaw)),
                new JProperty("content", content),
                new JProperty("redirectURL", HeaderValue(responseHeadersRaw, "location") ?? ""),
                new JProperty("headersSize", ByteCountOrMinusOne(responseHeadersRaw)),
                new JProperty("bodySize", responseBody.Length));

            return new JObject(
                new JProperty("startedDateTime",
                    DateTimeOffset.Now.AddMilliseconds(-elapsed).ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz")),
                new JProperty("time", elapsed),
                new JProperty("request", request),
                new JProperty("response", response),
                new JProperty("cache", new JObject()),
                new JProperty("timings", new JObject(
                    new JProperty("send", 0),
                    new JProperty("wait", elapsed),
                    new JProperty("receive", 0))),
                new JProperty("_resourceType", ResourceType(mime)),
                new JProperty("_blocked", item.IsBlocked),
                new JProperty("_hasResponse", item.HasResponse));
        }

        private static JArray Headers(string raw)
        {
            var result = new JArray();
            if (string.IsNullOrEmpty(raw)) return result;

            foreach (var sourceLine in raw.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                var line = sourceLine.Trim();
                if (line.Length == 0) continue;

                var searchFrom = line.StartsWith(":", StringComparison.Ordinal) ? 1 : 0;
                var separator = line.IndexOf(':', searchFrom);
                if (separator <= 0) continue;

                result.Add(new JObject(
                    new JProperty("name", line.Substring(0, separator).Trim()),
                    new JProperty("value", line.Substring(separator + 1).Trim())));
            }
            return result;
        }

        private static string HeaderValue(string raw, string name)
        {
            if (string.IsNullOrEmpty(raw)) return null;
            foreach (var line in raw.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                var separator = line.IndexOf(':');
                if (separator <= 0) continue;
                if (string.Equals(line.Substring(0, separator).Trim(), name,
                    StringComparison.OrdinalIgnoreCase))
                    return line.Substring(separator + 1).Trim();
            }
            return null;
        }

        private static JArray Query(string url)
        {
            var result = new JArray();
            var question = url.IndexOf('?');
            if (question < 0 || question == url.Length - 1) return result;

            var query = url.Substring(question + 1);
            var fragment = query.IndexOf('#');
            if (fragment >= 0) query = query.Substring(0, fragment);

            foreach (var pair in query.Split('&'))
            {
                if (pair.Length == 0) continue;
                var equals = pair.IndexOf('=');
                result.Add(new JObject(
                    new JProperty("name", UrlDecode(equals < 0 ? pair : pair.Substring(0, equals))),
                    new JProperty("value", equals < 0 ? "" : UrlDecode(pair.Substring(equals + 1)))));
            }
            return result;
        }

        private static JArray FormParams(string body)
        {
            var result = new JArray();
            if (string.IsNullOrEmpty(body)) return result;

            foreach (var pair in body.Split('&'))
            {
                if (pair.Length == 0) continue;
                var equals = pair.IndexOf('=');
                result.Add(new JObject(
                    new JProperty("name", UrlDecode(equals < 0 ? pair : pair.Substring(0, equals))),
                    new JProperty("value", equals < 0 ? "" : UrlDecode(pair.Substring(equals + 1)))));
            }
            return result;
        }

        private static JArray ParseRequestCookies(string raw)
        {
            var result = new JArray();
            if (string.IsNullOrWhiteSpace(raw)) return result;

            var value = raw.Trim();
            if (value.StartsWith("Cookie:", StringComparison.OrdinalIgnoreCase))
                value = value.Substring("Cookie:".Length).Trim();

            foreach (var part in value.Split(';'))
            {
                var cookie = CookiePair(part);
                if (cookie != null) result.Add(cookie);
            }
            return result;
        }

        private static JArray ParseResponseCookies(string raw)
        {
            var result = new JArray();
            if (string.IsNullOrWhiteSpace(raw)) return result;

            foreach (var sourceLine in raw.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var line = sourceLine.Trim();
                if (line.StartsWith("Set-Cookie:", StringComparison.OrdinalIgnoreCase))
                    line = line.Substring("Set-Cookie:".Length).Trim();

                var parts = line.Split(';');
                var cookie = CookiePair(parts[0]);
                if (cookie == null) continue;

                for (var i = 1; i < parts.Length; i++)
                {
                    var attribute = parts[i].Trim();
                    var equals = attribute.IndexOf('=');
                    var name = (equals < 0 ? attribute : attribute.Substring(0, equals)).Trim();
                    var value = equals < 0 ? "" : attribute.Substring(equals + 1).Trim();

                    if (name.Equals("path", StringComparison.OrdinalIgnoreCase)) cookie["path"] = value;
                    else if (name.Equals("domain", StringComparison.OrdinalIgnoreCase)) cookie["domain"] = value;
                    else if (name.Equals("expires", StringComparison.OrdinalIgnoreCase)) cookie["expires"] = value;
                    else if (name.Equals("httpOnly", StringComparison.OrdinalIgnoreCase)) cookie["httpOnly"] = true;
                    else if (name.Equals("secure", StringComparison.OrdinalIgnoreCase)) cookie["secure"] = true;
                }
                result.Add(cookie);
            }
            return result;
        }

        private static JObject CookiePair(string source)
        {
            var value = (source ?? "").Trim();
            if (value.Length == 0) return null;
            var equals = value.IndexOf('=');
            var name = (equals < 0 ? value : value.Substring(0, equals)).Trim();
            if (name.Length == 0) return null;

            return new JObject(
                new JProperty("name", name),
                new JProperty("value", equals < 0 ? "" : value.Substring(equals + 1).Trim()));
        }

        private static byte[] DecodeBody(byte[] data, string responseHeaders)
        {
            if (data == null || data.Length == 0) return new byte[0];
            var encoding = (HeaderValue(responseHeaders, "content-encoding") ?? "").ToLowerInvariant();
            if (encoding.Length == 0) return data;

            try
            {
                if (encoding.Contains("br"))
                    return DecodeBrotli(data) ?? data;

                if (encoding.Contains("zstd"))
                    return DecodeZstd(data) ?? data;

                if (encoding.Contains("gzip") || encoding.Contains("deflate"))
                {
                    using (var source = new MemoryStream(data))
                    using (var decoder = encoding.Contains("gzip")
                        ? (Stream)new GZipStream(source, CompressionMode.Decompress)
                        : new DeflateStream(source, CompressionMode.Decompress))
                    using (var destination = new MemoryStream())
                    {
                        decoder.CopyTo(destination);
                        return destination.ToArray();
                    }
                }
            }
            catch
            {
                // Some Zenno builds already return a decompressed ResponseBody while
                // retaining the original Content-Encoding header.
                return data;
            }

            return data;
        }

        private static byte[] DecodeBrotli(byte[] data)
        {
            var type = FindOptionalType("Brotli.BrotliExtensions", "Brotli.Core");
            if (type == null) return null;
            var method = type.GetMethod("DecompressFromBrotli", new[] { typeof(byte[]) });
            return method == null ? null : (byte[])method.Invoke(null, new object[] { data });
        }

        private static byte[] DecodeZstd(byte[] data)
        {
            var type = FindOptionalType("ZstdNet.Decompressor", "ZstdNet");
            if (type == null) return null;

            var decoder = Activator.CreateInstance(type);
            try
            {
                var method = type.GetMethod("Unwrap", new[] { typeof(byte[]), typeof(int) });
                return method == null
                    ? null
                    : (byte[])method.Invoke(decoder, new object[] { data, int.MaxValue });
            }
            finally
            {
                var disposable = decoder as IDisposable;
                if (disposable != null) disposable.Dispose();
            }
        }

        private static Type FindOptionalType(string typeName, string assemblyName)
        {
            var type = Type.GetType(typeName + ", " + assemblyName, false);
            if (type != null) return type;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName, false);
                if (type != null) return type;
            }

            try { return Assembly.Load(assemblyName).GetType(typeName, false); }
            catch { return null; }
        }

        private static bool IsText(string mime)
        {
            var value = (mime ?? "").ToLowerInvariant();
            return value.StartsWith("text/")
                   || value.Contains("json")
                   || value.Contains("xml")
                   || value.Contains("javascript")
                   || value.Contains("ecmascript")
                   || value.Contains("x-www-form-urlencoded")
                   || value.Contains("graphql");
        }

        private static string DecodeText(byte[] data, string mime, string responseCharset)
        {
            var charset = (responseCharset ?? "").Trim().Trim('"', '\'');
            if (charset.Length == 0)
            {
                var contentType = mime ?? "";
                var marker = contentType.IndexOf("charset=", StringComparison.OrdinalIgnoreCase);
                if (marker >= 0)
                {
                    charset = contentType.Substring(marker + "charset=".Length).Trim();
                    var semicolon = charset.IndexOf(';');
                    if (semicolon >= 0) charset = charset.Substring(0, semicolon);
                    charset = charset.Trim().Trim('"', '\'');
                }
            }

            try
            {
                return (charset.Length == 0 ? Encoding.UTF8 : Encoding.GetEncoding(charset)).GetString(data);
            }
            catch
            {
                return Encoding.UTF8.GetString(data);
            }
        }

        private static string ResourceType(string mime)
        {
            var value = (mime ?? "").ToLowerInvariant();
            if (value.Contains("html")) return "document";
            if (value.Contains("css")) return "stylesheet";
            if (value.Contains("javascript") || value.Contains("ecmascript")) return "script";
            if (value.Contains("json") || value.Contains("graphql")) return "xhr";
            if (value.StartsWith("image/")) return "image";
            if (value.StartsWith("font/") || value.Contains("woff")) return "font";
            if (value.StartsWith("video/") || value.StartsWith("audio/")) return "media";
            return "other";
        }

        private static string HttpVersion(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return "HTTP/1.1";
            foreach (var token in raw.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                if (token.StartsWith("HTTP/", StringComparison.OrdinalIgnoreCase))
                    return token.Trim().ToUpperInvariant();
            return "HTTP/1.1";
        }

        private static int ByteCountOrMinusOne(string value)
        {
            return string.IsNullOrEmpty(value) ? -1 : Encoding.UTF8.GetByteCount(value);
        }

        private static string UrlDecode(string value)
        {
            try { return Uri.UnescapeDataString((value ?? "").Replace("+", " ")); }
            catch { return value ?? ""; }
        }

        private static string StatusText(int status)
        {
            if (status <= 0 || !Enum.IsDefined(typeof(System.Net.HttpStatusCode), status)) return "";
            var source = ((System.Net.HttpStatusCode)status).ToString();
            var result = new StringBuilder(source.Length + 4);
            for (var i = 0; i < source.Length; i++)
            {
                if (i > 0 && char.IsUpper(source[i]) && char.IsLower(source[i - 1])) result.Append(' ');
                result.Append(source[i]);
            }
            return result.ToString();
        }
    }
}
