using System;
using System.Collections.Generic;
using System.Linq;

using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    public class NumlexInstance
    {
        
        private readonly IZennoPosterProjectModel _project;
        private readonly Instance _instance;
 

        public NumlexInstance(IZennoPosterProjectModel project, Instance instance)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }


        public string ChoseDirection(bool pruneErrors = false, bool log = false)
        {
            
            var direction = "";
            var addErr  =  (pruneErrors) ? " AND error = ''" :"";
            var cooldownFilter = " AND (\"limit\" = '' OR \"limit\" <= to_char(NOW(), 'YYYY-MM-DD\"T\"HH24:MI:SS'))";
            var wkMode = _project.Var("wkMode");
            var countrylist = _project.Var("countrylist")
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            var directions = new List<string>();
            var selectionSource = "none";
            var selectionWhere = "";

            if (log) _project.SendInfoToLog(
                $"[PrepareInstance:db] start wkMode='{wkMode}', pruneErrors={pruneErrors}, addErr='{addErr}', projectTable='{_project.Var("projectTable")}', countrylist_count={countrylist.Count}, countrylist=[{PreviewDirections(countrylist)}]",
                true);

            if (wkMode == "trace")
            {
                selectionSource = "DbGetLines(direction)";
                selectionWhere = $"success = 0 AND failed = 0 AND skip != true{addErr}{cooldownFilter}";
                directions = _project.DbGetLines("direction", where:selectionWhere);
            }
            
            else if (wkMode == "retest")
            {
                selectionSource = "DbGetLines(direction)";
                selectionWhere = $"success = 0 AND failed < 10 AND skip != true{addErr}{cooldownFilter}";
                directions = _project.DbGetLines("direction", where:selectionWhere);
            }
            
            else if (wkMode == "tested")
            {
                selectionSource = "DbGetLines(direction)";
                selectionWhere = $"success != 0 AND skip != true{cooldownFilter}";
                directions = _project.DbGetLines("direction", where:selectionWhere);
            }
            else if (wkMode == "chosen")
            {
                selectionSource = "proxy_location";
                directions = _project.Var("proxy_location").Split(',').ToList();
            }
            
            

            LogDirections("after source selection", selectionSource, selectionWhere, directions , log);

            if (countrylist.Count > 0)
            {
                var beforeCountryFilter = directions.Count;
                directions = directions
                    .Where(x => x.Length >= 2 && countrylist.Contains(x.Substring(0, 2)))
                    .ToList();
                if (log) _project.SendInfoToLog(
                    $"[PrepareInstance:db] after countrylist filter: before={beforeCountryFilter}, after={directions.Count}, allowed=[{PreviewDirections(countrylist)}], directions=[{PreviewDirections(directions)}]",
                    true);
            }
            else
            {
                if (log) _project.SendInfoToLog("[PrepareInstance:db] countrylist filter skipped: countrylist is empty", true);
            }

            if (directions.Count == 0 || directions.All(string.IsNullOrWhiteSpace))
            {
                if (log) _project.SendInfoToLog(
                    $"[PrepareInstance:db] no directions available before Rnd(): wkMode='{wkMode}', source='{selectionSource}', where='{selectionWhere}', projectTable='{_project.Var("projectTable")}', countrylist_count={countrylist.Count}",
                    true);
            }

            direction = directions.Rnd().Trim();

            if (direction.Length < 3)
                direction = direction + "-" + _project.Var("numProvider");

            if (log) _project.SendInfoToLog(direction, true);
            return direction;
        }


        public void PrepareInstance( string direction,  bool fixTime = true )
        {
            _instance.AudioContextMode = ZennoLab.InterfacesLibrary.Enums.Browser.AudioMode.Emulate;
            _instance.CanvasRenderMode = ZennoLab.InterfacesLibrary.Enums.Browser.CanvasMode.SuperEmulation;
            _instance.ClientRectWorkMode= ZennoLab.InterfacesLibrary.Enums.Browser.ClientRectMode.Emulate;
            _instance.UseMedia = false;
            _instance.SetWindowSize(1280, 720);
            _project.SpoofGpu(_instance);
            var proxy = new NumlexProxy(_project, _instance);
            
            string[] dir = direction.Split('-');
            _project.Var("proxy_location",dir[0]);	
            _project.Var("numProvider",dir[1]);
            _project.Var("numDirection",direction);
            
            proxy.SetProxy(_project.Var("proxy_location"));


            _instance.GetCookies(_project);
            _project.RndProfileData();

			if (fixTime)
            {
	            try	{_instance.FixTimezone(_project); }
                catch (Exception ex){_project.warn(ex);}
            }
        }

        private void LogDirections(string stage, string source, string where, List<string> directions , bool log)
        {
            if (log) _project.SendInfoToLog(
                $"[PrepareInstance:db] {stage}: source='{source}', where='{where}', count={directions.Count}, non_empty={directions.Count(x => !string.IsNullOrWhiteSpace(x))}, directions=[{PreviewDirections(directions)}]",
                true);
        }

        private static string PreviewDirections(IEnumerable<string> directions)
        {
            if (directions == null)
                return "<null>";

            var list = directions
                .Take(12)
                .Select(x => string.IsNullOrEmpty(x)
                    ? "<empty>"
                    : x.Replace("\r", "\\r").Replace("\n", "\\n"))
                .ToList();

            return list.Count == 0 ? "<empty list>" : string.Join(", ", list);
        }


    }
}
