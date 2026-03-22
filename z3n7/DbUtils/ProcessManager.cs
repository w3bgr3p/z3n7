using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.DbUtils
{
    public class ProcessManager
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly Db _db;
        private readonly string _machineName;
        private readonly string _tableName;
        private readonly bool _log;
        private readonly bool _isPostgres;


        public ProcessManager(IZennoPosterProjectModel project, string machineName = null, bool log = false)
        {
            _project = project ;
            _db = new Db(_project);
            _machineName = machineName ?? Environment.MachineName;
            _tableName = $"process_{SafeName(_machineName)}";
            _log = log;
            _isPostgres = _project.Var("DBmode") == "PostgreSQL";
            EnsureTable();
        }

        public static List<string[]> ZennoProcesses()
        {
            var zProcesses = new List<string[]>();
    
            string[] processNames = new[] { "ZennoPoster", "zbe1" }; 
    
            var allProcs = new List<System.Diagnostics.Process>();
            foreach (var processName in processNames)
            {
                allProcs.AddRange(System.Diagnostics.Process.GetProcessesByName(processName));
            }

            if (allProcs.Count > 0)
            {
                foreach (var proc in allProcs)
                {
                    TimeSpan Time_diff = DateTime.Now - proc.StartTime;
                    int runningTime = Convert.ToInt32(Time_diff.TotalMinutes);
                    long memoryUsage = proc.WorkingSet64 / (1024 * 1024);
                    zProcesses.Add(new string[]{ 
                        proc.ProcessName, 
                        memoryUsage.ToString(), 
                        runningTime.ToString(),
                        proc.Id.ToString() // <--- ДОБАВИТЬ PID
                    });
                }
            }
            return zProcesses;
        }
        
        
        private string SafeName(string name) => name.Replace("-", "_").Replace(" ", "_");
        
        private void Log(string msg)
        {
            if (_log) _project.SendInfoToLog($"⚙️ [ProcessManager] {msg}", false);
        }

        private void EnsureTable()
        {
            var structure = new Dictionary<string, string>
            {
                { "id", "INTEGER PRIMARY KEY" },
                { "name", "TEXT DEFAULT ''" },
                { "ram", "TEXT DEFAULT ''" },
                { "uptime", "TEXT DEFAULT ''" },
                { "command_line", "TEXT DEFAULT ''" },
                { "updated_at", "TEXT DEFAULT ''" }
            };
            
            _db.CreateTable(structure, _tableName, _log);
            Log($"Таблица {_tableName} готова");
        }

        public void CollectAndSave()
        {
            _db.Query($"DELETE FROM \"{_tableName}\"", _log);
    
            try
            {
                var zp = ZennoProcesses();
                var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
                foreach (string[] arr in zp)
                {
                    int pid = int.Parse(arr[3]);
                    string commandLine = GetCommandLine(pid);
            
                    string query = _isPostgres
                        ? $@"INSERT INTO ""{_tableName}"" (id, name, ram, uptime, command_line, updated_at)
                     VALUES ({pid}, '{Escape(arr[0])}', '{arr[1]}', '{arr[2]}', '{Escape(commandLine)}', '{now}')
                     ON CONFLICT (id) DO UPDATE SET 
                     name = EXCLUDED.name,
                     ram = EXCLUDED.ram,
                     uptime = EXCLUDED.uptime,
                     command_line = EXCLUDED.command_line,
                     updated_at = EXCLUDED.updated_at"
                        : $@"INSERT OR REPLACE INTO ""{_tableName}"" (id, name, ram, uptime, command_line, updated_at)
                     VALUES ({pid}, '{Escape(arr[0])}', '{arr[1]}', '{arr[2]}', '{Escape(commandLine)}', '{now}')";
            
                    _db.Query(query, _log);
                }
        
                Log($"Сохранено {zp.Count} процессов в {_tableName}");
            }
            catch (Exception ex)
            {
                Log($"Ошибка сбора процессов: {ex.Message}");
            }
        }
        private string GetCommandLine(int pid)
        {
            try
            {
                using (var searcher = new System.Management.ManagementObjectSearcher(
                           $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {pid}"))
                {
                    foreach (System.Management.ManagementObject obj in searcher.Get())
                    {
                        return obj["CommandLine"]?.ToString() ?? "";
                    }
                }
            }
            catch { }
            return "";
        }
        

        /// <summary>
        /// Экспорт процессов текущей машины в JSON (для отправки на другую машину)
        /// </summary>
        public string ExportToJson()
        {
            var processes = new List<ProcessInfo>();
    
            try
            {
                var zp = ZennoProcesses();
                foreach (string[] arr in zp)
                {
                    int pid = int.Parse(arr[3]);
                    string commandLine = GetCommandLine(pid);
            
                    processes.Add(new ProcessInfo
                    {
                        Pid = pid,
                        Name = arr[0],
                        Ram = arr[1],
                        Uptime = arr[2],
                        CommandLine = commandLine
                    });
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка экспорта: {ex.Message}");
            }
    
            return JsonConvert.SerializeObject(processes);
        }

        /// <summary>
        /// Генерирует JS файлы для всех машин из БД
        /// </summary>
        public void GenerateAllReports()
        {
            string reportsFolder = Path.Combine(_project.Path, ".reports");
            if (!Directory.Exists(reportsFolder)) Directory.CreateDirectory(reportsFolder);
            
            var tables = _db.GetTables(_log).Where(t => t.StartsWith("process_")).ToList();
            var machines = new List<string>();
            
            foreach (var table in tables)
            {
                string machineName = table.Replace("process_", "");
                machines.Add(machineName);
                
                GenerateReportFromDb(table, machineName, reportsFolder);
            }
            
            Log($"Сгенерированы отчеты для {machines.Count} машин: {string.Join(", ", machines)}");
        }

        /// <summary>
        /// Генерирует JS для конкретной машины из БД
        /// </summary>
        public void GenerateReportFromDb(string tableName = null, string machineName = null, string folder = null)
        {
            tableName = tableName ?? _tableName;
            machineName = machineName ?? _machineName;
            folder = folder ?? Path.Combine(_project.Path, ".reports");
            
            var lines = _db.GetLines("id, name, ram, uptime, command_line, updated_at", tableName, _log);
            
            var processes = new List<object>();
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split('¦');
                if (parts.Length < 5) continue;
                
                processes.Add(new {
                    pid = parts[0].Trim(),
                    name = parts[1].Trim(),
                    ram = parts[2].Trim(),
                    uptime = parts[3].Trim(),
                    commandLine = parts[4].Trim()
                });
                
                if (parts.Length >= 6 && !string.IsNullOrEmpty(parts[5].Trim()))
                    timestamp = parts[5].Trim();
            }
            
            var data = new {
                machineName = machineName.Replace("_", "-"),
                timestamp = timestamp,
                processes = processes
            };
            
            string safeName = SafeName(machineName);
            string path = Path.Combine(folder, $"process_{safeName}.js");
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, $"window.processData_{safeName} = {json};", Encoding.UTF8);
            
            Log($"Отчет сохранен: process_{safeName}.js ({processes.Count} процессов)");
        }

        /// <summary>
        /// Возвращает список всех машин с данными процессов
        /// </summary>
        public List<string> GetAllMachines()
        {
            return _db.GetTables(_log)
                .Where(t => t.StartsWith("process_"))
                .Select(t => t.Replace("process_", ""))
                .ToList();
        }

        private string GetCommandLine(string processName, out int pid)
        {
            pid = 0;
            try
            {
                var processes = System.Diagnostics.Process.GetProcessesByName(processName.Replace(".exe", ""));
                if (processes.Length > 0)
                {
                    var proc = processes[0];
                    pid = proc.Id;
                    
                    using (var searcher = new System.Management.ManagementObjectSearcher(
                        $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {proc.Id}"))
                    {
                        foreach (System.Management.ManagementObject obj in searcher.Get())
                        {
                            return obj["CommandLine"]?.ToString() ?? "";
                        }
                    }
                }
            }
            catch { }
            return "";
        }

        private string Escape(string s) => s?.Replace("'", "''") ?? "";

        private class ProcessInfo
        {
            public int Pid { get; set; }
            public string Name { get; set; }
            public string Ram { get; set; }
            public string Uptime { get; set; }
            public string CommandLine { get; set; }
        }
    }
}