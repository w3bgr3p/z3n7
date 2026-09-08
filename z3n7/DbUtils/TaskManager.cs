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


namespace z3n7.DbUtils
{
    public static class TaskManager
    {
        internal static (string xmlB64, string jsonB64) XmlToPayload(string xml)
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

        internal static string PayloadToXml(string xmlB64, string jsonB64)
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

        internal static string JsonToXml(string json)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (dict == null) return string.Empty;
            var sb = new StringBuilder();
            foreach (var kv in dict)
                sb.Append($"<{kv.Key}>{kv.Value}</{kv.Key}>");
            return sb.ToString();
        }
    }
}
