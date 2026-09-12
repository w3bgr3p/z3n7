

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    /// <summary>Отчёт SimRoute в PostgreSQL через DbQ. Весь SQL живёт здесь.</summary>
    public static partial class projectExtencions
    {
        

        public static void SaveDebugScreenshot(this IZennoPosterProjectModel project, Instance instance, string watermark = null)
        {
            watermark = watermark ?? string.Join(
                Environment.NewLine,
                project.LastErrorComment,
                instance.ActiveTab.URL,
                project.LastExecutedActionId
            );

            var directory = Path.Combine(
                project.Path,
                "debug_screens",
                DateTime.Today.ToString("yyyy-MM-dd"),
                project.Name
            );

            Directory.CreateDirectory(directory);

            var path = Path.Combine(
                directory,
                project.LastExecutedActionId + " - " + Time.Now() + ".png"
            );

            var overlayPath = Path.Combine(
                Path.GetTempPath(),
                "zennoposter-watermark-" + Guid.NewGuid() + ".png"
            );

            const int padding = 10;

            using (var font = new Font(
                "Iosevka",
                15f,
                FontStyle.Regular,
                GraphicsUnit.Point))
            {
                SizeF textSize;

                using (var measureBitmap = new Bitmap(1, 1))
                using (var measureGraphics = Graphics.FromImage(measureBitmap))
                {
                    textSize = measureGraphics.MeasureString(watermark, font);
                }

                using (var overlay = new Bitmap(
                    (int)Math.Ceiling(textSize.Width) + padding * 2,
                    (int)Math.Ceiling(textSize.Height) + padding * 2,
                    PixelFormat.Format32bppArgb))
                using (var graphics = Graphics.FromImage(overlay))
                using (var background = new SolidBrush(Color.FromArgb(210, 0, 0, 0)))
                using (var foreground = new SolidBrush(Color.White))
                {
                    graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    graphics.FillRectangle(
                        background,
                        0,
                        0,
                        overlay.Width,
                        overlay.Height
                    );

                    graphics.DrawString(
                        watermark,
                        font,
                        foreground,
                        padding,
                        padding
                    );

                    overlay.Save(overlayPath, ImageFormat.Png);
                }
            }

            try
            {
                ZennoPoster.ImageProcessingWaterMarkImageFromScreenshot(
                    instance.Port,
                    path,
                    "horizontally",
                    "lefttop",
                    overlayPath,
                    0,      // не делать всю подложку прозрачнее
                    5,
                    5,
                    100,
                    ""
                );
            }
            finally
            {
                if (File.Exists(overlayPath))
                    File.Delete(overlayPath);
            }
        }

        public static void CatchErrorFromTraffic(this IZennoPosterProjectModel project, Instance instance , string url, string errField, int sleepBefore = 5000)
        {
            Thread.Sleep(sleepBefore);
            var t = instance.GrabTrafficList(instance.ActiveTab.MainDomain);

            foreach (var el in t)
            {
                if (el.Url == url)
                {
                    var body = el.ResponseBody;

                    JObject rsp = JObject.Parse(body);

                    var errno = rsp[errField]?.ToString() ?? "";

                    if (errno != "")
                    {
                        project.Var("err", body);
                        project.warn(body, true);
                    }
                    else
                    {
                        project.log(body);
                    }
                    return;
                }
            }
        }

    }
}