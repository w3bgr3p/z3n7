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
    /// <summary>Extension methods on strings: hex, Base64, JSON, ranges, Markdown escaping, JWT, passwords.</summary>
    public static partial class StringExtensions
    {
        

        #region HEX

        /// <summary>Converts a decimal number to a <c>0x</c> hex string, optionally scaling it first.</summary>
        /// <param name="value">Number in invariant culture.</param>
        /// <param name="convert"><c>gwei</c> (×10⁹), <c>eth</c> (×10¹⁸), or empty for the plain number.</param>
        /// <returns>The hex value; <c>0x0</c> for empty or invalid input.</returns>
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

        /// <summary>
        /// Converts a hex number (with or without <c>0x</c>) to decimal text, optionally scaling it down.
        /// </summary>
        /// <param name="hexValue">Hex number.</param>
        /// <param name="convert"><c>gwei</c> (×10⁹), <c>eth</c> (×10¹⁸), or empty for the plain number.</param>
        /// <returns>The number; <c>0</c> for empty or invalid input.</returns>
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

        /// <summary>UTF-8 Base64 of the text; empty for empty input.</summary>
        public static string ToBase64(this string cookiesJson)
        {
            if (string.IsNullOrEmpty(cookiesJson))
                return string.Empty;
        
            byte[] bytes = Encoding.UTF8.GetBytes(cookiesJson);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>Decodes UTF-8 Base64; empty for empty input; the input unchanged when it is not Base64.</summary>
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

        /// <summary>
        /// Flattens a JSON object: nested keys are joined with <c>_</c>, array items get their index
        /// (<c>a_b_0</c>).
        /// </summary>
        /// <param name="json">JSON object.</param>
        /// <param name="ignoreEmpty">Leave out empty values.</param>
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

        /// <summary>
        /// Shows the query parameters of a URL, one per line. When there is an <c>addEthereumChainParameter</c>
        /// parameter with JSON, returns that JSON instead.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="oneline">Put everything on one line.</param>
        /// <returns>The text, or a message starting with <c>Error:</c>.</returns>
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

        /// <summary>
        /// Expands an account range: <c>1,4,7</c> as is, <c>1-10</c> to every number, a single number <c>n</c>
        /// to <c>1…n</c>.
        /// </summary>
        /// <returns>The numbers as strings. Throws for empty input.</returns>
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

        /// <summary>Removes characters that are not allowed in file names.</summary>
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

        /// <summary>File name from a URL, or from the <c>src</c>/<c>href</c> attribute in an HTML fragment.</summary>
        /// <param name="input">URL or HTML fragment.</param>
        /// <param name="withExtension">Keep the extension.</param>
        /// <returns>The file name, or the input when none is found.</returns>
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

        /// <summary>Escapes Telegram MarkdownV2 special characters with a backslash.</summary>
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
        /// <summary>Decodes a JWT without checking its signature.</summary>
        /// <returns>
        /// <c>alg</c>, <c>typ</c>, <c>kid</c>, <c>iss</c>, <c>sub</c>, <c>aud</c>, <c>iat</c>/<c>exp</c> with
        /// dates, <c>ttl_seconds</c>, <c>is_expired</c>, raw header and payload JSON and the signature; or
        /// <c>error</c>.
        /// </returns>
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

        
        /// <summary>
        /// Random password from lowercase letters plus the selected groups; at least one character of each
        /// selected group is included.
        /// </summary>
        /// <param name="length">Length.</param>
        /// <param name="includeDigits">Include digits.</param>
        /// <param name="randomizeCase">Include uppercase letters.</param>
        /// <param name="includeSymbols">Include <c>!@#$%^&amp;*()</c>.</param>
        /// <returns>The password. Throws when the length is below 1 or too small for the selected groups.</returns>
        public static string NewPassword(int length = 16, bool includeDigits= true, bool randomizeCase = true, bool includeSymbols = true)
        {
            if (length < 1)
                throw new ArgumentException("Length must be at least 1.");
		
            var random = new Random();
            string letters = "abcdefghijklmnopqrstuvwxyz";
            string digits  = "0123456789";
            string symbols = "!@#$%^&*()";
		
            // --- строим пул и обязательные символы ---
            var pool      = new StringBuilder(letters);
            var mandatory = new List<char>();
		
            if (includeDigits)
            {
                pool.Append(digits);
                mandatory.Add(digits[random.Next(digits.Length)]);
            }
		
            if (includeSymbols)
            {
                pool.Append(symbols);
                mandatory.Add(symbols[random.Next(symbols.Length)]);
            }
		
            // для randomizeCase добавляем uppercase в пул,
            // плюс один обязательный uppercase
            if (randomizeCase)
            {
                string upper = letters.ToUpper();
                pool.Append(upper);
                mandatory.Add(upper[random.Next(upper.Length)]);
            }
		
            if (mandatory.Count > length)
                throw new ArgumentException("Length too small to satisfy all required character groups.");
		
            string poolStr = pool.ToString();
		
            // --- заполняем остаток ---
            var password = new StringBuilder();
            foreach (char c in mandatory)
                password.Append(c);
		
            for (int i = mandatory.Count; i < length; i++)
                password.Append(poolStr[random.Next(poolStr.Length)]);
		
            // --- перемешиваем ---
            for (int i = 0; i < password.Length; i++)
            {
                int j    = random.Next(password.Length);
                char tmp = password[i];
                password[i] = password[j];
                password[j] = tmp;
            }
		
            return password.ToString();
        }
        #endregion
        
    }
    /// <summary>Extension methods on <c>IZennoPosterProjectModel</c>: strings and JSON.</summary>
    public static partial class ProjectExtensions
    {
        /// <summary>
        /// Loads JSON into <c>project.Json</c>. When the text is not JSON, takes the line starting with
        /// <c>{objIndex}:</c> and loads the rest of it. Failures are logged as warnings.
        /// </summary>
        /// <param name="json">JSON text, or numbered lines of JSON.</param>
        /// <param name="thrw">Throw when the second attempt also fails.</param>
        /// <param name="objIndex">Line number prefix to look for.</param>
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
