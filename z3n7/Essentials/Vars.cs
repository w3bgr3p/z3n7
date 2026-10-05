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
    /// <summary>Short accessors for project variables: read, write, parse, count.</summary>
    public static class Vars
    {
        private static readonly object LockObject = new object();
        
        /// <summary>
        /// Returns the value of a project variable. A missing variable is reported to the log and an empty
        /// string is returned.
        /// </summary>
        /// <param name="var">Variable name.</param>
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
        /// <summary>
        /// Sets a project variable to <c>value.ToString()</c>. <c>null</c> is ignored. A missing variable is
        /// reported to the log, nothing is thrown.
        /// </summary>
        /// <param name="var">Variable name.</param>
        /// <param name="value">New value.</param>
        /// <returns>Always an empty string.</returns>
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
        /// <summary>Returns a project variable parsed as <c>int</c>, or 0 when it is empty or not a number.</summary>
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
        /// <summary>Adds <c>input</c> to an integer project variable and stores the result.</summary>
        /// <returns>The new value.</returns>
        public static int Int(this IZennoPosterProjectModel project, string varName, int input)
        {
            var counter = project.Int(varName) + input;
            project.Var(varName, counter);
            return counter;
        }
        /// <summary>
        /// Returns a project variable parsed as <c>decimal</c> (current culture), or 0 when it cannot be
        /// parsed.
        /// </summary>
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
        /// <summary>Returns <c>true</c> when the project variable equals <c>True</c> exactly.</summary>
        public static bool Bool(this IZennoPosterProjectModel project, string var)
        {
            bool value = project.Var(var) == "True";
            return value;
        }

        /// <summary>
        /// Error counter for retry loops. Stores the error text in <c>err</c> and increments <c>maxErr</c>;
        /// once <c>maxErr</c> exceeds <c>maxAttempts</c>, writes a warning and throws.
        /// </summary>
        /// <param name="maxAttempts">Number of errors tolerated.</param>
        /// <param name="ex">The error; when null, <c>project.LastErrorComment</c> is used.</param>
        public static void MaxErr(this IZennoPosterProjectModel project, int maxAttempts, Exception ex = null)
        {
            var errCounter = project.Int("maxErr");
            var message =  ex != null  ? ex.Message : project.LastErrorComment;
            project.Var("err", message);    
            
            
            if (errCounter > maxAttempts)
            {
                project.SendWarningToLog($"max errors reached: {message}");
                throw  new Exception(message);
            }
            else
            {
                project.Int("maxErr", 1);
            }
        }

        /// <summary>
        /// Adds a variable to the project open in ProjectMaker through the local ZennoPoster API
        /// (<c>http://localhost:5299</c>).
        /// Development-time only: it edits ProjectMaker's in-memory copy of the project, so a task running in
        /// the runner is not affected. Save the project to keep the change. The API key is read from
        /// <c>ZENNO_API_KEY</c> in the <c>.env</c> next to <c>z3n7.dll</c>; the key tier must be T1 or higher.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="defaultValue">Initial value.</param>
        /// <param name="comment">Variable comment.</param>
        /// <returns>
        /// <c>true</c> when the API answered <c>RESULT_OK</c>; otherwise the answer is written to the log as a
        /// warning.
        /// </returns>
        public static bool VarAdd(this IZennoPosterProjectModel project, string name, string defaultValue = "", string comment = "")
        {
            var body = JsonConvert.SerializeObject(new { variables = new[] { new { name, defaultValue, comment } } });
            var answer = ZennoLab.CommandCenter.ZennoPoster.HTTP.Request(
                ZennoLab.InterfacesLibrary.Enums.Http.HttpMethod.POST,
                "http://localhost:5299/api/v1/projects/current/variables",
                body, "application/json", "", "UTF-8",
                ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly, 15000,
                "", "", true, 5,
                new[] { "Authorization: Bearer " + project.ReadEnv("ZENNO_API_KEY", true) });

            var ok = answer != null && answer.Contains("RESULT_OK");
            if (!ok) project.SendWarningToLog($"VarAdd({name}): {answer}", false);
            return ok;
        }
        

        /// <summary>
        /// Reads a project variable. A value like <c>10-20</c> returns a random integer from 10 (inclusive) to
        /// 20 (exclusive); any other value is returned trimmed.
        /// </summary>
        /// <param name="var">Variable name.</param>
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
        /// <summary>
        /// Adds <c>input</c> to an integer project variable and stores the result. Same as <c>Int(varName,
        /// input)</c>.
        /// </summary>
        /// <returns>The new value.</returns>
        public static int VarCounter(this IZennoPosterProjectModel project, string varName, int input)
        {
            var counter = project.Int(varName) + input;
            project.Var(varName, counter);
            return counter;
        }
        /// <summary>
        /// Applies <c>+</c>, <c>-</c>, <c>*</c> or <c>/</c> to two project variables parsed as <c>decimal</c>
        /// (invariant culture). Other operations throw.
        /// </summary>
        /// <param name="varA">Left operand variable.</param>
        /// <param name="operation">One of <c>+ - * /</c>.</param>
        /// <param name="varB">Right operand variable.</param>
        /// <param name="resultVar">Variable that receives the result; empty to skip.</param>
        /// <returns>The result.</returns>
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
            if (!string.IsNullOrEmpty(resultVar)) 
                try { project.Var(resultVar, $"{result}"); } catch { }
            return result;
        }

        /// <summary>Sets a project variable for every key of the dictionary.</summary>
        public static void VarsFromDict(this IZennoPosterProjectModel project, Dictionary<string, string> dict)
        {
            foreach (var pair in dict)
            {
                project.Var(pair.Key, pair.Value);
            }
        }
        
        /// <summary>Sets project variables from a flat JSON object of string values.</summary>
        /// <param name="json">JSON text, or the default <c>jVars</c> to read the JSON from the <c>jVars</c> variable.</param>
        public static void VarsFromJson(this IZennoPosterProjectModel project, string  json = "jVars")
        {
            if (json == "jVars") json = project.Var("jVars");
            var jVar = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            project.VarsFromDict(jVar);
        }     
        
        /// <summary>
        /// Parses an account range and stores it in <c>rangeStart</c>, <c>rangeEnd</c> and <c>range</c>
        /// (comma-separated list).
        /// Accepted forms: <c>5</c>, <c>1-10</c>, <c>1,4,7</c>. Anything after <c>:</c> is ignored.
        /// </summary>
        /// <param name="accRange">Range text; when empty, the <c>cfgAccRange</c> variable is used.</param>
        /// <param name="output">Not used.</param>
        /// <param name="log">Not used.</param>
        /// <returns>Account numbers as strings, or <c>null</c> (with a warning) when no range is given.</returns>
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
    /// <summary>Global ZennoPoster variables, kept in a namespace named after the current Windows user.</summary>
    public static class GVars
    {
        private static readonly object LockObject = new object();
        /// <summary>Returns a global variable, or an empty string when it does not exist.</summary>
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
        /// <summary>Sets a global variable, creating it when it does not exist. Errors are swallowed.</summary>
        /// <returns>Always an empty string.</returns>
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
        /// <summary>
        /// Lists accounts taken by running threads: every non-empty global variable <c>acc1</c> …
        /// <c>acc{rangeEnd}</c>.
        /// </summary>
        /// <param name="log">Write the list to the log.</param>
        /// <returns>Entries in the form <c>number:value</c>.</returns>
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

        /// <summary>
        /// Marks the current account (<c>acc0</c>) as taken by writing <c>input</c> to the global variable
        /// <c>acc{acc0}</c>.
        /// </summary>
        /// <param name="input">Value to store; default is the <c>projectName</c> variable.</param>
        /// <param name="force">Overwrite even if the account is already taken.</param>
        /// <param name="log">Write the outcome to the log.</param>
        /// <returns><c>false</c> when the account is already taken and <c>force</c> is false.</returns>
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
        /// <summary>Clears the global variables <c>acc1</c> … <c>acc{rangeEnd}</c>.</summary>
        /// <returns>Numbers of the variables that were cleared.</returns>
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
    
    /// <summary>Project name, its database table and the standard folders of the profile storage.</summary>
    public static class Constantes
    {
        private static readonly object LockObject = new object();

        /// <summary>Returns the project file name up to the first dot and stores it in <c>projectName</c>.</summary>
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

        /// <summary>Returns <c>__</c> + project name and stores it in <c>projectTable</c>.</summary>
        public static string ProjectTable(this IZennoPosterProjectModel project)
        {
            string table = "__" + ProjectName(project);
            project.Var("projectTable", table);
            return table;
        }

        /// <summary>Returns the full path of the project file.</summary>
        public static string FullPath(this IZennoPosterProjectModel project)
        {
            return Path.Combine(project.Path, project.Name);
        }


        //pathes
        /// <summary>
        /// Returns the profile storage root: the <c>profiles_folder</c> variable, else the global variable of
        /// the same name. Whichever is found is copied to the other one.
        /// </summary>
        /// <remarks>Throws when neither is set.</remarks>
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
        /// <summary>
        /// Returns <c>{profiles}/accounts/cookies/{acc0}.json</c>, or an empty string with a warning when
        /// <c>acc0</c> is empty.
        /// </summary>
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
        /// <summary>
        /// Returns <c>{profiles}/accounts/profilesFolder/{acc0}</c>, or an empty string with a warning when
        /// <c>acc0</c> is empty.
        /// </summary>
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

        /// <summary>
        /// Reads a value from the encrypted <c>jVars</c> variable: decrypts it with <c>SAFU.DecryptHWID</c>,
        /// decodes Base64 and looks the key up in the resulting JSON object.
        /// </summary>
        /// <returns>
        /// The value, or an empty string when <c>jVars</c> is empty, cannot be decrypted, or has no such key.
        /// </returns>
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