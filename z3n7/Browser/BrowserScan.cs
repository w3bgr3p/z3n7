
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3nIO
{
    public class BrowserScan
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly Instance _instance;
        private readonly Logger _logger;
        private readonly Time.Sleeper _idle;

        public BrowserScan(IZennoPosterProjectModel project, Instance instance, Logger log = null)
        {
            _project = project;
            _instance = instance;
            _logger =log;
            _idle = new Time.Sleeper(3000, 5000);
        }

        private void AddTable()
        {
            //var sql = new Sql(_project);
            var columns = new List<string> { "score", "webgl", "webglreport", "unmaskedrenderer", "audio", "clientRects", "WebGPUReport", "Fonts", "TimeZoneBasedonIP", "TimeFromIP" };

            var tableStructure = _project.TblForProject(columns);
            var tblName = "_browserscan";
            
            _project.TblAdd(tableStructure,tblName);
            _project.ClmnAdd(tableStructure,tblName);
            _project.ClmnPrune(tableStructure,tblName);
            _project.AddRange(tblName);
        }

        private void LoadStats()
        {
            _instance.Go("https://www.browserscan.net/", true);
            _project.Deadline();
            while (true)
            {
                _logger?.Send("still loading...");
                _idle.Sleep();
                try
                {
                    _project.Deadline(60);
                }
                catch
                {
                    _logger.Warn("took too long. Skipping... ");
                    break;
                }

                if (_instance.ActiveTab.FindElementByAttribute("div", "outerhtml", "use xlink:href=\"#etc2\"", "regexp", 0)
                    .IsNull)
                {
                    _logger?.Send("loaded");
                    break;
                }
            }
            
        }

        public Dictionary<string,string> ParseStats()
        {
            AddTable();
            //var _sql = new Sql(_project);
            var toParse = "WebGL,WebGLReport, Audio, ClientRects, WebGPUReport,Fonts,TimeZoneBasedonIP,TimeFromIP";
            var tableName = "_browserscan";
            string timezoneOffset = "";
            string timezoneName = "";

            LoadStats();
            var stats = new Dictionary<string, string>();
            var hardware = _instance.ActiveTab.FindElementById("webGL_anchor").ParentElement.GetChildren(false);

            foreach (ZennoLab.CommandCenter.HtmlElement child in hardware)
            {
                var text = child.GetAttribute("innertext");
                var varName = Regex.Replace(text.Split('\n')[0], " ", ""); var varValue = "";
                if (varName == "") continue;
                if (toParse.Contains(varName))
                {
                    try { varValue = text.Split('\n')[2]; } catch { Thread.Sleep(2000); continue; }
                    var upd = $"{varName} = '{varValue}'";
                    stats.Add(varName, varValue);
                    //upd = QuoteColumnNames(upd);
                    _project.DbUpd(upd, tableName);
                }
            }

            var software = _instance.ActiveTab.FindElementById("lang_anchor").ParentElement.GetChildren(false);
            foreach (ZennoLab.CommandCenter.HtmlElement child in software)
            {
                var text = child.GetAttribute("innertext");
                var varName = Regex.Replace(text.Split('\n')[0], " ", ""); var varValue = "";
                if (varName == "") continue;
                if (toParse.Contains(varName))
                {
                    if (varName == "TimeZone") continue;
                    try { varValue = text.Split('\n')[1]; } catch { continue; }
                    if (varName == "TimeFromIP") timezoneOffset = varValue;
                    if (varName == "TimeZoneBasedonIP") timezoneName = varValue;
                    var upd = $"{varName} = '{varValue}'";
                    stats.Add(varName, varValue);
                    //upd = QuoteColumnNames(upd);
                    _project.DbUpd(upd, tableName);
                }
            }
            return stats;

        }

        public string GetScore()
        {
            LoadStats();
            string heToWait = _instance.HeGet(("anchor_progress", "id"));
            var score = heToWait.Split(' ')[3].Split('\n')[0]; var problems = "";
            if (!score.Contains("100%"))
            {
                var probDic = Problems();
                problems = string.Join(" ,", probDic.Keys); 
            }
            score = $"[{score}] {problems}";
            return score;
        }
        
        public Dictionary<string,string> Problems()
        {
            LoadStats();
            var prblems = new Dictionary<string, string>();
            string heToWait = _instance.HeGet(("anchor_progress", "id"));
            var score = heToWait.Split(' ')[3].Split('\n')[0]; ;
            if (!score.Contains("100%"))
            {
                var problemsHe = _instance.ActiveTab.FindElementByAttribute("ul", "fulltagname", "ul", "regexp", 5).GetChildren(false);
                foreach (ZennoLab.CommandCenter.HtmlElement child in problemsHe)
                {
                    var text = child.GetAttribute("innertext");
                    var varValue = "";
                    var varName = text.Split('\n')[0];
                    try { varValue = text.Split('\n')[1]; } catch { continue; }
                    ;
                    prblems.Add(varName, varValue);
                }
            }
            return prblems;
        }
        
        public string FixTime()
        {
            LoadStats();
            string timezoneOffset = "";
            string timezoneName = "";
            var toParse = "TimeZoneBasedonIP,TimeFromIP";

            var software = _instance.ActiveTab.FindElementById("lang_anchor").ParentElement.GetChildren(false);
            foreach (ZennoLab.CommandCenter.HtmlElement child in software)
            {
                var text = child.GetAttribute("innertext");
                var varName = Regex.Replace(text.Split('\n')[0], " ", "");
                var varValue = "";
                if (varName == "") continue;
                if (toParse.Contains(varName))
                {
                    if (varName == "TimeZone") continue;
                    try { varValue = text.Split('\n')[1]; } catch { continue; }
                    if (varName == "TimeFromIP") timezoneOffset = varValue;
                    if (varName == "TimeZoneBasedonIP") timezoneName = varValue;
                }
            }

            int appliedOffset = 0;
            string appliedTimezoneName = timezoneName;

            var match = Regex.Match(timezoneOffset, @"GMT([+-]\d{2})");
            if (match.Success)
            {
                appliedOffset = int.Parse(match.Groups[1].Value);
                _logger?.Send($"Setting timezone offset to: {appliedOffset}");
                _instance.TimezoneWorkMode = ZennoLab.InterfacesLibrary.Enums.Browser.TimezoneMode.Emulate;
                _instance.SetTimezone(appliedOffset, 0);
            }
    
            _instance.SetIanaTimezone(timezoneName);
    
            // Формируем JSON строку
            var result = new
            {
                timezoneOffset = appliedOffset,
                timezoneName = appliedTimezoneName,
            };
            return System.Text.Json.JsonSerializer.Serialize(result);
            
        }
        

    }
    public static partial class ProjectExtensions
    {
        public static void FixTime(this IZennoPosterProjectModel project, Instance instance, Logger log = null)
        {
            try
            {
                instance.Go("https://www.browserscan.net/");
                new BrowserScan(project, instance, log).FixTime();
            }
            catch (Exception ex)
            {
                project.warn(ex.Message);
            }
        }
    }
}
