using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.Enums.Browser;
using ZennoLab.InterfacesLibrary.ProjectModel;
namespace z3n7
{
    public static partial class InstanceExtensions
    {
        private static readonly object _clipboardLock = new object();
        private static readonly SemaphoreSlim ClipboardSemaphore = new SemaphoreSlim(1, 1);
        private static readonly object LockObject = new object();
        private static readonly Time.Sleeper _clickSleep = new Time.Sleeper(1008, 1337);
        private static readonly Time.Sleeper _inputSleep = new Time.Sleeper(1337, 2077);
        private static readonly Time.Sleeper _longSleep = new Time.Sleeper(2, 4);
        private static Random _random = new Random();
        
        private class ElementNotFoundException : Exception
        {
            public ElementNotFoundException(string message) : base(message) { }
        }
        
        #region Element Getters
        
        public static HtmlElement GetHe(this Instance instance, object obj, string method = "")
        {
            if (obj is HtmlElement element)
            {
                if (element.IsVoid) throw new Exception("Provided HtmlElement is void");
                return element;
            }

            Type inputType = obj.GetType();
            int objLength = inputType.GetFields().Length;

            if (objLength == 2)
            {
                string value = inputType.GetField("Item1").GetValue(obj).ToString();
                method = inputType.GetField("Item2").GetValue(obj).ToString();

                if (method == "id")
                {
                    HtmlElement he = instance.ActiveTab.FindElementById(value);
                    if (he.IsVoid) throw new Exception($"no element by id=[{value}]");
                    return he;
                }
                else if (method == "name")
                {
                    HtmlElement he = instance.ActiveTab.FindElementByName(value);
                    if (he.IsVoid) throw new Exception($"no element by name=[{value}]");
                    return he;
                }
                else
                {
                    throw new Exception($"Unsupported method for tuple: {method}");
                }
            }
            else if (objLength == 5)
            {
                string tag = inputType.GetField("Item1").GetValue(obj).ToString();
                string attribute = inputType.GetField("Item2").GetValue(obj).ToString();
                string pattern = inputType.GetField("Item3").GetValue(obj).ToString();
                string mode = inputType.GetField("Item4").GetValue(obj).ToString();
                object posObj = inputType.GetField("Item5").GetValue(obj);
                int pos;
                if (!int.TryParse(posObj.ToString(), out pos)) throw new ArgumentException("5th element of Tupple must be (int).");

                if (method == "random")
                {
                    var elements = instance.ActiveTab.FindElementsByAttribute(tag, attribute, pattern, mode).ToList();
                    if (elements.Count == 0)
                    {
                        throw new Exception($"no elements for random: tag=[{tag}] attr=[{attribute}] pattern=[{pattern}]");
                    }
                    return elements.Rnd();
                }
                
                if (method == "last")
                {
                    var elements = instance.ActiveTab.FindElementsByAttribute(tag, attribute, pattern, mode).ToList();
                    if (elements.Count != 0)
                    {
                        return elements[elements.Count - 1];
                    }

                    int index = 0;
                    while (true)
                    {
                        HtmlElement he = instance.ActiveTab.FindElementByAttribute(tag, attribute, pattern, mode, index);
                        if (he.IsVoid)
                        {
                            he = instance.ActiveTab.FindElementByAttribute(tag, attribute, pattern, mode, index - 1);
                            if (he.IsVoid)
                            {
                                throw new Exception($"no element by: tag=[{tag}] attribute=[{attribute}] pattern=[{pattern}] mode=[{mode}]");
                            }
                            return he;
                        }
                        index++;
                    }
                }
                else
                {
                    HtmlElement he = instance.ActiveTab.FindElementByAttribute(tag, attribute, pattern, mode, pos);
                    if (he.IsVoid)
                    {
                        throw new Exception($"no element by: tag=[{tag}] attribute=[{attribute}] pattern=[{pattern}] mode=[{mode}] pos=[{pos}]");
                    }
                    return he;
                }
            }

            throw new ArgumentException($"Unsupported type: {obj?.GetType()?.ToString() ?? "null"}");
        }

