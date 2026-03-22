using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    public static class Vars
    {
        private static readonly object LockObject = new object();
        
        public static string Var(this IZennoPosterProjectModel project, string var)
        {
            string value = string.Empty;
            try
            {
                value = project.Variables[var].Value;
            }
            catch (Exception e)
            {
                project.SendInfoToLog(e.Message);
            }
            if (value == string.Empty)
            { }

            return value;
        }
        public static string Var(this IZennoPosterProjectModel project, string var, object value)
        {
            if (value == null ) return string.Empty;
            try
            {
                project.Variables[var].Value = value.ToString();
            }
            catch (Exception e)
            {
                project.SendInfoToLog(e.Message);
            }
            return string.Empty;
        }
        public static int Int(this IZennoPosterProjectModel project, string var)
        {
            int value = 0;
            try
            {
                value = int.Parse(project.Var(var));
            }
            catch
            {
            }
            return value;
        }
        public static decimal Decimal(this IZennoPosterProjectModel project, string var)
        {
            decimal value = 0;
            try
            {
                value = decimal.Parse(project.Var(var));
            }
            catch
            {
            }
            return value;
        }
        public static bool Bool(this IZennoPosterProjectModel project, string var)
        {
            bool value = project.Var(var) == "True";
            return value;
        }


        public static string VarRnd(this IZennoPosterProjectModel project, string var)
        {
            string value = string.Empty;
            try
            {
                value = project.Variables[var].Value;
            }
            catch (Exception e)
            {
                project.SendInfoToLog(e.Message);
            }
            if (value == string.Empty) project.SendInfoToLog($"no Value from [{var}] `w");

            if (value.Contains("-"))
            {
                var min = int.Parse(value.Split('-')[0].Trim());
                var max = int.Parse(value.Split('-')[1].Trim());
                return new Random().Next(min, max).ToString();
            }
            return value.Trim();
        }
        public static int VarCounter(this IZennoPosterProjectModel project, string varName, int input)
        {
            var counter = project.Int(varName) + input;
            project.Var(varName, counter);
            return counter;
        }
        public static decimal VarsMath(this IZennoPosterProjectModel project, string varA, string operation, string varB, string resultVar = null)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            decimal a = decimal.Parse(project.Var(varA));
            decimal b = decimal.Parse(project.Var(varB));
            decimal result;
            switch (operation)
            {
                case "+":
                    result = a + b;
                    break;
                case "-":
                    result = a - b;
                    break;
                case "*":
                    result = a * b;
                    break;
                case "/":
                    result = a / b;
                    break;
                default:
                    throw new Exception($"unsupported operation {operation}");
            }
            if (string.IsNullOrEmpty(resultVar)) 
                try { project.Var(resultVar, $"{result}"); } catch { }
            return result;
        }

        public static void VarsFromDict(this IZennoPosterProjectModel project, Dictionary<string, string> dict)
        {
            foreach (var pair in dict)
            {
                project.Var(pair.Key, pair.Value);
            }
        }
        
        public static void VarsFromJson(this IZennoPosterProjectModel project, string  json = "jVars")
        {
            if (json == "jVars") json = project.Var("jVars");
            var jVar = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            project.VarsFromDict(jVar);
        }     
        
        public static List<string> Range(this IZennoPosterProjectModel project, string accRange = null,
            string output = null, bool log = false)
        {
            if (string.IsNullOrEmpty(accRange)) accRange = project.Var("cfgAccRange");
            if (string.IsNullOrEmpty(accRange))
            {
                project.warn("range is not provided by input or project setting [cfgAccRange]");
                return null;
            }
            
            if (accRange.Contains(":"))
            {
                accRange = accRange.Split(':')[0];
            }
            
            int rangeS, rangeE;
            string range;

            if (accRange.Contains(","))
            {
                range = accRange;
                var rangeParts = accRange.Split(',').Select(int.Parse).ToArray();
                rangeS = rangeParts.Min();
                rangeE = rangeParts.Max();
            }
            else if (accRange.Contains("-"))
            {
                var rangeParts = accRange.Split('-').Select(int.Parse).ToArray();
                rangeS = rangeParts[0];
                rangeE = rangeParts[1];
                range = string.Join(",", Enumerable.Range(rangeS, rangeE - rangeS + 1));
            }
            else
            {
                rangeE = int.Parse(accRange);
                rangeS = int.Parse(accRange);
                range = accRange;
            }

            project.Variables["rangeStart"].Value = $"{rangeS}";
            project.Variables["rangeEnd"].Value = $"{rangeE}";
            project.Variables["range"].Value = range;

            return range.Split(',').ToList();
            //project.L0g($"{rangeS}-{rangeE}\n{range}");
        }
        
    }
    
    public static class GVars
    {
        private static readonly object LockObject = new object();
        public static string GVar(this IZennoPosterProjectModel project, string var)
        {
            string nameSpase = project.ExecuteMacro("{-Environment.CurrentUser-}");
            string value = string.Empty;
            lock (LockObject)
            {
                try
                {
                    value = project.GlobalVariables[nameSpase, var].Value;
                }
                catch { }
            } 
            return value;
        }
        public static string GVar(this IZennoPosterProjectModel project, string var, object value)
        {
            string nameSpase = project.ExecuteMacro("{-Environment.CurrentUser-}");
            lock (LockObject)
            {
                try
                {
                    project.GlobalVariables[nameSpase, var].Value = value.ToString();
                }
                catch
                {
                    try
                    {
                        project.GlobalVariables.SetVariable(nameSpase, var, value.ToString());
                    }
                    catch { }

                }
            }
            return string.Empty;
        }
        public static List<string> GGetBusyList(this IZennoPosterProjectModel project, bool log = false)
        {
            string nameSpase = project.ExecuteMacro("{-Environment.CurrentUser-}");
            var busyAccounts = new List<string>();
            
            lock (LockObject)
            {
                try
                {
                    for (int i = 1; i <= int.Parse(project.Variables["rangeEnd"].Value); i++)
                    {
                        string threadKey = $"acc{i}";
                        try
                        {
                            var globalVar = project.GlobalVariables[nameSpase, threadKey];
                            if (globalVar != null && !string.IsNullOrEmpty(globalVar.Value))
                            {
                                busyAccounts.Add($"{i}:{globalVar.Value}");
                            }
                        }
                        catch { }
                    }
                    
                    if (log)
                    {
                        project.SendInfoToLog($"busy Accounts: [{string.Join(" | ", busyAccounts)}]");
                    }
                    
                    return busyAccounts;
                }
                catch (Exception ex)
                {
                    if (log) project.SendInfoToLog($"⚙ GGet: {ex.Message}");
                    throw;
                }
            }
        }

        public static bool GSetAcc(this IZennoPosterProjectModel project, string input = null, bool force = false, bool log = false)
        {
            string nameSpase = project.ExecuteMacro("{-Environment.CurrentUser-}");
            
            lock (LockObject)
            {
                try
                {
                    int currentThread = int.Parse(project.Variables["acc0"].Value);
                    string currentThreadKey = $"acc{currentThread}";
                    
                    string valueToSet = input ?? project.Variables["projectName"].Value;
                    
                    if (!force)
                    {
                        var busyAccounts = project.GGetBusyList(false);
                        if (busyAccounts.Any(x => x.StartsWith($"{currentThread}:")))
                        {
                            if (log) project.SendInfoToLog($"{currentThreadKey} is already busy!");
                            return false;
                        }
                    }
                    
                    try
                    {
                        project.GlobalVariables.SetVariable(nameSpase, currentThreadKey, valueToSet);
                    }
                    catch (Exception ex)
                    {
                        if (log) project.SendWarningToLog(ex.Message, true);
                        project.GlobalVariables[nameSpase, currentThreadKey].Value = valueToSet;
                    }
                    
                    if (log) 
                    {
                        string forceText = force ? " (forced)" : "";
                        project.SendInfoToLog($"{currentThreadKey} bound to {valueToSet}{forceText}");
                    }
                    
                    return true;
                }
                catch (Exception ex)
                {
                    if (log) project.SendInfoToLog($"⚙ GSet: {ex.Message}");
                    throw;
                }
            }
        }
        public static List<int> GClean(this IZennoPosterProjectModel project, bool log = false)
        {
            string nameSpase = project.ExecuteMacro("{-Environment.CurrentUser-}");
            var cleaned = new List<int>();
            
            lock (LockObject)
            {
                try
                {
                    for (int i = 1; i <= int.Parse(project.Variables["rangeEnd"].Value); i++)
                    {
                        string threadKey = $"acc{i}";
                        try
                        {
                            var globalVar = project.GlobalVariables[nameSpase, threadKey];
                            if (globalVar != null)
                            {
                                globalVar.Value = string.Empty;
                                cleaned.Add(i);
                            }
                        }
                        catch { }
                    }
                    
                    if (log)
                    {
                        project.SendInfoToLog($"Cleaned accounts: {string.Join(",", cleaned)}");
                    }
                    
                    return cleaned;
                }
                catch (Exception ex)
                {
                    if (log) project.SendInfoToLog($"⚙ GClean: {ex.Message}");
                    throw;
                }
            }
        }
        
    }
    

    public static class Constantes
    {
        private static readonly object LockObject = new object();

        public static string ProjectName(this IZennoPosterProjectModel project)
        {
            var path = "";
    

            var pathToFolder = project.Path;
            var filename = project.Name;
    
            var actualFiles = Directory.GetFiles(pathToFolder, filename, SearchOption.TopDirectoryOnly);
    
            if (actualFiles.Length > 0)
            {
                path = Path.GetFileName(actualFiles[0]); 
            }
            else
            {
                path = project.Name; 
            }
            
    
            string name = ProjectName(path);
            project.Var("projectName", name);
            return name;
        }

        private static string ProjectName(string projectPath)
        {
            if (string.IsNullOrEmpty(projectPath)) throw new ArgumentNullException(nameof(projectPath));
            return System.IO.Path.GetFileName(projectPath).Split('.')[0];
        }

        public static string ProjectTable(this IZennoPosterProjectModel project)
        {
            string table = "__" + ProjectName(project);
            project.Var("projectTable", table);
            return table;
        }
        

        //pathes
        public static string PathProfiles(this IZennoPosterProjectModel project)
        {
            string pathLocal = project.Var("profiles_folder");
            string pathGlobal = project.GVar("profiles_folder");

            if (!string.IsNullOrEmpty(pathLocal))
            {
                if (string.IsNullOrEmpty(pathGlobal))
                    project.GVar("profiles_folder", pathLocal);
                return pathLocal;
            }

            if (!string.IsNullOrEmpty(pathGlobal))
            {
                project.Var("profiles_folder", pathGlobal);
                return pathGlobal;
            }

            throw new Exception("No profiles folder defined");

        }
        public static string PathCookies(this IZennoPosterProjectModel project)
        {
            string acc0 = project.Var("acc0");
            if (string.IsNullOrEmpty(acc0))
            {
                project.SendWarningToLog("acc0 isNullOrEmpty");
                return "";
            }
            return Path.Combine(project.PathProfiles(),"accounts","cookies",$"{acc0}.json");
        }
        public static string PathProfileFolder(this IZennoPosterProjectModel project)
        {
            string acc0 = project.Var("acc0");
            if (string.IsNullOrEmpty(acc0))
            {
                project.SendWarningToLog("acc0 isNullOrEmpty");
                return "";
            }
            return Path.Combine(project.PathProfiles(),"accounts","profilesFolder",acc0);
        }

        public static string SecureVar(this IZennoPosterProjectModel project, string key)
        {
            
            string encrypted = project.Var("jVars");
            
            if (string.IsNullOrEmpty(encrypted)) return string.Empty;
    
            string decrypted = SAFU.DecryptHWID(project, encrypted);
            if (string.IsNullOrEmpty(decrypted)) return string.Empty;
    
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(decrypted.FromBase64());
            return dict.TryGetValue(key, out var val) ? val : string.Empty;
        }
        
    }


    
    
    
    
    
    

}