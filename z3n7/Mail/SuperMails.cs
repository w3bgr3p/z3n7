using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;

using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Api
{
    public class SuperMails
    {
        private string baseurl = "https://api.supermails.ru";
        private string _apikey;
        private IZennoPosterProjectModel _project;
        bool netHttp;
        
        public SuperMails(IZennoPosterProjectModel project, bool useNetHttp = false)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            netHttp = useNetHttp;
            _apikey = project.ReadEnv("SUPERMAILS_API_KEY");
        }
        
        public string[] NewMail(string pool = null, string domain = null)
        {
            var json = _project.GET($"{baseurl}/email/order?token={_apikey}", log:true, useNetHttp:netHttp, thrw:true);
            _project.ToJson(json);
            string id = _project.Json.id.ToString();
            string email = _project.Json.email.ToString();
            _project.Var("mailId", id);
            _project.Var("email", email);
            _project.Profile.Email =  email;
            return new []{id, email};
        }

        public string GetMail(int deadline = 60)
        {
            var id = _project.Var("mailId");
            var d = new Time.Deadline();

            while (true)

            {
                Thread.Sleep(5000);
                d.Check(deadline);
                var body = _project.GET($"{baseurl}/email/getmessage?token={_apikey}&id={id}", log:true, useNetHttp:netHttp, thrw:true , parse:true);
                if (body.Contains("wait message"))
                    continue;
                else 
                    return body;
            }
        }

        public HashSet<string> GetHrefs(int deadline = 60)
        {
            var json = GetMail(deadline);
            _project.ToJson(json);

            var message = _project.Json.message?.ToString() ?? "";

            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var document = new HtmlDocument();
            document.LoadHtml(message);

            var nodes = document.DocumentNode.SelectNodes("//*[@href]");

            if (nodes == null)
                return result;

            for (int i = 0; i < nodes.Count; i++)
            {
                var href = NormalizeHref(nodes[i].GetAttributeValue("href", ""));

                if (string.IsNullOrEmpty(href))
                    continue;

                if (IsStaticHref(href))
                    continue;

                result.Add(href);
            }

            return result;
        }
        
        public string Otp()
        {
            var json = GetMail();
            _project.ToJson(json);

            var subject = _project.Json.subject?.ToString() ?? "";
            var message  = _project.Json.message?.ToString()  ?? "";

            // strip HTML
            var body = Regex.Replace(message, "<.*?>", "");

            var match = Regex.Match(subject, @"\b\d{6}\b");
            if (match.Success) return match.Value;

            match = Regex.Match(body, @"\b\d{6}\b");
            if (match.Success) return match.Value;

            throw new Exception("SuperMails: OTP not found");
        }

        private static string NormalizeHref(string href)
        {
            if (string.IsNullOrWhiteSpace(href))
                return "";

            href = WebUtility.HtmlDecode(href).Trim();

            if (href.Length == 0)
                return "";

            if (href.StartsWith("#") ||
                href.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ||
                href.StartsWith("tel:", StringComparison.OrdinalIgnoreCase) ||
                href.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase) ||
                href.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                return "";
            }

            return href;
        }

        private static bool IsStaticHref(string href)
        {
            string value = href;
            int queryIndex = value.IndexOf('?');

            if (queryIndex >= 0)
                value = value.Substring(0, queryIndex);

            int hashIndex = value.IndexOf('#');

            if (hashIndex >= 0)
                value = value.Substring(0, hashIndex);

            return
                value.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".js", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".woff", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
                value.EndsWith(".eot", StringComparison.OrdinalIgnoreCase);
        }
    }
}
