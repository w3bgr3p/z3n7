using System;
using System.IO;
using System.Collections.Generic;
using ZennoLab.InterfacesLibrary.ProjectModel;
using System.Linq;
using Newtonsoft.Json.Linq;


namespace z3n7
{
    /// <summary>ZennoBrowser (ZP8) profiles: their ids and running the helper project <c>ZB.zp</c>.</summary>
    public static class ZennoBrowser
    {
        private static readonly object _dbLock = new object();
        /// <summary>
        /// Reads <c>id</c> and <c>name</c> of every ZennoBrowser profile except <c>template</c> from the
        /// <c>ProfileInfos</c> table. While reading, the <c>DBmode</c> and <c>DBsqltPath</c> variables point at
        /// <c>%LOCALAPPDATA%\ZennoLab\ZP8\.zp8\ProfileManagement.db</c>; they are restored afterwards.
        /// </summary>
        /// <returns>Profile id → profile name. Throws when the ZennoBrowser database file is missing.</returns>
        /// <remarks>
        /// The read goes through <c>DbGetLines</c>/<c>DbGet</c> → <c>DbQ</c>, which picks the database by
        /// <c>dbSource</c> and does not consult <c>DBmode</c> or <c>DBsqltPath</c>.
        /// </remarks>
        public static Dictionary<string, string> ZBids(this IZennoPosterProjectModel project)
        {
            lock (_dbLock)
            {
                var modeBkp = project.Var("DBmode");
                var pathBkp = project.Var("DBsqltPath");

                try
                {
                    project.Var("DBmode", "SQLite");
                    string dbPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "ZennoLab", "ZP8", ".zp8", "ProfileManagement.db");

                    if (!File.Exists(dbPath))
                        throw new FileNotFoundException($"ZB db not found by path: {dbPath}");

                    project.Var("DBsqltPath", dbPath);

                    var current = project.DbGetLines("id, name", "ProfileInfos", where: "id = id");
                    var zbId_acc0 = new Dictionary<string, string>();

                    foreach (var line in current)
                    {
                        var parts = line.Split('¦');
                        if (parts.Length < 2) continue;

                        var id = parts[0].Trim();
                        var acc = parts[1].Trim();

                        if (acc == "template") continue;
                        zbId_acc0.Add(id, acc);
                    }

                    return zbId_acc0;
                }
                finally
                {
                    project.Var("DBsqltPath", pathBkp);
                    project.Var("DBmode", modeBkp);
                }
            }
        }
        
        /// <summary>
        /// Stores <c>toDo</c> in the <c>toDo</c> variable and runs <c>{project.Path}/.internal/ZB.zp</c>,
        /// passing <c>acc0</c>, <c>cfgLog</c>, <c>cfgPin</c>, <c>DBmode</c>, <c>DBpstgrPass</c>,
        /// <c>DBpstgrUser</c>, <c>DBsqltPath</c>, <c>instancePort</c>, <c>lastQuery</c>, <c>cookies</c>,
        /// <c>varSessionId</c> and <c>toDo</c> by name.
        /// </summary>
        /// <param name="toDo">Command for the helper project.</param>
        /// <returns>The result of <c>ExecuteProject</c>.</returns>
        public static bool ZB(this IZennoPosterProjectModel project, string toDo)
        {
            var path = Path.Combine(project.Path,".internal","ZB.zp");
            project.Var("toDo", toDo);
            var vars = new List<string>
            {
                "acc0", "cfgLog", "cfgPin",
                "DBmode", "DBpstgrPass", "DBpstgrUser", "DBsqltPath",
                "instancePort", "lastQuery", "cookies", "varSessionId", "toDo", 
            };
            var mapVars = new List<Tuple<string, string>>();
            foreach (var v in vars) mapVars.Add(new Tuple<string, string>(v, v));
            return project.ExecuteProject(path, mapVars, true, true, true);
        }
    }
    
        /// <summary>Reading the ZennoBrowser profile database and parsing its profile lists.</summary>
        public static class ZbDbManager
    {
        /// <summary>
        /// Reads <c>query</c> columns of the profile whose id is in the <c>zb_id</c> variable. While reading,
        /// <c>DBmode</c>, <c>DBsqltPath</c> and <c>acc0</c> point at the ZennoBrowser database and profile;
        /// they are restored afterwards.
        /// </summary>
        /// <param name="query">Comma-separated column names.</param>
        /// <param name="tableName">Table.</param>
        /// <param name="log">Write the query to the log.</param>
        /// <remarks>
        /// The read goes through <c>DbGetLines</c>/<c>DbGet</c> → <c>DbQ</c>, which picks the database by
        /// <c>dbSource</c> and does not consult <c>DBmode</c> or <c>DBsqltPath</c>.
        /// </remarks>
        public static string ZBDbGet(this IZennoPosterProjectModel project,string query, string tableName = "ProfileInfos", bool log = false)
        {
            var modeBkp = project.Var("DBmode");   
            var pathBkp = project.Var("DBsqltPath");   
            var acc0Bkp = project.Var("acc0");   
	
            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ZennoLab", "ZP8", ".zp8", "ProfileManagement.db"
            );
    
            if (!File.Exists(dbPath))
            {
                throw new FileNotFoundException(
                    $"База данных не найдена по пути: {dbPath}"
                );
            }
            project.Var("DBmode", "SQLite");
            project.Var("acc0", 1);
            project.Var("DBsqltPath", dbPath);

            
            project.Var("acc0",project.Var("zb_id")); 
            string resp = project.DbGet(query,tableName, log:log );
            
            project.Var("DBmode",modeBkp);   
            project.Var("DBsqltPath",pathBkp);  
            project.Var("acc0",acc0Bkp); 
            return resp;
            
        }
        /// <summary>
        /// Maps profile names to ids from a JSON array of ZennoBrowser profiles (<c>Name</c>, <c>Id</c>,
        /// <c>FolderName</c>); for duplicate names the first wins.
        /// </summary>
        /// <param name="json">JSON array of profiles.</param>
        /// <param name="folder">Only profiles of this folder; empty for all.</param>
        public static Dictionary<string,string> ZBIdDic(this IZennoPosterProjectModel project, string json, string folder = null)
        {
            var array = JArray.Parse(json);
    
            var filtered = string.IsNullOrEmpty(folder) 
                ? array 
                : array.Where(x => (string)x["FolderName"] == folder);
    
            var nameToId = filtered
                .GroupBy(x => (string)x["Name"])
                .ToDictionary(g => g.Key, g => (string)g.First()["Id"]);

            return nameToId;
        }
        
        /// <summary>Profile ids of a folder from a JSON array of profiles (see <c>ZBIdDic</c>).</summary>
        /// <param name="json">JSON array of profiles.</param>
        /// <param name="folder">Folder name.</param>
        public static List<string> ZBIdList(this IZennoPosterProjectModel project, string json, string folder = "Farm")
        {
            var dic = project.ZBIdDic(json, folder);
            var res = new List<string>();
            foreach(var p in dic){
                res.Add(p.Value);
            }

            return res;
        }
        
    }
    
}