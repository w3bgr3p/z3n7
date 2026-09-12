using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Captcha
{
    public sealed class HuntTest : IDisposable
    {
        private const string VerifyUrl = "/captcha-api/api/v4/captcha/verify";
        private readonly IZennoPosterProjectModel _project;
        private readonly Instance _instance;
        private readonly HttpClient _http;

        public HuntTest(
            IZennoPosterProjectModel project,
            Instance instance)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            var apiUrl = project.ReadEnv("HUNT_SOLVER_API_URL");
            var apiKey = project.ReadEnv("HUNT_SOLVER_API_KEY");
            if (string.IsNullOrWhiteSpace(apiUrl))
                throw new ArgumentException("API URL is required", nameof(apiUrl));
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API key is required", nameof(apiKey));

            _instance.UseTrafficMonitoring = true;
            _http = new HttpClient
            {
                BaseAddress = new Uri(apiUrl.TrimEnd('/') + "/"),
                Timeout = TimeSpan.FromSeconds(30)
            };
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public bool Solve(
            string type,
            int attempts = 5,
            float confidence = 0.30f,
            int minimumDelayMs = 700,
            int maximumDelayMs = 1300)
        {
            if (type != "shapes" && type != "football")
                throw new ArgumentOutOfRangeException(nameof(type), type,
                    "unknown type: " + type + ". [shapes | football] is available");
            if (confidence < 0 || confidence > 1)
                throw new ArgumentOutOfRangeException(nameof(confidence));
            if (minimumDelayMs < 0 || maximumDelayMs < minimumDelayMs)
                throw new ArgumentException("Invalid click delay range");

            for (var attempt = 0; attempt < attempts; attempt++)
            {
                var responseCount = VerificationBodies().Count;
                try
                {
                    if (type == "shapes")
                        SolveShapes(confidence, minimumDelayMs, maximumDelayMs);
                    else
                        SolveFootball(confidence);

                    Thread.Sleep(5000);
                    var responses = VerificationBodies().Skip(responseCount).ToList();
                    if (VerificationPassed(responses))
                    {
                        _project.SendInfoToLog("Hunt " + type + " | solved", true);
                        return true;
                    }

                    _project.SendInfoToLog(
                        responses.Count == 0
                            ? "Hunt " + type + " attempt " + (attempt + 1) + " | no verification response"
                            : "Hunt " + type + " attempt " + (attempt + 1) + " | verification failed");
                }
                catch (Exception error)
                {
                    _project.SendErrorToLog(
                        "Hunt " + type + " attempt failed:" + Environment.NewLine + error);
                    if (type == "shapes")
                        ClickMainButton();
                }
            }

            _project.SendErrorToLog("Hunt " + type + " | failed", true);
            return false;
        }

        private static bool VerificationPassed(IList<string> responses)
        {
            return responses != null && responses.Count > 0 &&
                   !responses.Any(body => body.Contains("Verification failed"));
        }

        private List<Detection> Detect(string type, string imageBase64, float confidence)
        {
            var comma = imageBase64.IndexOf(',');
            var raw = imageBase64.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && comma >= 0
                ? imageBase64.Substring(comma + 1)
                : imageBase64;
            using (var content = new ByteArrayContent(Convert.FromBase64String(raw)))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                var url = "v1/detect/" + type + "?confidence=" +
                          confidence.ToString(CultureInfo.InvariantCulture);
                using (var response = _http.PostAsync(url, content).GetAwaiter().GetResult())
                {
                    var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException(
                            "Hunt API returned " + (int)response.StatusCode + ": " + json);

                    return ((JArray)JObject.Parse(json)["detections"])
                        .Select(item => new Detection(item))
                        .ToList();
                }
            }
        }

        private void SolveShapes(float confidence, int minimumDelayMs, int maximumDelayMs)
        {
            var canvas = WaitForCanvas(true);

            string imageBase64 = null;
            List<Detection> detections = null;
            var deadline = DateTime.UtcNow.AddSeconds(10);
            do
            {
                imageBase64 = _instance.ActiveTab.MainDocument.EvaluateScript(@"
                    var canvas = document.querySelector('#huntCaptcha canvas');
                    return canvas ? canvas.toDataURL('image/png') : null;
                ");
                if (!string.IsNullOrWhiteSpace(imageBase64))
                {
                    detections = Detect("shapes", imageBase64, confidence);
                    if (detections.Any(item => item.ClassId % 2 == 0))
                        break;
                }
                Thread.Sleep(250);
            }
            while (DateTime.UtcNow < deadline);

            var prompts = (detections ?? new List<Detection>())
                .Where(item => item.ClassId % 2 == 0)
                .GroupBy(item => item.ClassId)
                .Select(group => group.OrderByDescending(item => item.Confidence).First())
                .OrderBy(item => item.CenterX)
                .ToList();
            if (prompts.Count == 0)
                throw new InvalidOperationException("Hunt prompt icons were not detected");

            List<Detection> fallback = null;
            var targets = new List<Detection>();
            foreach (var prompt in prompts)
            {
                var target = detections.Where(item => item.ClassId == prompt.ClassId + 1)
                    .OrderByDescending(item => item.Confidence).FirstOrDefault();
                if (target == null)
                {
                    fallback = fallback ?? Detect(
                        "shapes", imageBase64, Math.Min(confidence, 0.20f));
                    target = fallback.Where(item => item.ClassId == prompt.ClassId + 1)
                        .OrderByDescending(item => item.Confidence).FirstOrDefault();
                }
                if (target == null)
                    throw new InvalidOperationException(
                        "Hunt shape target for class " + (prompt.ClassId + 1) + " was not detected");
                targets.Add(target);
            }

            int imageWidth;
            int imageHeight;
            int displayWidth;
            int displayHeight;
            using (var image = ReadBitmap(imageBase64))
            using (var displayed = ReadBitmap(canvas.DrawToBitmap(true)))
            {
                imageWidth = image.Width;
                imageHeight = image.Height;
                displayWidth = displayed.Width;
                displayHeight = displayed.Height;
            }

            var position = canvas.DisplacementInBrowser;
            var scaleX = (double)displayWidth / imageWidth;
            var scaleY = (double)displayHeight / imageHeight;
            var random = new Random();
            foreach (var target in targets)
            {
                Thread.Sleep(random.Next(minimumDelayMs, maximumDelayMs + 1));
                _instance.ActiveTab.RiseEvent("click", new Rectangle(
                    position.X + (int)Math.Round(target.CenterX * scaleX),
                    position.Y + (int)Math.Round(target.CenterY * scaleY), 1, 1), "Left");
            }

            Thread.Sleep(2000);
            _instance.ActiveTab.RiseEvent("click", new Rectangle(
                position.X + displayWidth / 2,
                position.Y + (int)Math.Round(displayHeight * 0.94), 1, 1), "Left");
        }

        private void SolveFootball(float confidence)
        {
            var canvas = WaitForCanvas(false);

            var width = canvas.BoundingClientWidth;
            var height = canvas.BoundingClientHeight;
            var position = canvas.DisplacementInTabWindow;
            var sliderY = FindSliderY(canvas, position.Y, height);
            var sliderX = position.X + width / 2;
            var minimumX = position.X + 2;
            var maximumX = position.X + width - 2;
            var mouseDown = false;

            try
            {
                _instance.ActiveTab.MouseClick(sliderX, sliderY, "left", "down");
                mouseDown = true;
                var result = DetectFootball(canvas, confidence);
                var lastCursorDelta = 0;
                var lastBallDeltaX = 0f;
                var lastBallDeltaY = 0f;

                for (var attempt = 0; attempt < 6; attempt++)
                {
                    var ball = result[0];
                    var circle = result[1];
                    if (ball.CenterX >= circle.X1 && ball.CenterX <= circle.X2 &&
                        ball.CenterY >= circle.Y1 && ball.CenterY <= circle.Y2)
                    {
                        _instance.ActiveTab.MouseClick(sliderX, sliderY, "left", "up");
                        mouseDown = false;
                        return;
                    }

                    var previousX = sliderX;
                    var previousBallX = ball.CenterX;
                    var previousBallY = ball.CenterY;
                    int movement;
                    if (attempt == 0)
                    {
                        movement = sliderX + 12 <= maximumX ? 12 : -12;
                    }
                    else
                    {
                        var response = lastBallDeltaX * lastBallDeltaX +
                                       lastBallDeltaY * lastBallDeltaY;
                        if (Math.Abs(lastCursorDelta) < 1 || response < 1f)
                        {
                            movement = attempt % 2 == 0 ? 12 : -12;
                        }
                        else
                        {
                            var calculated = lastCursorDelta *
                                ((circle.CenterX - ball.CenterX) * lastBallDeltaX +
                                 (circle.CenterY - ball.CenterY) * lastBallDeltaY) / response;
                            movement = (int)Math.Round(Math.Max(-80f, Math.Min(80f, calculated)));
                            if (Math.Abs(movement) < 4)
                                movement = movement < 0 ? -4 : 4;
                        }
                    }

                    var nextX = Math.Max(minimumX, Math.Min(maximumX, sliderX + movement));
                    _instance.ActiveTab.MouseMove(sliderX, sliderY, nextX, sliderY, false, false);
                    sliderX = nextX;
                    Thread.Sleep(80);
                    result = DetectFootball(canvas, confidence);
                    lastCursorDelta = sliderX - previousX;
                    lastBallDeltaX = result[0].CenterX - previousBallX;
                    lastBallDeltaY = result[0].CenterY - previousBallY;
                }

                throw new InvalidOperationException("Hunt football target was not reached");
            }
            finally
            {
                if (mouseDown)
                    _instance.ActiveTab.MouseClick(sliderX, sliderY, "left", "up");
            }
        }

        private HtmlElement WaitForCanvas(bool insideHunt)
        {
            var deadline = DateTime.UtcNow.AddSeconds(30);
            do
            {
                if (insideHunt)
                {
                    var root = _instance.ActiveTab.FindElementById("huntCaptcha");
                    if (root != null && !root.IsVoid)
                    {
                        var nested = root.GetChildren(true).FirstOrDefault(item =>
                            string.Equals(item.TagName, "canvas", StringComparison.OrdinalIgnoreCase));
                        if (nested != null && !nested.IsVoid)
                            return nested;
                    }
                }
                else
                {
                    var canvas = _instance.ActiveTab.FindElementByAttribute(
                        "canvas", "fulltag", "canvas", "text", 0);
                    if (canvas != null && !canvas.IsVoid)
                        return canvas;
                }
                Thread.Sleep(250);
            }
            while (DateTime.UtcNow < deadline);

            throw new InvalidOperationException("Hunt canvas was not found");
        }

        private int FindSliderY(HtmlElement canvas, int canvasY, int canvasHeight)
        {
            var slider = _instance.ActiveTab.MainDocument.EvaluateScript(@"
                (function() {
                    var canvas = document.querySelector('canvas');
                    var control = document.querySelector('input[type=range], [role=slider]');
                    if (!canvas || !control) return null;
                    var c = canvas.getBoundingClientRect();
                    var r = control.getBoundingClientRect();
                    return JSON.stringify({ top: r.top - c.top, height: r.height });
                })();
            ");
            if (!string.IsNullOrWhiteSpace(slider))
            {
                var box = JObject.Parse(slider);
                return canvasY + (int)Math.Round((double)box["top"] + (double)box["height"] / 2.0);
            }

            return canvasY + (int)Math.Round(canvasHeight * 284.0 / 300.0);
        }

        private Detection[] DetectFootball(HtmlElement canvas, float confidence)
        {
            var deadline = DateTime.UtcNow.AddSeconds(10);
            do
            {
                var image = canvas.DrawToBitmap(true);
                var detections = Detect("football", image, confidence);
                var ball = detections.Where(item => item.ClassId == 0)
                    .OrderByDescending(item => item.Confidence).FirstOrDefault();
                var circle = detections.Where(item => item.ClassId == 1)
                    .OrderByDescending(item => item.Confidence).FirstOrDefault();
                if (ball == null || circle == null)
                {
                    detections = Detect("football", image, 0.10f);
                    ball = detections.Where(item => item.ClassId == 0)
                        .OrderByDescending(item => item.Confidence).FirstOrDefault();
                    circle = detections.Where(item => item.ClassId == 1)
                        .OrderByDescending(item => item.Confidence).FirstOrDefault();
                }
                if (ball != null && circle != null)
                    return new[] { ball, circle };
                Thread.Sleep(250);
            }
            while (DateTime.UtcNow < deadline);

            throw new InvalidOperationException("Hunt football ball or circle was not detected");
        }

        private void ClickMainButton()
        {
            var canvas = _instance.ActiveTab.FindElementByAttribute(
                "canvas", "fulltag", "canvas", "text", 0);
            if (canvas == null || canvas.IsVoid)
                return;
            var position = canvas.DisplacementInBrowser;
            using (var image = ReadBitmap(canvas.DrawToBitmap(true)))
            {
                _instance.ActiveTab.RiseEvent("click", new Rectangle(
                    position.X + image.Width / 2,
                    position.Y + image.Height - 10, 1, 1), "Left");
            }
        }

        private static Bitmap ReadBitmap(string imageBase64)
        {
            var comma = imageBase64.IndexOf(',');
            var raw = imageBase64.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && comma >= 0
                ? imageBase64.Substring(comma + 1)
                : imageBase64;
            using (var stream = new MemoryStream(Convert.FromBase64String(raw)))
            using (var image = new Bitmap(stream))
                return new Bitmap(image);
        }

        private List<string> VerificationBodies()
        {
            var bodies = new List<string>();
            foreach (var item in _instance.ActiveTab.GetTraffic(new[] { VerifyUrl }))
            {
                var url = (item.Url ?? "").ToString();
                if (!url.Contains(VerifyUrl) || (item.Method ?? "").ToString() == "OPTIONS")
                    continue;
                var bytes = item.ResponseBody as byte[] ?? new byte[0];
                bodies.Add(ReadResponse(bytes, (item.ResponseHeaders ?? "").ToString()));
            }
            return bodies;
        }

        private static string ReadResponse(byte[] bytes, string headers)
        {
            if (bytes.Length == 0)
                return "";
            var lowerHeaders = headers.ToLowerInvariant();
            if (!lowerHeaders.Contains("content-encoding: gzip") &&
                !lowerHeaders.Contains("content-encoding: deflate"))
                return Encoding.UTF8.GetString(bytes);

            try
            {
                using (var input = new MemoryStream(bytes))
                using (var output = new MemoryStream())
                {
                    Stream decoder = lowerHeaders.Contains("content-encoding: gzip")
                        ? (Stream)new GZipStream(input, CompressionMode.Decompress)
                        : new DeflateStream(input, CompressionMode.Decompress);
                    using (decoder)
                    {
                        decoder.CopyTo(output);
                        return Encoding.UTF8.GetString(output.ToArray());
                    }
                }
            }
            catch
            {
                return Encoding.UTF8.GetString(bytes);
            }
        }

        public void Dispose()
        {
            _http.Dispose();
        }

        private sealed class Detection
        {
            public readonly int ClassId;
            public readonly float Confidence;
            public readonly float X1;
            public readonly float Y1;
            public readonly float X2;
            public readonly float Y2;

            public Detection(JToken item)
            {
                ClassId = (int)item["class_id"];
                Confidence = (float)item["confidence"];
                X1 = (float)item["x1"];
                Y1 = (float)item["y1"];
                X2 = (float)item["x2"];
                Y2 = (float)item["y2"];
            }

            public float CenterX => (X1 + X2) / 2f;
            public float CenterY => (Y1 + Y2) / 2f;
        }
    }

    public static partial class CaptchaExtensions
    {
        public static bool SolveHuntTest(
            this Instance instance,
            IZennoPosterProjectModel project,
            string type,
            int attempts = 5)
        {
            using (var solver = new HuntTest(project, instance))
                return solver.Solve(type, attempts);
        }
    }
}


