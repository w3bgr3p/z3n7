using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace z3nIO.Api
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

        private static void EnsureModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Model is empty", nameof(model));
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

        private static Exception InvalidResponse(Exception exception, string raw)
        {
            return new InvalidOperationException(
                $"Invalid OmniRoute response: {exception.Message}\nRAW:\n{raw}", exception);
        }
    }
}
