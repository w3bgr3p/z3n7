
using System;
using System.Collections.Generic;
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
    public static partial class JsExtensions
    {


        #region JavaScript Methods
        
        public static void CenterMouse(this Instance instance)
        {
            int[] center = instance.GetCenter();
            instance.ActiveTab.MainDocument.EvaluateScript(
                $"document.elementFromPoint({center[0]}, {center[1]})?.dispatchEvent(new MouseEvent('mousemove', {{clientX: {center[0]}, clientY: {center[1]}, bubbles: true}}));"
            );
        }
        
        public static string JsClick(this Instance instance, string selector, double delay = 1.0)
        {
            Thread.Sleep((int)(delay * 1000));
            try
            {
                string escapedSelector = selector
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"");

                string jsCode = $@"
                (function() {{
                    function findElement(selector) {{
                        let element = document.querySelector(selector);
                        if (element) return element;
                        
                        function searchInShadowRoots(root) {{
                            let el = root.querySelector(selector);
                            if (el) return el;
                            
                            let allElements = root.querySelectorAll('*');
                            for (let elem of allElements) {{
                                if (elem.shadowRoot) {{
                                    let found = searchInShadowRoots(elem.shadowRoot);
                                    if (found) return found;
                                }}
                            }}
                            return null;
                        }}
                        
                        return searchInShadowRoots(document);
                    }}
                    
                    var element = findElement(""{escapedSelector}"");
                    if (!element) {{
                        throw new Error(""Элемент не найден по селектору: {escapedSelector}"");
                    }}
                    
                    element.scrollIntoView({{ block: 'center' }});
                    
                    if (element.focus) {{
                        element.focus();
                    }}
                    
                    var clickEvent = new MouseEvent('click', {{
                        bubbles: true,
                        cancelable: true,
                        view: window,
                        button: 0,
                        composed: true
                    }});
                    element.dispatchEvent(clickEvent);
                    
                    return 'Click successful';
                }})();
                ";

                string result = instance.ActiveTab.MainDocument.EvaluateScript(jsCode);
                return result;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        public static void JsClick(this Instance instance, int x, int y)
        {
            string js = $@"
                (function() {{
                    var canvas = document.querySelector('canvas');
                    if (!canvas) return 'no canvas';
                    
                    var rect = canvas.getBoundingClientRect();
                    var x = {x};
                    var y = {y};
                    
                    var events = ['mousedown', 'mouseup', 'click'];
                    events.forEach(function(eventType) {{
                        var evt = new MouseEvent(eventType, {{
                            view: window,
                            bubbles: true,
                            cancelable: true,
                            clientX: x,
                            clientY: y,
                            screenX: x,
                            screenY: y,
                            button: 0
                        }});
                        canvas.dispatchEvent(evt);
                    }});
                    
                    return 'clicked at ' + x + ',' + y;
                }})();
                ";

            instance.ActiveTab.MainDocument.EvaluateScript(js);

        }
        public static void JsClick(this Instance instance, int[] pos)
        {
            instance.JsClick(pos[0], pos[1]);
        }
        
        
        public static string JsSet(this Instance instance, string selector, string value, double delay = 1.0)
        {
            Thread.Sleep((int)(delay * 1000));
            try
            {
                string escapedValue = value
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t");
                
                string escapedSelector = selector
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"");

                string jsCode = $@"
                (function() {{
                    var element = document.querySelector(""{escapedSelector}"");
                    if (!element) {{
                        throw new Error(""Элемент не найден по селектору: {escapedSelector}"");
                    }}
                    
                    element.scrollIntoView({{ block: 'center' }});
                    
                    var clickEvent = new MouseEvent('click', {{
                        bubbles: true,
                        cancelable: true,
                        view: window
                    }});
                    element.dispatchEvent(clickEvent);
                    
                    element.focus();
                    
                    var focusinEvent = new FocusEvent('focusin', {{ bubbles: true }});
                    element.dispatchEvent(focusinEvent);
                    
                    element.value = '';
                    
                    document.execCommand('insertText', false, ""{escapedValue}"");
                    
                    var inputEvent = new Event('input', {{ bubbles: true }});
                    var changeEvent = new Event('change', {{ bubbles: true }});
                    element.dispatchEvent(inputEvent);
                    element.dispatchEvent(changeEvent);
                    
                    return 'Value set successfully';
                }})();
                ";

                string result = instance.ActiveTab.MainDocument.EvaluateScript(jsCode);
                return result;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        public static string JsGet(this Instance instance, string jsSelector, string property)
        {
            string json = instance.ActiveTab.MainDocument.EvaluateScript($@"
                var el = {jsSelector};
                if (!el) return null;
                
                var props = {{}};
                
                for (var i = 0; i < el.attributes.length; i++) {{
                    props[el.attributes[i].name] = el.attributes[i].value;
                }}
                
                props['innerText']   = el.innerText;
                props['innerHTML']   = el.innerHTML;
                props['textContent'] = el.textContent;
                props['value']       = el.value;
                props['checked']     = el.checked !== undefined ? String(el.checked) : undefined;
                props['tagName']     = el.tagName;
                
                Object.keys(props).forEach(function(k) {{
                    if (props[k] === undefined) delete props[k];
                }});
                
                return JSON.stringify(props);
            ");

            if (json == null)
                throw new Exception($"Element not found by selector : {jsSelector}");

            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
    
            if (!dict.TryGetValue(property, out var value))
                throw new Exception(
                    "Element '" + jsSelector + "' doesn't have '" + property + "'.\n" +
                    "Existed: " + json
                );

            return value;
        }
        public static string JsPost(this Instance instance, string script, int delay = 0)
        {
            Thread.Sleep(1000 * delay);
            //var jsCode = TextProcessing.Replace(script, "\"", "'", "Text", "All");
            var jsCode = script.Replace( "\"", "'");
            try
            {
                string result = instance.ActiveTab.MainDocument.EvaluateScript(jsCode);
                return result;
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }


        #endregion
        
       

    }
    
}