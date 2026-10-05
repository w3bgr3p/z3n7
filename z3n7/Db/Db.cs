using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace z3n7
{
    /// <summary>
    /// SQL helper over PostgreSQL or SQLite with one API for both.
    /// Every statement opens its own connection. A <c>SELECT</c> returns rows joined by <c>·</c> and
    /// columns joined by <c>¦</c>; other statements return the number of affected rows. On SQLite a
    /// "database is locked" error is retried up to 10 times with a growing pause.
    /// </summary>
    /// <remarks>
    /// SQLite is reached through the SQLite3 ODBC driver, which must be installed. Values passed as
    /// <c>id</c> or <c>where</c> are inserted into SQL as written.
    /// </remarks>
    public partial class Db
    {
        private readonly string _dbMode;
        private readonly string _sqLitePath;
        private readonly string _pgHost;
        private readonly string _pgPort;
        private readonly string _pgDbName;
        private readonly string _pgUser;
        private readonly string _pgPass;
        private readonly string _defaultTable;
        private readonly Logger _log;
        
        
        private const char RawSeparator = '·';
        private const char ColumnSeparator = '¦';
        private const string SchemaName = "public";

        /// <summary>Creates a database helper with explicit connection settings.</summary>
        /// <param name="dbMode"><c>pgSQL</c> for PostgreSQL; any other value uses SQLite.</param>
        /// <param name="sqLitePath">SQLite database file.</param>
        /// <param name="pgHost">PostgreSQL host.</param>
        /// <param name="pgPort">PostgreSQL port.</param>
        /// <param name="pgDbName">PostgreSQL database.</param>
        /// <param name="pgUser">PostgreSQL user.</param>
        /// <param name="pgPass">PostgreSQL password.</param>
        /// <param name="defaultTable">Table used when a method gets no table name.</param>
        /// <param name="logLevel">Level of the internal logger; queries and results are logged at <c>Info</c>.</param>
        public Db(string dbMode = "pgSQL", string sqLitePath = null,
            string pgHost = "localhost", string pgPort = "5432", string pgDbName = "postgres",
            string pgUser = "postgres", string pgPass = "",
            string defaultTable = null, LogLevel logLevel = LogLevel.Off)
        {
            _dbMode = dbMode;
            _sqLitePath = sqLitePath;
            _pgHost = pgHost;
            _pgPort = pgPort;
            _pgDbName = pgDbName;
            _pgUser = pgUser;
            _pgPass = pgPass;
            _defaultTable = defaultTable;
            _log =  new Logger(logLevel: logLevel);
        }
        
        #region Core Query
        /// <summary>Executes one SQL statement.</summary>
        /// <param name="query">SQL text.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        /// <param name="unSafe">Not used.</param>
        /// <returns>
        /// For <c>SELECT</c>: rows joined by <c>·</c>, columns by <c>¦</c>. Otherwise the affected row count as
        /// text. An empty string after an error when <c>thrw</c> is false.
        /// </returns>
        public string Query(string query, bool log = false, bool thrw = false, bool unSafe = false)
        {
            string result = string.Empty;
            int maxRetries = 10;
            int delay = 100;
            Random rnd = new Random();

            using (var db = _dbMode == "pgSQL"
                       ? new Sql($"Host={_pgHost};Port={_pgPort};Database={_pgDbName};Username={_pgUser};Password={_pgPass};Pooling=true;Connection Idle Lifetime=10;")
                       : new Sql(_sqLitePath, null))
            {
                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        if (Regex.IsMatch(query.TrimStart(), @"^\s*SELECT\b", RegexOptions.IgnoreCase))
                            result = db.DbReadAsync(query, ColumnSeparator.ToString(), RawSeparator.ToString()).GetAwaiter().GetResult();
                        else
                            result = db.DbWriteAsync(query).GetAwaiter().GetResult().ToString();

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (_dbMode == "SQLite" && ex.Message.Contains("locked") && i < maxRetries - 1)
                        {
                            delay = 50 * (1 << i) + rnd.Next(10, 50);
                            Thread.Sleep(delay);
                            continue;
                        }

                        _log.Send($"Database Error: {ex.Message}\n[{query}]");
                        if (thrw) throw;
                        return string.Empty;
                    }
                }
            }
            
            string toLog = query.Contains("SELECT") ? $"[{query}]\n[{result}]" : $"[{query}] - [{result}]";
            _log.Send($"[{(_dbMode == "pgSQL" ? "🐘" : "SQLite")}] {toLog}", show: log);
            return result;
        }
        #endregion

        #region Get Methods
        /// <summary>
        /// Selects columns from the row where <c>key</c> = <c>id</c>, or from the rows matching <c>where</c>.
        /// </summary>
        /// <param name="columns">Comma-separated column names; each is quoted.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">Value of <c>key</c>, inserted into the SQL as written: quote text values yourself.</param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        /// <returns>Raw result in the <c>Query</c> format.</returns>
        public string Get(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
        {
            if (string.IsNullOrWhiteSpace(columns))
                throw new ArgumentException("Column names cannot be null or empty", nameof(columns));

            columns = QuoteSelectColumns(columns.Trim().TrimEnd(','));
            tableName = tableName ?? _defaultTable;
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name must be provided");

            string query;
            if (string.IsNullOrEmpty(where))
            {
                if (id == null || string.IsNullOrEmpty(id.ToString()))
                    throw new ArgumentException("ID must be provided when where clause is empty");
                query = $"SELECT {columns} FROM {Quote(tableName)} WHERE {Quote(key)} = {id}";
            }
            else
            {
                query = $"SELECT {columns} FROM {Quote(tableName)} WHERE {where}";
            }

            return Query(query, log, thrw);
        }

        /// <summary>Like <c>Get</c>, but returns the first row as column → value.</summary>
        /// <param name="columns">Comma-separated column names; each is quoted.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">Value of <c>key</c>, inserted into the SQL as written: quote text values yourself.</param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        /// <returns>An empty dictionary when nothing was found.</returns>
        public Dictionary<string, string> GetColumns(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
        {
            string result = Get(columns, tableName, log, thrw, key, id, where);

            if (string.IsNullOrWhiteSpace(result))
                return new Dictionary<string, string>();

            var columnList = columns.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim().Trim('`', '"', '[', ']'))
                .ToList();
            var values = result.Split(ColumnSeparator);
            var dictionary = new Dictionary<string, string>();

            for (int i = 0; i < columnList.Count && i < values.Length; i++)
            {
                dictionary[columnList[i]] = values[i];
            }

            return dictionary;
        }

        /// <summary>Like <c>Get</c>, split into column values.</summary>
        /// <param name="columns">Comma-separated column names; each is quoted.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">Value of <c>key</c>, inserted into the SQL as written: quote text values yourself.</param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        public string[] GetLine(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
        {
            return Get(columns, tableName, log, thrw, key, id, where).Split(ColumnSeparator);
        }

        /// <summary>Like <c>Get</c>, split into rows. Each row still has its columns joined by <c>¦</c>.</summary>
        /// <param name="columns">Comma-separated column names; each is quoted.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">Value of <c>key</c>, inserted into the SQL as written: quote text values yourself.</param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        public List<string> GetLines(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
        {
            return Get(columns, tableName, log, thrw, key, id, where).Split(RawSeparator).ToList();
        }

        /// <summary>Selects <c>column</c> from random rows where it is not empty.</summary>
        /// <param name="column">Column to read.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        /// <param name="maxId">When above 0, only rows with <c>id</c> below it.</param>
        /// <param name="includeId">Prefix the result with the <c>id</c> column.</param>
        /// <param name="single">Return one row instead of all matching rows in random order.</param>
        /// <param name="invertEmpty">Select rows where the column is empty instead.</param>
        public string GetRandom(string column, string tableName = null, bool log = false, bool thrw = false, int maxId = 0, bool includeId = false, bool single = true, bool invertEmpty = false)
        {
            tableName = tableName ?? _defaultTable;
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name must be provided");

            string idColumn = includeId ? "id, " : "";
            string emptyCondition = invertEmpty ? "=" : "!=";
            
            string query = $@"
                SELECT {idColumn}{column.Trim().TrimEnd(',')} 
                FROM {Quote(tableName)} 
                WHERE TRIM({column}) {emptyCondition} ''";
            
            if (maxId > 0)
                query += $" AND id < {maxId}";
            
            query += " ORDER BY RANDOM()";
            
            if (single)
                query += " LIMIT 1";

            return Query(query, log, thrw);
        }
        
        
        #endregion

        #region Update Methods
        /// <summary>
        /// Runs <c>UPDATE … SET setClause</c> for the row where <c>key</c> = <c>id</c>, or for the rows
        /// matching <c>where</c>.
        /// </summary>
        /// <param name="setClause">
        /// Assignments such as <c>status = 'ok', note = ''</c>; column names are quoted, values are taken as
        /// written.
        /// </param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">Value of <c>key</c>, inserted into the SQL as written: quote text values yourself.</param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        public void Upd(string setClause, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
        {
            tableName = tableName ?? _defaultTable;
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name must be provided");

            setClause = QuoteColumns(setClause);
            string quotedTable = Quote(tableName);

            string query;
            if (string.IsNullOrEmpty(where))
            {
                if (id == null || string.IsNullOrEmpty(id.ToString()))
                    throw new ArgumentException("ID or where clause must be provided");
                query = $"UPDATE {quotedTable} SET {setClause} WHERE {Quote(key)} = {id}";
            }
            else
            {
                query = $"UPDATE {quotedTable} SET {setClause} WHERE {where}";
            }

            Query(query, log, thrw);
        }
        /// <summary>
        /// Updates the rows matching <c>where</c> from a dictionary, adding missing columns first.
        /// A key <c>id</c> is written to the column <c>_id</c>. Single quotes are removed from values.
        /// </summary>
        /// <param name="data">Column → value.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        /// <param name="where">Raw SQL condition; required.</param>
        public void UpdFromDict(Dictionary<string, string> data, string tableName = null, bool log = false, bool thrw = false, string where = "")
        {
            tableName = tableName ?? _defaultTable;
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name must be provided");

            if (data.ContainsKey("id"))
            {
                data["_id"] = data["id"];
                data.Remove("id");
            }

            var columns = data.Keys.ToList();
            AddColumns(columns, tableName, log);

            var updString = new StringBuilder();
            foreach (var kvp in data)
            {
                updString.Append($"{kvp.Key} = '{kvp.Value.Replace("'", "")}',");
            }

            Upd(updString.ToString().Trim(','), tableName, log, thrw, where: where);
        }
        /// <summary>
        /// Inserts one row from a dictionary. On PostgreSQL a conflicting row is skipped (<c>ON CONFLICT DO
        /// NOTHING</c>).
        /// </summary>
        /// <param name="data">Column → value.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        public void InsertDic(Dictionary<string, string> data, string tableName = null, bool log = false, bool thrw = false)
        {
            tableName = tableName ?? _defaultTable;
    
            var columns = string.Join(", ", data.Keys.Select(k => Quote(k)));
            var values = string.Join(", ", data.Values.Select(v => $"'{v.Replace("'", "''")}'"));
    
            string query = $"INSERT INTO {Quote(tableName)} ({columns}) VALUES ({values})";
    
            if (_dbMode == "pgSQL")
                query += " ON CONFLICT DO NOTHING";
    
            Query(query, log, thrw);
        }
        /// <summary>
        /// Writes a local timestamp <c>yyyy-MM-dd HH:mm:ss</c> to <c>taskColumn</c>: now, or now plus
        /// <c>cooldownMin</c>.
        /// </summary>
        /// <param name="taskColumn">Column to write.</param>
        /// <param name="cooldownMin">Minutes to add; 0 writes the current time.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">Value of <c>key</c>, inserted into the SQL as written: quote text values yourself.</param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        public void SetDone(string taskColumn = "daily", int cooldownMin = 0, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
        {
            string cooldown = cooldownMin == 0 ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Now.AddMinutes(cooldownMin).ToString("yyyy-MM-dd HH:mm:ss");
            Upd($"{taskColumn} = '{cooldown}'", tableName, log, thrw, key, id, where);
        }
        
        #endregion

        #region JSON Methods
        /// <summary>
        /// Flattens a JSON object into columns and writes it with <c>UpdFromDict</c>.
        /// Nested keys are joined with <c>_</c> (<c>a_b_0</c>). The original shape is saved in the
        /// <c>_json_structure</c> column so that <c>DbToJson</c> can rebuild it.
        /// </summary>
        /// <param name="json">JSON object.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        /// <param name="where">Raw SQL condition selecting the row.</param>
        public void JsonToDb(string json, string tableName = null, bool log = false, bool thrw = false, string where = "")
        {
            tableName = tableName ?? _defaultTable;
            var structure = ExtractJsonStructure(json);
            var dataDic = JsonToDictionary(json);
            dataDic["_json_structure"] = structure;

            UpdFromDict(dataDic, tableName, log, thrw, where);
        }

        /// <summary>
        /// Rebuilds the JSON stored by <c>JsonToDb</c> from the row with the given <c>id</c>. Columns starting
        /// with <c>_</c> and <c>id</c> are left out.
        /// </summary>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        /// <param name="id">Value of <c>key</c>, inserted into the SQL as written: quote text values yourself.</param>
        /// <returns>
        /// The JSON text, or <c>{}</c> when the table has no <c>_json_structure</c> column or it cannot be
        /// parsed.
        /// </returns>
        public string DbToJson(string tableName = null, bool log = false, bool thrw = false, object id = null)
        {
            tableName = tableName ?? _defaultTable;
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name must be provided");

            var columns = GetTableColumns(tableName, log);
            if (!columns.Contains("_json_structure"))
            {
                _log.Send("ERROR: _json_structure column not found");
                return "{}";
            }

            var columnsString = string.Join(",", columns);
            var allColumns = GetColumns(columnsString, tableName, log, thrw, id: id);

            if (!allColumns.ContainsKey("_json_structure"))
            {
                _log.Send("ERROR: _json_structure not in result");
                return "{}";
            }

            var structureJson = allColumns["_json_structure"];
            Dictionary<string, string> structure;
            
            try
            {
                structure = JsonConvert.DeserializeObject<Dictionary<string, string>>(structureJson);
            }
            catch (Exception ex)
            {
                _log.Send($"Structure parse error: {ex.Message}");
                return "{}";
            }

            var keysToRemove = allColumns.Keys.Where(k => k.StartsWith("_") || k == "id").ToList();
            foreach (var key in keysToRemove)
            {
                allColumns.Remove(key);
            }

            return BuildJson(allColumns, structure, log);
        }

        private string ExtractJsonStructure(string json)
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
                            var key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}_{property.Name}";
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

        private Dictionary<string, string> JsonToDictionary(string json)
        {
            var result = new Dictionary<string, string>();
            var jObject = JObject.Parse(json);
            FlattenJson(jObject, "", result);
            return result;

            void FlattenJson(JToken token, string prefix, Dictionary<string, string> dict)
            {
                if (token is JObject obj)
                {
                    foreach (var prop in obj.Properties())
                    {
                        var key = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}_{prop.Name}";
                        FlattenJson(prop.Value, key, dict);
                    }
                }
                else if (token is JArray arr)
                {
                    for (int i = 0; i < arr.Count; i++)
                    {
                        FlattenJson(arr[i], $"{prefix}_{i}", dict);
                    }
                }
                else
                {
                    dict[prefix] = token.ToString();
                }
            }
        }

        private string BuildJson(Dictionary<string, string> data, Dictionary<string, string> structure, bool log)
        {
            var root = new JObject();

            foreach (var kvp in data)
            {
                if (!structure.ContainsKey(kvp.Key))
                    continue;

                var type = structure[kvp.Key];
                if (type == "object" || type == "array")
                    continue;

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
                var value = kvp.Value;

                JToken token;
                if (type != "string" && IsJsonString(value))
                {
                    try
                    {
                        token = JToken.Parse(value);
                    }
                    catch
                    {
                        token = CreateTypedToken(value, kvp.Key, structure);
                    }
                }
                else
                {
                    token = CreateTypedToken(value, kvp.Key, structure);
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

        private bool IsJsonString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trimmed = value.Trim();
            return (trimmed.StartsWith("{") && trimmed.EndsWith("}")) ||
                   (trimmed.StartsWith("[") && trimmed.EndsWith("]"));
        }

        private JToken CreateTypedToken(string value, string fullKey, Dictionary<string, string> structure)
        {
            if (structure.ContainsKey(fullKey))
            {
                var type = structure[fullKey];

                switch (type)
                {
                    case "integer":
                        if (int.TryParse(value, out int intVal))
                            return new JValue(intVal);
                        break;
                    case "float":
                        if (double.TryParse(value, out double dblVal))
                            return new JValue(dblVal);
                        break;
                    case "boolean":
                        if (bool.TryParse(value, out bool boolVal))
                            return new JValue(boolVal);
                        break;
                    case "null":
                        return JValue.CreateNull();
                }
            }

            return new JValue(value);
        }
        #endregion
        
        #region Table Preparation Methods
        /// <summary>Creates the table if it does not exist and adds missing columns.</summary>
        /// <param name="tableStructure">Column → SQL type, e.g. <c>{"id", "INTEGER PRIMARY KEY"}</c>.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="prune">Also drop columns that are not in <c>tableStructure</c> (<c>PruneColumns</c>).</param>
        /// <param name="rearrange">Also reorder columns to match <c>tableStructure</c> (<c>RearrangeColumns</c>).</param>
        public void PrepareTable(Dictionary<string, string> tableStructure, string tableName = null, bool log = false, bool prune = false, bool rearrange = false)
        {
            tableName = tableName ?? _defaultTable;
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name must be provided");

            CreateTable(tableStructure, tableName, log);
            AddColumns(tableStructure, tableName, log);
            
            if (prune) 
                PruneColumns(tableStructure, tableName, log);
            
            if (rearrange) 
                RearrangeColumns(tableStructure, tableName, log);
        }

        /// <summary>
        /// Same as the dictionary overload, with an <c>id</c> primary key and one type for every other column.
        /// </summary>
        /// <param name="columns">Column names; <c>id</c> and duplicates are skipped.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="defaultType">SQL type of every column.</param>
        /// <param name="serial">
        /// Type of <c>id</c>. <c>INTEGER</c> becomes <c>INTEGER PRIMARY KEY AUTOINCREMENT</c>
        /// (<c>AUTOINCREMENT</c> is replaced by <c>SERIAL</c> on PostgreSQL).
        /// </param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="prune">Drop columns not in the list.</param>
        /// <param name="rearrange">Reorder columns to match the list.</param>
        public void PrepareTable(List<string> columns, string tableName = null, string defaultType = "TEXT DEFAULT ''", string serial = "INTEGER", bool log = false, bool prune = false, bool rearrange = false)
        {
            var tableStructure = new Dictionary<string, string>
            {
                { "id", (serial == "INTEGER") ? $"{serial} PRIMARY KEY AUTOINCREMENT" : $"{serial} PRIMARY KEY" }
            };
            foreach (var column in columns)
            {
                string trimmed = column.Trim();
                if (!string.IsNullOrEmpty(trimmed) && trimmed.ToLower() != "id" && !tableStructure.ContainsKey(trimmed))
                {
                    tableStructure.Add(trimmed, defaultType);
                }
            }
            PrepareTable(tableStructure, tableName, log, prune, rearrange);
        }
        #endregion

        #region Column Rearrange Method
        /// <summary>
        /// Reorders the table's columns: <c>id</c> first, then the columns of <c>tableStructure</c> that exist,
        /// then the rest.
        /// Works by copying the data into a new table, dropping the old one and renaming the new one. On
        /// failure the temporary table is dropped and an exception is thrown.
        /// </summary>
        /// <param name="tableStructure">Desired order (column → type).</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        public void RearrangeColumns(Dictionary<string, string> tableStructure, string tableName = null, bool log = false)
        {
            tableName = tableName ?? _defaultTable;
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name must be provided");

            ValidateName(tableName, "table name");

            string quotedTable = Quote(tableName);
            string tempTable = Quote($"{tableName}_temp_{DateTime.Now.Ticks}");

            try
            {
                var currentColumns = GetTableColumns(tableName, log);
                var idType = GetIdType(tableName, log);
                var newTableStructure = BuildNewStructure(tableStructure, currentColumns, idType, tableName, log);

                CreateTempTable(tempTable, newTableStructure, log);
                CopyDataToTemp(quotedTable, tempTable, newTableStructure, log);
                DropOldTable(quotedTable, log);
                RenameTempTable(tempTable, quotedTable, tableName, log);

       
                _log.Send($"Table {tableName} rearranged successfully. New column order: {string.Join(", ", newTableStructure.Keys)}");
                
            }
            catch (Exception ex)
            {
                try
                {
                    Query($"DROP TABLE IF EXISTS {tempTable}", log: false);
                }
                catch { }

                throw new Exception($"Failed to rearrange table {tableName}: {ex.Message}", ex);
            }
            finally
            {
                try
                {
                    var tempExists = _dbMode == "pgSQL"
                        ? Query($"SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_schema = '{SchemaName}' AND table_name = '{UnQuote(tempTable)}')", log: false)
                        : Query($"SELECT name FROM sqlite_master WHERE type='table' AND name='{UnQuote(tempTable)}'", log: false);

                    if (!string.IsNullOrEmpty(tempExists) && tempExists != "0" && tempExists.ToLower() != "false")
                    {
                        Query($"DROP TABLE {tempTable}", log: false);
                    }
                }
                catch { }
            }
        }

        private string GetIdType(string tableName, bool log)
        {
            string idType = "INTEGER PRIMARY KEY";

            if (_dbMode == "pgSQL")
            {
                string getIdTypeQuery = $@"
                    SELECT data_type, is_identity 
                    FROM information_schema.columns 
                    WHERE table_schema = '{SchemaName}' 
                    AND table_name = '{UnQuote(tableName)}' 
                    AND column_name = 'id'";

                var idInfo = Query(getIdTypeQuery, log);
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
                string getIdTypeQuery = $"SELECT type FROM pragma_table_info('{UnQuote(tableName)}') WHERE name='id'";
                var sqliteIdType = Query(getIdTypeQuery, log);
                if (!string.IsNullOrEmpty(sqliteIdType) && sqliteIdType.ToUpper().Contains("TEXT"))
                    idType = "TEXT PRIMARY KEY";
            }

            return idType;
        }

        private Dictionary<string, string> BuildNewStructure(Dictionary<string, string> tableStructure, List<string> currentColumns, string idType, string tableName, bool log)
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
                    string colType = GetColumnType(tableName, col, log);
                    newTableStructure.Add(col, colType);
                }
            }

            return newTableStructure;
        }

        private string GetColumnType(string tableName, string col, bool log)
        {
            string colType = "TEXT DEFAULT ''";

            if (_dbMode == "pgSQL")
            {
                string getTypeQuery = $@"
                    SELECT data_type, character_maximum_length, column_default
                    FROM information_schema.columns 
                    WHERE table_schema = '{SchemaName}' 
                    AND table_name = '{UnQuote(tableName)}' 
                    AND column_name = '{col}'";

                var typeInfo = Query(getTypeQuery, log);
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
                string getTypeQuery = $"SELECT type FROM pragma_table_info('{UnQuote(tableName)}') WHERE name='{col}'";
                var sqliteType = Query(getTypeQuery, log);
                if (!string.IsNullOrEmpty(sqliteType))
                    colType = sqliteType;
            }

            return colType;
        }

        private void CreateTempTable(string tempTable, Dictionary<string, string> newTableStructure, bool log)
        {
            string createTempTableQuery;
            if (_dbMode == "pgSQL")
            {
                createTempTableQuery = $@"CREATE TABLE {tempTable} ( 
                    {string.Join(", ", newTableStructure.Select(kvp => $"{Quote(kvp.Key)} {kvp.Value.Replace("AUTOINCREMENT", "SERIAL")}"))} )";
            }
            else
            {
                createTempTableQuery = $@"CREATE TABLE {tempTable} ( 
                    {string.Join(", ", newTableStructure.Select(kvp => $"{Quote(kvp.Key)} {kvp.Value}"))} )";
            }

            Query(createTempTableQuery, log);
        }

        private void CopyDataToTemp(string quotedTable, string tempTable, Dictionary<string, string> newTableStructure, bool log)
        {
            var columnsList = string.Join(", ", newTableStructure.Keys.Select(k => Quote(k)));
            string copyDataQuery = $@"
                INSERT INTO {tempTable} ({columnsList})
                SELECT {columnsList}
                FROM {quotedTable}";

            Query(copyDataQuery, log);
        }

        private void DropOldTable(string quotedTable, bool log)
        {
            string dropOldTableQuery = $"DROP TABLE {quotedTable}";
            Query(dropOldTableQuery, log);
        }

        private void RenameTempTable(string tempTable, string quotedTable, string tableName, bool log)
        {
            string renameTableQuery;
            if (_dbMode == "pgSQL")
            {
                renameTableQuery = $"ALTER TABLE {tempTable} RENAME TO {quotedTable}";
            }
            else
            {
                renameTableQuery = $"ALTER TABLE {tempTable} RENAME TO {UnQuote(tableName)}";
            }

            Query(renameTableQuery, log);
        }
        #endregion

        #region Prune Columns Method
        /// <summary>Drops every column except <c>id</c> that is not a key of <c>tableStructure</c>.</summary>
        /// <param name="tableStructure">Columns to keep.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        public void PruneColumns(Dictionary<string, string> tableStructure, string tableName = null, bool log = false)
        {
            tableName = tableName ?? _defaultTable;
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name must be provided");

            var currentColumns = GetTableColumns(tableName, log);

            foreach (var column in currentColumns)
            {
                if (!tableStructure.ContainsKey(column) && column.ToLower() != "id")
                {
                    DropColumn(column, tableName, log);
                }
            }
        }
        #endregion

        #region Table Methods
        /// <summary>
        /// Creates the table unless it exists. On PostgreSQL <c>AUTOINCREMENT</c> in a type is replaced by
        /// <c>SERIAL</c>.
        /// </summary>
        /// <param name="tableStructure">Column → SQL type.</param>
        /// <param name="tableName">Table to create.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        public void CreateTable(Dictionary<string, string> tableStructure, string tableName, bool log = false)
        {
            if (TableExists(tableName, log))
                return;

            string quotedTable = Quote(tableName);
            string query;

            if (_dbMode == "pgSQL")
            {
                query = $"CREATE TABLE {quotedTable} ( {string.Join(", ", tableStructure.Select(kvp => $"\"{kvp.Key}\" {kvp.Value.Replace("AUTOINCREMENT", "SERIAL")}"))} )";
            }
            else
            {
                query = $"CREATE TABLE {quotedTable} ({string.Join(", ", tableStructure.Select(kvp => $"{Quote(kvp.Key)} {kvp.Value}"))})";
            }

            Query(query, log);
        }
        /// <summary>Checks whether the table exists (in the <c>public</c> schema on PostgreSQL).</summary>
        /// <param name="tableName">Table name; double quotes are ignored.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        public bool TableExists(string tableName, bool log = false)
        {
            tableName = UnQuote(tableName);
            string query;

            if (_dbMode == "pgSQL")
            {
                query = $"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '{SchemaName}' AND table_name = '{tableName}'";
            }
            else
            {
                query = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'";
            }

            string resp = Query(query, log);
            return resp != "0" && !string.IsNullOrEmpty(resp);
        }
        /// <summary>Names of all tables, sorted (PostgreSQL: base tables of the <c>public</c> schema).</summary>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        public List<string> GetTables(bool log = false)
        {
            string query = _dbMode == "pgSQL"
                ? $"SELECT table_name FROM information_schema.tables WHERE table_schema = '{SchemaName}' AND table_type = 'BASE TABLE' ORDER BY table_name"
                : "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY name";

            return Query(query, log)
                .Split(RawSeparator)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
        }
        /// <summary>Column names of a table.</summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        public List<string> GetTableColumns(string tableName, bool log = false)
        {
            string query = _dbMode == "pgSQL"
                ? $"SELECT column_name FROM information_schema.columns WHERE table_schema = '{SchemaName}' AND table_name = '{UnQuote(tableName)}'"
                : $"SELECT name FROM pragma_table_info('{UnQuote(tableName)}')";

            return Query(query, log)
                .Split(RawSeparator)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
        }
        #endregion

        #region Column Methods
        /// <summary>
        /// Checks whether a column exists. Case-insensitive on PostgreSQL, case-sensitive on SQLite.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        public bool ColumnExists(string columnName, string tableName, bool log = false)
        {
            string query;

            if (_dbMode == "pgSQL")
            {
                query = $"SELECT COUNT(*) FROM information_schema.columns WHERE table_schema = '{SchemaName}' AND table_name = '{UnQuote(tableName)}' AND LOWER(column_name) = LOWER('{UnQuote(columnName)}')";
            }
            else
            {
                query = $"SELECT COUNT(*) FROM pragma_table_info('{UnQuote(tableName)}') WHERE name='{UnQuote(columnName)}'";
            }

            string resp = Query(query, log);
            return resp != "0" && !string.IsNullOrEmpty(resp);
        }

        /// <summary>Adds a column if the table does not have it yet.</summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="defaultValue">SQL type of the new column.</param>
        public void AddColumn(string columnName, string tableName = null, bool log = false, string defaultValue = "TEXT DEFAULT ''")
        {
            tableName = tableName ?? _defaultTable;
            var current = GetTableColumns(tableName, log);
            
            if (!current.Contains(columnName))
            {
                string quotedColumn = Quote(columnName);
                string quotedTable = Quote(tableName);
                Query($"ALTER TABLE {quotedTable} ADD COLUMN {quotedColumn} {defaultValue}", log);
            }
        }
        /// <summary>Adds each listed column that the table does not have yet.</summary>
        /// <param name="columns">Column names.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="defaultValue">SQL type of the new columns.</param>
        public void AddColumns(List<string> columns, string tableName = null, bool log = false, string defaultValue = "TEXT DEFAULT ''")
        {
            foreach (var column in columns)
            {
                AddColumn(column, tableName, log, defaultValue);
            }
        }

        /// <summary>
        /// Adds each column of <c>tableStructure</c> that the table does not have yet, with its type.
        /// </summary>
        /// <param name="tableStructure">Column → SQL type.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        public void AddColumns(Dictionary<string, string> tableStructure, string tableName = null, bool log = false)
        {
            tableName = tableName ?? _defaultTable;
            var current = GetTableColumns(tableName, log);
            
            foreach (var column in tableStructure)
            {
                var keyWd = column.Key.Trim();
                if (!current.Contains(keyWd))
                {
                    string quotedColumn = Quote(keyWd);
                    string quotedTable = Quote(tableName);
                    Query($"ALTER TABLE {quotedTable} ADD COLUMN {quotedColumn} {column.Value}", log);
                }
            }
        }

        /// <summary>Drops a column if it exists (<c>CASCADE</c> on PostgreSQL).</summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        public void DropColumn(string columnName, string tableName = null, bool log = false)
        {
            tableName = tableName ?? _defaultTable;
            var current = GetTableColumns(tableName, log);

            if (current.Contains(columnName))
            {
                string quotedColumn = Quote(columnName);
                string quotedTable = Quote(tableName);
                string cascade = _dbMode == "pgSQL" ? " CASCADE" : "";
                Query($"ALTER TABLE {quotedTable} DROP COLUMN {quotedColumn}{cascade}", log);
            }
        }

        /// <summary>Drops every column except <c>id</c> in which no row has a non-empty value.</summary>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        public void PruneEmptyColumns(string tableName = null, bool log = false)
        {
            tableName = tableName ?? _defaultTable;
            var current = GetTableColumns(tableName, log);

            foreach (var column in current)
            {
                if (column.ToLower() == "id") continue;
                
                string quotedColumn = Quote(column);
                string quotedTable = Quote(tableName);
                string countQuery = $"SELECT COUNT(*) FROM {quotedTable} WHERE {quotedColumn} != '' AND {quotedColumn} IS NOT NULL";
                string result = Query(countQuery, log);

                if (int.TryParse(result, out int count) && count == 0)
                {
                    DropColumn(column, tableName, log);
                }
            }
        }
        #endregion

        #region Range Methods
        /// <summary>
        /// Inserts rows with ids from the current maximum + 1 up to <c>range</c>, in batches of 500. Existing
        /// ids are skipped.
        /// </summary>
        /// <param name="tableName">Table with an <c>id</c> column.</param>
        /// <param name="range">Highest id to have.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        public void AddRange(string tableName, int range, bool log = false)
        {
            string quotedTable = Quote(tableName);
            string currentQuery = $"SELECT COALESCE(MAX(CAST({Quote("id")} AS INTEGER)), 0) FROM {quotedTable}";
            int current = int.Parse(Query(currentQuery));

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
                    Query($"INSERT INTO {quotedTable} ({Quote("id")}) VALUES {batchValues} ON CONFLICT DO NOTHING", log);
                }
            }
        }
        #endregion
        
        #region Delete Methods
        /// <summary>Deletes the row where <c>key</c> = <c>id</c>, or the rows matching <c>where</c>.</summary>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">Value of <c>key</c>, inserted into the SQL as written: quote text values yourself.</param>
        /// <param name="where">Raw SQL condition. When set, <c>key</c> and <c>id</c> are ignored.</param>
        public void Del(string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
        {
            tableName = tableName ?? _defaultTable;
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name must be provided");

            string quotedTable = Quote(tableName);
            string query;

            if (string.IsNullOrEmpty(where))
            {
                if (id == null || string.IsNullOrEmpty(id.ToString()))
                    throw new ArgumentException("ID or where clause must be provided");
                query = $"DELETE FROM {quotedTable} WHERE {Quote(key)} = {id}";
            }
            else
            {
                query = $"DELETE FROM {quotedTable} WHERE {where}";
            }

            Query(query, log, thrw);
        }

        /// <summary>
        /// Deletes all rows and resets the id counter (<c>TRUNCATE … RESTART IDENTITY CASCADE</c> on
        /// PostgreSQL, the <c>sqlite_sequence</c> entry on SQLite).
        /// </summary>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        public void Clear(string tableName = null, bool log = false, bool thrw = false)
        {
            tableName = tableName ?? _defaultTable;
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name must be provided");

            string quotedTable = Quote(tableName);
    
            if (_dbMode == "pgSQL")
            {
                Query($"TRUNCATE TABLE {quotedTable} RESTART IDENTITY CASCADE", log, thrw);
            }
            else
            {
                Query($"DELETE FROM {quotedTable}", log, thrw);
                Query($"DELETE FROM sqlite_sequence WHERE name='{UnQuote(tableName)}'", log: false);
            }
        }
        #endregion

        #region Line Operations
        /// <summary>Sets every column except <c>id</c> to an empty string in one row.</summary>
        /// <param name="id">Row id.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Rethrow a database error instead of returning an empty result.</param>
        public void ClearLine(int id, string tableName = null, bool log = false, bool thrw = false)
        {
            tableName = tableName ?? _defaultTable;
            string quotedTable = Quote(tableName);

            var columns = GetTableColumns(tableName, log);
            var columnsToClean = columns.Where(col => col.ToLower() != "id").ToList();

            if (columnsToClean.Count == 0)
            {
                _log.Send($"No columns to clear in table {tableName}");
                return;
            }

            var setClause = string.Join(", ", columnsToClean.Select(col => $"{Quote(col)} = ''"));
            var updateQuery = $"UPDATE {quotedTable} SET {setClause} WHERE {Quote("id")} = {id}";

            Query(updateQuery, log, thrw);

            _log.Send($"Cleared {columnsToClean.Count} columns in row id={id}");
        }

        /// <summary>Exchanges the values of all columns except <c>id</c> between two rows.</summary>
        /// <param name="id1">First row id.</param>
        /// <param name="id2">Second row id.</param>
        /// <param name="tableName">Table; default is the table given to the constructor.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <param name="thrw">Throw when a row is not found; otherwise it is logged and nothing changes.</param>
        public void SwapLines(int id1, int id2, string tableName = null, bool log = false, bool thrw = false)
        {
            tableName = tableName ?? _defaultTable;
            string quotedTable = Quote(tableName);

            var columns = GetTableColumns(tableName, log);
            var columnsToSwap = columns.Where(col => col.ToLower() != "id").ToList();

            if (columnsToSwap.Count == 0)
            {
                _log.Send($"No columns to swap in table {tableName}");
                return;
            }

            var columnsString = string.Join(", ", columnsToSwap);
            var data1 = GetColumns(columnsString, tableName, log, thrw, "id", id1);
            var data2 = GetColumns(columnsString, tableName, log, thrw, "id", id2);

            if (data1 == null || data1.Count == 0)
            {
                _log.Send($"Row id={id1} not found");
                if (thrw) throw new Exception($"Row id={id1} not found");
                return;
            }

            if (data2 == null || data2.Count == 0)
            {
                _log.Send($"Row id={id2} not found");
                if (thrw) throw new Exception($"Row id={id2} not found");
                return;
            }

            var setClause1 = string.Join(", ", data2.Select(kvp => $"{Quote(kvp.Key)} = '{kvp.Value.Replace("'", "''")}'"));
            var setClause2 = string.Join(", ", data1.Select(kvp => $"{Quote(kvp.Key)} = '{kvp.Value.Replace("'", "''")}'"));

            Query($"UPDATE {quotedTable} SET {setClause1} WHERE {Quote("id")} = {id1}", log, thrw);
            Query($"UPDATE {quotedTable} SET {setClause2} WHERE {Quote("id")} = {id2}", log, thrw);

            _log.Send($"Swapped data between id={id1} and id={id2}");
        }
        #endregion

        #region Helper Methods
        private static string UnQuote(string name)
        {
            return name.Replace("\"", "");
        }

        private static string Quote(string name)
        {
            return $"\"{name.Replace("\"", "\"\"")}\"";
        }

        private static string QuoteColumns(string updateString)
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

        private static string QuoteSelectColumns(string columnString)
        {
            return string.Join(", ",
                columnString.Split(',')
                    .Select(col => $"\"{col.Trim()}\""));
        }

        private static readonly Regex ValidNamePattern = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

        private static string ValidateName(string name, string paramName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"{paramName} cannot be null or empty");
            
            return name;
        }
        #endregion

        #region Bridge

        private static readonly Dictionary<string, string> PgToSqliteTypeMap = new Dictionary<string, string>
        {
            { "integer", "INTEGER" },
            { "int", "INTEGER" },
            { "int2", "INTEGER" },
            { "int4", "INTEGER" },
            { "int8", "INTEGER" },
            { "bigint", "INTEGER" },
            { "smallint", "INTEGER" },
            { "serial", "INTEGER" },
            { "bigserial", "INTEGER" },
            { "boolean", "INTEGER" },
            { "bool", "INTEGER" },
            { "real", "REAL" },
            { "float4", "REAL" },
            { "float8", "REAL" },
            { "double precision", "REAL" },
            { "numeric", "REAL" },
            { "decimal", "REAL" },
            { "text", "TEXT" },
            { "varchar", "TEXT" },
            { "character varying", "TEXT" },
            { "char", "TEXT" },
            { "character", "TEXT" },
            { "uuid", "TEXT" },
            { "json", "TEXT" },
            { "jsonb", "TEXT" },
            { "date", "TEXT" },
            { "time", "TEXT" },
            { "time without time zone", "TEXT" },
            { "time with time zone", "TEXT" },
            { "timestamp", "TEXT" },
            { "timestamp without time zone", "TEXT" },
            { "timestamp with time zone", "TEXT" },
            { "bytea", "BLOB" }
        };

        private static readonly Dictionary<string, string> SqliteToPgTypeMap = new Dictionary<string, string>
        {
            { "INTEGER", "bigint" },
            { "REAL", "double precision" },
            { "TEXT", "text" },
            { "BLOB", "bytea" }
        };

        private static string PgTypeToSqlite(string pgType)
        {
            string normalized = pgType.ToLower().Trim();
            normalized = Regex.Replace(normalized, @"\(.*?\)", "").Trim();

            if (PgToSqliteTypeMap.ContainsKey(normalized))
                return PgToSqliteTypeMap[normalized];

            return "TEXT";
        }

        private static string SqliteTypeToPg(string sqliteType)
        {
            string normalized = sqliteType.ToUpper().Trim();
            normalized = Regex.Replace(normalized, @"\(.*?\)", "").Trim();

            if (normalized.Contains("INT"))
                return SqliteToPgTypeMap["INTEGER"];
            if (normalized.Contains("REAL") || normalized.Contains("FLOA") || normalized.Contains("DOUB"))
                return SqliteToPgTypeMap["REAL"];
            if (normalized.Contains("BLOB") || string.IsNullOrEmpty(normalized))
                return SqliteToPgTypeMap["BLOB"];

            return SqliteToPgTypeMap.ContainsKey(normalized) ? SqliteToPgTypeMap[normalized] : "text";
        }

        /// <summary>
        /// Copies a PostgreSQL table into an SQLite file. The target table is dropped and recreated; PostgreSQL
        /// types are mapped to INTEGER, REAL, TEXT or BLOB.
        /// </summary>
        /// <param name="pgTable">Source table.</param>
        /// <param name="sqlitePath">SQLite database file.</param>
        /// <param name="sqliteTable">Target table.</param>
        /// <param name="pgSchema">Source schema.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <remarks>This instance must be in <c>pgSQL</c> mode.</remarks>
        public void PgToSqlite(string pgTable, string sqlitePath, string sqliteTable, string pgSchema = "public", bool log = false)
        {
            if (_dbMode != "pgSQL")
                throw new InvalidOperationException("Source database must be PostgreSQL");

            var pgColumns = FetchPgColumns(pgSchema, pgTable, log);
            var columnDefs = new Dictionary<string, string>();

            foreach (var col in pgColumns)
            {
                columnDefs[col.Key] = PgTypeToSqlite(col.Value);
            }

            var rows = FetchPgRows(pgSchema, pgTable, pgColumns.Keys.ToList(), log);

            using (var sqliteDb = new Sql(sqlitePath, null))
            {
                RecreateSqliteTable(sqliteDb, sqliteTable, columnDefs, log);
                InsertRowsIntoSqlite(sqliteDb, sqliteTable, columnDefs.Keys.ToList(), rows, log);
            }

            _log.Send($"Transferred {rows.Count} rows from PostgreSQL {pgSchema}.{pgTable} to SQLite {sqliteTable}");
        }

        /// <summary>
        /// Copies an SQLite table into PostgreSQL. The target table is dropped and recreated; SQLite types are
        /// mapped to bigint, double precision, text or bytea.
        /// </summary>
        /// <param name="sqlitePath">SQLite database file.</param>
        /// <param name="sqliteTable">Source table.</param>
        /// <param name="pgTable">Target table.</param>
        /// <param name="pgSchema">Target schema.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        /// <remarks>This instance must be in <c>pgSQL</c> mode; it is the target.</remarks>
        public void SqliteToPg(string sqlitePath, string sqliteTable, string pgTable, string pgSchema = "public", bool log = false)
        {
            if (_dbMode != "pgSQL")
                throw new InvalidOperationException("Target database must be PostgreSQL");

            CopySqliteToPg(sqlitePath, sqliteTable, pgTable, pgSchema, sql => Query(sql, log), log);
        }

        private void CopySqliteToPg(string sqlitePath, string sqliteTable, string pgTable, string pgSchema, Action<string> pgExec, bool log)
        {
            Dictionary<string, string> sqliteColumns;
            List<List<object>> rows;

            using (var sqliteDb = new Sql(sqlitePath, null))
            {
                sqliteColumns = FetchSqliteColumns(sqliteDb, sqliteTable, log);
                rows = FetchSqliteRows(sqliteDb, sqliteTable, sqliteColumns.Keys.ToList(), log);
            }

            RecreatePgTable(pgSchema, pgTable, sqliteColumns, pgExec);
            InsertRowsIntoPg(pgSchema, pgTable, sqliteColumns.Keys.ToList(), rows, pgExec);

            _log.Send($"Transferred {rows.Count} rows from SQLite {sqliteTable} to PostgreSQL {pgSchema}.{pgTable}");
        }

        /// <summary>Copies a table between two SQLite files. The target table is dropped and recreated.</summary>
        /// <param name="sourcePath">Source database file.</param>
        /// <param name="sourceTable">Source table.</param>
        /// <param name="targetPath">Target database file.</param>
        /// <param name="targetTable">Target table.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        public void SqliteToSqlite(string sourcePath, string sourceTable, string targetPath, string targetTable, bool log = false)
        {
            Dictionary<string, string> columns;
            List<List<object>> rows;

            using (var sourceDb = new Sql(sourcePath, null))
            {
                columns = FetchSqliteColumns(sourceDb, sourceTable, log);
                rows = FetchSqliteRows(sourceDb, sourceTable, columns.Keys.ToList(), log);
            }

            using (var targetDb = new Sql(targetPath, null))
            {
                RecreateSqliteTable(targetDb, targetTable, columns, log);
                InsertRowsIntoSqlite(targetDb, targetTable, columns.Keys.ToList(), rows, log);
            }

            _log.Send($"Transferred {rows.Count} rows from SQLite {sourceTable} to SQLite {targetTable}");
        }

        /// <summary>
        /// Copies a table from this database to another one: PostgreSQL → SQLite, SQLite → PostgreSQL or SQLite
        /// → SQLite. The target table is dropped and recreated.
        /// </summary>
        /// <param name="sourceTable">Table in this database.</param>
        /// <param name="targetDbPath">Target SQLite file, or the Npgsql connection string of the target database for <c>pgSQL</c>.</param>
        /// <param name="targetTable">Target table.</param>
        /// <param name="targetMode"><c>SQLite</c> or <c>pgSQL</c>.</param>
        /// <param name="schema">PostgreSQL schema.</param>
        /// <param name="log">
        /// Write the query and its result to the log even when the logger level given to the constructor is
        /// <c>Off</c>.
        /// </param>
        public void BridgeTable(string sourceTable, string targetDbPath, string targetTable, string targetMode = "SQLite", string schema = "public", bool log = false)
        {
            if (_dbMode == "pgSQL" && targetMode == "SQLite")
            {
                PgToSqlite(sourceTable, targetDbPath, targetTable, schema, log);
            }
            else if (_dbMode == "SQLite" && targetMode == "pgSQL")
            {
                // The target is a different database: targetDbPath carries its connection string.
                using (var pgDb = new Sql(targetDbPath))
                    CopySqliteToPg(_sqLitePath, sourceTable, targetTable, schema, sql => pgDb.DbWrite(sql), log);
            }
            else if (_dbMode == "SQLite" && targetMode == "SQLite")
            {
                SqliteToSqlite(_sqLitePath, sourceTable, targetDbPath, targetTable, log);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported bridge direction: {_dbMode} to {targetMode}");
            }
        }

        private Dictionary<string, string> FetchPgColumns(string schema, string table, bool log)
        {
            string query = $@"
                SELECT column_name, data_type
                FROM information_schema.columns
                WHERE table_schema = '{schema}' AND table_name = '{table}'
                ORDER BY ordinal_position";

            var result = Query(query, log);
            var columns = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(result))
                return columns;

            var rows = result.Split(RawSeparator);
            foreach (var row in rows)
            {
                if (string.IsNullOrEmpty(row)) continue;
                var parts = row.Split(ColumnSeparator);
                if (parts.Length >= 2)
                {
                    columns[parts[0]] = parts[1];
                }
            }

            return columns;
        }

        private List<List<object>> FetchPgRows(string schema, string table, List<string> columns, bool log)
        {
            var columnsList = string.Join(", ", columns.Select(c => Quote(c)));
            string query = $"SELECT {columnsList} FROM \"{schema}\".\"{table}\"";

            var result = Query(query, log);
            var rows = new List<List<object>>();

            if (string.IsNullOrEmpty(result))
                return rows;

            var rawRows = result.Split(RawSeparator);
            foreach (var rawRow in rawRows)
            {
                if (string.IsNullOrEmpty(rawRow)) continue;
                var values = rawRow.Split(ColumnSeparator);
                rows.Add(new List<object>(values));
            }

            return rows;
        }

        private Dictionary<string, string> FetchSqliteColumns(Sql sqliteDb, string table, bool log)
        {
            string query = $"SELECT name, type FROM pragma_table_info('{UnQuote(table)}')";
            var result = sqliteDb.DbReadAsync(query, ColumnSeparator.ToString(), RawSeparator.ToString()).GetAwaiter().GetResult();

            var columns = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(result))
                return columns;

            var rows = result.Split(RawSeparator);
            foreach (var row in rows)
            {
                if (string.IsNullOrEmpty(row)) continue;
                var parts = row.Split(ColumnSeparator);
                if (parts.Length >= 2)
                {
                    columns[parts[0]] = parts[1];
                }
            }

            return columns;
        }

        private List<List<object>> FetchSqliteRows(Sql sqliteDb, string table, List<string> columns, bool log)
        {
            var columnsList = string.Join(", ", columns.Select(c => Quote(c)));
            string query = $"SELECT {columnsList} FROM \"{table}\"";

            var result = sqliteDb.DbReadAsync(query, ColumnSeparator.ToString(), RawSeparator.ToString()).GetAwaiter().GetResult();
            var rows = new List<List<object>>();

            if (string.IsNullOrEmpty(result))
                return rows;

            var rawRows = result.Split(RawSeparator);
            foreach (var rawRow in rawRows)
            {
                if (string.IsNullOrEmpty(rawRow)) continue;
                var values = rawRow.Split(ColumnSeparator);
                rows.Add(new List<object>(values));
            }

            return rows;
        }

        private void RecreatePgTable(string schema, string table, Dictionary<string, string> columns, Action<string> exec)
        {
            exec($"DROP TABLE IF EXISTS \"{schema}\".\"{table}\"");

            var columnsSql = string.Join(", ", columns.Select(kvp =>
                $"\"{kvp.Key}\" {SqliteTypeToPg(kvp.Value)}"));

            exec($"CREATE TABLE \"{schema}\".\"{table}\" ({columnsSql})");
        }

        private void InsertRowsIntoPg(string schema, string table, List<string> columns, List<List<object>> rows, Action<string> exec)
        {
            if (rows.Count == 0)
                return;

            var columnNames = string.Join(", ", columns.Select(c => Quote(c)));

            foreach (var row in rows)
            {
                var values = string.Join(", ", row.Select(v =>
                    v == null ? "NULL" : $"'{v.ToString().Replace("'", "''")}'"));

                string insertSql = $"INSERT INTO \"{schema}\".\"{table}\" ({columnNames}) VALUES ({values})";
                exec(insertSql);
            }
        }

        private void RecreateSqliteTable(Sql sqliteDb, string table, Dictionary<string, string> columns, bool log)
        {
            sqliteDb.DbWriteAsync($"DROP TABLE IF EXISTS \"{table}\"").GetAwaiter().GetResult();

            var columnsSql = string.Join(", ", columns.Select(kvp =>
                $"\"{kvp.Key}\" {kvp.Value}"));

            sqliteDb.DbWriteAsync($"CREATE TABLE \"{table}\" ({columnsSql})").GetAwaiter().GetResult();
        }

        private void InsertRowsIntoSqlite(Sql sqliteDb, string table, List<string> columns, List<List<object>> rows, bool log)
        {
            if (rows.Count == 0)
                return;

            var columnNames = string.Join(", ", columns.Select(c => Quote(c)));
            var placeholders = string.Join(", ", columns.Select((_, i) => "?"));

            foreach (var row in rows)
            {
                var values = string.Join(", ", row.Select(v =>
                    v == null ? "NULL" : $"'{v.ToString().Replace("'", "''")}'"));

                string insertSql = $"INSERT INTO \"{table}\" ({columnNames}) VALUES ({values})";
                sqliteDb.DbWriteAsync(insertSql).GetAwaiter().GetResult();
            }
        }

        #endregion
    }
}