using System;
using System.Text.RegularExpressions;

using ZennoLab.InterfacesLibrary.ProjectModel;

using System.Collections.Generic;
using Newtonsoft.Json;

namespace z3n7
{
    public class FirstMail
    {

        private readonly IZennoPosterProjectModel _project;
        private readonly Logger _logger;

        private string _key;
        private string _login;
        private string _pass;
        private string _proxy;
        private string _auth;
        private string[] _headers;
       

        private Dictionary<string, string> _commands = new Dictionary<string, string>
        {
            { "delete", "https://api.firstmail.ltd/v1/mail/delete" },
            { "getAll", "https://api.firstmail.ltd/v1/get/messages" },
            { "getOne", "https://api.firstmail.ltd/v1/mail/one" },
            
        };

        
        public FirstMail(IZennoPosterProjectModel project, Logger log = null)
        {
            _project = project;
            _logger = log;
            LoadKeys();
        }
        public FirstMail(IZennoPosterProjectModel project, string mail, string password, Logger log = null)
        {
            _project = project;
            _logger = log;
            LoadKeys();
            _login = Uri.EscapeDataString(mail);
            _pass = Uri.EscapeDataString(password);
            _auth = $"?username={_login} &password={_pass}";
        }

        private void LoadKeys()
        {
            var creds = _project.DbGetColumns("apikey, apisecret, passphrase, proxy", "_api", where: "id = 'firstmail'");

            _key = creds["apikey"];
            _login = Uri.EscapeDataString(creds["apisecret"]);
            _pass = Uri.EscapeDataString(creds["passphrase"]);
            _proxy = creds["proxy"];
            _headers = new [] { $"accept: application/json", $"X-API-KEY: {_key}" };
            _auth = $"?username={_login} &password={_pass}";
        }
        
        public string Delete(string email, bool seen = false)
        {
            string url = _commands["delete"] + _auth;//$"https://api.firstmail.ltd/v1/mail/delete?username={_login} &password={_pass}";
            string additional = seen ? "seen=true" : null;
            url += additional;
            string result = _project.GET(url,_proxy, _headers, parse:true);
            return result;
        }
        public string GetOne(string email)
        {
            string url = _commands["getOne"] + _auth;
            //string url = $"https://api.firstmail.ltd/v1/mail/one?username={_login}&password={_pass}";
            string result = _project.GET(url,_proxy, _headers, parse:true);
            return result;
        }
        public string GetAll(string email)
        {
            string url = _commands["getAll"] + _auth;
            string result = _project.GET(url,_proxy, _headers, parse:true);
            return result;
        }

        public string Get(int limit = 5)
        {
            var body = new
            {
                email    = Uri.UnescapeDataString(_login),
                password = Uri.UnescapeDataString(_pass),
                limit = limit,
                folder = "INBOX"
            };
            var messages = _project.POST("https://firstmail.ltd/api/v1/email/messages", JsonConvert.SerializeObject(body), _proxy, _headers, parse:true);
            return messages;
        }
        
        public string GetOTP(string email)
        {

            GetOne(email);
            //_project.Json.FromString(json);
            string deliveredTo = _project.Json.to[0];
            string text = _project.Json.text;
            string subject = _project.Json.subject;
            string html = _project.Json.html;

            if (!deliveredTo.Contains(email)) throw new Exception($"Fmail: Email {email} not found in last message");
            else
            {
                
                Match match = Regex.Match(subject, @"\b\d{6}\b");
                if (match.Success) 
                    return match.Value;

                match = Regex.Match(text, @"\b\d{6}\b");
                if (match.Success)
                    return match.Value;

                match = Regex.Match(html, @"\b\d{6}\b");
                if (match.Success) 
                    return match.Value;
                else throw new Exception("Fmail: OTP not found in message with correct email");
            }

        }
        public string GetLink(string email)
        {
            GetOne(email);
            string deliveredTo = _project.Json.to[0];
            string text = _project.Json.text;

            if (!deliveredTo.Contains(email))
                throw new Exception($"Fmail: Email {email} not found in last message");

            int startIndex = text.IndexOf("https://");
            if (startIndex == -1) startIndex = text.IndexOf("http://");
            if (startIndex == -1) throw new Exception($"No Link found in message {text}");

            string potentialLink = text.Substring(startIndex);
            int endIndex = potentialLink.IndexOfAny(new[] { ' ', '\n', '\r', '\t', '"' });
            if (endIndex != -1)
                potentialLink = potentialLink.Substring(0, endIndex);

            return Uri.TryCreate(potentialLink, UriKind.Absolute, out _)
                ? potentialLink
                : throw new Exception($"No Link found in message {text}");
        }

        public string Otp(string email)
        {
            var json = Get();
            // Парсим массив сообщений
            var messages = JsonConvert.DeserializeObject<List<dynamic>>(json);
    
            foreach (var msg in messages)
            {
                string deliveredTo = msg.to?.ToString() ?? "";
                if (!deliveredTo.Contains(email)) continue;

                string subject = msg.subject?.ToString() ?? "";
                string text    = msg.text?.ToString() ?? "";
                string html    = msg.html?.ToString() ?? "";

                Match match = Regex.Match(subject, @"\b\d{6}\b");
                if (match.Success) return match.Value;

                match = Regex.Match(text, @"\b\d{6}\b");
                if (match.Success) return match.Value;

                match = Regex.Match(html, @"\b\d{6}\b");
                if (match.Success) return match.Value;
            }

            throw new Exception($"Fmail: OTP not found in last {messages?.Count ?? 0} messages for {email}");
        }

    }

    public static partial class ProjectExtensions
    {
        public static string OtpCode(this IZennoPosterProjectModel project, string source)
        {
            if (source.Contains("@"))
                return new FirstMail(project).GetOTP(source);
            else
                return Tools.Otp.Offline(source);
        }
    }

}
