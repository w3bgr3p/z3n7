using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace z3n7.Api
{
    public sealed class OmniRoute
    {
        private const string BaseUrl = "http://localhost:20128";
        private const string CompletionsUrl = BaseUrl + "/v1/chat/completions";
        private const string ModelsUrl = BaseUrl + "/v1/models";

        private static List<string> _modelsCache;

        public OmniRoute()
        {
        }

        public string Complete(
            string model,
            string systemPrompt,
            string userPrompt,
            double temperature = 0.3,
            int maxTokens = 800,
            int timeoutSec = 90)
        {
            return CompleteAsync(model, systemPrompt, userPrompt, temperature, maxTokens, timeoutSec)
                .GetAwaiter()
                .GetResult();
        }

        public async Task<string> CompleteAsync(
            string model,
            string systemPrompt,
            string userPrompt,
            double temperature = 0.3,
            int maxTokens = 800,
            int timeoutSec = 90)
        {
            EnsureModel(model);

            var body = JsonConvert.SerializeObject(new
            {
                model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt ?? "" },
                    new { role = "user", content = userPrompt ?? "" }
                },
                temperature,
                top_p = 0.9,
                stream = false,
                max_tokens = maxTokens
            });

            using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(timeoutSec) })
            using (var request = new HttpRequestMessage(HttpMethod.Post, CompletionsUrl))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                using (var response = await http.SendAsync(request))
                {
                    var raw = await response.Content.ReadAsStringAsync();
                    EnsureSuccess(response, raw);
                    return ReadCompletion(raw);
                }
            }
        }

        public string CompleteVision(
            string model,
            string systemPrompt,
            string userPrompt,
            IList<string> imagesBase64,
            double temperature = 0.1,
            int maxTokens = 800,
            int timeoutSec = 90)
        {
            return CompleteVisionAsync(
                    model,
                    systemPrompt,
                    userPrompt,
                    imagesBase64,
                    temperature,
                    maxTokens,
                    timeoutSec)
                .GetAwaiter()
                .GetResult();
        }

        public async Task<string> CompleteVisionAsync(
            string model,
            string systemPrompt,
            string userPrompt,
            IList<string> imagesBase64,
            double temperature = 0.1,
            int maxTokens = 800,
            int timeoutSec = 90)
        {
            EnsureModel(model);
            EnsureImages(imagesBase64);

            var canvasSize = ReadImageSize(imagesBase64[0]);
            var sizedPrompt = (userPrompt ?? "") +
                              $"\n\nThe first image is exactly {canvasSize.Width}x{canvasSize.Height} pixels. " +
                              "Use this original pixel coordinate system for all points and canvas dimensions.";
            var content = BuildVisionContent(imagesBase64, sizedPrompt);
            var body = BuildVisionBody(model, systemPrompt, content, temperature, maxTokens);

            using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(timeoutSec) })
            using (var request = new HttpRequestMessage(HttpMethod.Post, CompletionsUrl))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                using (var response = await http.SendAsync(request))
                {
                    var raw = await response.Content.ReadAsStringAsync();
                    EnsureSuccess(response, raw);
                    var result = StripOuterCodeFence(ReadCompletion(raw));
                    return SetCanvasSize(result, canvasSize);
                }
            }
        }

        public List<string> GetModels()
        {
            return GetModelsAsync().GetAwaiter().GetResult();
        }

        public async Task<List<string>> GetModelsAsync()
        {
            if (_modelsCache != null)
                return new List<string>(_modelsCache);

            using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) })
            using (var response = await http.GetAsync(ModelsUrl))
            {
                var raw = await response.Content.ReadAsStringAsync();
                EnsureSuccess(response, raw);

                try
                {
                    var json = JObject.Parse(raw);
                    _modelsCache = json["data"]
                        .Children<JObject>()
                        .Select(item => item["id"]?.ToString())
                        .Where(id => !string.IsNullOrWhiteSpace(id))
                        .OrderBy(id => id, StringComparer.Ordinal)
                        .ToList();
                    return new List<string>(_modelsCache);
                }
                catch (Exception ex)
                {
                    throw InvalidResponse(ex, raw);
                }
            }
        }

        public bool Check()
        {
            return CheckAsync().GetAwaiter().GetResult();
        }

        public async Task<bool> CheckAsync()
        {
            try
            {
                using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(3) })
                using (var response = await http.GetAsync(ModelsUrl))
                    return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public static void InvalidateModelsCache()
        {
            _modelsCache = null;
        }

        private static JArray BuildVisionContent(
            IList<string> imagesBase64,
            string userPrompt)
        {
            var content = new JArray
            {
                new JObject
                {
                    ["type"] = "text",
                    ["text"] = userPrompt ?? ""
                }
            };

            foreach (var image in imagesBase64)
            {
                ParseImageBase64(image, out var mediaType, out var base64Data);
                content.Add(new JObject
                {
                    ["type"] = "image_url",
                    ["image_url"] = new JObject
                    {
                        ["url"] = "data:" + mediaType + ";base64," + base64Data,
                        ["detail"] = "high"
                    }
                });
            }

            return content;
        }

        private static string BuildVisionBody(
            string model,
            string systemPrompt,
            JArray content,
            double temperature,
            int maxTokens)
        {
            var body = new JObject
            {
                ["model"] = model,
                ["messages"] = new JArray
                {
                    new JObject
                    {
                        ["role"] = "system",
                        ["content"] = systemPrompt ?? ""
                    },
                    new JObject
                    {
                        ["role"] = "user",
                        ["content"] = content
                    }
                },
                ["temperature"] = temperature,
                ["top_p"] = 0.9,
                ["max_tokens"] = maxTokens,
                ["stream"] = false
            };
            return JsonConvert.SerializeObject(body);
        }

        private static void EnsureModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Model is empty", nameof(model));
        }

        private static void EnsureImages(IList<string> imagesBase64)
        {
            if (imagesBase64 == null)
                throw new ArgumentNullException(nameof(imagesBase64));
            if (imagesBase64.Count == 0)
                throw new ArgumentException("No images provided", nameof(imagesBase64));
        }

        private static void ParseImageBase64(
            string value,
            out string mediaType,
            out string base64Data)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Image base64 string is empty", nameof(value));

            value = value.Trim();
            if (value.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                var commaIndex = value.IndexOf(',');
                if (commaIndex < 0)
                    throw new ArgumentException("Invalid image data URL", nameof(value));

                var header = value.Substring(5, commaIndex - 5);
                var semicolonIndex = header.IndexOf(';');
                mediaType = semicolonIndex >= 0
                    ? header.Substring(0, semicolonIndex)
                    : header;
                base64Data = value.Substring(commaIndex + 1);
            }
            else
            {
                base64Data = value;
                mediaType = DetectImageMediaType(base64Data);
            }

            if (string.IsNullOrWhiteSpace(base64Data))
                throw new ArgumentException("Image base64 data is empty", nameof(value));

            if (mediaType != "image/png" &&
                mediaType != "image/jpeg" &&
                mediaType != "image/webp" &&
                mediaType != "image/gif")
                throw new ArgumentException("Unsupported image media type: " + mediaType, nameof(value));
        }

        private static string DetectImageMediaType(string base64Data)
        {
            if (base64Data.StartsWith("iVBOR", StringComparison.Ordinal)) return "image/png";
            if (base64Data.StartsWith("/9j/", StringComparison.Ordinal)) return "image/jpeg";
            if (base64Data.StartsWith("UklGR", StringComparison.Ordinal)) return "image/webp";
            if (base64Data.StartsWith("R0lGOD", StringComparison.Ordinal)) return "image/gif";

            throw new ArgumentException(
                "Unknown base64 image format. Supported: PNG, JPEG, WebP, GIF",
                nameof(base64Data));
        }

        private static Size ReadImageSize(string imageBase64)
        {
            ParseImageBase64(imageBase64, out _, out var base64Data);
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(base64Data);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Invalid image base64 data", nameof(imageBase64), ex);
            }

            using (var stream = new MemoryStream(bytes, false))
            using (var image = Image.FromStream(stream, false, true))
                return new Size(image.Width, image.Height);
        }

        private static void EnsureSuccess(HttpResponseMessage response, string raw)
        {
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"OmniRoute HTTP {(int)response.StatusCode}\n{raw}");
        }

        private static string ReadCompletion(string raw)
        {
            try
            {
                var json = JObject.Parse(raw);
                var content = json["choices"]?[0]?["message"]?["content"];
                if (content == null)
                    throw new JsonException("choices[0].message.content is missing");
                return content.ToString();
            }
            catch (Exception ex)
            {
                throw InvalidResponse(ex, raw);
            }
        }

        private static string StripOuterCodeFence(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var trimmed = value.Trim();
            if (!trimmed.StartsWith("```", StringComparison.Ordinal) ||
                !trimmed.EndsWith("```", StringComparison.Ordinal))
                return value;

            var firstLineEnd = trimmed.IndexOf('\n');
            if (firstLineEnd < 0)
                return value;

            return trimmed.Substring(firstLineEnd + 1, trimmed.Length - firstLineEnd - 4).Trim();
        }

        private static string SetCanvasSize(string value, Size canvasSize)
        {
            try
            {
                var json = JObject.Parse(value);
                if (json["canvas_width"] == null || json["canvas_height"] == null)
                    return value;

                json["canvas_width"] = canvasSize.Width;
                json["canvas_height"] = canvasSize.Height;
                return JsonConvert.SerializeObject(json);
            }
            catch (JsonException)
            {
                return value;
            }
        }

        private static Exception InvalidResponse(Exception exception, string raw)
        {
            return new InvalidOperationException(
                $"Invalid OmniRoute response: {exception.Message}\nRAW:\n{raw}", exception);
        }
    }
}
