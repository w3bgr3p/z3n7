using System;
using System.Collections.Generic;
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
        private static readonly string _settingsTable = DbSchema.Settings;
        private static readonly string _tasksTable    = DbSchema.Tasks;
        private static readonly string _commandsTable = DbSchema.Commands;
        private static readonly string _machine       = Environment.MachineName;

        // ── ZP XML ────────────────────────────────────────────────────────────

        private static Dictionary<string, string> XmlToDict(string taskXml)
        {
            var doc  = XDocument.Parse(taskXml);
            var dict = new Dictionary<string, string>();

            foreach (var setting in doc.Descendants("InputSetting"))
            {
                var outputVar = setting.Element("OutputVariable")?.Value;
                if (string.IsNullOrWhiteSpace(outputVar)) continue;

                var key   = outputVar.Replace("{-Variable.", "").Replace("-}", "");
                var value = setting.Element("Value")?.Value ?? "";
                dict[key] = value;
            }

            dict["_xml"] = taskXml.ToBase64();
            return dict;
        }

        private static string DbToXml(IZennoPosterProjectModel project, string taskId)
        {
            var xmlBase64 = project.DbGet("_xml", _settingsTable,
                where: $"\"Id\" = '{taskId}' AND \"machine\" = '{_machine}'");
            xmlBase64 = xmlBase64.Split('·')[0];

            var xml = xmlBase64.FromBase64();
            if (string.IsNullOrEmpty(xml)) return string.Empty;

            var doc = XDocument.Parse(xml);

            foreach (var setting in doc.Descendants("InputSetting"))
            {
                var outputVar = setting.Element("OutputVariable")?.Value;
                if (string.IsNullOrWhiteSpace(outputVar)) continue;

                var key     = outputVar.Replace("{-Variable.", "").Replace("-}", "");
                var dbValue = project.DbGet(key, _settingsTable,
                    where: $"\"Id\" = '{taskId}' AND \"machine\" = '{_machine}'");
                if (!string.IsNullOrWhiteSpace(dbValue))
                    setting.Element("Value").Value = dbValue;
            }

            return doc.ToString();
        }

        // ── PUSH ──────────────────────────────────────────────────────────────

        public static void Push(this IZennoPosterProjectModel project, bool syncTasks = false)
        {
            if (syncTasks) SyncTasks(project);

            project.ClmnAdd("Id",   _settingsTable);
            project.ClmnAdd("Name", _settingsTable);
            project.DbQ($"DELETE FROM \"{_settingsTable}\" WHERE \"machine\" = '{_machine}'");

            var taskList = project.DbGetLines("Id", _tasksTable,
                where: $"\"Id\" != '' AND \"machine\" = '{_machine}'");

            // Один SELECT всех Name вместо N отдельных
            var nameRows = project.DbGetLines("Id,Name", _tasksTable,
                where: $"\"Id\" != '' AND \"machine\" = '{_machine}'");
            var nameMap = new Dictionary<string, string>();
            foreach (var row in nameRows)
            {
                var parts = row.Split('¦');
                if (parts.Length >= 1 && !string.IsNullOrEmpty(parts[0]))
                    nameMap[parts[0]] = parts.Length >= 2 ? parts[1] : "";
            }

            // Собираем xml и dict в памяти
            var exports = new List<(string taskId, string name, Dictionary<string, string> dict)>();
            foreach (var taskId in taskList)
            {
                if (string.IsNullOrWhiteSpace(taskId)) continue;
                nameMap.TryGetValue(taskId, out var name);
                var xml = ZennoPoster.ExportInputSettings(new Guid(taskId));
                // задачи без xml добавляем с пустым dict — как в оригинале они получают INSERT но не DicToDb
                var dict = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(xml))
                {
                    try   { dict = XmlToDict(xml); }
                    catch { project.warn($"[TaskManager.Push] XmlToDict failed: {taskId}"); }
                }
                exports.Add((taskId, name ?? "", dict));
            }

            // Один ClmnAdd для всех колонок
            var allColumns = new Dictionary<string, string> { { "machine", "TEXT DEFAULT ''" } };
            foreach (var (_, _, dict) in exports)
                foreach (var k in dict.Keys)
                    if (!allColumns.ContainsKey(k))
                        allColumns[k] = "TEXT DEFAULT ''";
            project.ClmnAdd(allColumns, _settingsTable);

            // Батч INSERT
            const int batchSize = 200;
            var insertValues = exports
                .Select(e => $"('{e.taskId}', '{e.name.Replace("'", "''")}', '{_machine}')")
                .ToList();
            for (int i = 0; i < insertValues.Count; i += batchSize)
            {
                var batch = string.Join(", ", insertValues.Skip(i).Take(batchSize));
                project.DbQ($"INSERT INTO \"{_settingsTable}\" (\"Id\", \"Name\", \"machine\") VALUES {batch}");
            }

            // UPDATE только для задач с dict
            foreach (var (taskId, _, dict) in exports)
            {
                if (dict.Count == 0) continue;
                var set = new StringBuilder();
                foreach (var kv in dict)
                    set.Append($"\"{kv.Key}\" = '{kv.Value.Replace("'", "''")}',");
                project.DbQ($"UPDATE \"{_settingsTable}\" SET {set.ToString().TrimEnd(',')} " +
                            $"WHERE \"Id\" = '{taskId}' AND \"machine\" = '{_machine}'");
            }

            project.log($"[TaskManager] Push done: {exports.Count} tasks");
        }
        public static void Push(this IZennoPosterProjectModel project, Guid taskId)
                {
                    var exists = project.DbGet("Id", _settingsTable,
                        where: $"\"Id\" = '{taskId}' AND \"machine\" = '{_machine}'").Split('·')[0];

                    if (string.IsNullOrEmpty(exists))
                    {
                        var name = project.DbGet("Name", _tasksTable,
                            where: $"\"Id\" = '{taskId}' AND \"machine\" = '{_machine}'").Split('·')[0];
                        project.DbQ($"INSERT INTO \"{_settingsTable}\" (\"Id\", \"Name\", \"machine\") " +
                                    $"VALUES ('{taskId}', '{name}', '{_machine}')");
                    }

                    var xml = ZennoPoster.ExportInputSettings(taskId);
                    if (string.IsNullOrEmpty(xml)) return;

                    try   { project.DicToDb(XmlToDict(xml), _settingsTable,
                                where: $"\"Id\" = '{taskId}' AND \"machine\" = '{_machine}'"); }
                    catch { project.warn($"[TaskManager.Push] XmlToDict failed: {taskId}"); }
                }

        // ── PULL ──────────────────────────────────────────────────────────────

         public static void Pull(this IZennoPosterProjectModel project, bool syncTasks = false)
        {
            if (syncTasks) SyncTasks(project);

            // Один SELECT всех колонок для всех задач машины
            var columns = project.TblColumns(_settingsTable);
            var colStr  = string.Join(",", columns);

            var rows = project.DbGetLines(colStr, _settingsTable,
                where: $"\"machine\" = '{_machine}'");

            // Строим taskId -> dict в памяти
            var byTask = new Dictionary<string, Dictionary<string, string>>();
            foreach (var row in rows)
            {
                var vals = row.Split('\u00a6'); // '¦'
                var dict = new Dictionary<string, string>();
                for (int i = 0; i < columns.Count && i < vals.Length; i++)
                    dict[columns[i]] = vals[i];

                if (dict.TryGetValue("Id", out var id) && !string.IsNullOrEmpty(id))
                    byTask[id] = dict;
            }

            int ok = 0, skip = 0;
            foreach (var kvp in byTask)
            {
                var taskId = kvp.Key;
                var data   = kvp.Value;

                if (!data.TryGetValue("_xml", out var xmlBase64) || string.IsNullOrEmpty(xmlBase64))
                    { skip++; continue; }

                string xml;
                try   { xml = xmlBase64.FromBase64(); }
                catch { skip++; continue; }
                if (string.IsNullOrEmpty(xml)) { skip++; continue; }

                var doc = XDocument.Parse(xml);
                foreach (var setting in doc.Descendants("InputSetting"))
                {
                    var outputVar = setting.Element("OutputVariable")?.Value;
                    if (string.IsNullOrWhiteSpace(outputVar)) continue;
                    var key = outputVar.Replace("{-Variable.", "").Replace("-}", "");
                    if (data.TryGetValue(key, out var val) && !string.IsNullOrWhiteSpace(val))
                        setting.Element("Value").Value = val;
                }

                ZennoPoster.ImportInputSettings(new Guid(taskId), doc.ToString());
                ok++;
            }

            project.log($"[TaskManager] Pull done: {ok} loaded, {skip} skipped");
        }
        public static void Pull(this IZennoPosterProjectModel project, Guid taskId)
        {
            var xml = DbToXml(project, taskId.ToString());
            if (string.IsNullOrEmpty(xml))
            {
                project.warn($"[TaskManager.Pull] No _xml for {taskId}");
                return;
            }
            ZennoPoster.ImportInputSettings(taskId, xml);
        }

        // ── SYNC TASKS ────────────────────────────────────────────────────────
        public static void SyncTasks(this IZennoPosterProjectModel project)
        {
            project.DbQ($"DELETE FROM \"{_tasksTable}\" WHERE \"machine\" = '{_machine}'");

            int i = 0;
            foreach (var task in ZennoPoster.TasksList)
            {
                i++;
                var doc  = XDocument.Parse("<root>" + task + "</root>");
                var jObj = JObject.Parse(JsonConvert.SerializeXNode(doc))["root"];
                var taskId = (string)jObj["Id"];

                project.DbQ($"INSERT INTO \"{_tasksTable}\" (\"Id\", \"machine\") VALUES ('{taskId}', '{_machine}')");
                project.JsonToDb(jObj.ToString(), _tasksTable,
                    where: $"\"Id\" = '{taskId}' AND \"machine\" = '{_machine}'");
            }

            project.log($"[TaskManager] SyncTasks done: {i} tasks");
        }

        // ── EXEC COMMANDS ─────────────────────────────────────────────────────

        public static void ExecCommands(this IZennoPosterProjectModel project, bool log = false)
        {
            var rows = project.DbGetLines("id,task_id,action,payload", _commandsTable,
                where: $"\"status\" = 'pending' AND \"machine\" = '{_machine}'");

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
                    var guid = new Guid(taskId);
                    Exec(guid, action, payload, project);
                    project.DbQ($"UPDATE \"{_commandsTable}\" SET \"status\" = 'done', \"result\" = 'ok' " +
                                $"WHERE \"id\" = '{cmdId}' AND \"machine\" = '{_machine}'");
                    done++;
                }
                catch (Exception ex)
                {
                    var msg = ex.Message.Replace("'", "''");
                    project.DbQ($"UPDATE \"{_commandsTable}\" SET \"status\" = 'error', \"result\" = '{msg}' " +
                                $"WHERE \"id\" = '{cmdId}' AND \"machine\" = '{_machine}'");
                    failed++;
                    if (log) project.warn($"[TaskManager.ExecCommands] {action} {taskId}: {ex.Message}");
                }
            }

            if (done + failed > 0)
                project.log($"[TaskManager] ExecCommands: {done} done, {failed} failed");
        }
        private static void Exec(Guid taskId, string action, string payload, IZennoPosterProjectModel project)
        {
            switch (action.ToLower())
            {
                case "start":             ZennoPoster.StartTask(taskId);                                              break;
                case "stop":              ZennoPoster.StopTask(taskId);                                               break;
                case "interrupt":         ZennoPoster.InterruptTask(taskId);                                          break;
                case "add_tries":         if (int.TryParse(payload, out var addCount))  ZennoPoster.AddTries(taskId, addCount);      break;
                case "set_tries":         if (int.TryParse(payload, out var setCount))  ZennoPoster.SetTries(taskId, setCount);      break;
                case "set_threads":       if (int.TryParse(payload, out var threads))   ZennoPoster.SetMaxThreads(taskId, threads);  break;
                case "clear_success":     ZennoPoster.ClearSuccess(taskId);                                           break;
                case "clear_fails":       ZennoPoster.ClearFails(taskId);                                             break;
                case "update_settings":   Pull(project, taskId);                                                      break;
                case "set_execution_settings":  ZennoPoster.SetExecutionSettings(taskId, JsonToXml(payload));         break;
                case "set_scheduler_settings":  ZennoPoster.SetSchedulerSettings(taskId, JsonToXml(payload));         break;
            }
        }

        private static string JsonToXml(string json)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (dict == null) return string.Empty;

            var sb = new System.Text.StringBuilder();
            foreach (var kv in dict)
                sb.Append($"<{kv.Key}>{kv.Value}</{kv.Key}>");
            return sb.ToString();
        }
    }
}