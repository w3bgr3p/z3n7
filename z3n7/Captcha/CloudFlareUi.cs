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
        
    }
 
}
