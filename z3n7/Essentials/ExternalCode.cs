using System;
using System.Collections.Generic;
using ZennoLab.InterfacesLibrary.ProjectModel;



namespace z3n7
{
    public static partial class ProjectExtensions
    {
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
