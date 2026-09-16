using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Captcha
{
    public sealed class z3nCap : IDisposable
    {
        private readonly HttpClient _http;

        public z3nCap(IZennoPosterProjectModel project, string apiUrl = null, string apiKey = null)
        {
            var url = apiUrl ?? project?.ReadEnv("Z3NCAP_API_URL");


            var key = apiKey ?? project?.ReadEnv("Z3NCAP_API_KEY");
                      

            _http = new HttpClient { BaseAddress = new Uri(url.TrimEnd('/') + "/"), Timeout = TimeSpan.FromSeconds(30) };
            if (!string.IsNullOrWhiteSpace(key))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key.Trim());
        }

        public z3nCap(string apiUrl, string apiKey = null) : this(null, apiUrl, apiKey) { }

        public JObject Solve(string type, object image)
        {
            var bytes = ToBytes(image);
            var content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var resp = _http.PostAsync("v1/solve/" + type.ToLowerInvariant(), content).GetAwaiter().GetResult();
            var body = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException(string.Format("z3nCap error ({0}): {1}", (int)resp.StatusCode, body));

            return JObject.Parse(body);
        }

        public string SolveText(string type, object image)
        {
            var json = Solve(type, image);
            var text = json["text"]?.ToString();
            if (string.IsNullOrEmpty(text))
                throw new InvalidOperationException(string.Format("No text returned for {0}: {1}", type, json));
            return text;
        }

        public JArray Detect(string type, object image, float confidence = 0.25f)
        {
            var json = Solve(type, image);
            return json["detections"] as JArray ?? new JArray();
        }

        public static byte[] ToBytes(object image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (image is byte[] b) return b;
            if (image is HtmlElement el) return ToBytes(el.DrawToBitmap(true));
            if (image is Bitmap bmp)
            {
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
            if (image is string s)
            {
                s = s.Trim();
                if (File.Exists(s)) return File.ReadAllBytes(s);
                var comma = s.IndexOf(',');
                return Convert.FromBase64String(comma >= 0 ? s.Substring(comma + 1) : s);
            }
            throw new ArgumentException(string.Format("Unsupported image type: {0}", image.GetType()));
        }

        public void Dispose()
        {
            _http.Dispose();
        }
    }

    public static partial class CaptchaExtensions
    {
        public static string SolveZ3nCapText(this Instance instance, IZennoPosterProjectModel project, string type, HtmlElement el)
        {
            using (var cap = new z3nCap(project))
                return cap.SolveText(type, el);
        }

        public static string SolveZ3nCapText(this IZennoPosterProjectModel project, string type, object image)
        {
            using (var cap = new z3nCap(project))
                return cap.SolveText(type, image);
        }

        public static JObject SolveZ3nCap(this IZennoPosterProjectModel project, string type, object image)
        {
            using (var cap = new z3nCap(project))
                return cap.Solve(type, image);
        }
    }
}