        private static void WriteToScript(this HtmlElement he, string pathToScript, string action)
        {
            if (!string.IsNullOrEmpty(pathToScript))
            {
                string line = action + "\t" + he.GetXPath() + "\n"; 
                File.AppendAllText(pathToScript, line);
            }
        }
        
        #endregion
        
        #region Element Actions
        
        public static string HeGet(this Instance instance, object obj, string method = "", int deadline = 10, string atr = "innertext", int delay = 1, bool thrw = true, bool thr0w = true, bool waitTillVoid = false, string pathToScript = null)
        {
            DateTime functionStart = DateTime.Now;
            string lastExceptionMessage = "";

            if (!thr0w) thrw = thr0w;
            
            while (true)
            {
                if ((DateTime.Now - functionStart).TotalSeconds > deadline)
                {
                    if (waitTillVoid)
                    {
                        return null;
                    }
                    else if (thrw)
                    {
                        string url = instance.ActiveTab.URL;
                        throw new ElementNotFoundException($"not found in {deadline}s: {lastExceptionMessage}. URL is: {url}");
                    }
                    else
                    {
                        return null;
                    }
                }

                try
                {
                    HtmlElement he = instance.GetHe(obj, method);
                    he.WriteToScript(pathToScript, "get");
                    
                    if (waitTillVoid)
                    {
                        throw new Exception($"element detected when it should not be: {atr}='{he.GetAttribute(atr)}'");
                    }
                    else
                    {
                        Thread.Sleep(delay * 1000);
                        return he.GetAttribute(atr);
                    }
                }
                catch (Exception ex)
                {
                    lastExceptionMessage = ex.Message;
                    if (waitTillVoid && ex.Message.Contains("no element by"))
                    {
                        // Element not found - expected, continue waiting
                    }
                    else if (!waitTillVoid)
                    {
                        // Normal behavior: element not found, log error and wait
                    }
                    else
                    {
                        // Unexpected error in waitTillVoid, rethrow
                        throw;
                    }
                }

                Thread.Sleep(500);
            }
        }
        
        public static string HeCatch(this Instance instance, object obj, string method = "", int deadline = 10, string atr = "innertext", int delay = 1, string pathToScript = null)
        {
            DateTime functionStart = DateTime.Now;
            string lastExceptionMessage = "";

            while (true)
            {
                if ((DateTime.Now - functionStart).TotalSeconds > deadline)
                {
                    return null;
                }

                try
                {
                    HtmlElement he = instance.GetHe(obj, method);
                    throw new Exception(he.GetAttribute(atr));
                }
                catch (Exception ex)
                {
                    lastExceptionMessage = ex.Message;
                    if (ex.Message.Contains("no element by"))
                    {
                        // Element not found - good, continue waiting
                    }
                    else
                    {
                        // Real error or "element detected" exception
                        throw;
                    }
                }

                Thread.Sleep(500);
            }
        }

        public static void HeMultiClick(this Instance instance, List<object> selectors)
        {
            foreach (var selector in selectors) 
                instance.HeClick(selector);
        }

