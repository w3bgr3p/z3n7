using System;
using System.Collections;
using System.Globalization;
using System.Reflection;

namespace z3n7
{
    /// <summary>
    /// Чтение состояния Windows Firewall и создание правила через COM
    /// (HNetCfg.FwPolicy2), поздним связыванием — чтобы не тянуть ссылку
    /// на NetFwTypeLib и Microsoft.CSharp.
    ///
    /// «Не знаю» возвращается как null, а не как false: отличать «правила нет»
    /// от «не смогли посмотреть» здесь важнее, чем выдать ответ любой ценой.
    /// </summary>
    internal static class Firewall
    {
        private const BindingFlags Get  = BindingFlags.GetProperty;
        private const BindingFlags Set  = BindingFlags.SetProperty;
        private const BindingFlags Call = BindingFlags.InvokeMethod;

        private const int ProfileDomain  = 1;
        private const int ProfilePrivate = 2;
        private const int ProfilePublic  = 4;
        private const int DirectionIn    = 1;
        private const int ActionAllow    = 1;
        private const int ProtocolTcp    = 6;

        public static string RuleName(int port) => $"z3n7-ZpServer-{port}";

        private static object Policy()
        {
            var t = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            return t == null ? null : Activator.CreateInstance(t);
        }

        /// <summary>true — включён хотя бы в одном профиле; null — узнать не удалось.</summary>
        public static bool? Enabled()
        {
            try
            {
                var fw = Policy();
                if (fw == null) return null;

                var t = fw.GetType();
                foreach (var profile in new[] { ProfileDomain, ProfilePrivate, ProfilePublic })
                    if ((bool)t.InvokeMember("FirewallEnabled", Get, null, fw, new object[] { profile }))
                        return true;

                return false;
            }
            catch { return null; }
        }

        /// <summary>
        /// Есть ли включённое inbound-allow правило, накрывающее порт.
        ///
        /// Ответ приблизительный, и это намеренно: правила «по пути к exe» сюда
        /// не попадают, block-правила не учитываются, привязка к профилю и
        /// remote address игнорируется. Подсказка, а не вердикт о доступности.
        /// </summary>
        public static bool? HasPortRule(int port)
        {
            try
            {
                var fw = Policy();
                if (fw == null) return null;

                var rules = fw.GetType().InvokeMember("Rules", Get, null, fw, null);
                if (!(rules is IEnumerable list)) return null;

                var self = SelfPath();

                foreach (var rule in list)
                {
                    var t = rule.GetType();
                    if (!(bool)t.InvokeMember("Enabled",   Get, null, rule, null)) continue;
                    if ((int) t.InvokeMember("Direction",  Get, null, rule, null) != DirectionIn) continue;
                    if ((int) t.InvokeMember("Action",     Get, null, rule, null) != ActionAllow) continue;
                    if ((int) t.InvokeMember("Protocol",   Get, null, rule, null) != ProtocolTcp) continue;

                    if (!Covers(t.InvokeMember("LocalPorts", Get, null, rule, null) as string, port))
                        continue;

                    // Правило, привязанное к службе, нас не касается.
                    var service = t.InvokeMember("ServiceName", Get, null, rule, null) as string;
                    if (!string.IsNullOrEmpty(service)) continue;

                    // Привязанное к чужому exe — тоже. Без проверки почти на любой
                    // машине нашлось бы LocalPorts="*" от постороннего приложения,
                    // и ответ был бы "правило есть" для любого порта.
                    var app = t.InvokeMember("ApplicationName", Get, null, rule, null) as string;
                    if (string.IsNullOrEmpty(app)) return true;
                    if (self != null && string.Equals(app, self, StringComparison.OrdinalIgnoreCase)) return true;
                }

                return false;
            }
            catch { return null; }
        }

        /// <summary>Создаёт inbound-allow правило на TCP-порт. Требует прав администратора.</summary>
        public static bool TryOpen(int port, out string error)
        {
            error = null;
            try
            {
                var fw = Policy();
                if (fw == null) { error = "HNetCfg.FwPolicy2 недоступен"; return false; }

                var t = Type.GetTypeFromProgID("HNetCfg.FWRule");
                if (t == null) { error = "HNetCfg.FWRule недоступен"; return false; }

                var rule = Activator.CreateInstance(t);

                // Protocol строго до LocalPorts — иначе COM отказывает в записи портов.
                t.InvokeMember("Name",       Set, null, rule, new object[] { RuleName(port) });
                t.InvokeMember("Protocol",   Set, null, rule, new object[] { ProtocolTcp });
                t.InvokeMember("LocalPorts", Set, null, rule, new object[] { port.ToString(CultureInfo.InvariantCulture) });
                t.InvokeMember("Direction",  Set, null, rule, new object[] { DirectionIn });
                t.InvokeMember("Action",     Set, null, rule, new object[] { ActionAllow });
                t.InvokeMember("Enabled",    Set, null, rule, new object[] { true });
                t.InvokeMember("Profiles",   Set, null, rule, new object[] { ProfileDomain | ProfilePrivate | ProfilePublic });

                var rules = fw.GetType().InvokeMember("Rules", Get, null, fw, null);
                rules.GetType().InvokeMember("Add", Call, null, rules, new[] { rule });
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>Полный путь к текущему exe, или null если узнать не вышло.</summary>
        private static string SelfPath()
        {
            try   { return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName; }
            catch { return null; }
        }

        /// <summary>Понимает форматы LocalPorts: "*", "22222", "80,443", "22200-22300".</summary>
        private static bool Covers(string localPorts, int port)
        {
            if (string.IsNullOrEmpty(localPorts)) return false;

            foreach (var part in localPorts.Split(','))
            {
                var s = part.Trim();
                if (s == "*") return true;

                var dash = s.IndexOf('-');
                if (dash > 0)
                {
                    if (int.TryParse(s.Substring(0, dash), out var lo) &&
                        int.TryParse(s.Substring(dash + 1), out var hi) &&
                        port >= lo && port <= hi)
                        return true;
                }
                else if (int.TryParse(s, out var one) && one == port)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
