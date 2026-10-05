
using System;
using Svg;

namespace z3n7
{
    /// <summary>SVG rendering.</summary>
    public class Img
    {
        
        /// <summary>Renders SVG markup to an image file; the format follows the file extension.</summary>
        /// <param name="svgContent">SVG markup.</param>
        /// <param name="pathToScreen">Target file.</param>
        public static void ImgFromSvg( string svgContent, string pathToScreen)
        {
            var svgDocument = SvgDocument.FromSvg<SvgDocument>(svgContent);
            using (var bitmap = svgDocument.Draw())
            {
                bitmap.Save(pathToScreen);
            }
        }
        
        /// <summary>Renders SVG markup to PNG.</summary>
        /// <param name="svgContent">SVG markup.</param>
        /// <returns>The PNG as Base64.</returns>
        public static string DrawSvgAsBase64( string svgContent)
        {
            var svgDocument = SvgDocument.FromSvg<SvgDocument>(svgContent);
            using (var bitmap = svgDocument.Draw())
            using (var ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
        
    }
}