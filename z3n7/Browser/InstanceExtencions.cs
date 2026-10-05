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
        private static Random _rnd = new Random();
        
        private class ElementNotFoundException : Exception
        {
            public ElementNotFoundException(string message) : base(message) { }
        }
        
        #region Element Getters
        
        /// <summary>Finds an element in the active tab.</summary>
        /// <param name="obj">
        /// Element: an <c>HtmlElement</c>; <c>(value, "id")</c> or <c>(value, "name")</c>; or <c>(tag,
        /// attribute, pattern, mode, index)</c> as in <c>FindElementByAttribute</c>.
        /// </param>
        /// <param name="method">
        /// For a 5-part selector: <c>random</c> picks a random match, <c>last</c> the last one; otherwise the
        /// index is used.
        /// </param>
        /// <returns>The element. Throws when it is not found or the selector shape is unsupported.</returns>
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
        
        /// <summary>Waits for an element and returns one of its attributes.</summary>
        /// <param name="obj">
        /// Element: an <c>HtmlElement</c>; <c>(value, "id")</c> or <c>(value, "name")</c>; or <c>(tag,
        /// attribute, pattern, mode, index)</c> as in <c>FindElementByAttribute</c>.
        /// </param>
        /// <param name="method">
        /// For a 5-part selector: <c>random</c> picks a random match, <c>last</c> the last one; otherwise the
        /// index is used.
        /// </param>
        /// <param name="deadline">Seconds to keep looking (every 0.5 s).</param>
        /// <param name="atr">Attribute to read.</param>
        /// <param name="delay">Seconds to wait after finding it.</param>
        /// <param name="thrw">Throw when the element is not found in time; otherwise return quietly.</param>
        /// <param name="thr0w">Legacy switch: <c>false</c> also turns <c>thrw</c> off.</param>
        /// <param name="waitTillVoid">
        /// Wait until the element is gone instead; returns <c>null</c> at the deadline and throws while it is
        /// present.
        /// </param>
        /// <param name="pathToScript">When set, appends the action and the element's XPath to this file.</param>
        /// <returns>The attribute value, or <c>null</c> when not found and <c>thrw</c> is false.</returns>
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
        
        /// <summary>
        /// Watches for an element that must not appear (e.g. an error message) for <c>deadline</c> seconds.
        /// </summary>
        /// <param name="obj">
        /// Element: an <c>HtmlElement</c>; <c>(value, "id")</c> or <c>(value, "name")</c>; or <c>(tag,
        /// attribute, pattern, mode, index)</c> as in <c>FindElementByAttribute</c>.
        /// </param>
        /// <param name="method">
        /// For a 5-part selector: <c>random</c> picks a random match, <c>last</c> the last one; otherwise the
        /// index is used.
        /// </param>
        /// <param name="deadline">Seconds to keep looking (every 0.5 s).</param>
        /// <param name="atr">Attribute used as the exception message.</param>
        /// <param name="delay">Seconds to wait before starting.</param>
        /// <param name="pathToScript">Not used.</param>
        /// <returns>
        /// <c>null</c> when the element never appeared. When it appears, throws an exception whose message is
        /// its <c>atr</c>.
        /// </returns>
        public static string HeCatch(this Instance instance, object obj,  string method = "", int deadline = 10, string atr = "innertext", int delay = 1, string pathToScript = null)
        {
            Thread.Sleep(1000 * delay);
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
        /// <summary>
        /// Same as the overload without <c>project</c>; also stores the message in the <c>err</c> variable
        /// before throwing.
        /// </summary>
        /// <param name="project">Project for the <c>err</c> variable.</param>
        /// <param name="obj">
        /// Element: an <c>HtmlElement</c>; <c>(value, "id")</c> or <c>(value, "name")</c>; or <c>(tag,
        /// attribute, pattern, mode, index)</c> as in <c>FindElementByAttribute</c>.
        /// </param>
        /// <param name="method">
        /// For a 5-part selector: <c>random</c> picks a random match, <c>last</c> the last one; otherwise the
        /// index is used.
        /// </param>
        /// <param name="deadline">Seconds to keep looking (every 0.5 s).</param>
        /// <param name="atr">Attribute used as the exception message.</param>
        /// <param name="delay">Seconds to wait before starting.</param>
        /// <param name="pathToScript">Not used.</param>
        public static string HeCatch(this Instance instance, IZennoPosterProjectModel project, object obj,  string method = "", int deadline = 10, string atr = "innertext", int delay = 1, string pathToScript = null)
        {
            Thread.Sleep(1000 * delay);
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
                        project.Var("err", ex.Message);
                        throw;
                    }
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>Clicks each element in turn with <c>HeClick</c> defaults.</summary>
        public static void HeMultiClick(this Instance instance, List<object> selectors)
        {
            foreach (var selector in selectors) 
                instance.HeClick(selector);
        }

        /// <summary>Waits for an element and clicks it after a random pause of about 1–1.3 s × <c>delay</c>.</summary>
        /// <param name="obj">
        /// Element: an <c>HtmlElement</c>; <c>(value, "id")</c> or <c>(value, "name")</c>; or <c>(tag,
        /// attribute, pattern, mode, index)</c> as in <c>FindElementByAttribute</c>.
        /// </param>
        /// <param name="method">
        /// For a 5-part selector: <c>random</c> picks a random match, <c>last</c> the last one; otherwise the
        /// index is used. <c>clickOut</c> keeps clicking until the element disappears.
        /// </param>
        /// <param name="deadline">Seconds to keep looking (every 0.5 s).</param>
        /// <param name="delay">Multiplier of the pause before clicking.</param>
        /// <param name="comment">Text added to the timeout message.</param>
        /// <param name="thrw">Throw when the element is not found in time; otherwise return quietly.</param>
        /// <param name="thr0w">Legacy switch: <c>false</c> also turns <c>thrw</c> off.</param>
        /// <param name="emu">
        /// 1 — use full mouse emulation for this action, −1 — turn it off, 0 — leave the instance setting.
        /// </param>
        /// <param name="pathToScript">When set, appends the action and the element's XPath to this file.</param>
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
        
        /// <summary>Waits for an element and holds the left button at a random point inside it.</summary>
        /// <param name="obj">
        /// Element: an <c>HtmlElement</c>; <c>(value, "id")</c> or <c>(value, "name")</c>; or <c>(tag,
        /// attribute, pattern, mode, index)</c> as in <c>FindElementByAttribute</c>.
        /// </param>
        /// <param name="holdMs">Hold time in milliseconds.</param>
        /// <param name="method">
        /// For a 5-part selector: <c>random</c> picks a random match, <c>last</c> the last one; otherwise the
        /// index is used.
        /// </param>
        /// <param name="deadline">Seconds to keep looking (every 0.5 s).</param>
        /// <param name="delay">Multiplier of the pause before pressing.</param>
        /// <param name="comment">Text added to the timeout message.</param>
        /// <param name="thrw">Throw when the element is not found in time; otherwise return quietly.</param>
        /// <param name="thr0w">Legacy switch: <c>false</c> also turns <c>thrw</c> off.</param>
        /// <param name="emu">
        /// 1 — use full mouse emulation for this action, −1 — turn it off, 0 — leave the instance setting.
        /// </param>
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

        /// <summary>Holds the left button at a point.</summary>
        /// <param name="x">X in the tab.</param>
        /// <param name="y">Y in the tab.</param>
        /// <param name="holdMs">Hold time in milliseconds.</param>
        /// <param name="delay">Multiplier of the pause before pressing.</param>
        /// <param name="emu">
        /// 1 — use full mouse emulation for this action, −1 — turn it off, 0 — leave the instance setting.
        /// </param>
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
                x = _rnd.Next(area.Left, area.Right);
                y = _rnd.Next(area.Top, area.Bottom);
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
        
        
        /// <summary>
        /// Waits for an input and enters <c>value</c> after a random pause of about 1.3–2 s × <c>delay</c>.
        /// </summary>
        /// <param name="obj">
        /// Element: an <c>HtmlElement</c>; <c>(value, "id")</c> or <c>(value, "name")</c>; or <c>(tag,
        /// attribute, pattern, mode, index)</c> as in <c>FindElementByAttribute</c>.
        /// </param>
        /// <param name="value">Text to enter.</param>
        /// <param name="method">
        /// For a 5-part selector: <c>random</c> picks a random match, <c>last</c> the last one; otherwise the
        /// index is used.
        /// </param>
        /// <param name="deadline">Seconds to keep looking (every 0.5 s).</param>
        /// <param name="delay">Multiplier of the pause.</param>
        /// <param name="comment">Text added to the timeout message.</param>
        /// <param name="thrw">Throw when the element is not found in time; otherwise return quietly.</param>
        /// <param name="thr0w">Legacy switch: <c>false</c> also turns <c>thrw</c> off.</param>
        /// <param name="emu">
        /// 0 — set the value with ZennoPoster's full emulation; above 0 — click the field and type the text;
        /// below 0 — nothing is entered.
        /// </param>
        /// <param name="pathToScript">When set, appends the action and the element's XPath to this file.</param>
        public static void HeSet(this Instance instance, object obj, string value, string method = "id", int deadline = 10, double delay = 1, string comment = "", bool thrw = true, bool thr0w = true,int emu = 0, string pathToScript = null)
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
                    if (emu == 0) 
                        he.SetValue(value, "Full", false);
                    else if (emu > 0)
                    {
                        instance.HeClick(he, emu:emu);
                        instance.SendText(value, 15);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    lastExceptionMessage = ex.Message;
                }

                Thread.Sleep(500);
            }
        }
        
        /// <summary>Waits for an element and removes it from the page.</summary>
        /// <param name="obj">
        /// Element: an <c>HtmlElement</c>; <c>(value, "id")</c> or <c>(value, "name")</c>; or <c>(tag,
        /// attribute, pattern, mode, index)</c> as in <c>FindElementByAttribute</c>.
        /// </param>
        /// <param name="method">
        /// For a 5-part selector: <c>random</c> picks a random match, <c>last</c> the last one; otherwise the
        /// index is used.
        /// </param>
        /// <param name="deadline">Seconds to keep looking (every 0.5 s).</param>
        /// <param name="thrw">Throw when the element is not found in time; otherwise return quietly.</param>
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
        
        /// <summary>
        /// Drags from the element's centre by the given offset with a human-like path: easing, slight vertical
        /// wobble and, for longer moves, a small overshoot and correction.
        /// </summary>
        /// <param name="element">Element to drag.</param>
        /// <param name="offsetX">Horizontal offset, px.</param>
        /// <param name="offsetY">Vertical offset, px.</param>
        /// <returns>The drop point.</returns>
        public static Point HeDragAndDrop(this Instance instance, HtmlElement element, int offsetX, int offsetY = 0)
        {
            var tab = instance.ActiveTab;
            var p0 = element.Center(element.DisplacementInTabWindow);
            int startX = p0.X;
            int startY = p0.Y;
            int targetX = startX + offsetX;
            int targetY = startY + offsetY;
            tab.MouseMove(startX, startY);
            Thread.Sleep(_rnd.Next(100, 200));
            tab.MouseClick(startX, startY, "left", "down");
            Thread.Sleep(_rnd.Next(60, 140)); 
            int overshoot = offsetX > 50 ? _rnd.Next(2, 5) : 0;
            int forwardDistance = offsetX + overshoot;
            int steps = _rnd.Next(18, 26);
            int currentX = startX;
            int currentY = startY;
            for (int i = 1; i <= steps; i++)
            {
                double t = (double)i / steps;
                
                double progress = 1.0 - Math.Pow(1.0 - t, 2.7);
                int nextX = startX + (int)Math.Round(progress * forwardDistance);
                
                int waveY = (int)Math.Round(Math.Sin(t * Math.PI) * _rnd.Next(-2, 3));
                int nextY = startY + offsetY + waveY;
                tab.MouseMove(nextX, nextY);
                int delay = (int)(10 + (t * t) * 35 + _rnd.Next(-3, 6));
                Thread.Sleep(Math.Max(6, delay));
                currentX = nextX;
                currentY = nextY;
            }
            if (overshoot > 0)
            {
                Thread.Sleep(_rnd.Next(30, 70)); 
                int corrSteps = _rnd.Next(3, 5);
                for (int j = 1; j <= corrSteps; j++)
                {
                    double backProgress = (double)j / corrSteps;
                    int backX = (int)Math.Round(currentX - backProgress * overshoot);
                    
                    tab.MouseMove(backX, targetY);
                    Thread.Sleep(_rnd.Next(25, 55));
                }
            }
            Thread.Sleep(_rnd.Next(120, 250));
            tab.MouseClick(targetX, targetY, "left", "up");
            Thread.Sleep(_rnd.Next(100, 200));
            return new Point(targetX, targetY);
        }

        /// <summary>Opens a drop-down by two long clicks and presses Down a random number of times.</summary>
        /// <param name="obj">
        /// Element: an <c>HtmlElement</c>; <c>(value, "id")</c> or <c>(value, "name")</c>; or <c>(tag,
        /// attribute, pattern, mode, index)</c> as in <c>FindElementByAttribute</c>.
        /// </param>
        /// <param name="min">Fewest presses.</param>
        /// <param name="max">Upper bound of presses (exclusive).</param>
        public static void HePeakRandom(this Instance instance, object obj, int min = 1, int max = 10)
        {
            
            instance.HeLongClick(obj);
            instance.WaitFieldEmulationDelay();
            instance.HeLongClick(obj);
            var keys = string.Concat(Enumerable.Repeat("{DOWN}", _rnd.Next(min, max)));
            instance.SendText(keys, 15);

        }




        #endregion

        #region Browser Management
        
        /// <summary>Closes all tabs, clears cache and cookies of <c>domain</c> and opens <c>about:blank</c>.</summary>
        public static void ClearShit(this Instance instance, string domain)
        {
            instance.CloseAllTabs();
            instance.ClearCache(domain);
            instance.ClearCookie(domain);
            Thread.Sleep(500);
            instance.ActiveTab.Navigate("about:blank", "");
        }
        
        /// <summary>Closes every tab after the first <c>tabToKeep</c>.</summary>
        /// <param name="blank">Then open <c>about:blank</c> in the active tab.</param>
        /// <param name="tabToKeep">How many tabs to keep.</param>
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

        /// <summary>Waits until the number of tabs equals <c>tabIndex</c> and closes all but the first.</summary>
        /// <param name="deadline">Seconds to wait.</param>
        /// <param name="tabIndex">Tab count to wait for.</param>
        /// <param name="thrw">Throw when it does not happen in time.</param>
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
        
        /// <summary>Navigates the active tab unless it is already on the URL.</summary>
        /// <param name="url">Target URL.</param>
        /// <param name="strict">Compare the whole URL; otherwise skip when the current URL contains it.</param>
        /// <param name="waitTdle">Wait for loading to finish.</param>
        /// <param name="newTab">Open a new tab first.</param>
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
        
        /// <summary>Reloads the page.</summary>
        /// <param name="WaitTillLoad">Wait for loading to finish.</param>
        public static void F5(this Instance instance, bool WaitTillLoad = true)
        {
            instance.ActiveTab.MainDocument.EvaluateScript("location.reload(true)");
            if (instance.ActiveTab.IsBusy && WaitTillLoad) instance.ActiveTab.WaitDownloading();
        }

        /// <summary>Scrolls with the emulated mouse wheel.</summary>
        /// <param name="y">Wheel delta.</param>
        public static void ScrollDown(this Instance instance, int y = 420)
        {
            bool emu = instance.UseFullMouseEmulation;
            instance.UseFullMouseEmulation = true;
            instance.ActiveTab.FullEmulationMouseWheel(0, y);
            instance.UseFullMouseEmulation = emu;
        }
        
        /// <summary>
        /// Pastes text through the Windows clipboard (Ctrl+V); the previous clipboard text is restored. Errors
        /// are ignored.
        /// </summary>
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
        
        /// <summary>Launches the browser with a profile folder.</summary>
        /// <param name="pathProfile">Profile folder.</param>
        /// <param name="useProfile">Apply the ZennoPoster profile too.</param>
        /// <param name="browserType">Browser to launch.</param>
        public static void UpFromFolder(this Instance instance, string pathProfile, bool useProfile = false, BrowserType browserType = BrowserType.Chromium)
        {
            ZennoLab.CommandCenter.Classes.BuiltInBrowserLaunchSettings settings =
                (ZennoLab.CommandCenter.Classes.BuiltInBrowserLaunchSettings)ZennoLab.CommandCenter.Classes.BrowserLaunchSettingsFactory.Create(browserType);
            settings.CachePath = pathProfile; 
            settings.ConvertProfileFolder = true;
            settings.UseProfile = useProfile;
            instance.Launch(settings);
        }
        
        /// <summary>Launches Chromium without a profile folder.</summary>
        public static void UpEmpty(this Instance instance)
        {
            instance.Launch(BrowserType.Chromium, false);
        }
        
        /// <summary>Closes the browser (launches "without browser") and waits.</summary>
        /// <param name="pauseAfterMs">Pause afterwards, ms.</param>
        public static void Down(this Instance instance, int pauseAfterMs = 5000)
        {
            try
            {
                instance.Launch(BrowserType.WithoutBrowser, false);
            }
            catch { }
            
            Thread.Sleep(pauseAfterMs);
        }
        
        /// <summary>Returns the instance's cookies as saved by <c>SaveCookie</c> (through a temporary file).</summary>
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

        /// <summary>
        /// Sets timezone emulation from the <c>timezone</c> JSON (<c>timezoneOffset</c>, <c>timezoneName</c>)
        /// of the account's <c>_instance</c> row; warns when there is none.
        /// </summary>
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

        /// <summary>
        /// Opens <c>browserscan.net</c>, takes the IP timezone from its visitor-IP request in the traffic and
        /// sets it as the instance's IANA timezone.
        /// </summary>
        /// <remarks>Throws when the response does not arrive within about 60 seconds or has no timezone.</remarks>
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
