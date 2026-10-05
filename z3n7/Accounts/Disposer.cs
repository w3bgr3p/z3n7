using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;
using System;
using System.IO;
using ZennoLab.InterfacesLibrary.Enums.Log;
using ZennoLab.InterfacesLibrary.Enums.Browser;

namespace z3n7
{

    /// <summary>End of an account session: report, save the browser profile, clean up.</summary>
    public class Disposer
    {
        #region Fields & Constructor

        private readonly IZennoPosterProjectModel _project;
        private readonly Instance _instance;
        private readonly Reporter _reporter;
        private readonly Logger _logger;
        private readonly InstanceManager _instanceMgr;
        
        /// <summary>Creates the helper.</summary>
        /// <param name="log">Logger for progress; <c>null</c> logs nothing.</param>
        public Disposer(IZennoPosterProjectModel project, Instance instance, Logger log = null)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            
            _reporter = new Reporter(project, instance);
            _instanceMgr = new InstanceManager(project, instance, log);
            _logger = log;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Finishes the session. When <c>acc0</c> is set, writes a success report (or, when <c>lastQuery</c>
        /// contains <c>dropped</c>, an error report with a screenshot) to the log and the account's row; then
        /// saves the profile (<c>InstanceManager.SaveProfile</c>), writes the final line to the log and cleans
        /// up (<c>InstanceManager.Cleanup</c>).
        /// </summary>
        public void FinishSession()
        {
            _logger?.Send("Starting session finish sequence");

            string acc0 = _project.Var("acc0");
            bool isSuccess = IsSessionSuccessful();
            
            _logger?.Send($"Session status: {(isSuccess ? "SUCCESS" : "FAILED")}");

            if (!string.IsNullOrEmpty(acc0))
            {
                GenerateReports(isSuccess);
            }
            
            _instanceMgr.SaveProfile();
            
            LogSessionComplete(isSuccess);

            _instanceMgr.Cleanup();

            _logger?.Send("Session finish sequence completed");
        }
        
        /// <summary>Same as <c>Reporter.ReportError</c>.</summary>
        /// <param name="toLog">Write it to the log.</param>
        /// <param name="toTelegram">Send it to Telegram.</param>
        /// <param name="toDb">Write it to the account's row.</param>
        /// <param name="screenshot">Save a screenshot.</param>
        public string ErrorReport(bool toLog = true, bool toTelegram = false, bool toDb = false, bool screenshot = false)
        {
            return _reporter.ReportError(toLog, toTelegram, toDb, screenshot);
        }
        
        /// <summary>Same as <c>Reporter.ReportSuccess</c>.</summary>
        /// <param name="toLog">Write it to the log.</param>
        /// <param name="toTelegram">Send it to Telegram.</param>
        /// <param name="toDb">Write it to the account's row.</param>
        /// <param name="customMessage">Extra line.</param>
        public string SuccessReport(bool toLog = true, bool toTelegram = false, bool toDb = false, string customMessage = null)
        {
            return _reporter.ReportSuccess(toLog, toTelegram, toDb, customMessage);
        }

        #endregion

        #region Private Methods

        private bool IsSessionSuccessful()
        {
            string lastQuery = _project.Var("lastQuery");
            bool isSuccess = !lastQuery.Contains("dropped");
            
            _logger?.Send($"Checking session success: lastQuery='{lastQuery}', result={isSuccess}");
            
            return isSuccess;
        }

        private void GenerateReports(bool isSuccess)
        {
            _logger?.Send($"Generating {(isSuccess ? "SUCCESS" : "ERROR")} report");
            
            try
            {
                if (isSuccess)
                {
                    _reporter.ReportSuccess(toLog: true, toTelegram: false, toDb: true);
                }
                else
                {
                    _reporter.ReportError(toLog: true, toTelegram: false, toDb: true, screenshot: true);
                }
                _logger?.Send("Report generated successfully");
            }
            catch (Exception ex)
            {
                _logger?.Send($"Report generation failed: {ex.GetType().Name} - {ex.Message}");
            }
        }

        private void LogSessionComplete(bool isSuccess)
        {
            try
            {
                double elapsed = _project.TimeElapsed();
                string statusText = isSuccess ? "SUCCESS" : "FAILED";
                
                _logger?.Send($"Session completed: status={statusText}, elapsed={elapsed}s");
                
                string message = $"Session {statusText}. Elapsed: {elapsed}s\n" +
                               "███ ██ ██  ██ █  █  █  ▓▓▓ ▓▓ ▓▓  ▓  ▓  ▓  ▒▒▒ ▒▒ ▒▒ ▒  ▒  ░░░ ░░  ░░ ░ ░ ░ ░ ░ ░  ░  ░  ░   ░   ░   ░    ░    ░    ░     ░        ░";

                LogColor color = isSuccess ? LogColor.Green : LogColor.Orange;
                _project.SendToLog(message.Trim(), LogType.Info, true, color);
            }
            catch (Exception ex)
            {
                _logger?.Send($"Session log entry failed: {ex.Message}");
            }
        }
        
        #endregion
    }
    
    
}