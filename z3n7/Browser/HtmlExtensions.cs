using System;
using System.Collections.Generic;
using System.Drawing;
using ZennoLab.CommandCenter;
using ZXing;

namespace z3n7
{
    /// <summary>Helpers for ZennoPoster <c>HtmlElement</c>: centre point, QR decoding, XPath.</summary>
    public static class HtmlExtensions
    {
        
        /// <summary>
        /// Centre of the element relative to <c>origin</c> (bounding-client size when known, else the element
        /// size).
        /// </summary>
        /// <param name="origin">Top-left corner of the element.</param>
        /// <returns>The point. Throws when the element is null or void.</returns>
        public static Point Center(this HtmlElement element, Point origin)
        {
            if (element == null || element.IsVoid || element.IsNull)
                throw new ArgumentException("element is null or void", "element");

            int w = element.BoundingClientWidth > 0 ? element.BoundingClientWidth : element.Width;
            int h = element.BoundingClientHeight > 0 ? element.BoundingClientHeight : element.Height;

            return new Point(origin.X + w / 2, origin.Y + h / 2);
        }

        /// <summary>Draws the element and decodes a QR code from the picture (ZXing).</summary>
        /// <returns>
        /// The decoded text, or one of <c>elementZeroSize</c>, <c>bitmapIsNull</c>, <c>qrIsNull</c>, or an
        /// exception message. Never throws.
        /// </returns>
        public static string DecodeQr(this HtmlElement element)
        {
            try
            {
                if (element.Width == 0 || element.Height == 0)
                    return "elementZeroSize";

                var bitmap = element.DrawPartAsBitmap(0, 0, element.Width, element.Height, false); // false вместо true

                if (bitmap == null)
                    return "bitmapIsNull";

                var reader = new BarcodeReader();
                var result = reader.Decode(bitmap);

                if (result == null || string.IsNullOrEmpty(result.Text))
                    return "qrIsNull";

                return result.Text;
            }
            catch (Exception ex) { return ex.Message; }
        }
        /// <summary>
        /// Builds an XPath for the element by walking up to <c>body</c>. Each step uses <c>@id</c>, else the
        /// first class, else <c>@name</c>, else the position among same-tag siblings.
        /// </summary>
        /// <returns>The XPath, starting with <c>//*</c>; empty for a void element.</returns>
        public static string GetXPath(this HtmlElement element)
        {
            if (element.IsVoid || element.IsNull)
                return string.Empty;
            
            List<string> parts = new List<string>();
            HtmlElement current = element;
            
            while (current != null && !current.IsVoid && !current.IsNull)
            {
                string part = BuildXPathPart(current);
                parts.Insert(0, part);
                
                HtmlElement parent = current.ParentElement;
                
                if (parent == null || parent.IsVoid || parent.IsNull)
                    break;
                    
                if (parent.TagName.ToLower() == "body")
                {
                    parts.Insert(0, "body");
                    break;
                }
                
                current = parent;
            }
            
            return "//*" + (parts.Count > 0 ? "/" + string.Join("/", parts) : "");
        }
        private static string BuildXPathPart(HtmlElement element)
        {
            string tag = element.TagName.ToLower();
            
            string id = element.GetAttribute("id");
            if (!string.IsNullOrEmpty(id))
                return tag + "[@id='" + id + "']";
            
            string className = element.GetAttribute("class");
            if (!string.IsNullOrEmpty(className))
            {
                string firstClass = className.Split(' ')[0].Trim();
                if (!string.IsNullOrEmpty(firstClass))
                    return tag + "[starts-with(@class,'" + firstClass + "')]";
            }
            
            string name = element.GetAttribute("name");
            if (!string.IsNullOrEmpty(name))
                return tag + "[@name='" + name + "']";
            
            int position = GetElementPosition(element);
            return tag + "[" + position + "]";
        }
        private static int GetElementPosition(HtmlElement element)
        {
            HtmlElement parent = element.ParentElement;
            if (parent == null || parent.IsVoid || parent.IsNull)
                return 1;
            
            string targetTag = element.TagName.ToLower();
            HtmlElementCollection siblings = parent.FindChildrenByTags(targetTag);
            
            if (siblings.IsVoid  || siblings.Count == 0)
                return 1;
            
            string targetOuterHtml = element.OuterHtml;
            
            for (int i = 0; i < siblings.Count; i++)
            {
                HtmlElement sibling = siblings.GetByNumber(i);
                if (sibling.OuterHtml == targetOuterHtml)
                    return i + 1;
            }
            
            return 1;
        }
        /// <summary>
        /// Checks that the first element found by <c>xpath</c> in <c>tab</c> has the same outer HTML as
        /// <c>originalElement</c>.
        /// </summary>
        public static bool VerifyXPath(Tab tab, HtmlElement originalElement, string xpath)
        {
            if (string.IsNullOrEmpty(xpath))
                return false;
            
            HtmlElement foundElement = tab.FindElementByXPath(xpath, 0);
            
            if (foundElement.IsVoid || foundElement.IsNull)
                return false;
            
            return foundElement.OuterHtml == originalElement.OuterHtml;
        }
                
    }
    
}