        public static void HeClick(this Instance instance, object obj, string method = "", int deadline = 10, double delay = 1, string comment = "", bool thrw = true, bool thr0w = true, int emu = 0, string pathToScript = null)
        {
            bool emuSnap = instance.UseFullMouseEmulation;
            if (emu > 0) instance.UseFullMouseEmulation = true;
            if (emu < 0) instance.UseFullMouseEmulation = false;
            
            if (!thr0w) thrw = thr0w;
            
            DateTime functionStart = DateTime.Now;
            string lastExceptionMessage = "";

            while (true)
            {
                if ((DateTime.Now - functionStart).TotalSeconds > deadline)
                {
                    if (thrw) throw new TimeoutException($"{comment} not found in {deadline}s: {lastExceptionMessage}");
                    else return;
                }

                try
                {
                    HtmlElement he = instance.GetHe(obj, method);
                    he.WriteToScript(pathToScript, "click");
                    _clickSleep.Sleep(delay);
                    he.RiseEvent("click", instance.EmulationLevel);
                    instance.UseFullMouseEmulation = emuSnap;
                    break;
                }
                catch (Exception ex)
                {
                    lastExceptionMessage = ex.Message;
                    instance.UseFullMouseEmulation = emuSnap;
                }
                Thread.Sleep(500);
            }

            if (method == "clickOut")
            {
                if ((DateTime.Now - functionStart).TotalSeconds > deadline)
                {
                    instance.UseFullMouseEmulation = emuSnap;
                    if (thr0w) throw new TimeoutException($"{comment} not found in {deadline}s: {lastExceptionMessage}");
                    else return;
                }
                
                while (true)
                {
                    try
                    {
                        HtmlElement he = instance.GetHe(obj, method);
                        _clickSleep.Sleep(delay);
                        he.RiseEvent("click", instance.EmulationLevel);
                        continue;
                    }
                    catch
                    {
                        instance.UseFullMouseEmulation = emuSnap;
                        break;
                    }
                }
            }
        }
        
        public static void HeLongClick(this Instance instance, object obj, int holdMs = 3 , string method = "", int deadline = 10, double delay = 1, string comment = "", bool thrw = true, bool thr0w = true, int emu = 0)
        {
            if (holdMs < 0) throw new ArgumentOutOfRangeException(nameof(holdMs));
            if (!thr0w) thrw = false;

            bool emuSnap = instance.UseFullMouseEmulation;
            DateTime functionStart = DateTime.Now;
            string lastExceptionMessage = "";

            try
            {
                if (emu > 0) instance.UseFullMouseEmulation = true;
                if (emu < 0) instance.UseFullMouseEmulation = false;

                while (true)
                {
                    if ((DateTime.Now - functionStart).TotalSeconds > deadline)
                    {
                        if (thrw) throw new TimeoutException($"{comment} not found in {deadline}s: {lastExceptionMessage}");
                        return;
                    }

                    try
                    {
                        HtmlElement he = instance.GetHe(obj, method);
                        
                        var position = he.DisplacementInTabWindow;
                        var area = new Rectangle(
                            position.X,
                            position.Y,
                            he.BoundingClientWidth,
                            he.BoundingClientHeight);

                        _clickSleep.Sleep(delay);
                        HoldClick(instance, area, holdMs);
                        return;
                    }
                    catch (Exception ex)
                    {
                        lastExceptionMessage = ex.Message;
                    }

                    Thread.Sleep(500);
                }
            }
            finally
            {
                instance.UseFullMouseEmulation = emuSnap;
            }
        }

        public static void HeLongClick(this Instance instance, int x, int y, int holdMs = 3, double delay = 1, int emu = 0)
        {
            if (holdMs < 0) throw new ArgumentOutOfRangeException(nameof(holdMs));

            bool emuSnap = instance.UseFullMouseEmulation;

            try
            {
                if (emu > 0) instance.UseFullMouseEmulation = true;
                if (emu < 0) instance.UseFullMouseEmulation = false;

                _clickSleep.Sleep(delay);
                HoldClick(instance, new Rectangle(x, y, 1, 1), holdMs);
            }
            finally
            {
                instance.UseFullMouseEmulation = emuSnap;
            }
        }

