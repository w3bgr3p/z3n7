using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Captcha
{
    public class Hunt : IDisposable
    {
        private readonly Instance _instance;
        private readonly IZennoPosterProjectModel _project;
        private HuntShapes _shapes;
        private HuntFootball _football;

        public Hunt(IZennoPosterProjectModel project,  Instance instance)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        public void SolveAndSubmit(float confidence = 0.30f, int minimumDelayMs = 700, int maximumDelayMs = 1300)
        {
            _instance.HeGet(("canvas", "fulltag", "canvas", "text", 0), deadline:30);
            Thread.Sleep(500);

            var root = _instance.ActiveTab.FindElementById("huntCaptcha");
            if (root == null || root.IsVoid)
            {
                SolveAndSubmitBall(confidence);
                return;
            }

            SolveAndSubmitShapes(confidence, minimumDelayMs, maximumDelayMs);
        }

        public HuntBallResult SolveAndSubmitBall(float confidence = 0.30f)
        {
            if (_football == null)
                _football = new HuntFootball(_project, _instance);

            return _football.SolveAndSubmit(confidence);
        }

        public HuntBallResult DetectBall(
            string imageBase64,
            float confidence = 0.30f)
        {
            if (_football == null)
                _football = new HuntFootball(_project, _instance);

            return _football.Detect(imageBase64, confidence);
        }

        public IReadOnlyList<HuntPoint> SolveAndSubmitShapes(float confidence = 0.30f, int minimumDelayMs = 700, int maximumDelayMs = 1300)
        {
            var root = _instance.ActiveTab.FindElementById("huntCaptcha");
            if (root == null || root.IsVoid)
            {
                SolveAndSubmitBall(confidence);
                return Array.Empty<HuntPoint>();
            }

            if (_shapes == null)
                _shapes = new HuntShapes(_project, _instance);

            return _shapes.SolveAndSubmit(
                confidence,
                minimumDelayMs,
                maximumDelayMs);
        }

        public bool CheckResult()
        {
            var t  = new  Traffic(_project, _instance, "/captcha-api/api/v4/captcha/verify").FindAll("/captcha-api/api/v4/captcha/verify");

            var result = true;
            foreach (var  item in t)
            {
                if (item.ResponseBody.Contains("Verification failed") )
                {
                    result =  false;
                    break;
                }
            }
            return result;
        }

        public void MainButtonClick()
        {
            HtmlElement he = _instance.ActiveTab.FindElementByAttribute(
                "canvas", "fulltag", "canvas", "text", 0);

            if (he == null || he.IsVoid)
                throw new Exception("Canvas не найден");

            string base64 = he.DrawToBitmap(true);
            int comma = base64.IndexOf(',');

            if (base64.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && comma >= 0)
                base64 = base64.Substring(comma + 1);

            var position = he.DisplacementInBrowser;

            using (var stream = new MemoryStream(Convert.FromBase64String(base64)))
            using (var bitmap = new Bitmap(stream))
            {
                int x = position.X + bitmap.Width / 2;
                int y = position.Y + bitmap.Height - 10;

                var clickPoint = new Rectangle(x, y, 1, 1);
                _instance.ActiveTab.RiseEvent("click", clickPoint, "Left");
            }
        }
        
        public void Dispose()
        {
            _football?.Dispose();
            _shapes?.Dispose();
        }
    }

    public class HuntShapes : IDisposable
    {
        private const float TargetFallbackConfidence = 0.20f;

        private static readonly string[] ShapeNames =
        {
            "black circle", "circle_bot", "cube", "cube_bot", "drop", "drop_bot",
            "flower", "flower_bot", "heart", "heart_bot", "mark", "mark_bot",
            "moon", "moon_bot", "quarter_circle", "quarter_circle_bottom",
            "rectangle", "rectangle_bot", "star", "star_bot", "tetris", "tetris_bot"
        };

        private readonly Instance _instance;
        private readonly HuntModel _model;

        public HuntShapes(IZennoPosterProjectModel project, Instance instance)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _model = new HuntModel(
                Path.Combine(project.ReadEnv("ONNX_FOLDER"), "hunt-shape.onnx"),
                ShapeNames);
        }

        public IReadOnlyList<HuntPoint> SolveAndSubmit(
            float confidence = 0.30f,
            int minimumDelayMs = 700,
            int maximumDelayMs = 1300)
        {
            _instance.HeGet(("canvas", "fulltag", "canvas", "text", 0), deadline:30);
            Thread.Sleep(5000);

            if (minimumDelayMs < 0 || maximumDelayMs < minimumDelayMs)
                throw new ArgumentException("Invalid click delay range");

            var root = _instance.ActiveTab.FindElementById("huntCaptcha");
            if (root == null || root.IsVoid)
                throw new InvalidOperationException("Hunt shapes container #huntCaptcha was not found");

            var canvas = root.GetChildren(true)
                .FirstOrDefault(element =>
                    string.Equals(element.TagName, "canvas", StringComparison.OrdinalIgnoreCase));
            if (canvas == null || canvas.IsVoid)
                throw new InvalidOperationException("Canvas inside #huntCaptcha was not found");

            var imageBase64 = _instance.ActiveTab.MainDocument.EvaluateScript(@"
                var canvas = document.querySelector('#huntCaptcha canvas');
                return canvas ? canvas.toDataURL('image/png') : null;
            ");
            if (string.IsNullOrWhiteSpace(imageBase64))
                throw new InvalidOperationException("Could not read Hunt canvas");

            int imageWidth;
            int imageHeight;
            using (var image = _model.ReadImage(imageBase64))
            {
                imageWidth = image.Width;
                imageHeight = image.Height;
            }

            int displayWidth;
            int displayHeight;
            using (var displayedCanvas = _model.ReadImage(canvas.DrawToBitmap(true)))
            {
                displayWidth = displayedCanvas.Width;
                displayHeight = displayedCanvas.Height;
            }

            var points = FindPoints(imageBase64, confidence);
            var canvasPosition = canvas.DisplacementInBrowser;
            var scaleX = (double)displayWidth / imageWidth;
            var scaleY = (double)displayHeight / imageHeight;
            var random = new Random();

            foreach (var point in points)
            {
                Thread.Sleep(random.Next(minimumDelayMs, maximumDelayMs + 1));
                var click = new Rectangle(
                    canvasPosition.X + (int)Math.Round(point.X * scaleX),
                    canvasPosition.Y + (int)Math.Round(point.Y * scaleY),
                    1,
                    1);
                _instance.ActiveTab.RiseEvent("click", click, "Left");
            }

            Thread.Sleep(2000);
            var submit = new Rectangle(
                canvasPosition.X + displayWidth / 2,
                canvasPosition.Y + (int)Math.Round(displayHeight * 0.94),
                1,
                1);
            _instance.ActiveTab.RiseEvent("click", submit, "Left");

            return points;
        }

        private IReadOnlyList<HuntPoint> FindPoints(string imageBase64, float confidence)
        {
            var detections = _model.Detect(imageBase64, confidence);
            var prompts = detections
                .Where(item => item.ClassId % 2 == 0)
                .GroupBy(item => item.ClassId)
                .Select(group => group.OrderByDescending(item => item.Confidence).First())
                .OrderBy(item => item.CenterX)
                .ToList();

            if (prompts.Count == 0)
                throw new InvalidOperationException("Hunt prompt icons were not detected");

            IReadOnlyList<HuntDetection> fallbackDetections = null;
            var points = new List<HuntPoint>();

            foreach (var prompt in prompts)
            {
                var targetClass = prompt.ClassId + 1;
                var target = detections
                    .Where(item => item.ClassId == targetClass)
                    .OrderByDescending(item => item.Confidence)
                    .FirstOrDefault();

                if (target == null)
                {
                    if (fallbackDetections == null)
                    {
                        fallbackDetections = _model.Detect(
                            imageBase64,
                            Math.Min(confidence, TargetFallbackConfidence));
                    }

                    target = fallbackDetections
                        .Where(item => item.ClassId == targetClass)
                        .OrderByDescending(item => item.Confidence)
                        .FirstOrDefault();
                }

                if (target == null)
                {
                    var diagnostics = _model.Detect(imageBase64, 0.01f);
                    throw new InvalidOperationException(
                        $"Hunt shape target for class {targetClass} was not detected. " +
                        "Detected: " + FormatDetections(detections) +
                        Environment.NewLine +
                        "Detections at confidence 0.01: " +
                        FormatDetections(diagnostics));
                }

                points.Add(new HuntPoint
                {
                    ClassId = target.ClassId,
                    Name = target.Name,
                    Confidence = target.Confidence,
                    X = target.CenterX,
                    Y = target.CenterY
                });
            }

            return points;
        }

        private string FormatDetections(IEnumerable<HuntDetection> detections)
        {
            var items = detections
                .OrderBy(item => item.ClassId)
                .ThenByDescending(item => item.Confidence)
                .Select(item => string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} {1} {2:0.000} @({3:0.0},{4:0.0})",
                    item.ClassId,
                    item.Name,
                    item.Confidence,
                    item.CenterX,
                    item.CenterY))
                .ToArray();

            return items.Length == 0 ? "<none>" : string.Join("; ", items);
        }

        public void Dispose()
        {
            _model.Dispose();
        }
    }

    public class HuntFootball : IDisposable
    {
        private static readonly string[] BallNames = { "ball", "circle" };

        private readonly IZennoPosterProjectModel _project;
        private readonly Instance _instance;
        private readonly HuntModel _model;

        public HuntFootball(IZennoPosterProjectModel project, Instance instance)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _model = new HuntModel(
                Path.Combine(project.ReadEnv("ONNX_FOLDER"), "hunt-ball.onnx"),
                BallNames);
        }

        public HuntBallResult SolveAndSubmit(float confidence = 0.30f)
        {
            var canvas = _instance.ActiveTab.FindElementByAttribute(
                "canvas", "fulltag", "canvas", "text", 0);
            if (canvas == null || canvas.IsVoid)
                throw new InvalidOperationException("Hunt football canvas was not found");

            int imageWidth;
            int imageHeight;
            using (var image = _model.ReadImage(canvas.DrawToBitmap(true)))
            {
                imageWidth = image.Width;
                imageHeight = image.Height;
            }

            var displayWidth = canvas.BoundingClientWidth;
            var displayHeight = canvas.BoundingClientHeight;
            var position = canvas.DisplacementInTabWindow;
            var sliderY = position.Y + (int)Math.Round(displayHeight * 268.0 / 300.0);
            var sliderX = position.X + displayWidth / 2;
            var minimumX = position.X + 2;
            var maximumX = position.X + displayWidth - 2;
            var mouseDown = false;
            HuntBallResult result = null;
            var diagnostics = new StringBuilder();
            diagnostics.AppendFormat(
                CultureInfo.InvariantCulture,
                "canvas image={0}x{1}, display={2}x{3}, position=({4},{5}), slider=({6},{7})",
                imageWidth,
                imageHeight,
                displayWidth,
                displayHeight,
                position.X,
                position.Y,
                sliderX,
                sliderY);

            try
            {
                _instance.ActiveTab.MouseClick(sliderX, sliderY, "left", "down");
                mouseDown = true;

                result = Detect(canvas.DrawToBitmap(true), confidence);
                if (result.Ball == null || result.Circle == null)
                    result = Detect(canvas.DrawToBitmap(true), 0.10f);
                if (result.Ball == null || result.Circle == null)
                    throw new InvalidOperationException(
                        "Hunt football ball or circle was not detected");

                diagnostics.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "\nstart ball=({0:0.0},{1:0.0}), circle=({2:0.0},{3:0.0})",
                    result.Ball.CenterX,
                    result.Ball.CenterY,
                    result.Circle.CenterX,
                    result.Circle.CenterY);

                var lastCursorDelta = 0;
                var lastBallDeltaX = 0f;
                var lastBallDeltaY = 0f;

                for (var attempt = 0; attempt < 6; attempt++)
                {
                    var ballX = result.Ball.CenterX;
                    var ballY = result.Ball.CenterY;
                    if (ballX >= result.Circle.X1 &&
                        ballX <= result.Circle.X2 &&
                        ballY >= result.Circle.Y1 &&
                        ballY <= result.Circle.Y2)
                    {
                        _instance.ActiveTab.MouseClick(sliderX, sliderY, "left", "up");
                        mouseDown = false;
                        diagnostics.AppendFormat(
                            CultureInfo.InvariantCulture,
                            "\nsuccess ball=({0:0.0},{1:0.0}), cursor={2}",
                            ballX,
                            ballY,
                            sliderX);
                        _project.SendInfoToLog(
                            "Hunt football calibration:\n" + diagnostics,
                            true);
                        return result;
                    }

                    var previousX = sliderX;
                    var previousBallX = ballX;
                    var previousBallY = ballY;
                    int movement;

                    if (attempt == 0)
                    {
                        movement = sliderX + 12 <= maximumX ? 12 : -12;
                    }
                    else
                    {
                        var response =
                            lastBallDeltaX * lastBallDeltaX +
                            lastBallDeltaY * lastBallDeltaY;
                        if (Math.Abs(lastCursorDelta) < 1 || response < 1f)
                        {
                            movement = attempt % 2 == 0 ? 12 : -12;
                        }
                        else
                        {
                            var targetDeltaX = result.Circle.CenterX - ballX;
                            var targetDeltaY = result.Circle.CenterY - ballY;
                            var calculated = lastCursorDelta *
                                (targetDeltaX * lastBallDeltaX +
                                 targetDeltaY * lastBallDeltaY) /
                                response;
                            movement = (int)Math.Round(
                                Math.Max(-80f, Math.Min(80f, calculated)));

                            if (Math.Abs(movement) < 4)
                                movement = movement < 0 ? -4 : 4;
                        }
                    }

                    var nextX = Math.Max(
                        minimumX,
                        Math.Min(maximumX, sliderX + movement));
                    _instance.ActiveTab.MouseMove(
                        sliderX, sliderY, nextX, sliderY, false, false);
                    sliderX = nextX;

                    Thread.Sleep(80);
                    var nextResult = Detect(canvas.DrawToBitmap(true), confidence);
                    if (nextResult.Ball == null || nextResult.Circle == null)
                    {
                        nextResult = Detect(canvas.DrawToBitmap(true), 0.10f);
                        if (nextResult.Ball == null || nextResult.Circle == null)
                            break;
                    }

                    diagnostics.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "\nmove {0}: cursor {1}->{2} ({9:+#;-#;0}); ball ({3:0.0},{4:0.0})->({5:0.0},{6:0.0}) ({10:+0.0;-0.0;0.0},{11:+0.0;-0.0;0.0}); target=({7:0.0},{8:0.0})",
                        attempt + 1,
                        previousX,
                        sliderX,
                        previousBallX,
                        previousBallY,
                        nextResult.Ball.CenterX,
                        nextResult.Ball.CenterY,
                        nextResult.Circle.CenterX,
                        nextResult.Circle.CenterY,
                        sliderX - previousX,
                        nextResult.Ball.CenterX - previousBallX,
                        nextResult.Ball.CenterY - previousBallY);
                    lastCursorDelta = sliderX - previousX;
                    lastBallDeltaX = nextResult.Ball.CenterX - previousBallX;
                    lastBallDeltaY = nextResult.Ball.CenterY - previousBallY;
                    result = nextResult;
                }

                _project.SendWarningToLog(
                    "Hunt football calibration failed:\n" + diagnostics,
                    true);
                throw new InvalidOperationException(
                    "Hunt football target was not reached. Ball: " +
                    (result?.Ball == null
                        ? "<none>"
                        : string.Format(
                            CultureInfo.InvariantCulture,
                            "({0:0.0},{1:0.0}) {2:0.000}",
                            result.Ball.CenterX,
                            result.Ball.CenterY,
                            result.Ball.Confidence)) +
                    "; circle: " +
                    (result?.Circle == null
                        ? "<none>"
                        : string.Format(
                            CultureInfo.InvariantCulture,
                            "({0:0.0},{1:0.0}) {2:0.000}",
                            result.Circle.CenterX,
                            result.Circle.CenterY,
                            result.Circle.Confidence)) +
                    Environment.NewLine +
                    diagnostics);
            }
            finally
            {
                if (mouseDown)
                    _instance.ActiveTab.MouseClick(sliderX, sliderY, "left", "up");
            }
        }

        public HuntBallResult Detect(string imageBase64, float confidence = 0.30f)
        {
            var detections = _model.Detect(imageBase64, confidence);

            return new HuntBallResult
            {
                Ball = detections.Where(item => item.ClassId == 0)
                    .OrderByDescending(item => item.Confidence)
                    .FirstOrDefault(),
                Circle = detections.Where(item => item.ClassId == 1)
                    .OrderByDescending(item => item.Confidence)
                    .FirstOrDefault()
            };
        }

        public void Dispose()
        {
            _model.Dispose();
        }
    }

    internal class HuntModel : IDisposable
    {
        private const int InputSize = 640;

        private readonly InferenceSession _session;
        private readonly IReadOnlyList<string> _classNames;

        public HuntModel(string path, IReadOnlyList<string> classNames)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Model path is required", nameof(path));

            var fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("ONNX model was not found", fullPath);

            _classNames = classNames ??
                throw new ArgumentNullException(nameof(classNames));
            _session = new InferenceSession(fullPath);
        }

        public IReadOnlyList<HuntDetection> Detect(
            string imageBase64,
            float confidence)
        {
            if (confidence < 0 || confidence > 1)
                throw new ArgumentOutOfRangeException(
                    nameof(confidence),
                    "Confidence must be between 0 and 1");

            using (var source = ReadImage(imageBase64))
            {
                Letterbox letterbox;
                var inputTensor = CreateInput(source, out letterbox);
                var input = NamedOnnxValue.CreateFromTensor("images", inputTensor);

                using (var results = _session.Run(new[] { input }))
                {
                    var output = results.First().AsTensor<float>();
                    var dimensions = output.Dimensions.ToArray();
                    if (dimensions.Length != 3 ||
                        dimensions[0] != 1 ||
                        dimensions[2] != 6)
                    {
                        throw new InvalidOperationException(
                            "Expected Hunt ONNX output [1,N,6], got [" +
                            string.Join(",", dimensions) + "]");
                    }

                    var values = output.ToArray();
                    var detections = new List<HuntDetection>();

                    for (var row = 0; row < dimensions[1]; row++)
                    {
                        var offset = row * 6;
                        var score = values[offset + 4];
                        if (score < confidence)
                            continue;

                        var classId = (int)Math.Round(values[offset + 5]);
                        if (classId < 0 || classId >= _classNames.Count)
                            continue;

                        var x1 = Clamp(
                            (values[offset] - letterbox.PadX) / letterbox.Scale,
                            0,
                            source.Width);
                        var y1 = Clamp(
                            (values[offset + 1] - letterbox.PadY) / letterbox.Scale,
                            0,
                            source.Height);
                        var x2 = Clamp(
                            (values[offset + 2] - letterbox.PadX) / letterbox.Scale,
                            0,
                            source.Width);
                        var y2 = Clamp(
                            (values[offset + 3] - letterbox.PadY) / letterbox.Scale,
                            0,
                            source.Height);

                        if (x2 <= x1 || y2 <= y1)
                            continue;

                        detections.Add(new HuntDetection
                        {
                            ClassId = classId,
                            Name = _classNames[classId],
                            Confidence = score,
                            X1 = x1,
                            Y1 = y1,
                            X2 = x2,
                            Y2 = y2
                        });
                    }

                    return detections;
                }
            }
        }

        private DenseTensor<float> CreateInput(
            Bitmap source,
            out Letterbox letterbox)
        {
            var scale = Math.Min(
                (double)InputSize / source.Width,
                (double)InputSize / source.Height);
            var width = Math.Max(1, (int)Math.Round(source.Width * scale));
            var height = Math.Max(1, (int)Math.Round(source.Height * scale));
            var padX = (InputSize - width) / 2;
            var padY = (InputSize - height) / 2;

            using (var resized = new Bitmap(
                InputSize,
                InputSize,
                PixelFormat.Format24bppRgb))
            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.Clear(Color.FromArgb(114, 114, 114));
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(
                    source,
                    new Rectangle(padX, padY, width, height));

                var tensor = new DenseTensor<float>(
                    new[] { 1, 3, InputSize, InputSize });
                var area = new Rectangle(0, 0, InputSize, InputSize);
                var data = resized.LockBits(
                    area,
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb);

                try
                {
                    var stride = Math.Abs(data.Stride);
                    var bytes = new byte[stride * InputSize];
                    Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

                    for (var y = 0; y < InputSize; y++)
                    for (var x = 0; x < InputSize; x++)
                    {
                        var row = data.Stride >= 0 ? y : InputSize - 1 - y;
                        var index = row * stride + x * 3;
                        tensor[0, 0, y, x] = bytes[index + 2] / 255f;
                        tensor[0, 1, y, x] = bytes[index + 1] / 255f;
                        tensor[0, 2, y, x] = bytes[index] / 255f;
                    }
                }
                finally
                {
                    resized.UnlockBits(data);
                }

                letterbox = new Letterbox
                {
                    Scale = (float)scale,
                    PadX = padX,
                    PadY = padY
                };

                return tensor;
            }
        }

        public Bitmap ReadImage(string imageBase64)
        {
            if (string.IsNullOrWhiteSpace(imageBase64))
                throw new ArgumentException("Image is required", nameof(imageBase64));

            var comma = imageBase64.IndexOf(',');
            var raw = imageBase64.StartsWith(
                          "data:",
                          StringComparison.OrdinalIgnoreCase) &&
                      comma >= 0
                ? imageBase64.Substring(comma + 1)
                : imageBase64;

            using (var stream = new MemoryStream(Convert.FromBase64String(raw)))
            using (var source = new Bitmap(stream))
                return new Bitmap(source);
        }

        public void Dispose()
        {
            _session.Dispose();
        }

        private float Clamp(float value, float minimum, float maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        private class Letterbox
        {
            public float Scale;
            public float PadX;
            public float PadY;
        }
    }

    public class HuntDetection
    {
        public int ClassId { get; set; }
        public string Name { get; set; }
        public float Confidence { get; set; }
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }
        public float CenterX => (X1 + X2) / 2f;
        public float CenterY => (Y1 + Y2) / 2f;
    }

    public class HuntPoint
    {
        public int ClassId { get; set; }
        public string Name { get; set; }
        public float Confidence { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class HuntBallResult
    {
        public HuntDetection Ball { get; set; }
        public HuntDetection Circle { get; set; }
    }
}

namespace z3n7.Captcha
{
    public static partial class CaptchaExtensions
    {
        public static void SolveHunt(this Instance instance, IZennoPosterProjectModel project , int attempts = 10)
        {
            var solved = false;
            
            using (var hunt = new Hunt( project,   instance))
            {
	    
                while ( attempts-- > 0)
                {
                    try
                    {
                        hunt.SolveAndSubmit();
                        Thread.Sleep(5000);
                        solved = hunt.CheckResult();
				
                    }
                    catch
                    {
                        hunt.MainButtonClick();
                        hunt.CheckResult();
                        
                    }
                    if (solved)
                        return ;
			
			
                }
		
                throw new  Exception ("too many attempts");
            }


        }
    }
}
