using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3nIO.Captcha
{

    public class CapMonsterSolver
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly Logger _log;
        
        public CapMonsterSolver()
        {
        }
        public CapMonsterSolver(IZennoPosterProjectModel project)
        {
            _project = project;
            _log =  new Logger(_project);
        }
        public string Solve(string siteKey, string websiteUrl, string clientKey, string task = "recapctcha")
        {

            switch (task)
            {
                case "cf":
                    task = "TurnstileTaskProxyless";
                    break;
                case "reV2":
                    task = "NoCaptchaTaskProxyless";
                    break;
                case "reV2_p":
                    task = "NoCaptchaTask";
                    break;
                case "reV3":
                    task = "RecaptchaV3TaskProxyless";
                    break;
                case "reV3e":
                    task = "RecaptchaV2EnterpriseTaskProxyless";
                    break;
                case "hc":
                    task = "HCaptchaTask";
                    break;
            }

   

            try
            {
                // ---------------------------
                // 1️⃣ Создаём задачу
                // ---------------------------
                string createBody = $@"{{
              ""clientKey"": ""{clientKey}"",
              ""task"": {{
                ""type"": ""{task}"",
                ""websiteURL"": ""{websiteUrl}"",
                ""websiteKey"": ""{siteKey}""
              }}
            }}";

                var request = (HttpWebRequest)WebRequest.Create("https://api.capmonster.cloud/createTask");
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";

                using (var stream = new StreamWriter(request.GetRequestStream()))
                    stream.Write(createBody);

                string createResponse;
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                    createResponse = reader.ReadToEnd();
               
                
                _log?.Send(createResponse);
               // ("CreateTask: " + createResponse).Debug();
               
                string taskId = Regex.Match(createResponse, @"""taskId"":\s*(\d+)").Groups[1].Value;
                if (string.IsNullOrEmpty(taskId))
                {
                    //"Не удалось получить taskId".Debug();
                    return null;
                }

                // ---------------------------
                // 2️⃣ Ждём решение
                // ---------------------------
                string token = "";
                for (int i = 0; i < 30; i++)
                {
                    Thread.Sleep(2000);

                    string getBody = $@"{{
                  ""clientKey"": ""{clientKey}"",
                  ""taskId"": {taskId}
                }}";

                    var getRequest = (HttpWebRequest)WebRequest.Create("https://api.capmonster.cloud/getTaskResult");
                    getRequest.Method = "POST";
                    getRequest.ContentType = "application/json";
                    getRequest.Accept = "application/json";

                    using (var stream = new StreamWriter(getRequest.GetRequestStream()))
                        stream.Write(getBody);

                    string getResponse;
                    using (var response = (HttpWebResponse)getRequest.GetResponse())
                    using (var reader = new StreamReader(response.GetResponseStream()))
                        getResponse = reader.ReadToEnd();

                    _log?.Send(getResponse);
                    
                   // ("CheckResult: " + getResponse).Debug();
                    if (getResponse.Contains(@"""status"":""ready"""))
                    {
                        token = Regex.Match(getResponse, @"""gRecaptchaResponse"":""([^""]+)""").Groups[1].Value;
                        if (string.IsNullOrEmpty(token))
                            token = Regex.Match(getResponse, @"""token"":""([^""]+)""").Groups[1].Value;
                        break;
                    }
                }

               // if (string.IsNullOrEmpty(token)) "Не удалось получить токен после ожидания".Debug();

                return token;
            }
            catch (Exception ex)
            {
                _log?.Warn(ex);
                //("SolveTurnstile ERROR: " + ex.Message).Debug();
                return null;
            }
        }
    }
}
