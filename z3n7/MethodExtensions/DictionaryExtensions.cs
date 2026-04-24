using System.Collections.Generic;
using ZennoLab.InterfacesLibrary.ProjectModel;


namespace z3nIO
{
    public static partial class ProjectExtensions
    {
        
        public static void DicToVars(this Dictionary<string, string> dict, IZennoPosterProjectModel project)
        {
            foreach (var pair in dict)
            {
                project.Var(pair.Key, pair.Value);
            }
        }

    }
    
    
}