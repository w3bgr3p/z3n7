using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    /// <summary>Extension methods on lists.</summary>
    public static class ListExtensions
    {
        [ThreadStatic]
        private static Random _random;
        
        private static Random Random => _random ?? (_random = new Random());

        /// <summary>Returns a random item.</summary>
        /// <param name="remove">Also remove it from the list.</param>
        /// <returns>The item. Throws when the list is empty.</returns>
        public static T Rnd<T>(this IList<T> list, bool remove = false)
        {
            if (list.Count == 0) 
                throw new InvalidOperationException("List is empty");

            var index = Random.Next(list.Count); // ← используй property Random, не _random!
            var result = list[index];
            if (remove) list.RemoveAt(index);
            return result;
        }
    }
    
    public static partial class ProjectExtensions
    {

        /// <summary>Returns a random line of a ZennoPoster list.</summary>
        /// <param name="listName">Project list name.</param>
        /// <param name="remove">Also remove it from the project list.</param>
        public static string RndFromList(this IZennoPosterProjectModel project, string listName, bool remove = false)
        {
            var localList = project.ListSync(listName);
            
            var item = localList.Rnd(remove);
            if (remove)
                project.ListSync(listName, localList);
            return item;
          
        }
        /// <summary>Copies a ZennoPoster list into a new <c>List&lt;string&gt;</c>.</summary>
        public static List<string> ListSync(this IZennoPosterProjectModel project, string listName)
        {
            var projectList = project.Lists[listName];
            var localList = new List<string>();
            foreach (var item in projectList)
            {
                localList.Add(item);
            }
            return localList;
            
        }
        /// <summary>Replaces the content of a ZennoPoster list with <c>localList</c>.</summary>
        /// <returns><c>localList</c>.</returns>
        public static List<string> ListSync(this IZennoPosterProjectModel project, string listName, List<string> localList)
        {
            var projectList = project.Lists[listName];
            projectList.Clear();
            foreach (var item in localList)
            {
                projectList.Add(item);
            }
    
            return localList;
        }
        
        /// <summary>Replaces the content of a ZennoPoster list with the lines of a file.</summary>
        /// <param name="listName">Project list name.</param>
        /// <param name="fileName">File to read.</param>
        /// <returns>The lines.</returns>
        public static List<string> ListFromFile(this IZennoPosterProjectModel project, string listName, string fileName)
        {
            string web3prompts = $"{project.Path}.data\\web3prompts.txt";
            var prjList = project.Lists[listName];
            prjList.Clear();
            
            var lines = File.ReadAllLines(fileName).ToList();
            try
            {
                project.ListSync(listName, lines);
            }
            catch
            {
            }
            return lines;
        }

    }
    
   

}