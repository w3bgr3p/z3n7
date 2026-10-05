using System;
using System.Collections.Generic;
using ZennoLab.InterfacesLibrary.ProjectModel;



namespace z3n7
{
    public static partial class ProjectExtensions
    {
        /// <summary>
        /// Runs the project whose path is stored in the <c>projectScript</c> variable, via
        /// <c>ExecuteProject</c>.
        /// Each name in <c>vars</c> is mapped to the variable of the same name in the called project. An
        /// exception is written to the log as a warning and rethrown.
        /// </summary>
        /// <param name="vars">Variable names to pass to the called project.</param>
        /// <returns>The result of <c>ExecuteProject</c>.</returns>
        public static bool RunZp(this IZennoPosterProjectModel project, List<string> vars = null)
        {
            string tempFilePath = project.Var("projectScript");
            var mapVars = new List<Tuple<string, string>>();

            if (vars != null)
                foreach (var v in vars)
                    try 
                    {
                        mapVars.Add(new Tuple<string, string>(v, v)); 
                    }
                    catch (Exception ex)
                    {
                        project.SendWarningToLog(ex.Message, true);
                        throw;
                    }
            try 
            { 
                return project.ExecuteProject(tempFilePath, mapVars, true, true, true); 
            }
            catch (Exception ex) 
            { 
                project.SendWarningToLog(ex.Message, true);
                throw;
            }
        }
        /// <summary>
        /// Runs the project at <c>path</c> via <c>ExecuteProject</c>, passing a fixed set of variables by name:
        /// <c>acc0</c>, <c>cfgLog</c>, <c>cfgPin</c>, <c>DBmode</c>, <c>DBpstgrPass</c>, <c>DBpstgrUser</c>,
        /// <c>DBsqltPath</c>, <c>instancePort</c>, <c>lastQuery</c>, <c>varSessionId</c>, <c>wkMode</c>.
        /// </summary>
        /// <param name="path">Path to the .zp file.</param>
        /// <returns>The result of <c>ExecuteProject</c>.</returns>
        public static bool RunZp(this IZennoPosterProjectModel project, string path)
        {
            var vars = new List<string> {
                "acc0", "cfgLog", "cfgPin",
                "DBmode", "DBpstgrPass", "DBpstgrUser", "DBsqltPath",          
                "instancePort",  "lastQuery",
                "varSessionId", "wkMode",
            };
            
            
            var mapVars = new List<Tuple<string, string>>();
            if (vars != null)
                foreach (var v in vars)
                    mapVars.Add(new Tuple<string, string>(v, v)); 
            return project.ExecuteProject(path, mapVars, true, true, true); 
        }
        
    }

}
