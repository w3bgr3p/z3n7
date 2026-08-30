using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Captcha
{
    public class GeeTestV4
    {
        private const int CanvasDifferenceThreshold = 25;
        private const int ShapeDifferenceThreshold = 70;
        private const int NormalizedShapeSize = 64;
        private static readonly object ReferenceLock = new object();
        private static List<RgbImage> ReferenceCanvases;
        private static string ReferenceCanvasesFolder;

        private readonly Instance _instance;
        private readonly IZennoPosterProjectModel _project;
        private string _solutionJson;
        private string _type;
        public GeeTestV4(IZennoPosterProjectModel project,Instance instance)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));

            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        public void SaveSamples()
        {

            var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            var sampleFolder = "";
            Thread.Sleep(3000);
            var raw = "";
            
            try{
	            var items = new Traffic(_instance).FindAll("https://gcaptcha4.geetest.com/load");
	            
	            
	            foreach (var item in items)
	            {
		            if (item.ResponseBody != "")
		            {
			            raw = item.ResponseBody;
			            break;
		            }
	            }
	            if (raw == "")
		            throw new Exception ("capthca type not found in traffic");
	            
	            var body = !raw.StartsWith("{") ? raw.Substring(raw.IndexOf('(') + 1,raw.LastIndexOf(')') - raw.IndexOf('(') - 1) : raw ;
	            
                var response = JObject.Parse(body);
                _type = response.SelectToken("data.captcha_type")?.Value<string>();
                if (string.IsNullOrWhiteSpace(_type))
                    throw new JsonException("data.captcha_type was not found");
                _project.Var("geetestType",_type);
	            
	            sampleFolder = Path.Combine(_project.Path, "geetest_cache", _type, ts);
                _project.Var("sampleFolder", sampleFolder);
	            Directory.CreateDirectory(sampleFolder);
	            
	            File.WriteAllText(Path.Combine(sampleFolder,"request.json"), body);
                
            }
            catch(Exception ex)
            {
                _project.warn(ex + $"\nraw: {raw}");
            }


            var json = "";
            if (_type == "icon")

            {
	            var geeCanvas = _instance.ActiveTab.FindElementByAttribute("div", "style", "background-image:\\ url\\(\"https://static.geetest.com/captcha_v4", "regexp", 0).DrawToBitmap(true);
	            var tips = _instance.ActiveTab.FindElementByAttribute("div", "class", "geetest_ques_tips_", "regexp", 0).GetChildren(false).ToList();
	            var geeTips = new  List<string>();
	            
	            foreach (var child in tips)
	            {
		            geeTips.Add(child.DrawToBitmap(true));
	            }
	            json = JsonConvert.SerializeObject(new
	            {
		            type = _type,
		            canvas = geeCanvas,
		            tips = geeTips
	            });
            }

            if (_type == "nine")
            {
	            var nine_prompt = _instance.ActiveTab.FindElementByAttribute("img", "src", "https://static.geetest.com/nerualpic/v4_pic/nine_prompt/", "regexp", 0).DrawToBitmap(true);
	            var nine_items = _instance.ActiveTab.FindElementsByAttribute("div", "class", "geetest_item_ghost", "regexp").ToList();
	            var geeNine = new  List<string>();
	            foreach (var item in nine_items)
	            {
		            geeNine.Add(item.DrawToBitmap(true));
	            }
	            json = JsonConvert.SerializeObject(new
	            {
		            type = _type,
		            prompt = nine_prompt,
		            items = geeNine
	            });
            }

            if (json != "")
            {
	            File.WriteAllText(Path.Combine(sampleFolder, "imgs.json"),json);
            }
            
        }

        public string GetResult()
        {
            Thread.Sleep(3000);
            var item = new Traffic(_instance).Find("https://gcaptcha4.geetest.com/verify");
            var raw = item.ResponseBody;
            var body = !raw.StartsWith("{") ? raw.Substring(raw.IndexOf('(') + 1,raw.LastIndexOf(')') - raw.IndexOf('(') - 1) : raw ;
            var response = JObject.Parse(body);
            var result = response.SelectToken("data.result")?.Value<string>();
            if (string.IsNullOrWhiteSpace(result))
                throw new JsonException("data.result was not found");
            _project.SendInfoToLog($"geetest: {result}", true);
            
            if (result == "success")
            {
                var sampleFolder = _project.Var("sampleFolder");
                File.WriteAllText(Path.Combine(sampleFolder, "solved.json"),body);
                if (!string.IsNullOrWhiteSpace(_solutionJson))
                    File.WriteAllText(Path.Combine(sampleFolder, "solution.json"), _solutionJson);
            }
            return result;
        }

        public string SolveAndSubmit()
        {
            if (_type != "icon")
                throw new Exception($"type {_type} not implemented");
            
            _project.SendInfoToLog($"solving: {_type}", true);
            
            var canvasElement = _instance.ActiveTab.FindElementByAttribute(
                "div",
                "style",
                "background-image:\\ url\\(\"https://static.geetest.com/captcha_v4",
                "regexp",
                0);
            if (canvasElement == null || canvasElement.IsVoid)
                throw new InvalidOperationException("GeeTest canvas was not found");

            var tipsElement = _instance.ActiveTab.FindElementByAttribute(
                "div",
                "class",
                "geetest_ques_tips_",
                "regexp",
                0);
            if (tipsElement == null || tipsElement.IsVoid)
                throw new InvalidOperationException("GeeTest target images were not found");

            var tips = tipsElement.GetChildren(false).ToList();
            if (tips.Count < 3)
                throw new InvalidOperationException($"Expected 3 target images, found {tips.Count}");

            var targetImages = new string[3];
            for (var i = 0; i < 3; i++)
                targetImages[i] = tips[i].DrawToBitmap(true);

            var canvasImage = canvasElement.DrawToBitmap(true);
            var referencesFolder = Path.Combine(_project.Path, "geetest_cache", "icon");
            var points = SolveImages(canvasImage, targetImages, referencesFolder);
            var position = canvasElement.DisplacementInBrowser;

            foreach (var point in points)
            {
                Thread.Sleep(1000);
                var click = new Rectangle(position.X + point.X, position.Y + point.Y, 1, 1);
                _instance.ActiveTab.RiseEvent("click", click, "Left");
            }

            _instance.HeClick(("div", "class", "geetest_submit", "regexp", 0));
            using (var canvas = DecodeBitmap(canvasImage))
            {
                var sampleFolder = _project.Var("sampleFolder");
                var sampleId = Path.GetFileName(sampleFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                _solutionJson = BuildIconSolutionJson(sampleId, canvas.Width, canvas.Height, points);
            }
            return _solutionJson;
        }

        private static string BuildIconSolutionJson(
            string sampleId,
            int canvasWidth,
            int canvasHeight,
            IEnumerable<GeeTestPoint> points)
        {
            return JsonConvert.SerializeObject(new
            {
                version = 1,
                type = "icon",
                sample_id = sampleId,
                canvas_width = canvasWidth,
                canvas_height = canvasHeight,
                points = points.Select(point => new
                {
                    index = point.Index,
                    x = point.X,
                    y = point.Y
                })
            }, Formatting.Indented);
        }

        private static List<GeeTestPoint> SolveImages(
            string canvasImage,
            IList<string> targetImages,
            string referencesFolder)
        {
            if (targetImages == null || targetImages.Count != 3)
                throw new ArgumentException("Exactly 3 target images are required", nameof(targetImages));

            using (var canvasBitmap = DecodeBitmap(canvasImage))
            using (var target1 = DecodeBitmap(targetImages[0]))
            using (var target2 = DecodeBitmap(targetImages[1]))
            using (var target3 = DecodeBitmap(targetImages[2]))
            {
                var canvas = ReadRgb(canvasBitmap);
                var targets = new[] { ReadAlpha(target1), ReadAlpha(target2), ReadAlpha(target3) };
                return Solve(canvas, targets, referencesFolder);
            }
        }

        private static Bitmap DecodeBitmap(string imageBase64)
        {
            var comma = imageBase64.IndexOf(',');
            var raw = imageBase64.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && comma >= 0
                ? imageBase64.Substring(comma + 1)
                : imageBase64;

            using (var stream = new MemoryStream(Convert.FromBase64String(raw)))
            using (var source = new Bitmap(stream))
                return new Bitmap(source);
        }

        private sealed class RgbImage
        {
            public int Width;
            public int Height;
            public byte[] R;
            public byte[] G;
            public byte[] B;
        }

        private sealed class Mask
        {
            public int Width;
            public int Height;
            public bool[] Pixels;
        }

        private sealed class Candidate
        {
            public int X0;
            public int Y0;
            public int X1;
            public int Y1;
            public int PixelCount;
            public double CenterX => (X0 + X1) / 2.0;
            public double CenterY => (Y0 + Y1) / 2.0;
        }

        private static List<GeeTestPoint> Solve(RgbImage canvas, Mask[] targets, string referencesFolder)
        {
            var references = LoadReferenceCanvases(referencesFolder, canvas.Width, canvas.Height);
            var peers = references
                .Select(reference => new { Image = reference, Match = PixelMatchRatio(canvas, reference) })
                .Where(item => item.Match > 0.45 && item.Match < 0.90)
                .Select(item => item.Image)
                .ToList();

            if (peers.Count == 0)
                throw new InvalidOperationException("No matching clean background was found in the labeled samples");

            var difference = MinimumDifference(canvas, peers);
            var candidates = FindCandidates(difference, canvas.Width, canvas.Height);
            if (candidates.Count != 3)
                throw new InvalidOperationException($"Expected 3 gesture candidates, found {candidates.Count}");

            var costs = new double[3, 3];
            for (var target = 0; target < 3; target++)
                for (var candidate = 0; candidate < 3; candidate++)
                    costs[target, candidate] = RotationInvariantDistance(
                        Normalize(targets[target]),
                        Normalize(CandidateShape(difference, canvas.Width, candidates[candidate])));

            var assignment = BestAssignment(costs);
            var result = new List<GeeTestPoint>();
            for (var target = 0; target < 3; target++)
            {
                var candidate = candidates[assignment[target]];
                result.Add(new GeeTestPoint
                {
                    Index = target + 1,
                    X = (int)Math.Round(candidate.CenterX),
                    Y = (int)Math.Round(candidate.CenterY),
                    Confidence = Math.Round(1.0 / (1.0 + costs[target, assignment[target]]), 3)
                });
            }

            return result;
        }

        private static List<RgbImage> LoadReferenceCanvases(string samplesFolder, int width, int height)
        {
            if (!Directory.Exists(samplesFolder))
                throw new DirectoryNotFoundException(samplesFolder);

            lock (ReferenceLock)
            {
                if (ReferenceCanvases != null &&
                    string.Equals(ReferenceCanvasesFolder, samplesFolder, StringComparison.OrdinalIgnoreCase) &&
                    ReferenceCanvases.All(image => image.Width == width && image.Height == height))
                    return ReferenceCanvases;

                var result = new List<RgbImage>();
                foreach (var sampleFolder in Directory.EnumerateDirectories(samplesFolder))
                {
                    var imgsPath = Path.Combine(sampleFolder, "imgs.json");
                    var solutionPath = Path.Combine(sampleFolder, "solution.json");
                    if (!File.Exists(imgsPath) || !File.Exists(solutionPath))
                        continue;

                    var canvasToken = JObject.Parse(File.ReadAllText(imgsPath)).GetValue("canvas");
                    var encoded = canvasToken == null ? null : canvasToken.ToString();
                    if (string.IsNullOrWhiteSpace(encoded))
                        continue;

                    using (var bitmap = DecodeBitmap(encoded.Trim()))
                    {
                        if (bitmap.Width == width && bitmap.Height == height)
                            result.Add(ReadRgb(bitmap));
                    }
                }

                if (result.Count == 0)
                    throw new InvalidOperationException("No labeled sample canvases were found");

                ReferenceCanvases = result;
                ReferenceCanvasesFolder = samplesFolder;
                return ReferenceCanvases;
            }
        }

        private static RgbImage ReadRgb(Bitmap source)
        {
            using (var bitmap = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawImageUnscaled(source, 0, 0);
                var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var data = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    var bytes = new byte[Math.Abs(data.Stride) * data.Height];
                    Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
                    var count = bitmap.Width * bitmap.Height;
                    var result = new RgbImage
                    {
                        Width = bitmap.Width,
                        Height = bitmap.Height,
                        R = new byte[count],
                        G = new byte[count],
                        B = new byte[count]
                    };
                    for (var y = 0; y < bitmap.Height; y++)
                    for (var x = 0; x < bitmap.Width; x++)
                    {
                        var sourceIndex = y * data.Stride + x * 4;
                        var targetIndex = y * bitmap.Width + x;
                        result.B[targetIndex] = bytes[sourceIndex];
                        result.G[targetIndex] = bytes[sourceIndex + 1];
                        result.R[targetIndex] = bytes[sourceIndex + 2];
                    }
                    return result;
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
            }
        }

        private static Mask ReadAlpha(Bitmap bitmap)
        {
            var pixels = new bool[bitmap.Width * bitmap.Height];
            for (var y = 0; y < bitmap.Height; y++)
            for (var x = 0; x < bitmap.Width; x++)
                pixels[y * bitmap.Width + x] = bitmap.GetPixel(x, y).A >= 100;
            return new Mask { Width = bitmap.Width, Height = bitmap.Height, Pixels = pixels };
        }

        private static double PixelMatchRatio(RgbImage left, RgbImage right)
        {
            var matches = 0;
            for (var i = 0; i < left.R.Length; i++)
            {
                var difference = Math.Max(Math.Abs(left.R[i] - right.R[i]),
                    Math.Max(Math.Abs(left.G[i] - right.G[i]), Math.Abs(left.B[i] - right.B[i])));
                if (difference <= 2)
                    matches++;
            }
            return matches / (double)left.R.Length;
        }

        private static byte[] MinimumDifference(RgbImage canvas, IList<RgbImage> peers)
        {
            var result = Enumerable.Repeat((byte)255, canvas.R.Length).ToArray();
            foreach (var peer in peers)
            for (var i = 0; i < result.Length; i++)
            {
                var difference = Math.Max(Math.Abs(canvas.R[i] - peer.R[i]),
                    Math.Max(Math.Abs(canvas.G[i] - peer.G[i]), Math.Abs(canvas.B[i] - peer.B[i])));
                if (difference < result[i])
                    result[i] = (byte)difference;
            }
            return result;
        }

        private static List<Candidate> FindCandidates(byte[] difference, int width, int height)
        {
            var raw = difference.Select(value => value >= CanvasDifferenceThreshold).ToArray();
            var grown = new bool[raw.Length];
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                if (!raw[y * width + x])
                    continue;
                for (var dy = -1; dy <= 1; dy++)
                for (var dx = -1; dx <= 1; dx++)
                {
                    var nx = x + dx;
                    var ny = y + dy;
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                        grown[ny * width + nx] = true;
                }
            }

            var visited = new bool[grown.Length];
            var queue = new int[grown.Length];
            var candidates = new List<Candidate>();
            for (var start = 0; start < grown.Length; start++)
            {
                if (!grown[start] || visited[start])
                    continue;
                var head = 0;
                var tail = 0;
                queue[tail++] = start;
                visited[start] = true;
                var candidate = new Candidate { X0 = width, Y0 = height, X1 = -1, Y1 = -1 };
                while (head < tail)
                {
                    var index = queue[head++];
                    var x = index % width;
                    var y = index / width;
                    if (raw[index])
                    {
                        candidate.PixelCount++;
                        candidate.X0 = Math.Min(candidate.X0, x);
                        candidate.Y0 = Math.Min(candidate.Y0, y);
                        candidate.X1 = Math.Max(candidate.X1, x);
                        candidate.Y1 = Math.Max(candidate.Y1, y);
                    }

                    for (var dy = -1; dy <= 1; dy++)
                    for (var dx = -1; dx <= 1; dx++)
                    {
                        var nx = x + dx;
                        var ny = y + dy;
                        if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                            continue;
                        var next = ny * width + nx;
                        if (grown[next] && !visited[next])
                        {
                            visited[next] = true;
                            queue[tail++] = next;
                        }
                    }
                }

                var candidateWidth = candidate.X1 - candidate.X0 + 1;
                var candidateHeight = candidate.Y1 - candidate.Y0 + 1;
                if (candidate.PixelCount >= 18 && candidateWidth >= 10 && candidateHeight >= 10 &&
                    candidateWidth <= 90 && candidateHeight <= 90)
                    candidates.Add(candidate);
            }

            return candidates.OrderByDescending(candidate => candidate.PixelCount).Take(3).ToList();
        }

        private static Mask CandidateShape(byte[] difference, int canvasWidth, Candidate candidate)
        {
            var width = candidate.X1 - candidate.X0 + 1;
            var height = candidate.Y1 - candidate.Y0 + 1;
            var pixels = new bool[width * height];
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                pixels[y * width + x] = difference[(candidate.Y0 + y) * canvasWidth + candidate.X0 + x] >=
                                         ShapeDifferenceThreshold;
            return new Mask { Width = width, Height = height, Pixels = pixels };
        }

        private static Mask Normalize(Mask source)
        {
            var x0 = source.Width;
            var y0 = source.Height;
            var x1 = -1;
            var y1 = -1;
            for (var y = 0; y < source.Height; y++)
            for (var x = 0; x < source.Width; x++)
            {
                if (!source.Pixels[y * source.Width + x])
                    continue;
                x0 = Math.Min(x0, x);
                y0 = Math.Min(y0, y);
                x1 = Math.Max(x1, x);
                y1 = Math.Max(y1, y);
            }

            var output = new Mask
            {
                Width = NormalizedShapeSize,
                Height = NormalizedShapeSize,
                Pixels = new bool[NormalizedShapeSize * NormalizedShapeSize]
            };
            if (x1 < x0 || y1 < y0)
                return output;

            var cropWidth = x1 - x0 + 1;
            var cropHeight = y1 - y0 + 1;
            var scale = 54.0 / Math.Max(cropWidth, cropHeight);
            var newWidth = Math.Max(1, (int)Math.Round(cropWidth * scale));
            var newHeight = Math.Max(1, (int)Math.Round(cropHeight * scale));
            var offsetX = (NormalizedShapeSize - newWidth) / 2;
            var offsetY = (NormalizedShapeSize - newHeight) / 2;
            for (var y = 0; y < newHeight; y++)
            for (var x = 0; x < newWidth; x++)
            {
                var sourceX = x0 + Math.Min(cropWidth - 1, (int)(x / scale));
                var sourceY = y0 + Math.Min(cropHeight - 1, (int)(y / scale));
                output.Pixels[(offsetY + y) * NormalizedShapeSize + offsetX + x] =
                    source.Pixels[sourceY * source.Width + sourceX];
            }
            return output;
        }

        private static Mask Rotate(Mask source, double degrees)
        {
            var side = (int)Math.Ceiling(Math.Sqrt(source.Width * source.Width + source.Height * source.Height)) + 2;
            var result = new Mask { Width = side, Height = side, Pixels = new bool[side * side] };
            var radians = degrees * Math.PI / 180.0;
            var cosine = Math.Cos(radians);
            var sine = Math.Sin(radians);
            var sourceCx = (source.Width - 1) / 2.0;
            var sourceCy = (source.Height - 1) / 2.0;
            var targetCenter = (side - 1) / 2.0;
            for (var y = 0; y < side; y++)
            for (var x = 0; x < side; x++)
            {
                var dx = x - targetCenter;
                var dy = y - targetCenter;
                var sourceX = (int)Math.Round(cosine * dx + sine * dy + sourceCx);
                var sourceY = (int)Math.Round(-sine * dx + cosine * dy + sourceCy);
                if (sourceX >= 0 && sourceX < source.Width && sourceY >= 0 && sourceY < source.Height)
                    result.Pixels[y * side + x] = source.Pixels[sourceY * source.Width + sourceX];
            }
            return result;
        }

        private static double RotationInvariantDistance(Mask target, Mask candidate)
        {
            var candidateDistance = DistanceMap(candidate);
            var best = double.MaxValue;
            for (var angle = 0; angle < 360; angle += 6)
            {
                var rotated = Normalize(Rotate(target, angle));
                var distance = Chamfer(candidate, rotated, candidateDistance, DistanceMap(rotated));
                best = Math.Min(best, distance);
            }
            return best;
        }

        private static double[] DistanceMap(Mask mask)
        {
            const double diagonal = 1.41421356237;
            var distance = mask.Pixels.Select(pixel => pixel ? 0.0 : 1000000.0).ToArray();
            for (var y = 0; y < mask.Height; y++)
            for (var x = 0; x < mask.Width; x++)
            {
                var index = y * mask.Width + x;
                if (x > 0) distance[index] = Math.Min(distance[index], distance[index - 1] + 1);
                if (y > 0) distance[index] = Math.Min(distance[index], distance[index - mask.Width] + 1);
                if (x > 0 && y > 0)
                    distance[index] = Math.Min(distance[index], distance[index - mask.Width - 1] + diagonal);
                if (x + 1 < mask.Width && y > 0)
                    distance[index] = Math.Min(distance[index], distance[index - mask.Width + 1] + diagonal);
            }
            for (var y = mask.Height - 1; y >= 0; y--)
            for (var x = mask.Width - 1; x >= 0; x--)
            {
                var index = y * mask.Width + x;
                if (x + 1 < mask.Width) distance[index] = Math.Min(distance[index], distance[index + 1] + 1);
                if (y + 1 < mask.Height)
                    distance[index] = Math.Min(distance[index], distance[index + mask.Width] + 1);
                if (x + 1 < mask.Width && y + 1 < mask.Height)
                    distance[index] = Math.Min(distance[index], distance[index + mask.Width + 1] + diagonal);
                if (x > 0 && y + 1 < mask.Height)
                    distance[index] = Math.Min(distance[index], distance[index + mask.Width - 1] + diagonal);
            }
            return distance;
        }

        private static double Chamfer(Mask left, Mask right, double[] leftDistance, double[] rightDistance)
        {
            var leftSum = 0.0;
            var rightSum = 0.0;
            var leftCount = 0;
            var rightCount = 0;
            for (var i = 0; i < left.Pixels.Length; i++)
            {
                if (left.Pixels[i])
                {
                    leftSum += rightDistance[i];
                    leftCount++;
                }
                if (right.Pixels[i])
                {
                    rightSum += leftDistance[i];
                    rightCount++;
                }
            }
            if (leftCount == 0 || rightCount == 0)
                return 1000000;
            return leftSum / leftCount + rightSum / rightCount;
        }

        private static int[] BestAssignment(double[,] costs)
        {
            var permutations = new[]
            {
                new[] { 0, 1, 2 }, new[] { 0, 2, 1 }, new[] { 1, 0, 2 },
                new[] { 1, 2, 0 }, new[] { 2, 0, 1 }, new[] { 2, 1, 0 }
            };
            return permutations.OrderBy(permutation =>
                costs[0, permutation[0]] + costs[1, permutation[1]] + costs[2, permutation[2]]).First();
        }

    }

    public class GeeTestPoint
    {
        public int Index { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public double Confidence { get; set; }
    }
}


