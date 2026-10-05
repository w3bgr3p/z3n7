using System;
using System.Text.RegularExpressions;

using ZennoLab.InterfacesLibrary.ProjectModel;

using System.Collections.Generic;
using Newtonsoft.Json;

namespace z3n7
{
    /// <summary>
    /// Client of the FirstMail mailbox API (<c>firstmail.ltd</c>).
    /// API key, default login, password and proxy come from the <c>_api</c> table, row <c>id =
    /// 'firstmail'</c> (<c>apikey</c>, <c>apisecret</c>, <c>passphrase</c>, <c>proxy</c>). Responses are
    /// loaded into <c>project.Json</c>. The client signs in to the mailbox that receives forwarded mail. An
    /// <c>email</c> argument is the original recipient: the address the message was sent to, which
    /// forwarded it to this mailbox.
    /// </summary>
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

        
        /// <summary>Creates a client for the mailbox stored in the database.</summary>
        /// <param name="log">Not used.</param>
        public FirstMail(IZennoPosterProjectModel project, Logger log = null)
        {
            _project = project;
            _logger = log;
            LoadKeys();
        }
        /// <summary>
        /// Creates a client for the given mailbox; the API key and proxy still come from the database.
        /// </summary>
        /// <param name="mail">Mailbox address.</param>
        /// <param name="password">Mailbox password.</param>
        /// <param name="log">Not used.</param>
        public FirstMail(IZennoPosterProjectModel project, string mail, string password, Logger log = null)
        {
            _project = project;
            _logger = log;
            LoadKeys();
            _login = Uri.EscapeDataString(mail);
            _pass = Uri.EscapeDataString(password);
            _auth = $"?username={_login}&password={_pass}";
        }

        private void LoadKeys()
        {
            var creds = _project.DbGetColumns("apikey, apisecret, passphrase, proxy", "_api", where: "id = 'firstmail'");

            _key = creds["apikey"];
            _login = Uri.EscapeDataString(creds["apisecret"]);
            _pass = Uri.EscapeDataString(creds["passphrase"]);
            _proxy = creds["proxy"];
            _headers = new [] { $"accept: application/json", $"X-API-KEY: {_key}" };
            _auth = $"?username={_login}&password={_pass}";
        }
        
        /// <summary>Calls <c>/v1/mail/delete</c> for the client's mailbox.</summary>
        /// <param name="email">Original recipient; not used.</param>
        /// <param name="seen">Append <c>seen=true</c> to the request URL.</param>
        public string Delete(string email, bool seen = false)
        {
            string url = _commands["delete"] + _auth;//$"https://api.firstmail.ltd/v1/mail/delete?username={_login} &password={_pass}";
            string additional = seen ? "&seen=true" : null;
            url += additional;
            string result = _project.GET(url,_proxy, _headers, parse:true);
            return result;
        }
        /// <summary>Latest message of the client's mailbox (<c>/v1/mail/one</c>).</summary>
        /// <param name="email">Original recipient; not used.</param>
        public string GetOne(string email)
        {
            string url = _commands["getOne"] + _auth;
            //string url = $"https://api.firstmail.ltd/v1/mail/one?username={_login}&password={_pass}";
            string result = _project.GET(url,_proxy, _headers, parse:true);
            return result;
        }
        /// <summary>Messages of the client's mailbox (<c>/v1/get/messages</c>).</summary>
        /// <param name="email">Original recipient; not used.</param>
        public string GetAll(string email)
        {
            string url = _commands["getAll"] + _auth;
            string result = _project.GET(url,_proxy, _headers, parse:true);
            return result;
        }

        /// <summary>Latest INBOX messages of the client's mailbox (<c>/api/v1/email/messages</c>).</summary>
        /// <param name="limit">How many.</param>
        /// <returns>JSON array of messages.</returns>
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
        
        /// <summary>
        /// Takes the latest message; when its first recipient contains <c>email</c>, returns the first 6-digit
        /// number of the subject, text or HTML.
        /// </summary>
        /// <param name="email">Original recipient the message was sent to.</param>
        /// <returns>The code. Throws when the latest message is for another address or has no code.</returns>
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
        /// <summary>
        /// Takes the latest message; when its first recipient contains <c>email</c>, returns the first http(s)
        /// link of its text.
        /// </summary>
        /// <param name="email">Original recipient the message was sent to.</param>
        /// <returns>The link. Throws when there is none.</returns>
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

        /// <summary>
        /// Looks through the latest 5 INBOX messages for one sent to <c>email</c> and returns the first 6-digit
        /// number of its subject, text or HTML.
        /// </summary>
        /// <param name="email">Original recipient the message was sent to.</param>
        /// <returns>The code. Throws when none is found.</returns>
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

    /// <summary>Extension methods on <c>IZennoPosterProjectModel</c>: one-time codes from mail.</summary>
    public static partial class ProjectExtensions
    {
        /// <summary>
        /// One-time code from a source: for an address (contains <c>@</c>), the code from its latest FirstMail
        /// message; otherwise <c>source</c> is a TOTP secret and the current code is computed locally.
        /// </summary>
        /// <param name="source">Mailbox address or TOTP secret.</param>
        public static string OtpCode(this IZennoPosterProjectModel project, string source)
        {
            if (source.Contains("@"))
                return new FirstMail(project).GetOTP(source);
            else
                return Tools.Otp.Offline(source);
        }
    }

}
