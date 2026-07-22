using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using z3nIO;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.Enums.Log;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    public class Test
    {
        private const string SamplesFolder = @"W:\code_hard\.net\z3n7\Numlex\samples";
        private const int CanvasDifferenceThreshold = 25;
        private const int ShapeDifferenceThreshold = 70;
        private const int NormalizedShapeSize = 64;
        private static readonly object ReferenceLock = new object();
        private static List<RgbImage> ReferenceCanvases;

        public static void GetImg(IZennoPosterProjectModel project, Instance instance)
        {
            var he = instance.ActiveTab.FindElementByAttribute(
                "div",
                "style",
                "background-image:\\ url\\(\"https://static.geetest.com/captcha_v4",
                "regexp",
                0);

            var canvas = he.DrawToBitmap(true);
            project.Var("canvasX", he.DisplacementInBrowser.X);
            project.Var("canvasY", he.DisplacementInBrowser.Y);
            project.Var("canvas", canvas);

            var tips = instance.ActiveTab
                .FindElementByAttribute("div", "class", "geetest_ques_tips_", "regexp", 0)
                .GetChildren(false)
                .ToList();

            if (tips.Count < 3)
                throw new InvalidOperationException($"Expected 3 target images, found {tips.Count}");

            for (var i = 0; i < 3; i++)
            {
                var target = tips[i].DrawToBitmap(true);
                project.Var($"img{i + 1}", target);
            }
        }

        public static void SolveWithOmni(IZennoPosterProjectModel project, Instance instance)
        {
            project.Var("aiVisionResult", SolveImages(
                project.Var("canvas"),
                new[] { project.Var("img1"), project.Var("img2"), project.Var("img3") }));
        }

        public static string SolveImages(string canvasImage, IList<string> targetImages)
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
                var points = Solve(canvas, targets);
                return JsonConvert.SerializeObject(new
                {
                    canvas_width = canvas.Width,
                    canvas_height = canvas.Height,
                    points
                });
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

        private sealed class ResultPoint
        {
            public int index { get; set; }
            public int x { get; set; }
            public int y { get; set; }
            public double confidence { get; set; }
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

        private static List<ResultPoint> Solve(RgbImage canvas, Mask[] targets)
        {
            var references = LoadReferenceCanvases(canvas.Width, canvas.Height);
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
            var result = new List<ResultPoint>();
            for (var target = 0; target < 3; target++)
            {
                var candidate = candidates[assignment[target]];
                result.Add(new ResultPoint
                {
                    index = target + 1,
                    x = (int)Math.Round(candidate.CenterX),
                    y = (int)Math.Round(candidate.CenterY),
                    confidence = Math.Round(1.0 / (1.0 + costs[target, assignment[target]]), 3)
                });
            }

            return result;
        }

        private static List<RgbImage> LoadReferenceCanvases(int width, int height)
        {
            if (!Directory.Exists(SamplesFolder))
                throw new DirectoryNotFoundException(SamplesFolder);

            lock (ReferenceLock)
            {
                if (ReferenceCanvases != null && ReferenceCanvases.All(image => image.Width == width && image.Height == height))
                    return ReferenceCanvases;

            var result = new List<RgbImage>();
            foreach (var path in Directory.EnumerateFiles(SamplesFolder, "case_*.txt"))
            {
                if (!File.Exists(Path.ChangeExtension(path, ".json")))
                    continue;

                var encoded = File.ReadLines(path).FirstOrDefault(line => !string.IsNullOrWhiteSpace(line));
                if (encoded == null)
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

        public static void Submit(IZennoPosterProjectModel project, Instance instance)
        {
            var root = JObject.Parse(project.Var("aiVisionResult"));
            var points = root["points"]
                .Select(p => new
                {
                    X = (int)p["x"],
                    Y = (int)p["y"]
                })
                .ToList();

            var x0 = project.Int("canvasX");
            var y0 = project.Int("canvasY");
            foreach (var point in points)
            {
                Thread.Sleep(1000);
                var clickPoint = new Rectangle(x0 + point.X, y0 + point.Y, 1, 1);
                instance.ActiveTab.RiseEvent("click", clickPoint, "Left");
            }

            instance.HeClick(("div", "class", "geetest_submit", "regexp", 0));
        }
    }

    public static class NumlexDb
    {
        
        
        public static void CreateTable(IZennoPosterProjectModel project)
        {
            var jsonPath = @"W:\code_hard\numlex\localhost-provider\routes.json";
            var routes = JsonConvert.DeserializeObject<List<JObject>>(
                File.ReadAllText(jsonPath)
            );
            var directions = routes
                .SelectMany(route =>
                {
                    var service = route["service"]?.ToString()?.Trim();

                    return (route["aliases"]?.Values<string>() ?? Enumerable.Empty<string>())
                        .Where(alias => !string.IsNullOrWhiteSpace(alias))
                        .Select(alias => $"{alias.Trim()}-{service}");
                })
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var tableName = $"numlex_sites.{project.Name.ToLower().Split('.')[1]}";
            project.Var("projectTable", tableName);
            var tableStructure = new Dictionary<string, string>
            {
                { "id",               "INTEGER PRIMARY KEY AUTOINCREMENT" },
                { "direction",        "TEXT NOT NULL UNIQUE" },
                { "captcha_type",     "TEXT DEFAULT ''" },
                { "success",          "INTEGER DEFAULT 0" },
                { "failed",           "INTEGER DEFAULT 0" },
                { "error",            "TEXT DEFAULT ''" },
            };

            project.PrepareProjectTable(tableStructure, tableName);

            foreach (var direction in directions)
            {
                project.DbQ($@"INSERT INTO {tableName} (""direction"", ""success"", ""failed"") VALUES ('{direction.Replace("'", "''")}', 0, 0) ON CONFLICT (""direction"") DO NOTHING;");
            }
        }
        public static void IncreaseSuccess(IZennoPosterProjectModel project)
        {
            var q =
                $@"UPDATE {project.Var("projectTable")} SET ""success"" = ""success"" + WHERE ""direction"" = '{project.Var("numDirection")}';";
            project.DbQ( q);
            project.SendToLog(q,LogType.Info, true, LogColor.Blue );

        }
        public static void IncreaseFailed(IZennoPosterProjectModel project)
        {
            var q =
                $@"UPDATE {project.Var("projectTable")} SET ""failed"" = ""failed"" + 1 WHERE ""direction"" = '{project.Var("numDirection")}';";
            project.DbQ(q);
            project.SendToLog(q,LogType.Info, true, LogColor.Orange );
        }
    }

}



