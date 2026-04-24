using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3nIO.DbUtils
{
    public static class ProcessManager
    {
        //private static readonly string _processTable =  DbSchema.Process.TableName;
        private static readonly string _machine = Environment.MachineName;

        private static string MakeId(int pid)   => $"{pid}|{_machine}";
        private static string MachineFilter()   => $"\"machine\" = '{_machine}'";

        // ── INIT ──────────────────────────────────────────────────────────────

        
        
        public static void EnsureProcessTable(this IZennoPosterProjectModel project, bool log = false)
        {
            project.TblAdd(DbSchema.Process.Columns, DbSchema.Process.Name);
        }
        
        // ── COLLECT ───────────────────────────────────────────────────────────

        public static void CollectAndSave(this IZennoPosterProjectModel project, bool log = false)
        {
            var isPg  = project.Var("DBmode") == "PostgreSQL";
            var now   = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var procs = ZennoProcesses();
            if (procs.Count == 0) return;

            var values = procs.Select(arr =>
            {
                int    pid         = int.Parse(arr[3]);
                string id          = MakeId(pid);
                string name        = Escape(arr[0]);
                string commandLine = Escape(GetCommandLine(pid));
                return $"('{id}', '{_machine}', '{name}', '{arr[1]}', '{arr[2]}', '{commandLine}', '{now}')";
            }).ToList();

            string cols   = "id, machine, name, ram, uptime, command_line, updated_at";
            string batch  = string.Join(", ", values);

            string query = isPg
                ? $@"INSERT INTO ""{DbSchema.Process.Name}"" ({cols}) VALUES {batch}
             ON CONFLICT (id) DO UPDATE SET
                 machine      = EXCLUDED.machine,
                 name         = EXCLUDED.name,
                 ram          = EXCLUDED.ram,
                 uptime       = EXCLUDED.uptime,
                 command_line = EXCLUDED.command_line,
                 updated_at   = EXCLUDED.updated_at"
                : $@"INSERT OR REPLACE INTO ""{DbSchema.Process.Name}"" ({cols}) VALUES {batch}";

            project.DbQ(query, log);
            
            var activePids = string.Join(", ", procs.Select(arr => $"'{MakeId(int.Parse(arr[3]))}'"));

            string cleanup = activePids.Length > 0
                ? $"DELETE FROM \"{DbSchema.Process.Name}\" WHERE \"machine\" = '{_machine}' AND \"id\" NOT IN ({activePids})"
                : $"DELETE FROM \"{DbSchema.Process.Name}\" WHERE \"machine\" = '{_machine}'";

            project.DbQ(cleanup, log);

            if (log) project.SendInfoToLog($"[ProcessManager] Upserted {procs.Count} rows for {_machine}", false);
        }

        // ── READ ──────────────────────────────────────────────────────────────

        public static List<string> GetAllMachines(this IZennoPosterProjectModel project, bool log = false)
        {
            var db = new Db(project);
            return db.GetLines("machine", DbSchema.Process.Name, log, where: "1=1")
                .Select(r => r.Trim())
                .Where(r => !string.IsNullOrEmpty(r))
                .Distinct()
                .ToList();
        }

        // ── KILL BY UPTIME ────────────────────────────────────────────────────

        public static void KillByUptime(this IZennoPosterProjectModel project, int maxUptimeMinutes, bool log = false)
        {
            int killed = 0;

            foreach (var proc in System.Diagnostics.Process.GetProcessesByName("zbe1"))
            {
                int uptime = (int)(DateTime.Now - proc.StartTime).TotalMinutes;
                if (uptime <= maxUptimeMinutes) continue;

                try
                {
                    proc.Kill();
                    killed++;
                    if (log) project.SendInfoToLog($"[ProcessManager] Killed zbe1 pid={proc.Id} uptime={uptime} min", false);
                }
                catch
                {
                    if (log) project.SendInfoToLog($"[ProcessManager] Kill failed pid={proc.Id}", false);
                }
            }

            if (log) project.SendInfoToLog($"[ProcessManager] KillByUptime: killed={killed}", false);

            if (killed > 0)
                project.CollectAndSave(log);
        }

        // ── ZP PROCESSES ──────────────────────────────────────────────────────

        public static List<string[]> ZennoProcesses()
        {
            var result = new List<string[]>();

            foreach (var name in new[] { "ZennoPoster", "zbe1" })
            {
                foreach (var proc in System.Diagnostics.Process.GetProcessesByName(name))
                {
                    int     uptime = (int)(DateTime.Now - proc.StartTime).TotalMinutes;
                    long    ram    = proc.WorkingSet64 / (1024 * 1024);
                    result.Add(new[] { proc.ProcessName, ram.ToString(), uptime.ToString(), proc.Id.ToString() });
                }
            }

            return result;
        }

        // ── PRIVATE ───────────────────────────────────────────────────────────

        private static string GetCommandLine(int pid)
        {
            try
            {
                using var searcher = new System.Management.ManagementObjectSearcher(
                    $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {pid}");
                foreach (System.Management.ManagementObject obj in searcher.Get())
                    return obj["CommandLine"]?.ToString() ?? "";
            }
            catch { }
            return "";
        }

        private static string Escape(string s) => s?.Replace("'", "''") ?? "";
    }
}