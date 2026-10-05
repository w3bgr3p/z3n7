

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json.Linq;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    
    /// <summary>Extension methods on <c>IZennoPosterProjectModel</c>: debugging aids.</summary>
    public static partial class ProjectExtensions
    {
        /// <summary>
        /// Saves a screenshot of the instance to
        /// <c>{project.Path}/debug_screens/{yyyy-MM-dd}/{project.Name}/{actionId} - {unix ms}.png</c> with a
        /// text box in the top-left corner (Iosevka 15 pt, white on dark).
        /// </summary>
        /// <param name="instance">Browser instance.</param>
        /// <param name="watermark">Text of the box; default is the last error, the current URL and the last action id.</param>
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

        /// <summary>
        /// After a pause, finds the request to <c>url</c> in the traffic of the main domain and checks its JSON
        /// response. When <c>errField</c> is set, stores the body in <c>err</c> and throws through <c>warn</c>;
        /// otherwise writes the body to the log. Does nothing when the request is not found.
        /// </summary>
        /// <param name="instance">Browser instance.</param>
        /// <param name="url">Exact request URL.</param>
        /// <param name="errField">Top-level JSON field that signals an error.</param>
        /// <param name="sleepBefore">Pause before reading, ms.</param>
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

    /// <summary>Versions of the node's environment. An empty string means the value could not be read.</summary>
    public sealed class VersionInfo
    {
        /// <summary>Version of <c>z3n7.dll</c>.</summary>
        public string z3n7        { get; set; } = "";
        /// <summary>Product version of the host process (ZennoPoster).</summary>
        public string zennoposter { get; set; } = "";
        /// <summary>Product name of the host process.</summary>
        public string product     { get; set; } = "";
        /// <summary>File name of the host process.</summary>
        public string process     { get; set; } = "";
        /// <summary>.NET runtime description.</summary>
        public string framework   { get; set; } = "";
        /// <summary>Machine name.</summary>
        public string machine     { get; set; } = "";
    }

    /// <summary>Environment information for logs and diagnostics.</summary>
    public static class Diagnostic
    {
        /// <summary>
        /// Reads library, ZennoPoster and runtime versions and the machine name. Each field is read separately
        /// and a failure leaves only that field empty; never throws.
        /// </summary>
        public static VersionInfo Info()
        {
            var info = new VersionInfo();

            info.z3n7 = Try(() => typeof(Diagnostic).Assembly.GetName().Version.ToString());

            var exe = Try(() => Process.GetCurrentProcess().MainModule.FileName);
            if (exe.Length > 0)
            {
                info.process     = Try(() => Path.GetFileName(exe));
                info.zennoposter = Try(() => FileVersionInfo.GetVersionInfo(exe).ProductVersion ?? "");
                info.product     = Try(() => FileVersionInfo.GetVersionInfo(exe).ProductName    ?? "");
            }

            info.framework = Try(() => System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
            info.machine   = Try(() => Environment.MachineName);

            return info;
        }

        /// <summary>
        /// Legacy form for the start banner: exactly <c>[z3n7, zennoposter, framework]</c>. Callers read it by
        /// index, so the order must not change.
        /// </summary>
        internal static string[] GetVersions()
        {
            var i = Info();
            return new[] { i.z3n7, i.zennoposter, i.framework };
        }

        private static string Try(Func<string> read)
        {
            try   { return read() ?? ""; }
            catch { return ""; }
        }
    }
    
    
    
}