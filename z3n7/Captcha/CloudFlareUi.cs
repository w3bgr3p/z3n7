using System;
using System.IO;
using System.Threading;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3nIO.Captcha
{
    public static partial  class CaptchaExtensions
    {
        public static void CFSolve(this Instance instance)
        {
            Random rnd = new Random(); string strX = ""; string strY = ""; Thread.Sleep(3000);
            HtmlElement he1 = instance.ActiveTab.FindElementById("cf-turnstile");
            HtmlElement he2 = instance.GetHe(("div", "outerhtml", "<div><input type=\"hidden\" name=\"cf-turnstile-response\"", "regexp", 0), "last");
            // instance.ActiveTab.FindElementByAttribute("div", "outerhtml", "<div><input type=\"hidden\" name=\"cf-turnstile-response\"", "regexp", 4);
            if (he1.IsVoid && he2.IsVoid) return;
            else if (!he1.IsVoid)
            {
                strX = he1.GetAttribute("leftInbrowser"); strY = he1.GetAttribute("topInbrowser");
            }
            else if (!he2.IsVoid)
            {
                strX = he2.GetAttribute("leftInbrowser"); strY = he2.GetAttribute("topInbrowser");
            }

            int rndX = rnd.Next(23, 26); int x = (int.Parse(strX) + rndX);
            int rndY = rnd.Next(27, 31); int y = (int.Parse(strY) + rndY);
            Thread.Sleep(rnd.Next(4, 5) * 1000);
            instance.WaitFieldEmulationDelay();
            instance.Click(x, x, y, y, "Left", "Normal");
            Thread.Sleep(rnd.Next(3, 4) * 1000);

        }
        public static string CFToken(this Instance instance, int deadline = 60, bool strict = false)
        {
            DateTime timeout = DateTime.Now.AddSeconds(deadline);
            while (true)
            {
                if (DateTime.Now > timeout) throw new Exception($"!W CF timeout");
                Random rnd = new Random();

                Thread.Sleep(rnd.Next(3, 4) * 1000);

                var token = instance.HeGet(("cf-turnstile-response", "name"), atr: "value");
                if (!string.IsNullOrEmpty(token)) return token;

                string strX = ""; string strY = "";

                try
                {
                    var cfBox = instance.GetHe(("cf-turnstile", "id"));
                    strX = cfBox.GetAttribute("leftInbrowser"); strY = cfBox.GetAttribute("topInbrowser");
                }
                catch
                {
                    var cfBox = instance.GetHe(("div", "outerhtml", "<div><input type=\"hidden\" name=\"cf-turnstile-response\"", "regexp", 4));
                    strX = cfBox.GetAttribute("leftInbrowser"); strY = cfBox.GetAttribute("topInbrowser");
                }

                int x = (int.Parse(strX) + rnd.Next(23, 26));
                int y = (int.Parse(strY) + rnd.Next(27, 31));
                instance.Click(x, x, y, y, "Left", "Normal");

            }
        }
        public static string CFInline(this Instance instance)
        {
            var d = new Time.Deadline();
            Random rnd = new Random();
            var trustline = ""; 
            string strX = ""; 
            string strY = ""; 

            while (string.IsNullOrEmpty(trustline))
            {
                d.Check(60);
	
                try{

                    HtmlElement challenge = instance.GetHe(("div", "innerhtml", "name=\"cf_challenge_response\"", "regexp", 0), "last");
                    if (!challenge.IsVoid)
                    {
                        strX = challenge.GetAttribute("leftInbrowser"); 
                        strY = challenge.GetAttribute("topInbrowser");
            
		
                        int x = (int.Parse(strX) + rnd.Next(23, 26));
                        int y = (int.Parse(strY) + rnd.Next(27, 31));
				
                        Thread.Sleep(rnd.Next(4, 5) * 1000);
                        instance.WaitFieldEmulationDelay();
                        instance.Click(x, x, y, y, "Left", "Normal");
                    }
                    Thread.Sleep(rnd.Next(3, 4) * 1000);	
                    challenge = instance.GetHe(("div", "innerhtml", "name=\"cf_challenge_response\"", "regexp", 0), "last");
                    trustline = challenge.FirstChild.GetAttribute("value");
                }
                catch(Exception ex){
		
                }
            }
            return trustline;

        }
        
        public static string CFBlank(this Instance instance)
        {
            var d = new Time.Deadline();
            Random rnd = new Random();
            var trustline = ""; 
            string strX = ""; 
            string strY = ""; 

            while (string.IsNullOrEmpty(trustline))
            {
                d.Check(60);
	
                try{

                    HtmlElement challenge = instance.GetHe(("div", "innerhtml", "name=\"cf-turnstile-response\"", "regexp", 0), "last");
                    if (!challenge.IsVoid)
                    {
                        strX = challenge.GetAttribute("leftInbrowser"); 
                        strY = challenge.GetAttribute("topInbrowser");
            
		
                        int x = (int.Parse(strX) + rnd.Next(23, 26));
                        int y = (int.Parse(strY) + rnd.Next(27, 31));
				
                        Thread.Sleep(rnd.Next(4, 5) * 1000);
                        instance.WaitFieldEmulationDelay();
                        instance.Click(x, x, y, y, "Left", "Normal");
                    }
                    Thread.Sleep(rnd.Next(3, 4) * 1000);	
                    challenge = instance.GetHe(("div", "innerhtml", "name=\"cf-turnstile-response\"", "regexp", 0), "last");
                    trustline = challenge.FirstChild.GetAttribute("value");
                }
                catch(Exception ex){
		
                }
            }
            return trustline;

        }

        public static string SolveCFInLine(this Instance instance, int deadline = 60)
        {
            var d = new Time.Deadline();
            Random rnd = new Random();
            var trustline = ""; 
            string strX = ""; 
            string strY = ""; 

            while (string.IsNullOrEmpty(trustline))
            {
                d.Check(deadline);
	
                try{
		
                    instance.HeClick(("button", "innertext", "Accept\\ All\\ Cookies", "regexp", 0),deadline:1, thrw:false);

                    HtmlElement challenge = instance.GetHe(("div", "innerhtml", "name=\"cf_challenge_response\"", "regexp", 0), "last");
                    if (!challenge.IsVoid)
                    {
                        strX = challenge.GetAttribute("leftInbrowser"); 
                        strY = challenge.GetAttribute("topInbrowser");
            
		
                        int x = (int.Parse(strX) + rnd.Next(23, 26));
                        int y = (int.Parse(strY) + rnd.Next(27, 31));
				
                        Thread.Sleep(rnd.Next(4, 5) * 1000);
                        instance.WaitFieldEmulationDelay();
                        instance.Click(x, x, y, y, "Left", "Normal");
                    }
                    Thread.Sleep(rnd.Next(3, 4) * 1000);	
                    challenge = instance.GetHe(("div", "innerhtml", "name=\"cf_challenge_response\"", "regexp", 0), "last");
                    trustline = challenge.FirstChild.GetAttribute("value");
                }
                catch(Exception ex){
                    Console.WriteLine(ex.Message);
                }
            }
            
            return trustline;
        }
        
        public static string SolveCFInBlank(this Instance instance, int deadline = 60, int waitBeforeMs = 5000)
        {
            var d = new Time.Deadline();
            Random rnd = new Random();
            var trustline = ""; 
            string strX = ""; 
            string strY = ""; 
            Thread.Sleep(waitBeforeMs);
            while (!instance.ActiveTab.FindElementByAttribute("h1", "innertext", "dash.cloudflare.com", "regexp", 0).IsVoid)
            {
                d.Check(deadline);
	
                try{

                    HtmlElement challenge = instance.GetHe(("div", "innerhtml", "name=\"cf-turnstile-response\"", "regexp", 0), "last");
                    if (!challenge.IsVoid)
                    {
                        strX = challenge.GetAttribute("leftInbrowser"); 
                        strY = challenge.GetAttribute("topInbrowser");
            
		
                        int x = (int.Parse(strX) + rnd.Next(23, 26));
                        int y = (int.Parse(strY) + rnd.Next(27, 31));
				
                        Thread.Sleep(rnd.Next(4, 5) * 1000);
                        instance.WaitFieldEmulationDelay();
                        instance.Click(x, x, y, y, "Left", "Normal");
                    }
                    Thread.Sleep(rnd.Next(3, 4) * 1000);	
                    challenge = instance.GetHe(("div", "innerhtml", "name=\"cf-turnstile-response\"", "regexp", 0), "last");
                    trustline = challenge.FirstChild.GetAttribute("value");
                }
                catch(Exception ex){
		
                }
            }
            return trustline;
        }

    }
 
}
