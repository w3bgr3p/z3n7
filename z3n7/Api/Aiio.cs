using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Api
{
    public sealed class Aiio
    {
        private const string CompletionsUrl =
            "https://api.intelligence.io.solutions/api/v1/chat/completions";
        private const string ModelsUrl =
            "https://api.intelligence.io.solutions/api/v1/models?page=1&page_size=200";

        private readonly IZennoPosterProjectModel _project;
        private static List<string> _modelsCache;

        public Aiio(IZennoPosterProjectModel project)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
        }

        public string Complete(
            string model,
            string systemPrompt,
            string userPrompt,
            double temperature = 0.8,
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
            double temperature = 0.8,
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
                request.Headers.Add("Authorization", $"Bearer {GetKey()}");
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
            using (var request = new HttpRequestMessage(HttpMethod.Get, ModelsUrl))
            {
                request.Headers.Add("Authorization", $"Bearer {GetKey()}");

                using (var response = await http.SendAsync(request))
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
        }

        public bool HasKey()
        {
            return GetKeyOrNull() != null;
        }

        public static void InvalidateModelsCache()
        {
            _modelsCache = null;
        }

        private string GetKey()
        {
            var key = GetKeyOrNull();
            if (key == null)
                throw new InvalidOperationException("No valid aiio key in __aiio table");
            return key;
        }

        private string GetKeyOrNull()
        {
            var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var db = new Db(_project, defaultTable: "__aiio");
            var keys = db.GetLines(
                    "api",
                    tableName: "__aiio",
                    where: $"(\"expire\" = '' OR \"expire\" IS NULL OR \"expire\" > '{now}')")
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .ToList();

            return keys.Count == 0 ? null : keys[new Random().Next(keys.Count)];
        }

        private static void EnsureModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Model is empty", nameof(model));
        }

        private static void EnsureSuccess(HttpResponseMessage response, string raw)
        {
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"AI.IO HTTP {(int)response.StatusCode}\n{raw}");
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
                $"Invalid AI.IO response: {exception.Message}\nRAW:\n{raw}", exception);
        }
    }
}
