using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;
using z3n7;

namespace z3n7.DbUtils
{
    public static class TaskManager
    {
        private static readonly string _settingsTable = DbSchema.Settings.Name;
        private static readonly string _tasksTable    = DbSchema.Tasks.Name;
        private static readonly string _commandsTable = DbSchema.Commands.Name;
        private static readonly string _machine       = Environment.MachineName;

        private static string MakeId(string guid) => $"{_machine}|{guid}";
        private static string MachineFilter()      => $"\"id\" LIKE '{_machine}|%'";

        // ── INIT ──────────────────────────────────────────────────────────────

        public static void TblEnsure(this IZennoPosterProjectModel project)
        {
            project.TblAdd(new Dictionary<string, string>
            {
                { "id",        "TEXT PRIMARY KEY" },
                { "_xml_b64",  "TEXT DEFAULT ''"  },
                { "_json_b64", "TEXT DEFAULT ''"  },
                { "name",      "TEXT DEFAULT ''"  },
            }, _settingsTable);

            project.TblAdd(new Dictionary<string, string>
            {
                { "id",        "TEXT PRIMARY KEY" },
                { "name",      "TEXT DEFAULT ''"  },
                { "_json_b64", "TEXT DEFAULT ''"  },
            }, _tasksTable);

            project.TblAdd(new Dictionary<string, string>
            {
                { "id",         "TEXT PRIMARY KEY"      },
                { "task_id",    "TEXT DEFAULT ''"       },
                { "action",     "TEXT DEFAULT ''"       },
                { "payload",    "TEXT DEFAULT ''"       },
                { "status",     "TEXT DEFAULT 'pending'" },
                { "result",     "TEXT DEFAULT ''"       },
                { "created_at", "TEXT DEFAULT ''"       },
            }, _commandsTable);
        }

        // ── HELPERS ───────────────────────────────────────────────────────────

        private static (string xmlB64, string jsonB64) XmlToPayload(string xml)
        {
            var doc  = XDocument.Parse(xml);
            var dict = new Dictionary<string, string>();
            foreach (var setting in doc.Descendants("InputSetting"))
            {
                var outputVar = setting.Element("OutputVariable")?.Value;
                if (string.IsNullOrWhiteSpace(outputVar)) continue;
                var key   = outputVar.Replace("{-Variable.", "").Replace("-}", "");
                dict[key] = setting.Element("Value")?.Value ?? "";
            }
            return (xml.ToBase64(), JsonConvert.SerializeObject(dict).ToBase64());
        }

        private static string PayloadToXml(string xmlB64, string jsonB64)
        {
            var xml = xmlB64.FromBase64();
            if (string.IsNullOrEmpty(xml)) return string.Empty;

            Dictionary<string, string> dict;
            try   { dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonB64.FromBase64()) ?? new(); }
            catch { dict = new(); }

            var doc = XDocument.Parse(xml);
            foreach (var setting in doc.Descendants("InputSetting"))
            {
                var outputVar = setting.Element("OutputVariable")?.Value;
                if (string.IsNullOrWhiteSpace(outputVar)) continue;
                var key = outputVar.Replace("{-Variable.", "").Replace("-}", "");
                if (dict.TryGetValue(key, out var val) && !string.IsNullOrWhiteSpace(val))
                    setting.Element("Value").Value = val;
            }
            return doc.ToString();
        }

        // ── PUSH ──────────────────────────────────────────────────────────────

