using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.ML.OnnxRuntime;

namespace HuntOnnxProbe
{
    internal static class ProbeWorker
    {
        public static WorkerResult Run(string modelPath, string mode)
        {
            var result = new WorkerResult
            {
                Mode = mode,
                Status = "error",
                ManagedAssembly = FileFingerprint.DescribeAssembly(
                    typeof(InferenceSession).Assembly)
            };

            try
            {
                var fullPath = Path.GetFullPath(modelPath);
                if (!File.Exists(fullPath))
                    throw new FileNotFoundException("Model was not found", fullPath);

                Progress(mode, "session-create-enter");
                var sessionTimer = Stopwatch.StartNew();
                using (var options = SessionFactory.CreateOptions(mode))
                using (var session = new InferenceSession(fullPath, options))
                {
                    sessionTimer.Stop();
                    result.SessionMilliseconds = sessionTimer.ElapsedMilliseconds;

                    var nativePath = FileFingerprint.FindLoadedOnnxRuntime();
                    result.NativeLibrary = nativePath;
                    if (!string.IsNullOrWhiteSpace(nativePath) && File.Exists(nativePath))
                        result.NativeSha256 = FileFingerprint.Sha256(nativePath);

                    var inputName = session.InputMetadata.Keys.Single();
                    result.InputName = inputName;
                    var tensor = TensorFactory.Create();
                    var input = NamedOnnxValue.CreateFromTensor(inputName, tensor);

                    Progress(mode, "session-run-enter");
                    var runTimer = Stopwatch.StartNew();
                    using (var outputs = session.Run(new[] { input }))
                    {
                        runTimer.Stop();
                        result.RunMilliseconds = runTimer.ElapsedMilliseconds;
                        var output = outputs.First().AsTensor<float>();
                        result.OutputDimensions = string.Join(
                            ",",
                            output.Dimensions.ToArray());
                    }
                }

                Progress(mode, "session-run-exit");
                result.Status = "success";
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ex.ToString();
                return result;
            }
        }

        private static void Progress(string mode, string stage)
        {
            Console.Error.WriteLine(
                DateTime.UtcNow.ToString("O") + " | " + mode + " | " + stage);
        }
    }
}