        private static void HoldClick(Instance instance, Rectangle area, int holdMs)
        {
            if (area.Width <= 0 || area.Height <= 0)
                throw new ArgumentException("Click area must have positive width and height", nameof(area));

            int x;
            int y;

            lock (LockObject)
            {
                x = _random.Next(area.Left, area.Right);
                y = _random.Next(area.Top, area.Bottom);
            }

            try
            {
                instance.ActiveTab.MouseClick(x, y, "left", "down");
                Thread.Sleep(holdMs);
            }
            finally
            {
                instance.ActiveTab.MouseClick(x, y, "left", "up");
            }
        }
        
        
        public static void HeSet(this Instance instance, object obj, string value, string method = "id", int deadline = 10, double delay = 1, string comment = "", bool thrw = true, bool thr0w = true, string pathToScript = null)
        {
            DateTime functionStart = DateTime.Now;
            string lastExceptionMessage = "";

            if (!thr0w) thrw = thr0w;
            
            while (true)
            {
                if ((DateTime.Now - functionStart).TotalSeconds > deadline)
                {
                    if (thrw) throw new TimeoutException($"{comment} not found in {deadline}s: {lastExceptionMessage}");
                    else return;
                }

                try
                {
                    HtmlElement he = instance.GetHe(obj, method);
                    _inputSleep.Sleep(delay);
                    he.WriteToScript(pathToScript, "set");
                    instance.WaitFieldEmulationDelay();
                    he.SetValue(value, "Full", false);
                    break;
                }
                catch (Exception ex)
                {
                    lastExceptionMessage = ex.Message;
                }

                Thread.Sleep(500);
            }
        }
        
        public static void HeDrop(this Instance instance, object obj, string method = "", int deadline = 10, bool thrw = true)
        {
            DateTime functionStart = DateTime.Now;
            string lastExceptionMessage = "";

            while (true)
            {
                if ((DateTime.Now - functionStart).TotalSeconds > deadline)
                {
                    if (thrw) throw new TimeoutException($"not found in {deadline}s: {lastExceptionMessage}");
                    else return;
                }
                
                try
                {
                    HtmlElement he = instance.GetHe(obj, method);
                    HtmlElement heParent = he.ParentElement;
                    heParent.RemoveChild(he);
                    break;
                }
                catch (Exception ex)
                {
                    lastExceptionMessage = ex.Message;
                }
                
                Thread.Sleep(500);
            }
        }
        


        #endregion

        #region Browser Management
        
        public static void ClearShit(this Instance instance, string domain)
        {
            instance.CloseAllTabs();
            instance.ClearCache(domain);
            instance.ClearCookie(domain);
            Thread.Sleep(500);
            instance.ActiveTab.Navigate("about:blank", "");
        }
        
        public static void CloseExtraTabs(this Instance instance, bool blank = false, int tabToKeep = 1)
        {
            for (; ; )
            {
                try
                {
                    instance.AllTabs[tabToKeep].Close();
                    Thread.Sleep(1000);
                }
                catch
                {
                    break;
                }
            }
            
            Thread.Sleep(500);
            if (blank) instance.ActiveTab.Navigate("about:blank", "");
        }

        public static void CloseNewTab(this Instance instance, int deadline = 10, int tabIndex = 2, bool thrw = true)
        {
            int i = 0;

            while (i < deadline)
            {
                i++;
                Thread.Sleep(1000);
                if (instance.AllTabs.ToList().Count == tabIndex)
                {
                    instance.CloseExtraTabs();
                    return;
                }
            }
            
            if (thrw) throw new Exception("no new tab found");
        }
        
        public static void Go(this Instance instance, string url, bool strict = false, bool waitTdle = false, bool newTab = false)
        {
            if (newTab)
            {
                Tab tab = instance.NewTab("new");
            }
            
            bool go = false;
            string current = instance.ActiveTab.URL;
            if (strict) if (current != url) go = true;
            if (!strict) if (!current.Contains(url)) go = true;
            if (go) instance.ActiveTab.Navigate(url, "");
            
            if (instance.ActiveTab.IsBusy && waitTdle) instance.ActiveTab.WaitDownloading();
        }
        
        public static void F5(this Instance instance, bool WaitTillLoad = true)
        {
            instance.ActiveTab.MainDocument.EvaluateScript("location.reload(true)");
            if (instance.ActiveTab.IsBusy && WaitTillLoad) instance.ActiveTab.WaitDownloading();
        }