        public static void Push(this IZennoPosterProjectModel project, bool syncTasks = false)
        {
            var swTotal = Stopwatch.StartNew();
            var sw      = new Stopwatch();

            if (syncTasks)
            {
                sw.Restart();
                SyncTasks(project);
                project.log($"[TaskManager.Push] SyncTasks: {sw.ElapsedMilliseconds} ms");
            }

            sw.Restart();
            var nameRows = project.DbGetLines("id,name", _tasksTable, where: MachineFilter());
            var nameMap  = new Dictionary<string, string>(); // guid → name
            foreach (var row in nameRows)
            {
                var parts = row.Split('¦');
                if (parts.Length < 1 || string.IsNullOrEmpty(parts[0])) continue;
                var guid = parts[0].Split('|').LastOrDefault() ?? "";
                if (!string.IsNullOrEmpty(guid))
                    nameMap[guid] = parts.Length > 1 ? parts[1] : "";
            }
            project.log($"[TaskManager.Push] Read task list ({nameMap.Count}): {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            var exports = new List<(string guid, string name, string xmlB64, string jsonB64)>();
            foreach (var guid in nameMap.Keys)
            {
                var xml = ZennoPoster.ExportInputSettings(new Guid(guid));
                if (string.IsNullOrEmpty(xml))
                {
                    exports.Add((guid, nameMap[guid], "", ""));
                    continue;
                }
                try
                {
                    var (xmlB64, jsonB64) = XmlToPayload(xml);
                    exports.Add((guid, nameMap[guid], xmlB64, jsonB64));
                }
                catch
                {
                    project.warn($"[TaskManager.Push] XmlToPayload failed: {guid}");
                    exports.Add((guid, nameMap[guid], "", ""));
                }
            }
            project.log($"[TaskManager.Push] ExportInputSettings x{exports.Count}: {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            const int batchSize = 200;
            var values = exports
                .Select(e => $"('{MakeId(e.guid)}', '{e.name.Replace("'", "''")}', '{e.xmlB64}', '{e.jsonB64}')")
                .ToList();
            for (int i = 0; i < values.Count; i += batchSize)
            {
                var batch = string.Join(", ", values.Skip(i).Take(batchSize));
                project.UpsertQ(_settingsTable, "\"id\", \"name\", \"_xml_b64\", \"_json_b64\"", batch);
            }
            project.log($"[TaskManager.Push] UPSERT ({exports.Count} rows): {sw.ElapsedMilliseconds} ms");

            swTotal.Stop();
            project.log($"[TaskManager] Push done: {exports.Count} | TOTAL: {swTotal.ElapsedMilliseconds} ms");
        }

        public static void Push(this IZennoPosterProjectModel project, Guid taskId)
        {
            var sw  = Stopwatch.StartNew();
            var xml = ZennoPoster.ExportInputSettings(taskId);
            if (string.IsNullOrEmpty(xml)) return;

            string xmlB64, jsonB64;
            try   { (xmlB64, jsonB64) = XmlToPayload(xml); }
            catch { project.warn($"[TaskManager.Push(1)] XmlToPayload failed: {taskId}"); return; }

            var name = project.DbGet("name", _tasksTable,
                where: $"\"id\" = '{MakeId(taskId.ToString())}'").Split('·')[0];

            project.UpsertQ(_settingsTable, "\"id\", \"name\", \"_xml_b64\", \"_json_b64\"",
                $"('{MakeId(taskId.ToString())}', '{name}', '{xmlB64}', '{jsonB64}')");

            project.log($"[TaskManager.Push(1)] UPSERT: {sw.ElapsedMilliseconds} ms");
        }

        // ── PULL ──────────────────────────────────────────────────────────────

        public static void Pull(this IZennoPosterProjectModel project, bool syncTasks = false)
        {
            var swTotal = Stopwatch.StartNew();
            var sw      = new Stopwatch();

            if (syncTasks)
            {
                sw.Restart();
                SyncTasks(project);
                project.log($"[TaskManager.Pull] SyncTasks: {sw.ElapsedMilliseconds} ms");
            }

            sw.Restart();
            var rows = project.DbGetLines("id,_xml_b64,_json_b64", _settingsTable, where: MachineFilter());
            project.log($"[TaskManager.Pull] SELECT ({rows.Count} rows): {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            int ok = 0, skip = 0;
            foreach (var row in rows)
            {
                var parts = row.Split('¦');
                if (parts.Length < 3) { skip++; continue; }

                var guid    = parts[0].Split('|').LastOrDefault() ?? "";
                var xmlB64  = parts[1];
                var jsonB64 = parts[2];

                if (string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(xmlB64)) { skip++; continue; }

                string xml;
                try   { xml = PayloadToXml(xmlB64, jsonB64); }
                catch { skip++; continue; }
                if (string.IsNullOrEmpty(xml)) { skip++; continue; }

                ZennoPoster.ImportInputSettings(new Guid(guid), xml);
                ok++;
            }
            project.log($"[TaskManager.Pull] ImportInputSettings x{ok}: {sw.ElapsedMilliseconds} ms");

            swTotal.Stop();
            project.log($"[TaskManager] Pull done: {ok} loaded, {skip} skipped | TOTAL: {swTotal.ElapsedMilliseconds} ms");
        }

        public static void Pull(this IZennoPosterProjectModel project, Guid taskId)
        {
            var sw  = Stopwatch.StartNew();
            var row = project.DbGet("_xml_b64,_json_b64", _settingsTable,
                where: $"\"id\" = '{MakeId(taskId.ToString())}'");

            var parts = row.Split('·')[0].Split('¦');
            if (parts.Length < 2 || string.IsNullOrEmpty(parts[0]))
            {
                project.warn($"[TaskManager.Pull(1)] No payload for {taskId}");
                return;
            }

            string xml;
            try   { xml = PayloadToXml(parts[0], parts.Length > 1 ? parts[1] : ""); }
            catch { project.warn($"[TaskManager.Pull(1)] PayloadToXml failed: {taskId}"); return; }

            ZennoPoster.ImportInputSettings(taskId, xml);
            project.log($"[TaskManager.Pull(1)] ImportInputSettings: {sw.ElapsedMilliseconds} ms");
        }

        // ── SYNC TASKS ────────────────────────────────────────────────────────

        public static void SyncTasks(this IZennoPosterProjectModel project)
        {
            var swTotal = Stopwatch.StartNew();
            var sw      = new Stopwatch();

            sw.Restart();
            var exports = new List<(string guid, string name, string jsonB64)>();
            foreach (var task in ZennoPoster.TasksList)
            {
                var doc    = XDocument.Parse("<root>" + task + "</root>");
                var jObj   = JObject.Parse(JsonConvert.SerializeXNode(doc))["root"];
                var guid   = (string)jObj["Id"];
                if (string.IsNullOrEmpty(guid)) continue;
                exports.Add((guid, (string)jObj["Name"] ?? "", jObj.ToString().ToBase64()));
            }
            project.log($"[TaskManager.SyncTasks] Parse x{exports.Count}: {sw.ElapsedMilliseconds} ms");

            if (exports.Count == 0)
            {
                project.log($"[TaskManager] SyncTasks done: 0 | TOTAL: {swTotal.ElapsedMilliseconds} ms");
                return;
            }

            sw.Restart();
            const int batchSize = 200;
            var values = exports
                .Select(e => $"('{MakeId(e.guid)}', '{e.name.Replace("'", "''")}', '{e.jsonB64}')")
                .ToList();
            for (int i = 0; i < values.Count; i += batchSize)
            {
                var batch = string.Join(", ", values.Skip(i).Take(batchSize));
                project.UpsertQ(_tasksTable, "\"id\", \"name\", \"_json_b64\"", batch);
            }
            project.log($"[TaskManager.SyncTasks] UPSERT x{exports.Count}: {sw.ElapsedMilliseconds} ms");

            swTotal.Stop();
            project.log($"[TaskManager] SyncTasks done: {exports.Count} | TOTAL: {swTotal.ElapsedMilliseconds} ms");
        }

        // ── EXEC COMMANDS ─────────────────────────────────────────────────────

        public static void ExecCommands(this IZennoPosterProjectModel project, bool log = false)
        {
            var rows = project.DbGetLines("id,task_id,action,payload", _commandsTable,
                where: $"\"status\" = 'pending' AND {MachineFilter()}");

            int done = 0, failed = 0;
            foreach (var row in rows)
            {
                var parts = row.Split('¦');
                if (parts.Length < 4) continue;

                var cmdId   = parts[0];
                var taskId  = parts[1];
                var action  = parts[2];
                var payload = parts[3];

                try
                {
                    var sw = Stopwatch.StartNew();
                    Exec(new Guid(taskId), action, payload, project);
                    project.DbQ($"UPDATE \"{_commandsTable}\" SET \"status\" = 'done', \"result\" = 'ok' WHERE \"id\" = '{cmdId}'");
                    done++;
                    if (log) project.log($"[TaskManager.ExecCommands] {action} {taskId}: {sw.ElapsedMilliseconds} ms");
                }
                catch (Exception ex)
                {
                    var msg = ex.Message.Replace("'", "''");
                    project.DbQ($"UPDATE \"{_commandsTable}\" SET \"status\" = 'error', \"result\" = '{msg}' WHERE \"id\" = '{cmdId}'");
                    failed++;
                    if (log) project.warn($"[TaskManager.ExecCommands] {action} {taskId}: {ex.Message}");
                }
            }

            if (done + failed > 0 && log)
                project.log($"[TaskManager] ExecCommands: {done} done, {failed} failed");
        }

        private static void Exec(Guid taskId, string action, string payload, IZennoPosterProjectModel project)
        {
            switch (action.ToLower())
            {
                case "start":                  ZennoPoster.StartTask(taskId);                                                         break;
                case "stop":                   ZennoPoster.StopTask(taskId);                                                          break;
                case "interrupt":              ZennoPoster.InterruptTask(taskId);                                                      break;
                case "add_tries":              if (int.TryParse(payload, out var add))     ZennoPoster.AddTries(taskId, add);          break;
                case "set_tries":              if (int.TryParse(payload, out var set))     ZennoPoster.SetTries(taskId, set);          break;
                case "set_threads":            if (int.TryParse(payload, out var threads)) ZennoPoster.SetMaxThreads(taskId, threads); break;
                case "clear_success":          ZennoPoster.ClearSuccess(taskId);                                                       break;
                case "clear_fails":            ZennoPoster.ClearFails(taskId);                                                         break;
                case "update_settings":        Pull(project, taskId);                                                                  break;
                case "set_execution_settings": ZennoPoster.SetExecutionSettings(taskId, JsonToXml(payload));                           break;
                case "set_scheduler_settings": ZennoPoster.SetSchedulerSettings(taskId, JsonToXml(payload));                           break;
            }
        }

        private static string JsonToXml(string json)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (dict == null) return string.Empty;
            var sb = new StringBuilder();
            foreach (var kv in dict)
                sb.Append($"<{kv.Key}>{kv.Value}</{kv.Key}>");
            return sb.ToString();
        }

        // ── UPSERT ────────────────────────────────────────────────────────────

        /// INSERT с заменой по PK "id". Универсально для SQLite и PostgreSQL.
        private static void UpsertQ(this IZennoPosterProjectModel project,
            string table, string columns, string valuesSql)
        {
            bool isPg = project.Var("DBmode") == "PostgreSQL";

            string query = isPg
                ? BuildPgUpsert(table, columns, valuesSql)
                : $"INSERT OR REPLACE INTO \"{table}\" ({columns}) VALUES {valuesSql}";

            project.DbQ(query);
        }

        /// Для PostgreSQL строит batch-совместимый UPSERT через ON CONFLICT (id) DO UPDATE SET.
        /// valuesSql — одно или несколько значений через запятую: (...), (...)
        private static string BuildPgUpsert(string table, string columns, string valuesSql)
        {
            // columns: "id", "name", "_xml_b64", "_json_b64"
            var cols = columns.Split(',')
                .Select(c => c.Trim().Trim('"'))
                .ToList();

            // SET clause для всех колонок кроме id
            var setClauses = cols
                .Where(c => c != "id")
                .Select(c => $"\"{c}\" = EXCLUDED.\"{c}\"");

            return $"INSERT INTO \"{table}\" ({columns}) VALUES {valuesSql} " +
                   $"ON CONFLICT (\"id\") DO UPDATE SET {string.Join(", ", setClauses)}";
        }
    }
}