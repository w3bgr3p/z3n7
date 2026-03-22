using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.CommandCenter;


namespace z3n7
{


    public class GraphQL
    {
        #region Fields & Constructor

        private readonly IZennoPosterProjectModel _project;
        private readonly Instance _instance;
        private readonly Logger _logger;

      

        public GraphQL(IZennoPosterProjectModel project, Instance instance, Logger log = null)
        {
            _project = project;
            _instance = instance;
            _logger = log;
            _instance.UseTrafficMonitoring = true;
        }

        #endregion
        
public string GetGraphQLStructure(string urlFilter)
{
    var t = new Traffic(_project, _instance, log: _logger);
    var all = t.FindAllTrafficElements(urlFilter);
    
    _logger?.Send($"Found {all.Count} traffic elements");
    
    var uniqueOperations = new Dictionary<string, JObject>();
    int skippedNoBody = 0;
    int skippedNoKey = 0;
    int skippedDuplicate = 0;

    foreach (var el in all)
    {
        _logger?.Send($"Processing: {el.Method} {el.Url}");
        _logger?.Send($"RequestBody length: {el.RequestBody?.Length ?? 0}");
        _logger?.Send($"RequestBody preview: {el.RequestBody?.Substring(0, Math.Min(200, el.RequestBody?.Length ?? 0))}");
        
        if (string.IsNullOrEmpty(el.RequestBody))
        {
            skippedNoBody++;
            _logger?.Send("Skipped: no request body");
            continue;
        }

        string operationKey = ExtractGraphQLOperationKey(el.RequestBody);
        
        if (string.IsNullOrEmpty(operationKey))
        {
            skippedNoKey++;
            _logger?.Send("Skipped: could not extract operation key");
            continue;
        }

        if (uniqueOperations.ContainsKey(operationKey))
        {
            skippedDuplicate++;
            _logger?.Send("Skipped: duplicate operation");
            continue;
        }

        var item = new JObject();
        item["operationType"] = GetOperationType(el.RequestBody);
        item["operationName"] = ExtractOperationName(el.RequestBody);
        item["url"] = el.Url;
        item["statusCode"] = el.StatusCode;
        
        var requestJson = JObject.Parse(el.RequestBody);
        if (requestJson["extensions"]?["persistedQuery"] != null)
        {
            item["isPersistedQuery"] = true;
            item["queryHash"] = requestJson["extensions"]["persistedQuery"]["sha256Hash"]?.ToString();
        }
        else
        {
            item["isPersistedQuery"] = false;
        }
        

        try { item["requestBody"] = JToken.Parse(el.RequestBody); }
        catch { item["requestBody"] = el.RequestBody; }

        if (!string.IsNullOrEmpty(el.ResponseBody))
        {
            try { item["responseBody"] = JToken.Parse(el.ResponseBody); }
            catch { item["responseBody"] = el.ResponseBody; }
        }

        uniqueOperations[operationKey] = item;
        _logger?.Send($"Added operation: {item["operationName"]}");
    }

    _logger?.Send($"Stats - NoBody: {skippedNoBody}, NoKey: {skippedNoKey}, Duplicate: {skippedDuplicate}, Added: {uniqueOperations.Count}");

    var snapshot = new JObject();
    snapshot["totalOperations"] = uniqueOperations.Count;
    snapshot["operations"] = new JArray(uniqueOperations.Values);

    string json = JsonConvert.SerializeObject(snapshot, Formatting.Indented);
    return json;
}		
private string ExtractGraphQLOperationKey(string requestBody)
{
    try
    {
        var json = JObject.Parse(requestBody);
        
        // Проверяем наличие полного query
        string query = json["query"]?.ToString();
        
        if (!string.IsNullOrEmpty(query))
        {
            // Обычный GraphQL запрос
            query = System.Text.RegularExpressions.Regex.Replace(query, @"\s+", " ").Trim();
            return query;
        }
        
        // Если query нет, проверяем Persisted Query (Apollo)
        string operationName = json["operationName"]?.ToString();
        string hash = json["extensions"]?["persistedQuery"]?["sha256Hash"]?.ToString();
        
        if (!string.IsNullOrEmpty(operationName) && !string.IsNullOrEmpty(hash))
        {
            // Используем комбинацию operationName + hash как ключ
            return $"{operationName}:{hash}";
        }
        
        // Если есть только operationName (без query и без hash)
        if (!string.IsNullOrEmpty(operationName))
        {
            return operationName;
        }
        
        return null;
    }
    catch
    {
        return null;
    }
}		
		private string GetOperationType(string requestBody)
		{
		    try
		    {
		        var json = JObject.Parse(requestBody);
		        string query = json["query"]?.ToString()?.Trim();
		        
		        if (string.IsNullOrEmpty(query))
		            return "unknown";
		
		        // Ищем тип операции в начале запроса
		        if (query.StartsWith("query", StringComparison.OrdinalIgnoreCase))
		            return "query";
		        if (query.StartsWith("mutation", StringComparison.OrdinalIgnoreCase))
		            return "mutation";
		        if (query.StartsWith("subscription", StringComparison.OrdinalIgnoreCase))
		            return "subscription";
		        
		        // Если не указан тип, по умолчанию это query
		        return "query";
		    }
		    catch
		    {
		        return "unknown";
		    }
		}
		
		private string ExtractOperationName(string requestBody)
		{
		    try
		    {
		        var json = JObject.Parse(requestBody);
		        
		        // Сначала проверяем поле operationName
		        string operationName = json["operationName"]?.ToString();
		        if (!string.IsNullOrEmpty(operationName))
		            return operationName;
		
		        // Если нет, пытаемся извлечь из query
		        string query = json["query"]?.ToString();
		        if (string.IsNullOrEmpty(query))
		            return "anonymous";
		
		        // Ищем имя после типа операции: "query GetUser {" или "mutation CreateUser {"
		        var match = System.Text.RegularExpressions.Regex.Match(
		            query, 
		            @"(?:query|mutation|subscription)\s+(\w+)"
		        );
		        
		        if (match.Success)
		            return match.Groups[1].Value;
		
		        return "anonymous";
		    }
		    catch
		    {
		        return "unknown";
		    }
		}
    }
}