        public static void ScrollDown(this Instance instance, int y = 420)
        {
            bool emu = instance.UseFullMouseEmulation;
            instance.UseFullMouseEmulation = true;
            instance.ActiveTab.FullEmulationMouseWheel(0, y);
            instance.UseFullMouseEmulation = emu;
        }
        
        public static void CtrlV(this Instance instance, string ToPaste)
        {
            lock (_clipboardLock)
            {
                string originalClipboard = null;
                try
                {
                    if (System.Windows.Forms.Clipboard.ContainsText())
                        originalClipboard = System.Windows.Forms.Clipboard.GetText();

                    System.Windows.Forms.Clipboard.SetText(ToPaste);
                    instance.ActiveTab.KeyEvent("v", "press", "ctrl");

                    if (!string.IsNullOrEmpty(originalClipboard))
                        System.Windows.Forms.Clipboard.SetText(originalClipboard);
                }
                catch { }
            }
        }
        
        public static void UpFromFolder(this Instance instance, string pathProfile, bool useProfile = false, BrowserType browserType = BrowserType.Chromium)
        {
            ZennoLab.CommandCenter.Classes.BuiltInBrowserLaunchSettings settings =
                (ZennoLab.CommandCenter.Classes.BuiltInBrowserLaunchSettings)ZennoLab.CommandCenter.Classes.BrowserLaunchSettingsFactory.Create(browserType);
            settings.CachePath = pathProfile; 
            settings.ConvertProfileFolder = true;
            settings.UseProfile = useProfile;
            instance.Launch(settings);
        }
        
        public static void UpEmpty(this Instance instance)
        {
            instance.Launch(BrowserType.Chromium, false);
        }
        
        public static void Down(this Instance instance, int pauseAfterMs = 5000)
        {
            try
            {
                instance.Launch(BrowserType.WithoutBrowser, false);
            }
            catch { }
            
            Thread.Sleep(pauseAfterMs);
        }
        
        public static string SaveCookies(this Instance instance)
        {
            string tmp = Path.Combine(
                Path.GetTempPath(),
                $"cookies_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{Guid.NewGuid().ToString("N").Substring(0, 8)}.txt"
            );

            try
            {
                instance.SaveCookie(tmp);
                var cookieContent = File.ReadAllText(tmp);
                return cookieContent;
            }
            finally
            {
                try
                {
                    if (File.Exists(tmp))
                    {
                        File.Delete(tmp);
                    }
                }
                catch { }
            }
        }

        public static void SetTimeFromDb(this Instance instance,  IZennoPosterProjectModel project)
        {
            var timezone = project.DbGet("timezone", "_instance");
            if (string.IsNullOrEmpty(timezone))
            {
                project.warn("no time zone data found in db"); 
                return;
            }
            JObject tz = JObject.Parse(timezone);
            instance.TimezoneWorkMode = ZennoLab.InterfacesLibrary.Enums.Browser.TimezoneMode.Emulate;
            instance.SetTimezone((int)tz["timezoneOffset"], 0);
            instance.SetIanaTimezone(tz["timezoneName"].ToString());
        }

        public static void FixTimezone(this Instance instance, IZennoPosterProjectModel project)
        {
            instance.Go("https://www.browserscan.net/");

            var resp = "";
            var d = new Time.Deadline();

            while (string.IsNullOrEmpty(resp))
            {
                d.Check(60);
                Thread.Sleep(5000);

                try
                {
                    resp = new Traffic(instance)
                        .Find("https://ip-scan.browserscan.net/sys/config/ip/get-visitor-ip")
                        .ResponseBody;
                }
                catch
                {
                }
            }

            var json = Newtonsoft.Json.Linq.JObject.Parse(resp);

            var timeZone = json["data"]?["ip_data"]?["timezone"]?.ToString();

            if (string.IsNullOrWhiteSpace(timeZone))
                throw new Exception("Timezone not found in response:\r\n" + resp);

            instance.SetIanaTimezone(timeZone);

            project.SendInfoToLog(timeZone);
        }

        #endregion
        

    }
    
}
