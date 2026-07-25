using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.ServiceProcess;


namespace z3n7.Tools
{
public static class SystemSnapshot
    {
        // ── Public API ────────────────────────────────────────────────────────
 
        public static string Collect()
        {
            var sb = new StringBuilder(1 << 20);
            Build(sb);
            return sb.ToString();
        }
 
        public static void CollectToFile(string path)
        {
            var text = Collect();
            File.WriteAllText(path, text, Encoding.UTF8);
        }
 
        // ── Builder ───────────────────────────────────────────────────────────
 
        private static void Build(StringBuilder sb)
        {
            var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var hr = new string('=', 72);
 
            Action<string> line    = s => sb.AppendLine(s);
            Action<string> section = t => { line(""); line(hr); line("## " + t); line(hr); };
 
            // ── COLLECT ONCE ──────────────────────────────────────────────────
            var allProcs = Process.GetProcesses();
 
            var pidName = new Dictionary<int, string>(allProcs.Length);
            foreach (var p in allProcs)
                pidName[p.Id] = p.ProcessName;
 
            var tcpRows   = GetTcpRowsWithPid();
            var connByPid = new Dictionary<int, int>();
            foreach (var r in tcpRows)
            {
                int cur;
                connByPid.TryGetValue(r.Pid, out cur);
                connByPid[r.Pid] = cur + 1;
            }
 
            System.Net.IPEndPoint[] udpListeners;
            try   { udpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners(); }
            catch { udpListeners = new System.Net.IPEndPoint[0]; }
 
            // ── HEADER ────────────────────────────────────────────────────────
            var uptime = TimeSpan.FromMilliseconds((double)GetTickCount64());
 
            line("SYSTEM SNAPSHOT FOR LLM ANALYSIS");
            line("Captured : " + ts);
            line("Hostname : " + Environment.MachineName);
            line("OS       : " + Environment.OSVersion.ToString());
            line("Uptime   : " + uptime.Days + "d " + uptime.Hours + "h " + uptime.Minutes + "m");
 
            // ── MEMORY ────────────────────────────────────────────────────────
            section("SYSTEM MEMORY SUMMARY");
            var mem = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX)) };
            if (GlobalMemoryStatusEx(ref mem))
            {
                var totalGB = Math.Round(mem.ullTotalPhys / 1073741824.0, 2);
                var freeGB  = Math.Round(mem.ullAvailPhys / 1073741824.0, 2);
                var usedGB  = Math.Round(totalGB - freeGB, 2);
                var pct     = Math.Round(usedGB / totalGB * 100.0, 1);
                line("Total    : " + totalGB + " GB");
                line("Used     : " + usedGB + " GB  (" + pct + "%)");
                line("Free     : " + freeGB + " GB");
            }
 
            // ── CPU ───────────────────────────────────────────────────────────
            section("CPU SUMMARY");
            line("Logical CPUs : " + Environment.ProcessorCount);
            try
            {
                using (var c = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
                {
                    c.NextValue();
                    Thread.Sleep(400);
                    line("Load         : " + Math.Round(c.NextValue(), 1) + "%");
                }
            }
            catch { line("Load         : n/a"); }
 
            // ── PROCESS AGGREGATION ───────────────────────────────────────────
            section("PROCESS AGGREGATION BY NAME (ALL INSTANCES SUMMED)");
            line(Fmt("NAME", -35) + Fmt("TOTAL_MEM_MB", -12) + Fmt("INSTANCES", -10) + Fmt("TCP_CONNS", -12) + "AVG_MEM_MB");
            line(new string('-', 80));
 
            var aggregated = allProcs
                .GroupBy(p => p.ProcessName)
                .Select(g =>
                {
                    long totalMem = 0;
                    foreach (var p in g)
                        try { totalMem += p.WorkingSet64; } catch { }
                    var totalMB = Math.Round(totalMem / 1048576.0, 1);
                    var tcp = g.Sum(p => { int c; return connByPid.TryGetValue(p.Id, out c) ? c : 0; });
                    return new ProcGroup
                    {
                        Name    = g.Key,
                        TotalMB = totalMB,
                        Count   = g.Count(),
                        Tcp     = tcp,
                        AvgMB   = Math.Round(totalMB / g.Count(), 1)
                    };
                })
                .OrderByDescending(x => x.TotalMB);
 
            foreach (var g in aggregated)
            {
                var name = g.Name.Length > 35 ? g.Name.Substring(0, 35) : g.Name;
                line(Fmt(name, -35) + Fmt(g.TotalMB.ToString(), -12) + Fmt(g.Count.ToString(), -10) +
                     Fmt(g.Tcp.ToString(), -12) + g.AvgMB);
            }
 
            // ── ALL PROCESSES ─────────────────────────────────────────────────
            section("ALL PROCESSES (PID | NAME | MEM_MB | CPU_SEC | THREADS | START_TIME)");
            line(Fmt("PID", -8) + Fmt("NAME", -35) + Fmt("MEM_MB", -10) + Fmt("CPU_SEC", -12) + Fmt("THREADS", -8) + "STARTED");
            line(new string('-', 90));
 
            foreach (var p in allProcs.OrderBy(p => p.ProcessName))
            {
                var mem2    = 0.0; var cpu = 0.0; var thr = 0; var started = "n/a";
                try { mem2    = Math.Round(p.WorkingSet64 / 1048576.0, 1); }             catch { }
                try { cpu     = Math.Round(p.TotalProcessorTime.TotalSeconds, 1); }      catch { }
                try { thr     = p.Threads.Count; }                                        catch { }
                try { started = p.StartTime.ToString("HH:mm:ss"); }                       catch { }
                var name = p.ProcessName.Length > 35 ? p.ProcessName.Substring(0, 35) : p.ProcessName;
                line(Fmt(p.Id.ToString(), -8) + Fmt(name, -35) + Fmt(mem2.ToString(), -10) +
                     Fmt(cpu.ToString(), -12) + Fmt(thr.ToString(), -8) + started);
            }
 
            // ── ACTIVE CONNECTIONS ────────────────────────────────────────────
            section("ACTIVE NETWORK CONNECTIONS (TCP + UDP)");
            line(Fmt("PID", -10) + Fmt("PROTO", -7) + Fmt("LOCAL", -26) + Fmt("REMOTE", -26) + Fmt("STATE", -14) + "PROCESS");
            line(new string('-', 100));
 
            foreach (var r in tcpRows.OrderBy(r => r.LocalPort))
            {
                string pn;
                var proc   = pidName.TryGetValue(r.Pid, out pn) ? pn : "?";
                var local  = r.LocalAddr + ":" + r.LocalPort;
                var remote = r.RemotePort > 0 ? r.RemoteAddr + ":" + r.RemotePort : "-";
                line(Fmt(r.Pid.ToString(), -10) + Fmt("TCP", -7) + Fmt(local, -26) + Fmt(remote, -26) + Fmt(r.State, -14) + proc);
            }
            foreach (var ep in udpListeners.OrderBy(e => e.Port))
                line(Fmt("?", -10) + Fmt("UDP", -7) + Fmt(ep.Address + ":" + ep.Port, -26) + Fmt("-", -26) + Fmt("LISTEN", -14) + "?");
 
            // ── LISTENING ─────────────────────────────────────────────────────
            section("LISTENING PORTS SUMMARY (TCP)");
            line(Fmt("PID", -8) + Fmt("PORT", -8) + Fmt("BIND_ADDR", -20) + "PROCESS");
            line(new string('-', 55));
            foreach (var r in tcpRows.Where(r => r.State == "Listen").OrderBy(r => r.LocalPort))
            {
                string pn;
                line(Fmt(r.Pid.ToString(), -8) + Fmt(r.LocalPort.ToString(), -8) + Fmt(r.LocalAddr, -20) +
                     (pidName.TryGetValue(r.Pid, out pn) ? pn : "?"));
            }
 
            // ── ESTABLISHED ───────────────────────────────────────────────────
            section("ESTABLISHED TCP CONNECTIONS");
            line(Fmt("PID", -10) + Fmt("LOCAL", -26) + Fmt("REMOTE", -26) + "PROCESS");
            line(new string('-', 80));
            foreach (var r in tcpRows.Where(r => r.State == "Established").OrderBy(r => r.Pid))
            {
                string pn;
                line(Fmt(r.Pid.ToString(), -10) +
                     Fmt(r.LocalAddr + ":" + r.LocalPort, -26) +
                     Fmt(r.RemoteAddr + ":" + r.RemotePort, -26) +
                     (pidName.TryGetValue(r.Pid, out pn) ? pn : "?"));
            }
 
            // ── SERVICES ──────────────────────────────────────────────────────
            section("RUNNING WINDOWS SERVICES");
            line(Fmt("NAME", -50) + Fmt("STATUS", -12) + "DISPLAY");
            line(new string('-', 100));
            try
            {
                foreach (var s in ServiceController.GetServices()
                    .Where(s => s.Status == ServiceControllerStatus.Running)
                    .OrderBy(s => s.ServiceName))
                {
                    var sname   = s.ServiceName.Length > 50 ? s.ServiceName.Substring(0, 50) : s.ServiceName;
                    var display = s.DisplayName.Length  > 60 ? s.DisplayName.Substring(0, 60)  : s.DisplayName;
                    line(Fmt(sname, -50) + Fmt("Running", -12) + display);
                }
            }
            catch (Exception ex) { line("Error: " + ex.Message); }
 
            // ── DISK ──────────────────────────────────────────────────────────
            section("DISK USAGE");
            line(Fmt("DRIVE", -6) + Fmt("TOTAL_GB", -12) + Fmt("USED_GB", -12) + Fmt("FREE_GB", -12) + "PCT_USED");
            line(new string('-', 55));
            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    if (drive.DriveType != DriveType.Fixed && drive.DriveType != DriveType.Network) continue;
                    if (!drive.IsReady) continue;
                    var total = Math.Round(drive.TotalSize      / 1073741824.0, 1);
                    var free  = Math.Round(drive.TotalFreeSpace / 1073741824.0, 1);
                    var used  = Math.Round(total - free, 1);
                    var pct   = total > 0 ? Math.Round(used / total * 100.0, 1) : 0.0;
                    line(Fmt(drive.Name.TrimEnd('\\'), -6) + Fmt(total.ToString(), -12) +
                         Fmt(used.ToString(), -12) + Fmt(free.ToString(), -12) + pct + "%");
                }
                catch { }
            }
 
            // ── ENVIRONMENT ───────────────────────────────────────────────────
            section("ENVIRONMENT MARKERS");
            line("USERNAME    : " + Environment.UserName);
            line("USERPROFILE : " + Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            line("TEMP        : " + Path.GetTempPath());
            line("CLR ver     : " + Environment.Version);
            line("OS 64-bit   : " + Environment.Is64BitOperatingSystem);
 
            // ── PATH ──────────────────────────────────────────────────────────
            section("PATH ENTRIES");
            line(Fmt("IDX", -5) + "PATH");
            line(new string('-', 80));
            var pathVar     = Environment.GetEnvironmentVariable("PATH") ?? "";
            var pathEntries = pathVar.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < pathEntries.Length; i++)
                line(Fmt((i + 1).ToString(), -5) + pathEntries[i]);
            line("");
            line("Total entries: " + pathEntries.Length);
 
            // ── FOOTER ────────────────────────────────────────────────────────
            line("");
            line(hr);
            line("END OF SNAPSHOT");
            line(hr);
        }
 
        // ── P/Invoke: GetExtendedTcpTable ─────────────────────────────────────
 
        private class TcpRow
        {
            public int    Pid;
            public string LocalAddr;
            public int    LocalPort;
            public string RemoteAddr;
            public int    RemotePort;
            public string State;
        }
 
        private static List<TcpRow> GetTcpRowsWithPid()
        {
            var rows = new List<TcpRow>();
            try
            {
                int size = 0;
                GetExtendedTcpTable(IntPtr.Zero, ref size, false, 2, 4, 0);
                var buf = Marshal.AllocHGlobal(size);
                try
                {
                    if (GetExtendedTcpTable(buf, ref size, false, 2, 4, 0) != 0) return rows;
                    int count    = Marshal.ReadInt32(buf);
                    const int rowSize = 24; // MIB_TCPROW_OWNER_PID
                    for (int i = 0; i < count; i++)
                    {
                        var ptr   = IntPtr.Add(buf, 4 + i * rowSize);
                        var state = Marshal.ReadInt32(ptr, 0);
                        var lAddr = Marshal.ReadInt32(ptr, 4);
                        var lPort = Marshal.ReadInt32(ptr, 8);
                        var rAddr = Marshal.ReadInt32(ptr, 12);
                        var rPort = Marshal.ReadInt32(ptr, 16);
                        var pid   = Marshal.ReadInt32(ptr, 20);
                        rows.Add(new TcpRow
                        {
                            Pid        = pid,
                            LocalAddr  = Ip(lAddr),
                            LocalPort  = Ntohs(lPort),
                            RemoteAddr = Ip(rAddr),
                            RemotePort = Ntohs(rPort),
                            State      = TcpStateName(state)
                        });
                    }
                }
                finally { Marshal.FreeHGlobal(buf); }
            }
            catch { }
            return rows;
        }
 
        private static string Ip(int raw)
        {
            var b = BitConverter.GetBytes(raw);
            return b[0] + "." + b[1] + "." + b[2] + "." + b[3];
        }
 
        private static int Ntohs(int raw)
        {
            var b = BitConverter.GetBytes(raw);
            return (b[2] << 8) | b[3];
        }
 
        private static string TcpStateName(int s)
        {
            switch (s)
            {
                case 1:  return "Closed";
                case 2:  return "Listen";
                case 3:  return "SynSent";
                case 4:  return "SynReceived";
                case 5:  return "Established";
                case 6:  return "FinWait1";
                case 7:  return "FinWait2";
                case 8:  return "CloseWait";
                case 9:  return "Closing";
                case 10: return "LastAck";
                case 11: return "TimeWait";
                case 12: return "DeleteTcb";
                default: return "Unknown";
            }
        }
 
        // ── String helpers ────────────────────────────────────────────────────
 
        // PadRight for positive width, PadLeft for negative (mimics C# composite format alignment)
        private static string Fmt(string s, int width)
        {
            if (width < 0)
            {
                int w = -width;
                return s.Length >= w ? s.Substring(0, w) : s.PadRight(w);
            }
            return s.Length >= width ? s.Substring(0, width) : s.PadLeft(width);
        }
 
        private static string Fmt(object o, int width) => Fmt(o?.ToString() ?? "", width);
 
        // ── Helper struct ─────────────────────────────────────────────────────
 
        private class ProcGroup
        {
            public string Name;
            public double TotalMB;
            public int    Count;
            public int    Tcp;
            public double AvgMB;
        }
 
        // ── P/Invoke declarations ─────────────────────────────────────────────
 
        [DllImport("kernel32.dll")]
        private static extern ulong GetTickCount64();
 
        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern int GetExtendedTcpTable(IntPtr pTcpTable, ref int dwSize, bool sort,
            int ipVersion, int tableClass, int reserved);
 
        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            public uint  dwLength;
            public uint  dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }
 
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);
    }
}