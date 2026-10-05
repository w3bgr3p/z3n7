using System.Collections.Generic;
using ZennoLab.InterfacesLibrary.ProjectModel;


namespace z3n7
{
    /// <summary>Extension methods: dictionaries and ZennoPoster lists.</summary>
    public static partial class ProjectExtensions
    {
        
        /// <summary>Sets a project variable for every key of the dictionary.</summary>
        public static void DicToVars(this Dictionary<string, string> dict, IZennoPosterProjectModel project)
        {
            foreach (var pair in dict)
            {
                project.Var(pair.Key, pair.Value);
            }
        }

    }
    
    
}