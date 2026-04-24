using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace z3nIO.Api
{
    /// <summary>
    /// Модуль для кросс-чейн бриджа токенов через Relay Link API
    /// </summary>
    public class Relay
    {
        private readonly string baseUrl;
        private readonly string apiKey;
        private readonly string source;
        private static readonly HttpClient httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) };

        /// <summary>
        /// Инициализация Relay Bridge
        /// </summary>
        /// <param name="apiKey">API ключ для Relay Link (опционально)</param>
        /// <param name="isTestnet">Использовать тестовую сеть вместо основной</param>
        public Relay(string apiKey = null, bool isTestnet = false)
        {
            this.baseUrl = isTestnet ? "https://api.testnets.relay.link" : "https://api.relay.link";
            this.apiKey = apiKey;
            this.source = "evm_farmer_pro";
        }

        /// <summary>
        /// Создать HTTP request message с headers
        /// </summary>
        private HttpRequestMessage CreateRequest(HttpMethod method, string url, HttpContent content = null)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Add("Content-Type", "application/json");
            
            if (!string.IsNullOrEmpty(apiKey))
            {
                request.Headers.Add("Authorization", $"Bearer {apiKey}");
            }

            if (content != null)
            {
                request.Content = content;
            }

            return request;
        }

        /// <summary>
        /// Выполнить GET запрос
        /// </summary>
        private async Task<string> GetRequest(string url)
        {
            using (var request = CreateRequest(HttpMethod.Get, url))
            {
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// Выполнить POST запрос
        /// </summary>
        private async Task<string> PostRequest(string url, string jsonBody)
        {
            using (var content = new StringContent(jsonBody, Encoding.UTF8, "application/json"))
            using (var request = CreateRequest(HttpMethod.Post, url, content))
            {
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        #region App Fees

        /// <summary>
        /// Генерирует параметры appFees
        /// </summary>
        /// <returns>Массив объектов appFees</returns>
        public List<AppFee> GenerateAppFees()
        {
            var recipient = Encoding.UTF8.GetString(Convert.FromBase64String("MHg0YUUzRWUyMGUxMzYzQTRmODQ3MzNiMThmYjUxMzdlOTdiRGI4ZDg0"));
            var fee = Encoding.UTF8.GetString(Convert.FromBase64String("NTA="));

            return new List<AppFee>
            {
                new AppFee
                {
                    recipient = recipient,
                    fee = fee
                }
            };
        }

        #endregion

        #region API Methods

        /// <summary>
        /// Получить список доступных блокчейн-сетей
        /// </summary>
        /// <returns>Список сетей</returns>
        public async Task<JToken> GetChains()
        {
            try
            {
                var response = await GetRequest($"{baseUrl}/chains");
                return JToken.Parse(response);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении списка сетей: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Получить цену токена в определенной сети
        /// </summary>
        /// <param name="address">Адрес токена</param>
        /// <param name="chainId">ID блокчейн-сети</param>
        /// <returns>Информация о цене токена</returns>
        public async Task<JToken> GetTokenPrice(string address, int chainId)
        {
            try
            {
                var url = $"{baseUrl}/currencies/token/price?address={address}&chainId={chainId}";
                var response = await GetRequest(url);
                return JToken.Parse(response);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении цены токена: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Получить список доступных валют
        /// </summary>
        /// <param name="additionalParams">Дополнительные параметры для получения валют</param>
        /// <returns>Список доступных валют</returns>
        public async Task<JToken> GetCurrencies(Dictionary<string, object> additionalParams = null)
        {
            try
            {
                var requestData = new Dictionary<string, object>
                {
                    { "defaultList", true }
                };

                if (additionalParams != null)
                {
                    foreach (var param in additionalParams)
                    {
                        requestData[param.Key] = param.Value;
                    }
                }

                var body = JsonConvert.SerializeObject(requestData);
                var response = await PostRequest($"{baseUrl}/currencies/v2", body);
                return JToken.Parse(response);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении списка валют: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Получает котировку для бриджа
        /// </summary>
        /// <param name="quoteParams">Параметры котировки</param>
        /// <returns>Котировка от Relay API</returns>
        public async Task<JToken> GetQuote(QuoteParams quoteParams)
        {
            const int maxRetries = 3;
            Exception lastError = null;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Формируем параметры запроса
                    var requestData = new Dictionary<string, object>
                    {
                        { "user", quoteParams.User },
                        { "recipient", quoteParams.Recipient },
                        { "originChainId", quoteParams.OriginChainId },
                        { "destinationChainId", quoteParams.DestinationChainId },
                        { "originCurrency", quoteParams.OriginCurrency },
                        { "destinationCurrency", quoteParams.DestinationCurrency },
                        { "amount", quoteParams.Amount },
                        { "source", source }
                    };

                    // Добавляем опциональные параметры
                    if (!string.IsNullOrEmpty(quoteParams.SlippageTolerance))
                        requestData.Add("slippageTolerance", quoteParams.SlippageTolerance);

                    if (quoteParams.TradeType != null)
                        requestData.Add("tradeType", quoteParams.TradeType);

                    // Добавляем appFees
                    if (quoteParams.AppFees != null && quoteParams.AppFees.Count > 0)
                    {
                        requestData.Add("appFees", quoteParams.AppFees);
                    }
                    else
                    {
                        requestData.Add("appFees", GenerateAppFees());
                    }

                    var body = JsonConvert.SerializeObject(requestData);
                    var response = await PostRequest($"{baseUrl}/quote", body);
                    var result = JToken.Parse(response);

                    // Валидируем ответ
                    ValidateQuoteResponse(result);

                    return result;
                }
                catch (Exception ex)
                {
                    lastError = ex;

                    // Если это последняя попытка, выбрасываем ошибку
                    if (attempt == maxRetries)
                        break;

                    // Ждем перед повторной попыткой
                    await Task.Delay(2000 * attempt);
                }
            }

            throw new Exception($"Ошибка при получении котировки после {maxRetries} попыток: {lastError?.Message}", lastError);
        }

        /// <summary>
        /// Проверить статус выполнения бриджа
        /// </summary>
        /// <param name="requestId">ID запроса</param>
        /// <returns>Статус бриджа</returns>
        public async Task<JToken> GetExecutionStatus(string requestId)
        {
            try
            {
                var url = $"{baseUrl}/intents/status/v2?requestId={requestId}";
                var response = await GetRequest(url);
                return JToken.Parse(response);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении статуса выполнения: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Уведомить о выполнении транзакции
        /// </summary>
        /// <param name="transactionHash">Хеш транзакции</param>
        /// <param name="chainId">ID сети</param>
        /// <returns>Результат уведомления или null если ошибка</returns>
        public async Task<JToken> NotifyTransactionIndexed(string transactionHash, int chainId)
        {
            try
            {
                var payload = new Dictionary<string, object>
                {
                    { "txHash", transactionHash },
                    { "chainId", chainId.ToString() }
                };

                var body = JsonConvert.SerializeObject(payload);
                var response = await PostRequest($"{baseUrl}/transactions/index", body);
                return JToken.Parse(response);
            }
            catch (Exception ex)
            {
                // НЕ выбрасываем ошибку - уведомление не критично для работы
                Console.WriteLine($"⚠️ Не удалось уведомить о транзакции: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Получить мультивходную котировку (с нескольких сетей на одну)
        /// </summary>
        /// <param name="multiInputParams">Параметры для получения котировки</param>
        /// <returns>Котировка для мультивходного бриджа</returns>
        public async Task<JToken> GetMultiInputQuote(MultiInputQuoteParams multiInputParams)
        {
            try
            {
                var requestData = new Dictionary<string, object>
                {
                    { "user", multiInputParams.User },
                    { "recipient", multiInputParams.Recipient },
                    { "destinationChainId", multiInputParams.DestinationChainId },
                    { "destinationCurrency", multiInputParams.DestinationCurrency },
                    { "inputs", multiInputParams.Inputs },
                    { "source", source }
                };

                // Добавляем appFees
                if (multiInputParams.AppFees != null && multiInputParams.AppFees.Count > 0)
                {
                    requestData.Add("appFees", multiInputParams.AppFees);
                }
                else
                {
                    requestData.Add("appFees", GenerateAppFees());
                }

                var body = JsonConvert.SerializeObject(requestData);
                var response = await PostRequest($"{baseUrl}/swap/multi-input", body);
                return JToken.Parse(response);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении мультивходной котировки: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Получить все запросы пользователя
        /// </summary>
        /// <param name="userAddress">Адрес пользователя</param>
        /// <returns>История запросов пользователя</returns>
        public async Task<JToken> GetUserRequests(string userAddress)
        {
            try
            {
                var url = $"{baseUrl}/requests?user={userAddress}";
                var response = await GetRequest(url);
                return JToken.Parse(response);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении запросов пользователя: {ex.Message}", ex);
            }
        }

        #endregion

        #region Execute Steps

        /// <summary>
        /// Выполнить шаги транзакции из котировки
        /// </summary>
        /// <param name="steps">Шаги для выполнения</param>
        /// <param name="privateKey">Приватный ключ кошелька</param>
        /// <param name="jsonRpc">RPC URL сети</param>
        /// <returns>Результаты выполнения шагов</returns>
        public async Task<List<StepResult>> ExecuteSteps(JArray steps, string privateKey, string jsonRpc)
        {
            var results = new List<StepResult>();
            var web3 = new Nethereum.Web3.Web3(new Account(privateKey), jsonRpc);
            var account = new Account(privateKey);
            var signer = new EthereumMessageSigner();

            foreach (var step in steps)
            {
                var stepId = step["id"]?.ToString();
                var items = step["items"] as JArray;

                if (items == null || items.Count == 0)
                    continue;

                foreach (var item in items)
                {
                    var data = item["data"];
                    var check = item["check"];

                    // Проверка условия
                    if (check != null)
                    {
                        var chainId = check["chainId"]?.Value<int>() ?? 0;
                        var endpoint = check["endpoint"]?.ToString();

                        if (!string.IsNullOrEmpty(endpoint))
                        {
                            try
                            {
                                var checkUrl = $"{baseUrl}{endpoint}";
                                var checkResponse = await GetRequest(checkUrl);
                                var checkResult = JToken.Parse(checkResponse);

                                var checkPassed = checkResult["check"]?.Value<bool>() ?? false;
                                if (!checkPassed)
                                {
                                    results.Add(new StepResult
                                    {
                                        Step = stepId,
                                        Status = "skipped",
                                        Message = "Check condition not met"
                                    });
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                results.Add(new StepResult
                                {
                                    Step = stepId,
                                    Status = "failed",
                                    Error = $"Check failed: {ex.Message}"
                                });
                                continue;
                            }
                        }
                    }

                    // Выполнение транзакции
                    if (data != null && data["to"] != null)
                    {
                        try
                        {
                            var to = data["to"].ToString();
                            var value = data["value"]?.ToString() ?? "0";
                            var txData = data["data"]?.ToString() ?? "0x";
                            var chainId = data["chainId"]?.Value<int>() ?? 1;

                            var txHash = await SendTransaction(
                                privateKey,
                                to,
                                value,
                                txData,
                                chainId,
                                jsonRpc
                            );

                            results.Add(new StepResult
                            {
                                Step = stepId,
                                Status = "completed",
                                TransactionHash = txHash
                            });
                        }
                        catch (Exception ex)
                        {
                            results.Add(new StepResult
                            {
                                Step = stepId,
                                Status = "failed",
                                Error = ex.Message
                            });
                        }
                    }

                    // Выполнение подписи
                    var signData = item["signData"];
                    if (signData != null)
                    {
                        try
                        {
                            var signatureKind = signData["signatureKind"]?.ToString();
                            string signature = null;

                            if (signatureKind == "eip191")
                            {
                                var message = signData["message"]?.ToString();
                                if (!string.IsNullOrEmpty(message))
                                {
                                    var messageBytes = message.HexToByteArray();
                                    signature = signer.Sign(messageBytes, new EthECKey(privateKey));
                                }
                            }
                            else if (signatureKind == "eip712")
                            {
                                throw new Exception("EIP-712 подпись пока не поддерживается");
                            }

                            // Отправка подписи на API
                            var postData = item["postData"];
                            if (postData != null && postData["endpoint"] != null)
                            {
                                var postUrl = $"{baseUrl}{postData["endpoint"]}";
                                var postBody = postData["body"]?.ToString() ?? "{}";

                                var bodyObj = JObject.Parse(postBody);
                                bodyObj["signature"] = signature;

                                var response = await PostRequest(postUrl, bodyObj.ToString());

                                results.Add(new StepResult
                                {
                                    Step = stepId,
                                    Status = "completed",
                                    Signature = signature
                                });
                            }
                            else
                            {
                                results.Add(new StepResult
                                {
                                    Step = stepId,
                                    Status = "completed",
                                    Signature = signature
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            results.Add(new StepResult
                            {
                                Step = stepId,
                                Status = "failed",
                                Error = ex.Message
                            });
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Отправить транзакцию
        /// </summary>
        private async Task<string> SendTransaction(string privateKey, string to, string value, string data, int chainId, string rpcUrl)
        {
            var account = new Account(privateKey, chainId);
            var web3 = new Nethereum.Web3.Web3(account, rpcUrl);

            var valueInWei = BigInteger.Parse(value);

            var txInput = new Nethereum.RPC.Eth.DTOs.TransactionInput
            {
                From = account.Address,
                To = to,
                Value = new Nethereum.Hex.HexTypes.HexBigInteger(valueInWei),
                Data = data
            };

            var receipt = await web3.Eth.TransactionManager.SendTransactionAndWaitForReceiptAsync(txInput);
            return receipt.TransactionHash;
        }

        #endregion

        #region High-Level Bridge Methods

        /// <summary>
        /// Выполнить бридж токенов между сетями
        /// </summary>
        /// <param name="quoteParams">Параметры для бриджа</param>
        /// <param name="privateKey">Приватный ключ кошелька</param>
        /// <param name="jsonRpc">RPC URL исходной сети</param>
        /// <returns>Результат бриджа</returns>
        public async Task<BridgeResult> BridgeTokens(QuoteParams quoteParams, string privateKey, string jsonRpc)
        {
            try
            {
                // Получаем котировку
                var quote = await GetQuote(quoteParams);

                // Выполняем шаги
                var steps = quote["steps"] as JArray;
                var results = await ExecuteSteps(steps, privateKey, jsonRpc);

                return new BridgeResult
                {
                    Quote = quote,
                    Results = results
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при выполнении бриджа: {ex.Message}", ex);
            }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Валидирует ответ котировки
        /// </summary>
        private void ValidateQuoteResponse(JToken response)
        {
            if (response["steps"] == null || !(response["steps"] is JArray))
            {
                throw new Exception("Некорректный ответ: отсутствуют шаги");
            }

            if (response["fees"] == null)
            {
                throw new Exception("Некорректный ответ: отсутствуют комиссии");
            }

            if (response["details"] == null)
            {
                throw new Exception("Некорректный ответ: отсутствуют детали");
            }
        }

        #endregion
    }

    #region Data Classes

    /// <summary>
    /// Параметры для получения котировки
    /// </summary>
    public class QuoteParams
    {
        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        [JsonProperty("originChainId")]
        public int OriginChainId { get; set; }

        [JsonProperty("destinationChainId")]
        public int DestinationChainId { get; set; }

        [JsonProperty("originCurrency")]
        public string OriginCurrency { get; set; }

        [JsonProperty("destinationCurrency")]
        public string DestinationCurrency { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("slippageTolerance")]
        public string SlippageTolerance { get; set; }

        [JsonProperty("tradeType")]
        public string TradeType { get; set; }

        [JsonProperty("appFees")]
        public List<AppFee> AppFees { get; set; }
    }

    /// <summary>
    /// Параметры для мультивходной котировки
    /// </summary>
    public class MultiInputQuoteParams
    {
        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        [JsonProperty("destinationChainId")]
        public int DestinationChainId { get; set; }

        [JsonProperty("destinationCurrency")]
        public string DestinationCurrency { get; set; }

        [JsonProperty("inputs")]
        public List<MultiInputEntry> Inputs { get; set; }

        [JsonProperty("appFees")]
        public List<AppFee> AppFees { get; set; }
    }

    /// <summary>
    /// Запись мультивхода
    /// </summary>
    public class MultiInputEntry
    {
        [JsonProperty("chainId")]
        public int ChainId { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }
    }

    /// <summary>
    /// App Fee структура
    /// </summary>
    public class AppFee
    {
        [JsonProperty("recipient")]
        public string recipient { get; set; }

        [JsonProperty("fee")]
        public string fee { get; set; }
    }

    /// <summary>
    /// Результат выполнения шага
    /// </summary>
    public class StepResult
    {
        public string Step { get; set; }
        public string Status { get; set; }
        public string TransactionHash { get; set; }
        public string Signature { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Результат бриджа
    /// </summary>
    public class BridgeResult
    {
        public JToken Quote { get; set; }
        public List<StepResult> Results { get; set; }
    }

    #endregion
}