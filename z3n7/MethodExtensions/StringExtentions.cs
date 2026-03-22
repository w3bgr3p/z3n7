using ZennoLab.InterfacesLibrary.ProjectModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace z3n7
{
    public static partial class StringExtensions
    {
        

        #region HEX

        public static string StringToHex(this string value, string convert = "")
        {
            try
            {
                if (string.IsNullOrEmpty(value)) return "0x0";

                value = value?.Trim();
                if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
                    return "0x0";

                BigInteger result;
                switch (convert.ToLower())
                {
                    case "gwei":
                        result = (BigInteger)(number * 1000000000m);
                        break;
                    case "eth":
                        result = (BigInteger)(number * 1000000000000000000m);
                        break;
                    default:
                        result = (BigInteger)number;
                        break;
                }

                string hex = result.ToString("X").TrimStart('0');
                return string.IsNullOrEmpty(hex) ? "0x0" : "0x" + hex;
            }
            catch
            {
                return "0x0";
            }
        }

        public static string HexToString(this string hexValue, string convert = "")
        {
            try
            {
                hexValue = hexValue?.Replace("0x", "").Trim();
                if (string.IsNullOrEmpty(hexValue)) return "0";
                BigInteger number = BigInteger.Parse("0" + hexValue, NumberStyles.AllowHexSpecifier);
                switch (convert.ToLower())
                {
                    case "gwei":
                        decimal gweiValue = (decimal)number / 1000000000m;
                        return gweiValue.ToString("0.#########", CultureInfo.InvariantCulture);
                    case "eth":
                        decimal ethValue = (decimal)number / 1000000000000000000m;
                        return ethValue.ToString("0.##################", CultureInfo.InvariantCulture);
                    default:
                        return number.ToString();
                }
            }
            catch
            {
                return "0";
            }
        }

        #endregion

        #region Base64

        public static string ToBase64(this string cookiesJson)
        {
            if (string.IsNullOrEmpty(cookiesJson))
                return string.Empty;
        
            byte[] bytes = Encoding.UTF8.GetBytes(cookiesJson);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string base64Cookies)
        {
            if (string.IsNullOrEmpty(base64Cookies))
                return string.Empty;
        
            try
            {
                byte[] bytes = Convert.FromBase64String(base64Cookies);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException)
            {
                return base64Cookies;
            }
        }
        

        #endregion

        #region JSON

        public static Dictionary<string, string> JsonToDic(this string json, bool ignoreEmpty = true)
        {
            var result = new Dictionary<string, string>();
    
            if (string.IsNullOrWhiteSpace(json)) return result;

            var jObject = JObject.Parse(json);

            FlattenJson(jObject, "", result);

            return result;

            void FlattenJson(JToken token, string prefix, Dictionary<string, string> dict)
            {
                switch (token.Type)
                {
                    case JTokenType.Object:
                        foreach (var property in token.Children<JProperty>())
                        {
                            var key = string.IsNullOrEmpty(prefix) 
                                ? property.Name 
                                : $"{prefix}_{property.Name}";
                            FlattenJson(property.Value, key, dict);
                        }
                        break;
        
                    case JTokenType.Array:
                        var index = 0;
                        foreach (var item in token.Children())
                        {
                            FlattenJson(item, $"{prefix}_{index}", dict);
                            index++;
                        }
                        break;
        
                    default:
                        var value = token.ToString();

                        if (ignoreEmpty && string.IsNullOrEmpty(value))
                        {
                            return;
                        }

                        dict[prefix] = value;
                        break;
                }
            }
        }

        public static string ConvertUrl(this string url, bool oneline = false)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "Error: URL is empty or null";
            }

            string queryString = url.Contains("?") ? url.Substring(url.IndexOf('?') + 1) : string.Empty;
            if (string.IsNullOrEmpty(queryString))
            {
                return "Error: No query parameters found in URL";
            }

            if (queryString.Contains("#"))
            {
                int hashIndex = queryString.IndexOf('#');
                int nextQueryIndex = queryString.IndexOf('?', hashIndex);
                if (nextQueryIndex != -1)
                {
                    queryString = queryString.Substring(nextQueryIndex + 1);
                }
                else
                {
                    queryString = queryString.Substring(0, hashIndex);
                }
            }

            var parameters = new NameValueCollection();
            string[] queryParts = queryString.Split('&');
            foreach (string part in queryParts)
            {
                if (string.IsNullOrEmpty(part)) continue;
                string[] keyValue = part.Split(new[] { '=' }, 2);
                if (keyValue.Length == 2)
                {
                    string key = Uri.UnescapeDataString(keyValue[0]);
                    string value = Uri.UnescapeDataString(keyValue[1]);
                    parameters.Add(key, value);
                }
            }

            string chainParam = parameters["addEthereumChainParameter"];
            if (!string.IsNullOrEmpty(chainParam))
            {
                try
                {
                    var json = JObject.Parse(chainParam);
                    string jsonResult = JsonConvert.SerializeObject(json, oneline ? Formatting.None : Formatting.Indented);

                    return oneline ? jsonResult.Replace('\n', ' ').Replace('\r', ' ') : jsonResult;
                }
                catch (JsonException)
                {
                }
            }

            StringBuilder result = new StringBuilder();
            foreach (string key in parameters.AllKeys)
            {
                if (oneline)
                {
                    result.Append($"{key}: {parameters[key]} | ");
                }
                else
                {
                    result.AppendLine($"{key}: {parameters[key]}");
                }
            }

            string finalResult = result.ToString();

            finalResult =  finalResult.Length > 0 ? finalResult : "Error: No valid parameters found";
            return oneline ? finalResult.Replace('\n', ' ').Replace('\r', ' ') : finalResult;
        }

        #endregion
        
        #region STRING UTILITIES

        public static string[] Range(this string accRange)
        {
            if (string.IsNullOrEmpty(accRange))  
                throw new Exception("range cannot be empty");
            if (accRange.Contains(","))
                return accRange.Split(',');
            else if (accRange.Contains("-"))
            {
                var rangeParts = accRange.Split('-').Select(int.Parse).ToArray();
                int rangeS = rangeParts[0];
                int rangeE = rangeParts[1];
                accRange = string.Join(",", Enumerable.Range(rangeS, rangeE - rangeS + 1));
                return accRange.Split(',');
            }
            else
            {
                int rangeS = 1;
                int rangeE = int.Parse(accRange);
                accRange = string.Join(",", Enumerable.Range(rangeS, rangeE - rangeS + 1));
                return accRange.Split(',');
            }
        }

        public static string CleanFilePath(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            char[] invalidChars = Path.GetInvalidFileNameChars();

            string cleaned = text;
            foreach (char c in invalidChars)
            {
                cleaned = cleaned.Replace(c.ToString(), "");
            }
            return cleaned;
        }

        public static string GetFileNameFromUrl(string input, bool withExtension = false)
        {
            try
            {
                var urlMatch = Regex.Match(input, @"(?:src|href)=[""']?([^""'\s>]+)", RegexOptions.IgnoreCase);
                var url = urlMatch.Success ? urlMatch.Groups[1].Value : input;

                var fileMatch = Regex.Match(url, @"([^/\\?#]+)(?:\?[^/]*)?$");
                if (fileMatch.Success)
                {
                    var fileName = fileMatch.Groups[1].Value;
            
                    if (withExtension)
                    {
                        return fileName;
                    }
            
                    return Regex.Replace(fileName, @"\.[^.]+$", "");
                }

                return input;
            }
            catch
            {
                return input;
            }
        }

        public static string EscapeMarkdown(this string text)
        {
            string[] specialChars = new[] { "_", "*", "[", "]", "(", ")", "~", "`", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!" };
            foreach (var ch in specialChars)
            {
                text = text.Replace(ch, "\\" + ch);
            }
            return text;
        }

        #endregion

        #region SECURITY
        public static Dictionary<string, object> ParseJwt( this string jwt)
        {
            var result = new Dictionary<string, object>();
            
            if (string.IsNullOrEmpty(jwt))
            {
                result["error"] = "Empty token";
                return result;
            }
            
            var parts = jwt.Split('.');
            if (parts.Length != 3)
            {
                result["error"] = "Invalid JWT format";
                return result;
            }
            
            try
            {
                // Decode header
                string headerPayload = parts[0].Replace('-', '+').Replace('_', '/');
                switch (headerPayload.Length % 4)
                {
                    case 2: headerPayload += "=="; break;
                    case 3: headerPayload += "="; break;
                }
                var headerJson = Encoding.UTF8.GetString(Convert.FromBase64String(headerPayload));
                var header = JObject.Parse(headerJson);
                
                // Decode payload
                string payloadB64 = parts[1].Replace('-', '+').Replace('_', '/');
                switch (payloadB64.Length % 4)
                {
                    case 2: payloadB64 += "=="; break;
                    case 3: payloadB64 += "="; break;
                }
                var payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(payloadB64));
                var payload = JObject.Parse(payloadJson);
                
                // Header info
                result["alg"] = header["alg"]?.ToString();
                result["typ"] = header["typ"]?.ToString();
                result["kid"] = header["kid"]?.ToString();
                
                // Payload info
                result["iss"] = payload["iss"]?.ToString();
                result["sub"] = payload["sub"]?.ToString();
                result["aud"] = payload["aud"]?.ToString();
                
                // Timestamps
                long iat = payload["iat"]?.Value<long>() ?? 0;
                long exp = payload["exp"]?.Value<long>() ?? 0;
                
                if (iat > 0)
                {
                    result["iat"] = iat;
                    result["iat_dt"] = DateTimeOffset.FromUnixTimeSeconds(iat).UtcDateTime;
                }
                
                if (exp > 0)
                {
                    result["exp"] = exp;
                    result["exp_dt"] = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                    result["ttl_seconds"] = exp - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    result["is_expired"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds() > exp;
                }
                
                // Raw payloads
                result["header_json"] = headerJson;
                result["payload_json"] = payloadJson;
                result["signature"] = parts[2];
                
                return result;
            }
            catch (Exception ex)
            {
                result["error"] = ex.Message;
                return result;
            }
        }
        #endregion
        
        #region NEW
        public static string NewPassword(int length)
        {
            if (length < 8)
            {
                throw new ArgumentException("Length must be at least 8 characters.");
            }

            string lowercase = "abcdefghijklmnopqrstuvwxyz";
            string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numbers = "0123456789";
            string special = "!@#$%^&*()";
            string allChars = lowercase + uppercase + numbers + special;
            Random random = new Random();
            StringBuilder password = new StringBuilder();

            password.Append(lowercase[random.Next(lowercase.Length)]);
            password.Append(uppercase[random.Next(uppercase.Length)]);
            password.Append(numbers[random.Next(numbers.Length)]);
            password.Append(special[random.Next(special.Length)]);

            for (int i = 4; i < length; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            for (int i = 0; i < password.Length; i++)
            {
                int randomIndex = random.Next(password.Length);
                char temp = password[i];
                password[i] = password[randomIndex];
                password[randomIndex] = temp;
            }

            return password.ToString();
        }
        #endregion
        
    }
    public static partial class ProjectExtensions
    {
        public static void ToJson(this IZennoPosterProjectModel project, string json, bool thrw = false, int objIndex = 1)
        {
            try
            {
                project.Json.FromString(json);
                return;
            }
            catch (Exception ex)
            {
                project.SendWarningToLog(ex.Message);
            }

            try
            {
                string[] lines = json.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                string jsonData = "";
                for (int i = 0; i < lines.Length; i++) 
                {
                    if (lines[i].StartsWith($"{objIndex}:")) 
                    {
                        jsonData = lines[i].Substring(2);
                        break;
                    }
                }
                if (jsonData == "") {
                    throw new Exception($"Не найдены данные с индексом {objIndex}");
                }
                project.Json.FromString(jsonData);
                return;
            }
            catch (Exception ex)
            {
                project.SendWarningToLog(ex.Message);
                if (thrw)throw;
            }
            
        }
    }
    
}
