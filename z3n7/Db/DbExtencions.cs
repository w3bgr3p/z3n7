using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using ZennoLab.InterfacesLibrary.ProjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZennoLab.InterfacesLibrary.Enums.Log;

namespace z3n7
{
    #region DbHelpers - Вспомогательные методы
    internal static class DbHelpers
    {
        internal const char RawSeparator = '·';
        internal const char ColumnSeparator = '¦';
        internal static string UnQuote(string name)
        {
            return name.Replace("\"","");
        }
        internal static string Quote(string name)
        {
            return string.Join(".", name.Split('.').Select(part => $"\"{part.Replace("\"", "\"\"")}\""));
        }
        internal static string Schema(string name)
        {
            var parts = UnQuote(name).Split('.');
            return parts.Length > 1 ? parts[0] : "public";
        }
        internal static string Table(string name)
        {
            return UnQuote(name).Split('.').Last();
        }
        
        internal static string QuoteColumns(this string updateString)
        {
            var parts = updateString.Split(',').Select(p => p.Trim()).ToList();
            var result = new List<string>();

            foreach (var part in parts)
            {
                int equalsIndex = part.IndexOf('=');
                if (equalsIndex > 0)
                {
                    string columnName = part.Substring(0, equalsIndex).Trim();
                    string valuePart = part.Substring(equalsIndex).Trim();

                    result.Add($"\"{columnName}\" {valuePart}");
                }
                else
                {
                    result.Add(part);
                }
            }
            return string.Join(", ", result);
        }
        
        internal static string QuoteSelectColumns(this string columnString)
        {
            return string.Join(", ", 
                columnString.Split(',')
                    .Select(col => $"\"{col.Trim()}\""));
        }
        
        internal static readonly Regex ValidNamePattern = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);
        
        internal static bool IsValidRange(string range)
        {
            if (string.IsNullOrEmpty(range)) return false;
            return Regex.IsMatch(range, @"^[\d\s,\-]+$");
        }

        internal static string TableName(this IZennoPosterProjectModel project, string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) 
                return project.ProjectTable();
            return tableName;
        }
    }
    #endregion
    /// <summary>Reading from the project database. The row defaults to the current account (<c>acc0</c>).</summary>
    public static class Get
    {
        /// <summary>Reads columns of one row; same as <c>SqlGet</c>.</summary>
        /// <param name="toGet">Comma-separated column names; each is quoted.</param>
        /// <param name="tableName">Table; default is the project table (<c>projectTable</c> variable).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="acc">Value of <c>key</c>; default <c>acc0</c>.</param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        /// <returns>Columns joined by <c>¦</c>; several rows are joined by <c>·</c>.</returns>
        public static string DbGet(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", string acc = null, string where = "")
        {
            return project.SqlGet(toGet, tableName, log, thrw, key, acc, where);
        }
        
        /// <summary>Reads columns of one row as column → value.</summary>
        /// <param name="toGet">Comma-separated column names; each is quoted.</param>
        /// <param name="tableName">Table; default is the project table (<c>projectTable</c> variable).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">
        /// Value of <c>key</c>; default is the current account, <c>acc0</c>. Inserted into the SQL as written.
        /// </param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        /// <returns>An empty dictionary when nothing was found.</returns>
        public static Dictionary<string, string> DbGetColumns(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
        {
            return  project.SqlGetDicFromLine(toGet, tableName, log, thrw, key, id, where);
        }
        
        /// <summary>Reads columns of one row as an array.</summary>
        /// <param name="toGet">Comma-separated column names; each is quoted.</param>
        /// <param name="tableName">Table; default is the project table (<c>projectTable</c> variable).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">
        /// Value of <c>key</c>; default is the current account, <c>acc0</c>. Inserted into the SQL as written.
        /// </param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        public static string[] DbGetLine(this IZennoPosterProjectModel project, string toGet, string tableName = null,  bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
        {
            return project.SqlGetArrFromLine(toGet, tableName, log, thrw, key, id, where);
        }
        
        /// <summary>Reads several rows; each item keeps its columns joined by <c>¦</c>.</summary>
        /// <param name="toGet">Comma-separated column names; each is quoted.</param>
        /// <param name="tableName">Table; default is the project table (<c>projectTable</c> variable).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">
        /// Value of <c>key</c>; default is the current account, <c>acc0</c>. Inserted into the SQL as written.
        /// </param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        /// <param name="toList">When set, the result is also written to this ZennoPoster list.</param>
        public static List<string> DbGetLines(this IZennoPosterProjectModel project, string toGet, string tableName = null,  bool log = false, bool thrw = false, string key = "id", object id = null, string where = "", string toList = null)
        {
            var list =  project.SqlGetListFromLines(toGet, tableName, log, thrw, key, id, where);
            if (!string.IsNullOrEmpty(toList)) project.ListSync(toList,list);
            return list;
        }
        
        /// <summary>Reads columns of one row and sets a project variable of the same name for each.</summary>
        /// <param name="toGet">Comma-separated column names; each is quoted.</param>
        /// <param name="tableName">Table; default is the project table (<c>projectTable</c> variable).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">
        /// Value of <c>key</c>; default is the current account, <c>acc0</c>. Inserted into the SQL as written.
        /// </param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        /// <returns>The values that were set.</returns>
        public static Dictionary<string, string> DbToVars(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
        {
            var data = project.SqlGetDicFromLine(toGet, tableName, log, thrw, key, id, where);
            project.VarsFromDict(data);
            return data;
        }
        
        /// <summary>
        /// Reads <c>toGet</c> from random rows where it is not empty and <c>id</c> is below <c>range</c>.
        /// </summary>
        /// <param name="toGet">Column to read.</param>
        /// <param name="tableName">Table; default is <c>project.ProjectTable()</c> (<c>__</c> + project name).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="acc">Prefix the result with the <c>id</c> column.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="range">Upper id bound (exclusive); 0 uses the last account of <c>project.Range()</c>.</param>
        /// <param name="single">One row instead of all matching rows.</param>
        /// <param name="invert">Select rows where the column is empty instead.</param>
        public static string DbGetRandom(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool acc = false, bool thrw = false, int range = 0, bool single = true, bool invert = false)
        {
            if (range == 0)
            {
                var rng = project.Range();
                range = int.Parse(rng[rng.Count-1]);
            }
            if (string.IsNullOrEmpty(tableName)) tableName = project.ProjectTable();;

            string acc0 = string.Empty;
            if (acc) acc0 = "id, ";
            string query = $@"
                SELECT {acc0}{toGet.Trim().TrimEnd(',')} 
                from {tableName} 
                WHERE TRIM({toGet}) != ''
	            AND id < {range}
                ORDER BY RANDOM()";

            if (single) query += " LIMIT 1;";
            if (invert) query = query.Replace("!=", "=");

            return project.DbQ(query, log: log, thrw: thrw);
        }
        
        /// <summary>
        /// Reads the current account's key from the wallet table (<c>DbSchema.Wlt</c>) and decrypts it with
        /// <c>SAFU.Decode</c>.
        /// </summary>
        /// <param name="chainType">
        /// <c>evm</c> (column <c>secp256k1</c>), <c>sol</c> (<c>base58</c>) or <c>seed</c> (<c>bip39</c>).
        /// Anything else throws.
        /// </param>
        public static string DbKey(this IZennoPosterProjectModel project, string chainType = "evm")
        {
            chainType = chainType.ToLower().Trim();
            switch (chainType)
            {
                case "evm":
                    chainType = "secp256k1";
                    break;
                case "sol":
                    chainType = "base58";
                    break;
                case "seed":
                    chainType = "bip39";
                    break;
                default:
                    throw new Exception("unexpected input. Use (evm|sol|seed|pkFromSeed)");
            }

            var resp = project.SqlGet(chainType, DbSchema.Wlt);
            
            string decoded = SAFU.Decode(project,resp) ;
            return decoded;
        }
        /// <summary>
        /// Returns the <c>cfgRefCode</c> variable; when it is empty, picks a random non-empty
        /// <c>inviteColumn</c> from the project table and stores it in <c>cfgRefCode</c>.
        /// </summary>
        /// <param name="limit">
        /// When set, only rows with <c>id</c> up to this number. Must be a positive integer, otherwise it
        /// throws.
        /// </param>
        /// <param name="inviteColumn">Column with invite codes.</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        public static string RndInvite(this IZennoPosterProjectModel project, object limit = null, string inviteColumn = "refcode", bool log = false)
        {
            string refCode = project.Variables["cfgRefCode"].Value;

            if (string.IsNullOrEmpty(refCode))
            {
                string whereClause = $"TRIM({inviteColumn}) != ''";
    
                if (limit != null)
                {
                    string parsedLimit = limit.ToString();
                    if (int.TryParse(parsedLimit, out int limitValue) && limitValue > 0)
                    {
                        whereClause += $" AND id <= {limitValue}";
                    }
                    else
                    {
                        project.SendErrorToLog($"Invalid limit. {limit}", log);
                        throw new Exception($"Invalid limit. {limit}");
                    }
                }
    
                whereClause += " ORDER BY RANDOM() LIMIT 1";
                refCode = project.SqlGet(inviteColumn, log: log, where: whereClause);
                project.Variables["cfgRefCode"].Value = refCode;
            }

            
            return refCode;
        }
        
        
    }
    
    /// <summary>Writing to the project database.</summary>
    public static class DbUpdate
    {
        /// <summary>
        /// Inserts one row. The <c>id</c> key is skipped; an empty dictionary inserts a row of defaults.
        /// </summary>
        /// <param name="dataDic">Column → value.</param>
        /// <param name="tableName">Table; default is the project table (<c>projectTable</c> variable).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <returns>Affected row count as text.</returns>
        public static string DbInsert(this IZennoPosterProjectModel project, Dictionary<string, string> dataDic, string tableName = null, bool log = false, bool thrw = false)
        {
            tableName = project.TableName(tableName);
            var data = dataDic.Where(p => !p.Key.Equals("id", StringComparison.OrdinalIgnoreCase)).ToList();
            var table = DbHelpers.Quote(tableName);

            if (data.Count == 0)
                return project.DbQ($"INSERT INTO {table} DEFAULT VALUES;", log, thrw: thrw);

            var columns = string.Join(", ", data.Select(p => DbHelpers.Quote(p.Key)));
            var values = string.Join(", ", data.Select(p => $"'{p.Value.Replace("'", "''")}'"));
            return project.DbQ($"INSERT INTO {table} ({columns}) VALUES ({values});", log, thrw: thrw);
        }

        /// <summary>
        /// Writes a dictionary to the current account's row (or the rows matching <c>where</c>), adding missing
        /// columns first.
        /// A key <c>id</c> is written to the column <c>_id</c>.
        /// </summary>
        /// <param name="dataDic">Column → value. Modified in place when it has an <c>id</c> key.</param>
        /// <param name="tableName">Table; default is the <c>projectTable</c> variable.</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        public static void DicToDb(this IZennoPosterProjectModel project, Dictionary<string,string> dataDic, string tableName = null, bool log = false, bool thrw = false, string where = "")
        {
            if (string.IsNullOrWhiteSpace(tableName)) tableName = project.Var("projectTable");
            
            if (dataDic.ContainsKey("id"))
            {
                dataDic["_id"] = dataDic["id"];
                dataDic.Remove("id");
            }
            
            var columns = new List<string>();
            var updString = new StringBuilder();
            
            foreach(var p in dataDic)
            {
                columns.Add(p.Key);
            }
            project.ClmnAdd(project.TblForProject(columns), tableName, log:log);
            
            foreach(var p in dataDic)
            {
                string escapedValue = p.Value.Replace("'", "''");
                updString.Append($"{p.Key} = '{escapedValue}',");
            }
            project.DbUpd(updString.ToString().Trim(','), tableName, log, thrw, where:where, saveToVar : "");
        }
    
        /// <summary>
        /// Runs <c>UPDATE … SET toUpd</c> for the current account's row, or for the rows matching <c>where</c>.
        /// </summary>
        /// <param name="toUpd">
        /// Assignments such as <c>status = 'ok'</c>; column names are quoted, values are taken as written.
        /// </param>
        /// <param name="tableName">Table; default is the project table (<c>projectTable</c> variable).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="acc">
        /// Value of <c>key</c>; default is the current account, <c>acc0</c>. Inserted into the SQL as written.
        /// </param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        /// <param name="saveToVar">Variable that receives <c>toUpd</c> before the update; empty to skip.</param>
        public static void DbUpd(this IZennoPosterProjectModel project, string toUpd, string tableName = null, bool log = false, bool thrw = false, string key = "id", object acc = null, string where = "", string saveToVar = "lastQuery")
        {
            if (!string.IsNullOrEmpty(saveToVar))
                project.Var(saveToVar, toUpd);
            project.SqlUpd(toUpd, tableName, log, thrw, key, acc, where);
        }
        /// <summary>
        /// Writes a cooldown timestamp (<c>Time.Cd</c>, ISO UTC) to the <c>task</c> column of the current
        /// account's row (or the row selected by <c>key</c>/<c>acc</c>, or the rows matching <c>where</c>): end
        /// of today, or now plus <c>cooldownMin</c>.
        /// </summary>
        /// <param name="task">Column to write.</param>
        /// <param name="cooldownMin">Minutes from now; 0 means today 23:59:59 UTC.</param>
        /// <param name="tableName">Table; default is the project table (<c>projectTable</c> variable).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="acc">
        /// Value of <c>key</c>; default is the current account, <c>acc0</c>. Inserted into the SQL as written.
        /// </param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        public static void DbDone(this IZennoPosterProjectModel project, string task = "daily", int cooldownMin = 0, string tableName = null, bool log = false, bool thrw = false, string key = "id", object acc = null, string where = "")
        {
            var cd = (cooldownMin == 0) ? Time.Cd() : Time.Cd(cooldownMin);
            project.DbUpd($"{task} = '{cd}'", tableName, log, thrw, key, acc, where);
        }
        
    }

    /// <summary>Storing a JSON object as table columns and rebuilding it.</summary>
    public static class DbJson
    {
        /// <summary>
        /// Flattens a JSON object into columns (nested keys joined with <c>_</c>) and writes them with
        /// <c>DicToDb</c>.
        /// </summary>
        /// <param name="json">JSON object.</param>
        /// <param name="tableName">Table; default is the project table (<c>projectTable</c> variable).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        /// <param name="saveStructure">
        /// Also save the shape in <c>_json_structure</c> so that <c>DbToJson</c> can rebuild the object.
        /// </param>
        public static void JsonToDb(this IZennoPosterProjectModel project, string json, string tableName = null, bool log = false, bool thrw = false, string where = "",bool saveStructure = false)
        {
            tableName = project.TableName(tableName);
            var structure = ExtractStructure(json);
            var dataDic = json.JsonToDic(true);
            if (saveStructure) dataDic["_json_structure"] = structure; 
            project.DicToDb(dataDic, tableName, log, thrw,where:where);
        }
        private static string ExtractStructure(string json)
        {
            var structure = new Dictionary<string, string>();
            var jObject = JObject.Parse(json);
    
            MapStructure(jObject, "");
            return JsonConvert.SerializeObject(structure);
    
            void MapStructure(JToken token, string prefix)
            {
                switch (token.Type)
                {
                    case JTokenType.Object:
                        if (!string.IsNullOrEmpty(prefix))
                            structure[prefix] = "object";
                    
                        foreach (var property in token.Children<JProperty>())
                        {
                            var key = string.IsNullOrEmpty(prefix) 
                                ? property.Name 
                                : $"{prefix}_{property.Name}";
                            MapStructure(property.Value, key);
                        }
                        break;

                    case JTokenType.Array:
                        structure[prefix] = "array";
                        var index = 0;
                        foreach (var item in token.Children())
                        {
                            MapStructure(item, $"{prefix}_{index}");
                            index++;
                        }
                        break;
                
                    default:
                        structure[prefix] = token.Type.ToString().ToLower();
                        break;
                }
            }
        }     
        /// <summary>
        /// Rebuilds the JSON saved by <c>JsonToDb</c> with <c>saveStructure</c> from the current account's row.
        /// Writes detailed progress to the project log.
        /// </summary>
        /// <param name="tableName">Table; default is <c>project.ProjectTable()</c> (<c>__</c> + project name).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <returns>
        /// The JSON text, or <c>{}</c> when there is no <c>_json_structure</c> or it cannot be parsed.
        /// </returns>
        public static string DbToJson(this IZennoPosterProjectModel project, string tableName = null, bool log = false, bool thrw = false)
        {
            if (string.IsNullOrEmpty(tableName)) 
                tableName = project.ProjectTable();
            
            project.SendInfoToLog("=== DbToJson START ===", true);
            
            var columns = project.TblColumns(tableName, log);
            project.SendInfoToLog($"Columns in table: {string.Join(", ", columns)}", true);
            
            if (!columns.Contains("_json_structure"))
            {
                project.SendErrorToLog("ОШИБКА: Колонка _json_structure отсутствует в таблице!", true);
                return "{}";
            }
            
            var columnsString = string.Join(",", columns);
            project.SendInfoToLog($"Fetching columns: {columnsString}", true);
            
            var allColumns = project.DbGetColumns(columnsString, tableName, log, thrw);
            project.SendInfoToLog($"Fetched {allColumns.Count} columns", true);
            
            foreach (var col in allColumns)
            {
                var preview = col.Value.Length > 100 ? col.Value.Substring(0, 100) + "..." : col.Value;
                project.SendInfoToLog($"  [{col.Key}] = {preview}", true);
            }
            
            if (!allColumns.ContainsKey("_json_structure"))
            {
                project.SendErrorToLog("ОШИБКА: _json_structure отсутствует в результате!", true);
                return "{}";
            }
            
            var structureJson = allColumns["_json_structure"];
            project.SendInfoToLog($"Structure JSON length: {structureJson.Length}", true);
            
            Dictionary<string, string> structure;
            try
            {
                structure = JsonConvert.DeserializeObject<Dictionary<string, string>>(structureJson);
                project.SendInfoToLog($"Structure parsed, elements: {structure.Count}", true);
            }
            catch (Exception ex)
            {
                project.SendErrorToLog($"Ошибка парсинга structure: {ex.Message}", true);
                return "{}";
            }
            
            // ИСПРАВЛЕНО: Удаляем ВСЕ служебные поля (начинающиеся с _)
            var keysToRemove = allColumns.Keys.Where(k => k.StartsWith("_") || k == "id").ToList();
            foreach (var key in keysToRemove)
            {
                project.SendInfoToLog($"Removing service field: {key}", true);
                allColumns.Remove(key);
            }
            
            project.SendInfoToLog($"Data fields for building: {allColumns.Count}", true);
            
            var result = BuildJson(project, allColumns, structure);
            
            project.SendInfoToLog($"=== Result ===\n{result}", true);
            
            return result;
        }
        private static string BuildJson(IZennoPosterProjectModel project, Dictionary<string, string> data, Dictionary<string, string> structure)
        {
            
            var root = new JObject();
            
            foreach (var kvp in data)
            {
                if (!structure.ContainsKey(kvp.Key))
                {
                    project.SendInfoToLog($"  SKIP (not in structure): {kvp.Key}");
                    continue;
                }
                
                var type = structure[kvp.Key];
                
                if (type == "object" || type == "array")
                {
                    continue;
                }
                
                project.SendInfoToLog($"  Processing: {kvp.Key}");
                
                var path = kvp.Key.Split('_');
                JToken current = root;
                
                var containers = new List<(int segmentCount, string path, bool isArray)>();
                
                for (int i = 1; i < path.Length; i++)
                {
                    var testPath = string.Join("_", path.Take(i));
                    if (structure.ContainsKey(testPath))
                    {
                        var testType = structure[testPath];
                        if (testType == "object" || testType == "array")
                        {
                            containers.Add((i, testPath, testType == "array"));
                            project.SendInfoToLog($"    Found container: {testPath} ({testType})");
                        }
                    }
                }
                
                int lastContainerDepth = 0;
                foreach (var (segmentCount, containerPath, isArray) in containers)
                {
                    var containerSegments = containerPath.Split('_');
                    var containerKey = string.Join("_", containerSegments.Skip(lastContainerDepth));
                    
                    if (current is JObject jObj)
                    {
                        if (jObj[containerKey] == null)
                        {
                            jObj[containerKey] = isArray ? (JToken)new JArray() : (JToken)new JObject();
                            project.SendInfoToLog($"      Created: {containerKey} as {(isArray ? "array" : "object")}");
                        }
                        current = jObj[containerKey];
                    }
                    else if (current is JArray jArr && int.TryParse(containerKey, out int idx))
                    {
                        while (jArr.Count <= idx)
                        {
                            jArr.Add(isArray ? (JToken)new JArray() : (JToken)new JObject());
                        }
                        current = jArr[idx];
                    }
                    
                    lastContainerDepth = segmentCount;
                }
                
                var finalKey = string.Join("_", path.Skip(lastContainerDepth));
                project.SendInfoToLog($"    Final key: '{finalKey}'");
                
                var value = kvp.Value;
                
                // ИСПРАВЛЕНО: Парсим JSON только если тип НЕ string
                JToken token;
                if (type != "string" && IsJsonString(value))
                {
                    try
                    {
                        token = JToken.Parse(value);
                        project.SendInfoToLog($"    Parsed as JSON (type={type})");
                    }
                    catch
                    {
                        token = CreateTypedToken(project, value, kvp.Key, structure);
                    }
                }
                else
                {
                    token = CreateTypedToken(project, value, kvp.Key, structure);
                }
                
                if (current is JObject jObj2)
                {
                    jObj2[finalKey] = token;
                }
                else if (current is JArray jArr2)
                {
                    jArr2.Add(token);
                }
            }
            
            return root.ToString(Formatting.None);
        }
        private static bool IsJsonString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            
            var trimmed = value.Trim();
            return (trimmed.StartsWith("{") && trimmed.EndsWith("}")) ||
                   (trimmed.StartsWith("[") && trimmed.EndsWith("]"));
        }
                
        private static void BuildPath(IZennoPosterProjectModel project, JToken parent, string[] path, int index, string fullKey, 
            Dictionary<string, string> data, Dictionary<string, string> structure, HashSet<string> processed)
        {
            if (index >= path.Length)
            {
                project.SendInfoToLog($"      BuildPath: index >= path.Length, выход", true);
                return;
            }
            
            var segment = path[index];
            var isLast = index == path.Length - 1;
            
            project.SendInfoToLog($"      BuildPath: segment={segment}, index={index}/{path.Length}, isLast={isLast}, parent type={parent.Type}", true);
            
            if (isLast)
            {
                var value = data[fullKey];
                project.SendInfoToLog($"        LEAF: устанавливаю значение '{value}' для ключа '{fullKey}'", true);
                
                // НОВОЕ: проверяем, является ли значение JSON-строкой
                JToken tokenValue;
                if (IsJsonString(value))
                {
                    try
                    {
                        // Парсим JSON-строку в объект
                        tokenValue = JToken.Parse(value);
                        project.SendInfoToLog($"        Parsed JSON string into object/array", true);
                    }
                    catch
                    {
                        // Если не удалось распарсить, оставляем как строку
                        tokenValue = CreateTypedToken(project, value, fullKey, structure);
                    }
                }
                else
                {
                    tokenValue = CreateTypedToken(project, value, fullKey, structure);
                }
                
                project.SendInfoToLog($"        Token создан: type={tokenValue.Type}, value={tokenValue}", true);
                
                if (parent is JObject jObj)
                {
                    project.SendInfoToLog($"        Добавляю в JObject[{segment}]", true);
                    jObj[segment] = tokenValue;
                }
                else if (parent is JArray jArr)
                {
                    project.SendInfoToLog($"        Добавляю в JArray (count={jArr.Count})", true);
                    jArr.Add(tokenValue);
                }
                else
                {
                    project.SendErrorToLog($"        ОШИБКА: неизвестный тип parent: {parent.GetType()}", true);
                }
                
                processed.Add(fullKey);
            }
            else
            {
                var currentPath = string.Join("_", path.Take(index + 1));
                bool isArray = structure.ContainsKey(currentPath) && structure[currentPath] == "array";
                
                project.SendInfoToLog($"        NODE: currentPath={currentPath}, isArray={isArray}", true);
                
                JToken child = null;
                
                if (parent is JObject jObj)
                {
                    child = jObj[segment];
                    project.SendInfoToLog($"        Parent=JObject, child exists={child != null}", true);
                    
                    if (child == null)
                    {
                        child = isArray ? (JToken)new JArray() : (JToken)new JObject();
                        project.SendInfoToLog($"        Создаю новый child: {child.Type}", true);
                        jObj[segment] = child;
                    }
                }
                else if (parent is JArray jArr)
                {
                    project.SendInfoToLog($"        Parent=JArray, count={jArr.Count}, segment={segment}", true);
                    
                    if (int.TryParse(segment, out int idx))
                    {
                        project.SendInfoToLog($"        Segment - число: {idx}", true);
                        if (idx < jArr.Count)
                        {
                            child = jArr[idx];
                            project.SendInfoToLog($"        Взял существующий элемент [{idx}]", true);
                        }
                        else
                        {
                            child = isArray ? (JToken)new JArray() : (JToken)new JObject();
                            project.SendInfoToLog($"        Создаю новый элемент: {child.Type}", true);
                            jArr.Add(child);
                        }
                    }
                    else
                    {
                        project.SendErrorToLog($"        ОШИБКА: segment '{segment}' не число для массива!", true);
                        child = isArray ? (JToken)new JArray() : (JToken)new JObject();
                        jArr.Add(child);
                    }
                }
                else
                {
                    project.SendErrorToLog($"        ОШИБКА: неизвестный тип parent: {parent.GetType()}", true);
                    return;
                }
                
                if (child != null)
                {
                    BuildPath(project, child, path, index + 1, fullKey, data, structure, processed);
                }
                else
                {
                    project.SendErrorToLog($"        ОШИБКА: child == null после обработки!", true);
                }
            }
        }
        
        private static JToken CreateTypedToken(IZennoPosterProjectModel project, string value, string fullKey, Dictionary<string, string> structure)
        {
            if (structure.ContainsKey(fullKey))
            {
                var type = structure[fullKey];
                project.SendInfoToLog($"          CreateToken: key={fullKey}, type={type}, value={value}", true);
                
                switch (type)
                {
                    case "integer":
                        if (int.TryParse(value, out int intVal))
                        {
                            project.SendInfoToLog($"          -> Integer: {intVal}", true);
                            return new JValue(intVal);
                        }
                        break;
                    case "float":
                        if (double.TryParse(value, out double dblVal))
                        {
                            project.SendInfoToLog($"          -> Float: {dblVal}", true);
                            return new JValue(dblVal);
                        }
                        break;
                    case "boolean":
                        if (bool.TryParse(value, out bool boolVal))
                        {
                            project.SendInfoToLog($"          -> Boolean: {boolVal}", true);
                            return new JValue(boolVal);
                        }
                        break;
                    case "null":
                        project.SendInfoToLog($"          -> Null", true);
                        return JValue.CreateNull();
                }
            }
            
            project.SendInfoToLog($"          CreateToken: key={fullKey} -> String: {value}", true);
            return new JValue(value);
        }
        
    }

    /// <summary>Operations on whole rows.</summary>
    public static class DbLine
    {
        /// <summary>Sets every column except <c>id</c> to an empty string in one row.</summary>
        /// <param name="id">Row id.</param>
        /// <param name="tableName">Table; default is <c>project.ProjectTable()</c> (<c>__</c> + project name).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        public static void DbClearLine(this IZennoPosterProjectModel project, int id, string tableName = null, bool log = false, bool thrw = false)
        {
            tableName = project.TableName(tableName);
            var quotedTable = DbHelpers.Quote(tableName);
            
            var columns = project.TblColumns(tableName, log);
            var columnsToClean = columns.Where(col => col.ToLower() != "id").ToList();
            
            if (columnsToClean.Count == 0)
            {
                if (log) project.SendInfoToLog($"No columns to clear in table {tableName} (only 'id' column exists)", true);
                return;
            }
            
            var setClause = string.Join(", ", columnsToClean.Select(col => $"{DbHelpers.Quote(col)} = ''"));
            var updateQuery = $"UPDATE {quotedTable} SET {setClause} WHERE {DbHelpers.Quote("id")} = {id}";
            
            project.DbQ(updateQuery, log, thrw: thrw);
            
            if (log) project.SendInfoToLog($"Cleared {columnsToClean.Count} columns in row with id={id} in table {tableName}", true);
        }
        
        /// <summary>Exchanges the values of all columns except <c>id</c> between two rows.</summary>
        /// <param name="id1">First row id.</param>
        /// <param name="id2">Second row id.</param>
        /// <param name="tableName">Table; default is <c>project.ProjectTable()</c> (<c>__</c> + project name).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw when a row is not found; otherwise nothing changes.</param>
        public static void DbSwapLines(this IZennoPosterProjectModel project, int id1, int id2, string tableName = null, bool log = false, bool thrw = false)
        {
            tableName = project.TableName(tableName);
            var quotedTable = DbHelpers.Quote(tableName);
            
            var columns = project.TblColumns(tableName, log);
            var columnsToSwap = columns.Where(col => col.ToLower() != "id").ToList();
            
            if (columnsToSwap.Count == 0)
            {
                if (log) project.SendInfoToLog($"No columns to swap in table {tableName} (only 'id' column exists)", true);
                return;
            }
            
            var columnsString = string.Join(", ", columnsToSwap);
            var data1 = project.DbGetColumns(columnsString, tableName, log, thrw, "id", id1);
            var data2 = project.DbGetColumns(columnsString, tableName, log, thrw, "id", id2);
            
            if (data1 == null || data1.Count == 0)
            {
                var msg = $"Row with id={id1} not found in table {tableName}";
                if (log) project.SendWarningToLog(msg, true);
                if (thrw) throw new Exception(msg);
                return;
            }
            
            if (data2 == null || data2.Count == 0)
            {
                var msg = $"Row with id={id2} not found in table {tableName}";
                if (log) project.SendWarningToLog(msg, true);
                if (thrw) throw new Exception(msg);
                return;
            }
            
            var setClause1 = string.Join(", ", data2.Select(kvp => $"{DbHelpers.Quote(kvp.Key)} = '{kvp.Value.Replace("'", "''")}'"));
            var setClause2 = string.Join(", ", data1.Select(kvp => $"{DbHelpers.Quote(kvp.Key)} = '{kvp.Value.Replace("'", "''")}'"));
            
            var updateQuery1 = $"UPDATE {quotedTable} SET {setClause1} WHERE {DbHelpers.Quote("id")} = {id1}";
            var updateQuery2 = $"UPDATE {quotedTable} SET {setClause2} WHERE {DbHelpers.Quote("id")} = {id2}";
            
            project.DbQ(updateQuery1, log, thrw: thrw);
            project.DbQ(updateQuery2, log, thrw: thrw);
            
            if (log) project.SendInfoToLog($"Swapped data between rows id={id1} and id={id2} in table {tableName}", true);
        }
    }
    
    /// <summary>Lower-level SELECT and UPDATE helpers behind the <c>Db*</c> methods.</summary>
    public static class DbSql
    {
        /// <summary>
        /// Selects columns from the row where <c>key</c> = <c>id</c>, or from the rows matching <c>where</c>.
        /// </summary>
        /// <param name="toGet">Comma-separated column names; each is quoted.</param>
        /// <param name="tableName">Table; default is the project table (<c>projectTable</c> variable).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">
        /// Value of <c>key</c>; default is the current account, <c>acc0</c>. Inserted into the SQL as written.
        /// </param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        /// <returns>Columns joined by <c>¦</c>, rows by <c>·</c>.</returns>
        public static string SqlGet(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
        {
            if (string.IsNullOrWhiteSpace(toGet))
                throw new ArgumentException("Column names cannot be null or empty", nameof(toGet));

            toGet = DbHelpers.QuoteSelectColumns(toGet.Trim().TrimEnd(','));
            if (string.IsNullOrEmpty(tableName)) 
                tableName = project.Variables["projectTable"].Value;
            
            if (id is null) id = project.Variables["acc0"].Value;

            string query;
            if (string.IsNullOrEmpty(where))
            {
                if (string.IsNullOrEmpty(id.ToString())) 
                    throw new ArgumentException("variable \"acc0\" is null or empty", nameof(id));
                query = $"SELECT {toGet} from {DbHelpers.Quote(tableName)} WHERE {DbHelpers.Quote(key)} = {id}";
            }
            else
            {
                query = $@"SELECT {toGet} from {DbHelpers.Quote(tableName)} WHERE {where};";
            }

            return project.DbQ(query, log: log, thrw: thrw);
        }
        
        /// <summary>Like <c>SqlGet</c>, returning the first row as column → value.</summary>
        /// <param name="toGet">Comma-separated column names; each is quoted.</param>
        /// <param name="tableName">Table; default is the project table (<c>projectTable</c> variable).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">
        /// Value of <c>key</c>; default is the current account, <c>acc0</c>. Inserted into the SQL as written.
        /// </param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        /// <param name="set">Also set project variables named after the columns.</param>
        public static Dictionary<string, string> SqlGetDicFromLine(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "", bool set = false)
        {
            string result = project.SqlGet(toGet, tableName, log, thrw, key, id, where);
    
            if (string.IsNullOrWhiteSpace(result))
                return new Dictionary<string, string>();
           
            var columns = toGet.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim().Trim('`', '"', '[', ']'))
                .ToList();
            var values = result.Split(DbHelpers.ColumnSeparator);
            var dictionary = new Dictionary<string, string>();
    
            for (int i = 0; i < columns.Count && i < values.Length; i++)
            {
                dictionary[columns[i]] = values[i];
            }
            if (set) project.VarsFromDict(dictionary);
            return dictionary;
        }
        
        /// <summary>Like <c>SqlGet</c>, split into column values.</summary>
        /// <param name="toGet">Comma-separated column names; each is quoted.</param>
        /// <param name="tableName">Table; default is the project table (<c>projectTable</c> variable).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">
        /// Value of <c>key</c>; default is the current account, <c>acc0</c>. Inserted into the SQL as written.
        /// </param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        public static string[] SqlGetArrFromLine(this IZennoPosterProjectModel project, string toGet, string tableName = null,  bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
        {
            return project.SqlGet(toGet, tableName, log, thrw, key, id, where).Split(DbHelpers.ColumnSeparator);
        }
        
        /// <summary>Like <c>SqlGet</c>, split into rows.</summary>
        /// <param name="toGet">Comma-separated column names; each is quoted.</param>
        /// <param name="tableName">Table; default is the project table (<c>projectTable</c> variable).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">
        /// Value of <c>key</c>; default is the current account, <c>acc0</c>. Inserted into the SQL as written.
        /// </param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        public static List<string> SqlGetListFromLines(this IZennoPosterProjectModel project, string toGet, string tableName = null,  bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
        {
            return project.SqlGet(toGet, tableName, log, thrw, key, id, where).Split(DbHelpers.RawSeparator).ToList();
        }
        
        /// <summary>
        /// Runs <c>UPDATE … SET toUpd</c> for the row where <c>key</c> = <c>id</c>, or for the rows matching
        /// <c>where</c>.
        /// </summary>
        /// <param name="toUpd">Assignments; column names are quoted, values are taken as written.</param>
        /// <param name="tableName">Table; default is the project table (<c>projectTable</c> variable).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">
        /// Value of <c>key</c>; default is the current account, <c>acc0</c>. Inserted into the SQL as written.
        /// </param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        /// <returns>Affected row count as text.</returns>
        public static string SqlUpd(this IZennoPosterProjectModel project, string toUpd, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
        {          
            if (string.IsNullOrEmpty(tableName)) tableName = project.Var("projectTable");
            if (string.IsNullOrEmpty(tableName)) throw new Exception("TableName is null");
            
            toUpd = DbHelpers.QuoteColumns(toUpd);
            tableName = DbHelpers.Quote(tableName);
            
            if (id is null)
                id = project.Variables["acc0"].Value;
            
            string query;
            if (string.IsNullOrEmpty(where))
            {
                if (string.IsNullOrEmpty(id.ToString()))
                    throw new ArgumentException("variable \"acc0\" && \"where\" can't be empty", nameof(id));
                query = $"UPDATE {tableName} SET {toUpd} WHERE {DbHelpers.Quote(key)} = {id}";
            }
            else
            {
                query = $"UPDATE {tableName} SET {toUpd} WHERE {where}";
            }
            return project.DbQ(query, log:log, thrw: thrw);
        }
    }
    
    /// <summary>Creating and inspecting tables of the project database.</summary>
    public static class DbTable
    {
        
        /// <summary>Creates the table described by <c>schema</c> if it does not exist.</summary>
        /// <param name="schema">Table name and columns, e.g. <c>DbSchema.Process</c>.</param>
        public static void EnsureTable(this IZennoPosterProjectModel project, TableSchema schema)
        {
            project.TblAdd(schema.Columns, schema.Name);
        }
        
        /// <summary>
        /// Creates a table unless it exists. On PostgreSQL <c>INTEGER PRIMARY KEY AUTOINCREMENT</c> becomes
        /// <c>SERIAL PRIMARY KEY</c>.
        /// </summary>
        /// <param name="tableStructure">Column → SQL type.</param>
        /// <param name="tblName">Table; may be <c>schema.table</c>.</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        public static void TblAdd(this IZennoPosterProjectModel project,  Dictionary<string, string> tableStructure, string tblName, bool log = false)
        {
            if (project.TblExist(tblName, log:log)) return;

            tblName = DbHelpers.Quote(tblName);
            

            string query;
            if (project.DbMode() == "pgSQL")
                query = ($@"CREATE TABLE {tblName} ( {string.Join(", ", tableStructure.Select(kvp => $"\"{kvp.Key}\" {kvp.Value.Replace("INTEGER PRIMARY KEY AUTOINCREMENT", "SERIAL PRIMARY KEY")}"))} );");
            else
                query = ($"CREATE TABLE {tblName} (" + string.Join(", ", tableStructure.Select(kvp => $"{DbHelpers.Quote(kvp.Key)} {kvp.Value}")) + ");");
            project.DbQ(query, log: log);
        }
        
        /// <summary>Checks whether a table exists.</summary>
        /// <param name="tblName">Table; on PostgreSQL may be <c>schema.table</c> (default schema <c>public</c>).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        public static bool TblExist(this IZennoPosterProjectModel project, string tblName, bool log = false)
        {
            var schema = DbHelpers.Schema(tblName);
            tblName = DbHelpers.Table(tblName);
            string query;

            if (project.DbMode() == "pgSQL")
                query = ($"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '{schema}' AND table_name = '{tblName}';");
            else 
                query = ($"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tblName}';");

            string resp = project.DbQ(query, log);

            if (resp == "0" || resp == string.Empty) return false;
            else return true;
        }

        /// <summary>Names of all tables, sorted.</summary>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="schema">PostgreSQL schema.</param>
        public static List<string> TblList(this IZennoPosterProjectModel project, bool log = false, string schema = "public")
        {
            string query = (project.DbMode() == "pgSQL")
                ? $@"SELECT table_name FROM information_schema.tables WHERE table_schema = '{schema}' AND table_type = 'BASE TABLE' ORDER BY table_name;"
                : @"SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY name;";
            
            var result = project.DbQ(query, log: log)
                .Split(DbHelpers.RawSeparator)
                .Select(s => s.Trim())
                .ToList();
            return result;
        }
        
        /// <summary>Column names of a table.</summary>
        /// <param name="tblName">Table; on PostgreSQL may be <c>schema.table</c>.</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        public static List<string> TblColumns(this IZennoPosterProjectModel project, string tblName, bool log = false)
        {
            var result = new List<string>();
            var schema = DbHelpers.Schema(tblName);
            string query = (project.DbMode() == "pgSQL")
                ? $@"SELECT column_name FROM information_schema.columns WHERE table_schema = '{schema}' AND table_name = '{DbHelpers.Table(tblName)}';"
                : $"SELECT name FROM pragma_table_info('{DbHelpers.UnQuote(tblName)}');";

            result = project.DbQ(query, log: log)
                .Split(DbHelpers.RawSeparator)
                .Select(s => s.Trim())
                .ToList();
            return result;
        }
        
        /// <summary>
        /// Builds a table layout: <c>id INTEGER PRIMARY KEY AUTOINCREMENT</c>, the given columns, and one
        /// column per item of the comma-separated <c>cfgToDo</c> variable.
        /// </summary>
        /// <param name="projectColumns">Extra columns.</param>
        /// <param name="defaultType">SQL type of every column except <c>id</c>.</param>
        public static Dictionary<string, string> TblForProject(this IZennoPosterProjectModel project, List<string> projectColumns = null,  string defaultType = "TEXT DEFAULT ''")
        {
            string cfgToDo = project.Var("cfgToDo");
            var tableStructure = new Dictionary<string, string>
            {
                { "id", "INTEGER PRIMARY KEY AUTOINCREMENT" },
            };

            if (projectColumns != null)
            {
                foreach (string column in projectColumns)
                {
                    string trimmed = column.Trim();
                    if (!string.IsNullOrEmpty(trimmed) && !tableStructure.ContainsKey(trimmed))
                    {
                        tableStructure.Add(trimmed, defaultType);
                    }
                }
            }

            if (!string.IsNullOrEmpty(cfgToDo))
            {
                string[] toDoItems = (cfgToDo ?? "").Split(',');
                foreach (string taskId in toDoItems)
                {
                    string trimmedTaskId = taskId.Trim();
                    if (!string.IsNullOrEmpty(trimmedTaskId) && !tableStructure.ContainsKey(trimmedTaskId))
                    {
                        tableStructure.Add(trimmedTaskId, defaultType);
                    }
                }
            }
            return tableStructure;
        }
        
        /// <summary>
        /// Creates the project table (<c>projectTable</c> variable) with the <c>TblForProject</c> layout and
        /// adds missing columns.
        /// </summary>
        /// <param name="log">Write the query and its result to the project log.</param>
        public static void TblPrepareDefault(this IZennoPosterProjectModel project, bool log = false)
        {
            var tableStructure = project.TblForProject();
            var tblName = project.Var("projectTable");

            project.TblAdd(tableStructure, tblName, log: log);
            project.ClmnAdd(tableStructure, tblName, log: log);
            
        }
        
        /// <summary>Array form of <c>PrepareProjectTable(List&lt;string&gt;, …)</c>.</summary>
        public static void PrepareProjectTable(this IZennoPosterProjectModel project, string[] projectColumns, string tblName = null, bool log = false, bool prune = false, bool rearrange = false)
        {
            var projectColumnsList = projectColumns.ToList();
            project.PrepareProjectTable(projectColumnsList, tblName, log, prune, rearrange);
        }

        /// <summary>
        /// Creates the table with the <c>TblForProject</c> layout if needed and adds missing columns.
        /// </summary>
        /// <param name="projectColumns">Extra columns.</param>
        /// <param name="tblName">Table; default is the <c>projectTable</c> variable.</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="prune">Also drop columns that are not in the layout.</param>
        /// <param name="rearrange">Also reorder columns to match the layout.</param>
        public static void PrepareProjectTable(this IZennoPosterProjectModel project, List<string> projectColumns = null, string tblName = null, bool log = false, bool prune = false, bool rearrange = false)
        {
            var tableStructure = project.TblForProject(projectColumns);
            if (string.IsNullOrEmpty(tblName)) tblName = project.Var("projectTable");
            project.TblAdd(tableStructure, tblName, log: log);
            project.ClmnAdd(tableStructure, tblName, log: log);
            if (prune) project.ClmnPrune(tableStructure,tblName, log: log);
            if (rearrange) project.ClmnRearrange(tableStructure,tblName, log: log);
        }
        
        /// <summary>Creates the table if needed and adds missing columns.</summary>
        /// <param name="tableStructure">Column → SQL type.</param>
        /// <param name="tblName">Table; default is the <c>projectTable</c> variable.</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="prune">Also drop columns that are not in <c>tableStructure</c>.</param>
        /// <param name="rearrange">Also reorder columns to match <c>tableStructure</c>.</param>
        public static void PrepareProjectTable(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false, bool prune = false, bool rearrange = false)
        {
            if (string.IsNullOrEmpty(tblName))
                tblName = project.Var("projectTable");

            project.TblAdd(tableStructure, tblName, log: log);
            project.ClmnAdd(tableStructure, tblName, log: log);

            if (prune)
                project.ClmnPrune(tableStructure, tblName, log: log);

            if (rearrange)
                project.ClmnRearrange(tableStructure, tblName, log: log);
        }
        private static int TableCopy(this IZennoPosterProjectModel project, string sourceTable, string destinationTable, string sqLitePath = null, string pgHost = null, string pgPort = null, string pgDbName = null, string pgUser = null, string pgPass = null, bool thrw = false)
        {
            if (string.IsNullOrEmpty(sourceTable)) throw new ArgumentNullException(nameof(sourceTable));
            if (string.IsNullOrEmpty(destinationTable)) throw new ArgumentNullException(nameof(destinationTable));
            if (string.IsNullOrEmpty(sqLitePath)) sqLitePath = project.Var("DBsqltPath");
            if (string.IsNullOrEmpty(pgHost)) pgHost = "localhost";
            if (string.IsNullOrEmpty(pgPort)) pgPort = "5432";
            if (string.IsNullOrEmpty(pgDbName)) pgDbName = "postgres";
            if (string.IsNullOrEmpty(pgUser)) pgUser = "postgres";
            if (string.IsNullOrEmpty(pgPass)) pgPass = project.SecureVar("DBpstgrPass");
            string dbMode = project.DbMode();

            using (var db = dbMode == "pgSQL"
                       ? new Sql($"Host={pgHost};Port={pgPort};Database={pgDbName};Username={pgUser};Password={pgPass};Pooling=true;Connection Idle Lifetime=10;")
                       : new Sql(sqLitePath, null))
            {
                try
                {
                    return db.CopyTableAsync(sourceTable, destinationTable).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    project.SendWarningToLog(ex.Message, true);
                    if (thrw) throw;
                    return 0;
                }
            }
        }
        

        
    }
    
    /// <summary>Adding, dropping and reordering columns.</summary>
    public static class DbColumn
    {
        /// <summary>
        /// Checks whether a column exists. Case-insensitive on PostgreSQL, case-sensitive on SQLite.
        /// </summary>
        /// <param name="clmnName">Column.</param>
        /// <param name="tblName">Table.</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        public static bool ClmnExist(this IZennoPosterProjectModel project, string clmnName, string tblName, bool log = false)
        {
            var schema = DbHelpers.Schema(tblName);
            string query;

            if (project.DbMode() == "pgSQL")
                query = $@"SELECT COUNT(*) FROM information_schema.columns WHERE table_schema = '{schema}' AND table_name = '{DbHelpers.Table(tblName)}' AND lower(column_name) = lower('{DbHelpers.UnQuote(clmnName)}');";
            else
                query = $"SELECT COUNT(*) FROM pragma_table_info('{DbHelpers.UnQuote(tblName)}') WHERE name='{DbHelpers.UnQuote(clmnName)}';";
            string resp = project.DbQ(query, log);

            if (resp == "0" || resp == string.Empty) return false;
            else return true;
        }
        
        /// <summary>Adds a column if the table does not have it.</summary>
        /// <param name="clmnName">Column.</param>
        /// <param name="tblName">Table; default is <c>project.ProjectTable()</c> (<c>__</c> + project name).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="defaultValue">SQL type of the new column.</param>
        public static void ClmnAdd(this IZennoPosterProjectModel project, string clmnName, string tblName = null,  bool log = false, string defaultValue = "TEXT DEFAULT ''")
        {
            tblName =  project.TableName(tblName);
            var current = project.TblColumns(tblName, log: log);
            if (!current.Contains(clmnName))
            {
                clmnName = DbHelpers.Quote(clmnName);
                project.DbQ($@"ALTER TABLE {DbHelpers.Quote(tblName)} ADD COLUMN {clmnName} {defaultValue};", log: log);
            }
        }
        /// <summary>Adds each listed column that the table does not have.</summary>
        /// <param name="columns">Columns.</param>
        /// <param name="tblName">Table; default is <c>project.ProjectTable()</c> (<c>__</c> + project name).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="defaultValue">SQL type of the new columns.</param>
        public static void ClmnAdd(this IZennoPosterProjectModel project, List<string> columns, string tblName,  bool log = false, string defaultValue = "TEXT DEFAULT ''")
        {
            foreach (var column in columns)
                project.ClmnAdd(column, tblName, log:log, defaultValue: defaultValue);
        }      
        /// <summary>Adds each listed column that the table does not have.</summary>
        /// <param name="columns">Columns.</param>
        /// <param name="tblName">Table; default is <c>project.ProjectTable()</c> (<c>__</c> + project name).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="defaultValue">SQL type of the new columns.</param>
        public static void ClmnAdd(this IZennoPosterProjectModel project, string[] columns, string tblName,  bool log = false, string defaultValue = "TEXT DEFAULT ''")
        {
            foreach (var column in columns)
                project.ClmnAdd(column, tblName, log:log, defaultValue: defaultValue);
        }
        
        /// <summary>Adds each column of <c>tableStructure</c> that the table does not have, with its type.</summary>
        /// <param name="tableStructure">Column → SQL type.</param>
        /// <param name="tblName">Table; default is <c>project.ProjectTable()</c> (<c>__</c> + project name).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        public static void ClmnAdd(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null,  bool log = false)
        {
            tblName =  project.TableName(tblName);
            var current = project.TblColumns(tblName,log:log);
            foreach (var column in tableStructure)
            {
                var keyWd = column.Key.Trim();
                if (!current.Contains(keyWd))
                {
                    keyWd = DbHelpers.Quote(keyWd);
                    project.DbQ($@"ALTER TABLE {DbHelpers.Quote(tblName)} ADD COLUMN {keyWd} {column.Value};", log: log);
                }
            }
        }
        
        /// <summary>Column names of a table.</summary>
        /// <param name="tableName">Table; default is <c>project.ProjectTable()</c> (<c>__</c> + project name).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        public static List<string> ClmnList(this IZennoPosterProjectModel project, string tableName = null, bool log = false)
        {
            tableName =  project.TableName(tableName);
            var schema = DbHelpers.Schema(tableName);
            string Q = (project.DbMode() == "pgSQL")?
                $@"SELECT column_name FROM information_schema.columns WHERE table_schema = '{schema}' AND table_name = '{DbHelpers.Table(tableName)}'" :
                $@"SELECT name FROM pragma_table_info('{tableName}')";
            return project.DbQ(Q, log: log).Split(DbHelpers.RawSeparator).ToList();
        }
        
        /// <summary>Drops a column if it exists (<c>CASCADE</c> on PostgreSQL).</summary>
        /// <param name="clmnName">Column.</param>
        /// <param name="tblName">Table; default is <c>project.ProjectTable()</c> (<c>__</c> + project name).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        public static void ClmnDrop(this IZennoPosterProjectModel project, string clmnName, string tblName = null,  bool log = false)
        {
            tblName =  project.TableName(tblName);

            var current = project.TblColumns(tblName, log: log);

            if (current.Contains(clmnName))
            {
                clmnName = DbHelpers.Quote(clmnName);
                string cascade = (project.DbMode() == "pgSQL") ? " CASCADE" : null;
                project.DbQ($@"ALTER TABLE {DbHelpers.Quote(tblName)} DROP COLUMN {clmnName}{cascade};", log: log);
            }
        }
        
        /// <summary>Drops each column named by a key of <c>tableStructure</c> that the table has.</summary>
        /// <param name="tableStructure">Column → type; only the keys are used.</param>
        /// <param name="tblName">Table; default is <c>project.ProjectTable()</c> (<c>__</c> + project name).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        public static void ClmnDrop(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null,  bool log = false)
        {
            tblName =  project.TableName(tblName);
            var current = project.TblColumns(tblName, log: log);
            foreach (var column in tableStructure)
            {
                if (current.Contains(column.Key))
                {
                    string clmnName = DbHelpers.Quote(column.Key);
                    string cascade = (project.DbMode() == "pgSQL") ? " CASCADE" : null;
                    project.DbQ($@"ALTER TABLE {DbHelpers.Quote(tblName)} DROP COLUMN {clmnName}{cascade};", log: log);
                }
            }
        }
        
        /// <summary>Drops every column except <c>id</c> in which no row has a non-empty value.</summary>
        /// <param name="tblName">Table; default is <c>project.ProjectTable()</c> (<c>__</c> + project name).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        public static void ClmnPrune(this IZennoPosterProjectModel project, string tblName = null, bool log = false)
        {
            tblName =  project.TableName(tblName);
            var current = project.TblColumns(tblName, log: log);
    
            foreach (var column in current)
            {
                if (column.ToLower() == "id") continue;
                string quotedColumn = DbHelpers.Quote(column);
                string quotedTable = DbHelpers.Quote(tblName);
                
                string countQuery = $"SELECT COUNT(*) FROM {quotedTable} WHERE {quotedColumn} != '' AND {quotedColumn} IS NOT NULL";
                string result = project.DbQ(countQuery, log: log);
        
                int count = int.Parse(result);
        
                if (count == 0)
                {
                    project.ClmnDrop(column, tblName, log: log);
                }
            }
        }
        
        /// <summary>
        /// Drops every column that is not a key of <c>tableStructure</c> (including <c>id</c> if it is not
        /// listed).
        /// </summary>
        /// <param name="tableStructure">Columns to keep.</param>
        /// <param name="tblName">Table; default is <c>project.ProjectTable()</c> (<c>__</c> + project name).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        public static void ClmnPrune(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null,  bool log = false)
        {
            tblName =  project.TableName(tblName);
            var current = project.TblColumns(tblName, log: log);
            foreach (var column in current)
            {
                if (!tableStructure.ContainsKey(column))
                {
                    project.ClmnDrop(column,tblName, log: log);
                }
            }
        }
        
        /// <summary>
        /// Reorders columns: <c>id</c> first, then the columns of <c>tableStructure</c> that exist, then the
        /// rest.
        /// Copies the data into a new table, drops the old one and renames the new one. On failure the
        /// temporary table is dropped and an exception is thrown.
        /// </summary>
        /// <param name="tableStructure">Desired order.</param>
        /// <param name="tblName">Table; default is <c>project.ProjectTable()</c> (<c>__</c> + project name).</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        public static void ClmnRearrange(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false)
        {
            tblName =  project.TableName(tblName);
            
            bool _pstgr = (project.DbMode() == "pgSQL");
            string quotedTable = DbHelpers.Quote(tblName);
            string tempTable = DbHelpers.Quote($"{tblName}_temp_{DateTime.Now.Ticks}");
            
            try
            {
                var currentColumns = project.TblColumns(tblName, log: log);
                var idType = GetIdType(project, tblName, _pstgr, log);
                var newTableStructure = BuildNewStructure(project, tableStructure, currentColumns, idType, tblName, _pstgr, log);
                
                CreateTempTable(project, tempTable, newTableStructure, _pstgr, log);
                CopyDataToTemp(project, quotedTable, tempTable, newTableStructure, log);
                DropOldTable(project, quotedTable, log);
                RenameTempTable(project, tempTable, quotedTable, tblName, _pstgr, log);
                
                if (log)
                {
                    project.SendInfoToLog($"Table {tblName} rearranged successfully. New column order: {string.Join(", ", newTableStructure.Keys)}", true);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    project.DbQ($"DROP TABLE IF EXISTS {tempTable}", log: false, unSafe: true);
                }
                catch { }
                
                throw new Exception($"Failed to rearrange table {tblName}: {ex.Message}", ex);
            }
            finally
            {
                // Гарантированная очистка
                try
                {
                    var tempExists = _pstgr 
                        ? project.DbQ($"SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_schema = '{DbHelpers.Schema(tempTable)}' AND table_name = '{DbHelpers.Table(tempTable)}')", log: false)
                        : project.DbQ($"SELECT name FROM sqlite_master WHERE type='table' AND name='{DbHelpers.UnQuote(tempTable)}'", log: false);
                
                    if (!string.IsNullOrEmpty(tempExists) && tempExists != "0" && tempExists.ToLower() != "false")
                    {
                        project.DbQ($"DROP TABLE {tempTable}", log: false, unSafe: true);
                    }
                }
                catch { }
            }
        }
        
        /// <summary>Reorders columns to the <c>TblForProject(projectColumns)</c> layout.</summary>
        public static void ClmnRearrange(this IZennoPosterProjectModel project, List<string> projectColumns, string tblName = null, bool log = false)
        {
            var tableStructure = project.TblForProject(projectColumns);
            project.ClmnRearrange(tableStructure, tblName, log);
        }
        #region _ClmnRearrange_private
        private static string GetIdType(IZennoPosterProjectModel project, string tblName, bool _pstgr, bool log)
        {
            string idType = "INTEGER PRIMARY KEY";
            var schema = DbHelpers.Schema(tblName);
            
            if (_pstgr)
            {
                string getIdTypeQuery = $@"
                    SELECT data_type, is_identity 
                    FROM information_schema.columns 
                    WHERE table_schema = '{schema}' 
                    AND table_name = '{DbHelpers.Table(tblName)}' 
                    AND column_name = 'id'";
                
                var idInfo = project.DbQ(getIdTypeQuery, log: log);
                if (!string.IsNullOrEmpty(idInfo))
                {
                    if (idInfo.Contains("character") || idInfo.Contains("text"))
                        idType = "TEXT PRIMARY KEY";
                    else if (idInfo.Contains("integer"))
                        idType = "SERIAL PRIMARY KEY";
                }
            }
            else
            {
                string getIdTypeQuery = $"SELECT type FROM pragma_table_info('{DbHelpers.UnQuote(tblName)}') WHERE name='id'";
                var sqliteIdType = project.DbQ(getIdTypeQuery, log: log);
                if (!string.IsNullOrEmpty(sqliteIdType) && sqliteIdType.ToUpper().Contains("TEXT"))
                    idType = "TEXT PRIMARY KEY";
            }
            
            return idType;
        }
        
        private static Dictionary<string, string> BuildNewStructure(IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, List<string> currentColumns, string idType, string tblName, bool _pstgr, bool log)
        {
            var newTableStructure = new Dictionary<string, string>();
            newTableStructure.Add("id", idType);
            
            foreach (var col in tableStructure)
            {
                if (col.Key.ToLower() != "id" && currentColumns.Contains(col.Key))
                {
                    newTableStructure.Add(col.Key, col.Value);
                }
            }
            
            foreach (var col in currentColumns)
            {
                if (col.ToLower() != "id" && !newTableStructure.ContainsKey(col))
                {
                    string colType = GetColumnType(project, tblName, col, _pstgr, log);
                    newTableStructure.Add(col, colType);
                }
            }
            
            return newTableStructure;
        }
        
        private static string GetColumnType(IZennoPosterProjectModel project, string tblName, string col, bool _pstgr, bool log)
        {
            string colType = "TEXT DEFAULT ''";
            var schema = DbHelpers.Schema(tblName);
            
            if (_pstgr)
            {
                string getTypeQuery = $@"
                    SELECT data_type, character_maximum_length, column_default
                    FROM information_schema.columns 
                    WHERE table_schema = '{schema}' 
                    AND table_name = '{DbHelpers.Table(tblName)}' 
                    AND column_name = '{col}'";
                
                var typeInfo = project.DbQ(getTypeQuery, log: log);
                if (!string.IsNullOrEmpty(typeInfo))
                {
                    if (typeInfo.Contains("integer")) colType = "INTEGER";
                    else if (typeInfo.Contains("text") || typeInfo.Contains("character")) colType = "TEXT";
                    else if (typeInfo.Contains("timestamp")) colType = "TIMESTAMP";
                    else if (typeInfo.Contains("boolean")) colType = "BOOLEAN";
                    
                    if (typeInfo.Contains("''::")) colType += " DEFAULT ''";
                }
            }
            else
            {
                string getTypeQuery = $"SELECT type FROM pragma_table_info('{DbHelpers.UnQuote(tblName)}') WHERE name='{col}'";
                var sqliteType = project.DbQ(getTypeQuery, log: log);
                if (!string.IsNullOrEmpty(sqliteType))
                    colType = sqliteType;
            }
            
            return colType;
        }
        
        private static void CreateTempTable(IZennoPosterProjectModel project, string tempTable, Dictionary<string, string> newTableStructure, bool _pstgr, bool log)
        {
            string createTempTableQuery;
            if (_pstgr)
            {
                createTempTableQuery = $@"CREATE TABLE {tempTable} ( 
                    {string.Join(", ", newTableStructure.Select(kvp => $"{DbHelpers.Quote(kvp.Key)} {kvp.Value.Replace("AUTOINCREMENT", "SERIAL")}"))} )";
            }
            else
            {
                createTempTableQuery = $@"CREATE TABLE {tempTable} ( 
                    {string.Join(", ", newTableStructure.Select(kvp => $"{DbHelpers.Quote(kvp.Key)} {kvp.Value}"))} )";
            }
            
            project.DbQ(createTempTableQuery, log: log);
        }
        
        private static void CopyDataToTemp(IZennoPosterProjectModel project, string quotedTable, string tempTable, Dictionary<string, string> newTableStructure, bool log)
        {
            var columnsList = string.Join(", ", newTableStructure.Keys.Select(k => DbHelpers.Quote(k)));
            string copyDataQuery = $@"
                INSERT INTO {tempTable} ({columnsList})
                SELECT {columnsList}
                FROM {quotedTable}";
            
            project.DbQ(copyDataQuery, log: log);
        }
        
        private static void DropOldTable(IZennoPosterProjectModel project, string quotedTable, bool log)
        {
            string dropOldTableQuery = $"DROP TABLE {quotedTable}";
            project.DbQ(dropOldTableQuery, log: log, unSafe: true);
        }
        
        private static void RenameTempTable(IZennoPosterProjectModel project, string tempTable, string quotedTable, string tblName, bool _pstgr, bool log)
        {
            string renameTableQuery;
            if (_pstgr)
            {
                renameTableQuery = $"ALTER TABLE {tempTable} RENAME TO {DbHelpers.Quote(DbHelpers.Table(tblName))}";
            }
            else
            {
                renameTableQuery = $"ALTER TABLE {tempTable} RENAME TO {DbHelpers.UnQuote(tblName)}";
            }
            
            project.DbQ(renameTableQuery, log: log);
        }
        #endregion
    }
    
    
    /// <summary>Filling a table with account rows.</summary>
    public static class DbRange
    {
        /// <summary>
        /// Inserts rows with ids from the current maximum + 1 up to <c>range</c>, in batches of 500. Existing
        /// ids are skipped.
        /// </summary>
        /// <param name="tblName">Table with an <c>id</c> column.</param>
        /// <param name="range">
        /// Highest id; 0 reads <c>rangeEnd</c>, and 10 is used with a warning when that is not a number.
        /// </param>
        /// <param name="log">Write the query and its result to the project log.</param>
        public static void AddRange(this IZennoPosterProjectModel project, string tblName, int range = 0, bool log = false)
        {
            tblName = DbHelpers.Quote(tblName);
            if (range == 0)
                try
                {
                    range = int.Parse(project.Variables["rangeEnd"].Value);
                }
                catch
                {
                    project.SendWarningToLog("var rangeEnd is empty or 0, used default \"10\"", true);                  
                    range = 10;
                }

            int current = int.Parse(project.DbQ($@"SELECT COALESCE(MAX(CAST({DbHelpers.Quote("id")} AS INTEGER)), 0) FROM {tblName};"));
    
            if (current >= range) return; 
    
            var values = new List<string>();
            for (int i = current + 1; i <= range; i++)
            {
                values.Add($"('{i}')");
            }
    
            if (values.Count > 0)
            {
                const int batchSize = 500;
                for (int i = 0; i < values.Count; i += batchSize)
                {
                    var batch = values.Skip(i).Take(batchSize);
                    var batchValues = string.Join(", ", batch);
                    project.DbQ($@"INSERT INTO {tblName} ({DbHelpers.Quote("id")}) VALUES {batchValues} ON CONFLICT DO NOTHING;", log: log);
                }
            }
        }
    }
    
    /// <summary>Copying tables inside the project database and between PostgreSQL and SQLite.</summary>
    public static class DbMigration
    {
        /// <summary>
        /// Copies <c>source</c> to a new table <c>dest</c> in the same database, then renames a column
        /// <c>acc0</c> or <c>key</c> to <c>id</c> if there is one.
        /// </summary>
        /// <param name="source">Existing table.</param>
        /// <param name="dest">Table to create.</param>
        /// <remarks>
        /// The database kind follows <c>dbSource</c>. PostgreSQL is reached at localhost:5432, database and
        /// user <c>postgres</c>, password from the <c>DBpstgrPass</c> variable; SQLite at the <c>DBsqltPath</c>
        /// variable.
        /// </remarks>
        public static void MigrateTable(this IZennoPosterProjectModel project, string source, string dest)
        {

            project.SendInfoToLog($"{source} -> {dest}", true);
            project.TableCopy(source, dest);
            try { project.DbQ($"ALTER TABLE {DbHelpers.Quote(dest)} RENAME COLUMN {DbHelpers.Quote("acc0")} to {DbHelpers.Quote("id")}"); } catch { }
            try { project.DbQ($"ALTER TABLE {DbHelpers.Quote(dest)} RENAME COLUMN {DbHelpers.Quote("key")} to {DbHelpers.Quote("id")}"); } catch { }
        }
        
        /// <summary>
        /// Copies every user table from the current database to the other kind: PostgreSQL → SQLite or SQLite →
        /// PostgreSQL (see <c>Sql.MigrateAllTablesAsync</c>). Errors are written to the log as a warning.
        /// </summary>
        /// <remarks>
        /// PostgreSQL is reached at localhost:5432, database and user <c>postgres</c>, password from the
        /// <c>DBpstgrPass</c> variable; SQLite at the <c>DBsqltPath</c> variable.
        /// </remarks>
        public static void MigrateAllTables(this IZennoPosterProjectModel project)
        {
            string dbMode = project.DbMode() ;
            if (dbMode != "pgSQL" && dbMode != "SQLite") throw new ArgumentException("DBmode must be 'PostgreSQL' or 'SQLite'");

            string direction = dbMode == "PostgreSQL" ? "toSQLite" : "toPostgreSQL";

            string sqLitePath = project.Var("DBsqltPath");
            string pgHost = "localhost";
            string pgPort = "5432";
            string pgDbName = "postgres";
            string pgUser = "postgres";
            string pgPass = project.Var("DBpstgrPass");

            string pgConnection = $"Host={pgHost};Port={pgPort};Database={pgDbName};Username={pgUser};Password={pgPass};Pooling=true;Connection Idle Lifetime=10;";

            project.SendInfoToLog($"Migrating all tables from {dbMode} to {(direction == "toSQLite" ? "SQLite" : "PostgreSQL")}", true);

            using (var sourceDb = (project.DbMode() == "pgSQL") ? new Sql(pgConnection) : new Sql(sqLitePath, null))
            using (var destinationDb = (project.DbMode() == "pgSQL") ? new Sql(sqLitePath, null) : new Sql(pgConnection))
            {
                try
                {
                    int rowsMigrated = Sql.MigrateAllTablesAsync(sourceDb, destinationDb).GetAwaiter().GetResult();
                    project.SendInfoToLog($"Successfully migrated {rowsMigrated} rows", true);
                }
                catch (Exception ex)
                {
                    project.SendWarningToLog($"Error during migration: {ex.Message}", true);
                }
            }
        }
        
        private static int TableCopy(this IZennoPosterProjectModel project, string sourceTable, string destinationTable, string sqLitePath = null, string pgHost = null, string pgPort = null, string pgDbName = null, string pgUser = null, string pgPass = null, bool thrw = false)
        {
            if (string.IsNullOrEmpty(sourceTable)) throw new ArgumentNullException(nameof(sourceTable));
            if (string.IsNullOrEmpty(destinationTable)) throw new ArgumentNullException(nameof(destinationTable));
            if (string.IsNullOrEmpty(sqLitePath)) sqLitePath = project.Var("DBsqltPath");
            if (string.IsNullOrEmpty(pgHost)) pgHost = "localhost";
            if (string.IsNullOrEmpty(pgPort)) pgPort = "5432";
            if (string.IsNullOrEmpty(pgDbName)) pgDbName = "postgres";
            if (string.IsNullOrEmpty(pgUser)) pgUser = "postgres";
            if (string.IsNullOrEmpty(pgPass)) pgPass = project.Var("DBpstgrPass");
            string dbMode = project.Var("DBmode");

            using (var db = (project.DbMode() == "pgSQL")
                       ? new Sql($"Host={pgHost};Port={pgPort};Database={pgDbName};Username={pgUser};Password={pgPass};Pooling=true;Connection Idle Lifetime=10;")
                       : new Sql(sqLitePath, null))
            {
                try
                {
                    return db.CopyTableAsync(sourceTable, destinationTable).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    project.SendWarningToLog(ex.Message, true);
                    if (thrw) throw;
                    return 0;
                }
            }
        }
    }
    
    
    /// <summary>Single entry point that runs SQL against the project database.</summary>
    public static class DbCore
    {
        private static void Log(this IZennoPosterProjectModel project, string query)
        {
            var color = LogColor.Default;
            var type = LogType.Info;
            if (query.Contains("error"))
            {
                color = LogColor.Orange;
                type = LogType.Error;
            }
            else if (query.Contains("success"))
            {
                color = LogColor.Blue;
            }
            else if (query.Contains("failed"))
            {
                color = LogColor.Yellow;
            }
            project.SendToLog(query,type,true,color);
        }
        

        /// <summary>
        /// Executes one SQL statement against the database named by <c>dbSource</c> (project variable, else
        /// global variable).
        /// A <c>dbSource</c> starting with <c>Host=</c> is a PostgreSQL connection string; anything else
        /// selects SQLite. On SQLite a "database is locked" error is retried up to 10 times with a growing
        /// pause.
        /// </summary>
        /// <param name="query">SQL text.</param>
        /// <param name="log">Write the query and its result to the project log.</param>
        /// <param name="sqLitePath">SQLite database file. Only this argument is used as the SQLite path.</param>
        /// <param name="thrw">Throw on a database error instead of logging a warning and returning an empty result.</param>
        /// <param name="unSafe">Not used.</param>
        /// <returns>
        /// For <c>SELECT</c>: rows joined by <c>·</c>, columns by <c>¦</c>. Otherwise the affected row count as
        /// text. An empty string after an error when <c>thrw</c> is false.
        /// </returns>
        /// <remarks>Throws when <c>dbSource</c> is not set.</remarks>
        public static string DbQ(this IZennoPosterProjectModel project, string query, bool log = false, string sqLitePath = null,  bool thrw = false, bool unSafe = false)
        {
            
            string dbSource = project.Var("dbSource"); 
            if (string.IsNullOrEmpty(dbSource)) 
                dbSource = project.GVar("dbSource");
            if (string.IsNullOrEmpty(dbSource)) 
                project.warn("dbSource undefined", thrw: true);
            
            var dbMode = dbSource.StartsWith("Host=") ? "pgSQL" : "SQLite";
            
            string result = string.Empty;
            int maxRetries = 10;
            int delay = 100;     
            Random rnd = new Random();
            using (var db = dbMode == "pgSQL" ? new Sql(dbSource) : new Sql(sqLitePath, null))
            {
                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        if (Regex.IsMatch(query.TrimStart(), @"^\s*SELECT\b", RegexOptions.IgnoreCase))
                            result = db.DbReadAsync(query, "¦", "·").GetAwaiter().GetResult();
                        else
                            result = db.DbWriteAsync(query).GetAwaiter().GetResult().ToString();

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (dbMode == "SQLite" && ex.Message.Contains("locked") && i < maxRetries - 1)
                        {
                            delay = 50 * (1 << i) + rnd.Next(10, 50);
                            Thread.Sleep(delay);
                            continue; 
                        }

                        project.warn(ex.Message + $"\n [{query}]", thrw: thrw);
                        return string.Empty;
                    }
                }
            } 
            string toLog = (query.Contains("SELECT")) ? $"[{query}]\n[{result}]" : $"[{query}] - [{result}]";
            if (log) project.Log("[SQL] " + toLog);
            return result;
            
        }

        internal static string DbMode(this IZennoPosterProjectModel project)
        {
            string dbSource = project.Var("dbSource");
            if (string.IsNullOrEmpty(dbSource)) 
                dbSource = project.GVar("dbSource");
            if (string.IsNullOrEmpty(dbSource)) 
                project.warn("dbSource undefined", thrw: true);
            
            var dbMode = dbSource.StartsWith("Host=") ? "pgSQL" : "SQLite";
            return dbMode;
        }
    }
    
}
