using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3nIO
{
    public static class ListExtensions
    {
        [ThreadStatic]
        private static Random _random;
        
        private static Random Random => _random ?? (_random = new Random());

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

        public static string RndFromList(this IZennoPosterProjectModel project, string listName, bool remove = false)
        {
            var localList = project.ListSync(listName);
            
            var item = localList.Rnd(remove);
            if (remove)
                project.ListSync(listName, localList);
            return item;
          
        }
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