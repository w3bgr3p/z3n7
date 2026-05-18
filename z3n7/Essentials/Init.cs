using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3nIO
{
    public class Init
    {
        #region Fields & Constructor
        
        private readonly IZennoPosterProjectModel _project;
        private readonly Instance _instance;
        private readonly Logger _logger;
        private readonly bool _log;

        public Init(IZennoPosterProjectModel project, Instance instance, bool log = false)
        {
            _project = project;
            _log = log;
            _logger =  new Logger(_project,null, classEmoji: "►", logLevel: (_log) ? LogLevel.Info : LogLevel.Off);
            _instance = instance;
        }
        
        #endregion
        
        #region Project Initialization

        public void InitVariables(string author = "")
        {
            LogDisabler.DisableLogs();
            _SAFU();
            _project.StartSession();
            string projectName = _project.ProjectName();
            string projectTable = _project.ProjectTable();
            string dllTitle = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyTitleAttribute>()
                ?.Title ?? "z3n7";
            
            _project.Range();
            string keyPath = Path.Combine(_project.Path, ".internal", "safu.key");
            SAFU.InitZ3n8(_project, keyPath);
            Logo(author, dllTitle, projectName);
            
        }
        private void Logo(string author, string dllTitle, string projectName)
        {
            var v = GetVersions();
            string dllVer = v[0];
            string zpVer = v[1];
            
            if (author != "") author = $" script author: @{author}";
            string frameworkVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            
            string logo = $@"using ZennoPoster v{zpVer} && {frameworkVersion}; 
             using {dllTitle} v{dllVer}  
            ┌by─┐					
            │    w3bgr3p;		
            └─→┘
                        ► init {projectName} ░ ▒ ▓ █  {author}";
            _project.SendInfoToLog(logo, true);
        }


        private void _SAFU()
        {
            string jFile = _project.Var("jVars");
            _project.Var("jVars",File.ReadAllText(jFile));
            //var mapVars = new List<Tuple<string, string>>();
            //mapVars.Add(new Tuple<string, string>("jVars", "jVars"));
            //try { _project.ExecuteProject(tempFilePath, mapVars, true, true, true); }
            //catch (Exception ex) { _logger.Warn(ex.Message); }
            //string decrypted = SAFU.DecryptHWID(_project, _project.Var("jVars"));
            //_project.VarsFromJson(decrypted);
        }

        private void BuildNewDatabase()
        {
            if (_project.Var("cfgBuildDb") != "True") return;

            string filePath = Path.Combine(_project.Path, "DbBuilder.zp");
            if (File.Exists(filePath))
            {
                _project.Var("wkMode", "Build");
                _project.Var("cfgAccRange", _project.Var("rangeEnd"));
                
                var vars = new List<string> {
                    "cfgLog", "cfgPin", "cfgAccRange", "DBmode", "DBpstgrPass", "DBpstgrUser", 
                    "DBsqltPath", "debug", "lastQuery", "wkMode",
                };
                _project.RunZp(vars);
            }
            else
            {
                _logger.Warn($"file {filePath} not found. Last version can be downloaded by link \nhttps://raw.githubusercontent.com/w3bgrep/z3nFarm/master/DbBuilder.zp");
            }
        }
        
        #endregion
        
        #region Utilities & Helpers

        private string[] GetVersions()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var referencedAssembly = typeof(Init).Assembly;
            var executingVersion = executingAssembly.GetName().Version.ToString();
            var referencedVersion = referencedAssembly.GetName().Version.ToString();

            if (executingVersion != referencedVersion)
            {
                string errorMessage = $"Version mismatch detected! " +
                                      $"Executing assembly ({executingAssembly.Location}) version: {executingVersion}, " +
                                      $"Referenced assembly ({referencedAssembly.Location}) version: {referencedVersion}. " +
                                      $"Ensure both assemblies are the same version.";
                _logger.Warn(errorMessage);
            }
            
            string currentProcessPath = Process.GetCurrentProcess().MainModule.FileName;
            string processDir = Path.GetDirectoryName(currentProcessPath);
            string DllVer = referencedVersion;
            string ZpVer = processDir.Split('\\')[5];
            
            return new[] { DllVer, ZpVer };
        }

        
        #endregion
    }


}

namespace z3nIO//ProjectExtensions
{
    
    public static partial class ProjectExtensions
    {
        
        
        public static void InitVariables(this IZennoPosterProjectModel project, Instance instance, string author = "w3bgr3p")
        {
            new Init(project, instance).InitVariables(author);
            try
            {
                project.StartZpServer();
            }
            catch (Exception ex)
            {
                project.warn(ex, false, true);
            }
        }
    }
}