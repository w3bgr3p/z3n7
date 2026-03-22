
using System;
using Svg;

namespace z3n7
{
    public class Img
    {
        
        public static void ImgFromSvg( string svgContent, string pathToScreen)
        {
            var svgDocument = SvgDocument.FromSvg<SvgDocument>(svgContent);
            using (var bitmap = svgDocument.Draw())
            {
                bitmap.Save(pathToScreen);
            }
        }
        
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