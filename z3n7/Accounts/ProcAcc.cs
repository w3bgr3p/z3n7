
using System;
using System.Collections.Generic;
using ZennoLab.InterfacesLibrary.ProjectModel;


using System.Diagnostics;

using System.IO;
using System.Linq;

using System.Text.RegularExpressions;



namespace z3n7.Utilities
{
/// <summary>
    /// Управление связями между процессами (PID) и аккаунтами (ACC)
    /// </summary>
    internal static class ProcAcc
    {
        // Кеш: сканируем процессы только 1 раз
        private static Dictionary<int, string> _cache = null;
        private static DateTime _cacheTime = DateTime.MinValue;
        private static readonly object _cacheLock = new object();
        private static readonly int _cacheLifetimeMs = 2000; // 2 секунды
        
        // ============== ОСНОВНЫЕ МЕТОДЫ ==============
        
        /// <summary>
        /// Получить все связи PID → ACC (с кешированием)
        /// </summary>
        private static Dictionary<int, string> GetAllPidAcc(bool forceRefresh = false)
        {
            lock (_cacheLock)
            {
                if (!forceRefresh && _cache != null && 
                    (DateTime.Now - _cacheTime).TotalMilliseconds < _cacheLifetimeMs)
                {
                    return new Dictionary<int, string>(_cache);
                }
                
                _cache = ScanAll();
                _cacheTime = DateTime.Now;
                
                return new Dictionary<int, string>(_cache);
            }
        }
        
        /// <summary>
        /// Получить все PID для аккаунта
        /// </summary>
        private static List<int> GetPids(string acc)
        {
            if (string.IsNullOrEmpty(acc)) return new List<int>();
            
            acc = Normalize(acc);
            var all = GetAllPidAcc();
            
            return all.Where(x => Normalize(x.Value) == acc)
                      .Select(x => x.Key)
                      .ToList();
        }
        

        /// <summary>
        /// Сбросить кеш (вызывать после Kill)
        /// </summary>
        private static void ClearCache()
        {
            lock (_cacheLock)
            {
                _cache = null;
            }
        }
        
        // ============== FAST API ==============

        /// <summary>
        /// Быстрый поиск нового PID для только что запущенного браузера
        /// Ищет только среди процессов, которых не было до запуска
        /// </summary>
        internal static int GetNewlyLaunchedPid(string acc, HashSet<int> pidsBeforeLaunch, int maxAttempts = 10, int delayMs = 100)
        {
            if (string.IsNullOrEmpty(acc)) return 0;
            acc = Normalize(acc);
            
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Получаем только новые процессы zbe1
                var currentPids = zbe1();
                var newPids = currentPids.Where(p => !pidsBeforeLaunch.Contains(p)).ToList();
                
                if (newPids.Count == 0)
                {
                    if (attempt < maxAttempts - 1)
                        System.Threading.Thread.Sleep(delayMs);
                    continue;
                }
                
                // Проверяем только новые процессы
                foreach (var pid in newPids)
                {
                    try
                    {
                        var foundAcc = GetAccFromPid(pid);
                        if (Normalize(foundAcc) == acc)
                        {
                            // Нашли! Обновляем кеш если нужно
                            ClearCache();
                            return pid;
                        }
                    }
                    catch { }
                }
                
                if (attempt < maxAttempts - 1)
                    System.Threading.Thread.Sleep(delayMs);
            }
            
            return 0;
        }

        /// <summary>
        /// Получить снимок всех PID процессов zbe1 (быстрый метод)
        /// </summary>
        internal static HashSet<int> GetPidSnapshot()
        {
            return new HashSet<int>(zbe1());
        }


        
        
        // ============== ВЫБОР ПО КРИТЕРИЯМ ==============
        
        /// <summary>
        /// Получить самый новый (молодой) PID
        /// </summary>
        internal static int GetNewest(string acc)
        {
            return GetBySelector(acc, (proc) => proc.StartTime, selectMax: true);
        }
        


        
        // ============== СЛУЖЕБНЫЕ МЕТОДЫ ==============
        
        /// <summary>
        /// ЕДИНСТВЕННОЕ место где происходит полное сканирование
        /// Все остальные методы используют результат этого метода
        /// </summary>
        private static Dictionary<int, string> ScanAll()
        {
            var result = new Dictionary<int, string>();
            var allPids = zbe1();
            
            foreach (var pid in allPids)
            {
                try
                {
                    var acc = GetAccFromPid(pid);
                    result[pid] = acc;
                }
                catch { }
            }
            
            return result;
        }
        private static List<int> zbe1()
        {
            var zProcesses = new List<int>();
            string[] processNames = new[] { "zbe1" }; 
            var allProcs = new List<System.Diagnostics.Process>();
    
            try
            {
                foreach (var processName in processNames)
                {
                    allProcs.AddRange(System.Diagnostics.Process.GetProcessesByName(processName));
                }

                if (allProcs.Count > 0)
                {
                    foreach (var proc in allProcs)
                    {
                        zProcesses.Add(proc.Id);
                    }
                }
        
                return zProcesses;
            }
            finally
            {
                // ОБЯЗАТЕЛЬНО освобождаем все Process объекты
                foreach (var proc in allProcs)
                {
                    proc?.Dispose();
                }
            }
        }
        
        /// <summary>
        /// Универсальный селектор процесса по критерию
        /// </summary>
        private static int GetBySelector<T>(string acc, Func<Process, T> selector, bool selectMax) 
            where T : IComparable<T>
        {
            var pids = GetPids(acc);
            if (pids.Count == 0) return 0;
            if (pids.Count == 1) return pids[0];
            
            int selectedPid = 0;
            T selectedValue = default(T);
            bool first = true;
            
            foreach (var pid in pids)
            {
                try
                {
                    using (var proc = Process.GetProcessById(pid))
                    {
                        T value = selector(proc);
                        
                        if (first)
                        {
                            selectedPid = pid;
                            selectedValue = value;
                            first = false;
                        }
                        else
                        {
                            int comparison = value.CompareTo(selectedValue);
                            
                            if ((selectMax && comparison > 0) || (!selectMax && comparison < 0))
                            {
                                selectedPid = pid;
                                selectedValue = value;
                            }
                        }
                    }
                }
                catch { }
            }
            
            return selectedPid;
        }
        
        private static string GetAccFromPid(int pid)
        {
            using (var searcher = new System.Management.ManagementObjectSearcher(
                "SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + pid))
            using (var collection = searcher.Get())
            {
                foreach (System.Management.ManagementObject obj in collection)
                {
                    using (obj)
                    {
                        var cmdLineObj = obj["CommandLine"];
                        if (cmdLineObj == null) return null;
                        
                        string commandLine = cmdLineObj.ToString();
                        var match = Regex.Match(commandLine, @"--user-data-dir=""([^""]+)""");
                        
                        if (!match.Success || string.IsNullOrEmpty(match.Groups[1].Value))
                            return null;
                        
                        var path = match.Groups[1].Value.Trim('\\');
                        return Path.GetFileName(path);
                    }
                }
            }
            
            return null;
        }
        
        private static string Normalize(string acc)
        {
            if (string.IsNullOrEmpty(acc)) return string.Empty;
            return acc.Replace("acc", "").Replace("ACC", "").Trim();
        }
        
        
    }
    



}

