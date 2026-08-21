using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace HuntOnnxProbe
{
    internal static class ProbeCoordinator
    {
        public static ProbeReport Run(ProbeOptions options)
        {
            var modelPath = Path.GetFullPath(options.ModelPath);
            if (!File.Exists(modelPath))
                throw new FileNotFoundException("Model was not found", modelPath);

            var report = new ProbeReport
            {
                Machine = Environment.MachineName,
                Os = Environment.OSVersion.ToString(),
                Clr = Environment.Version.ToString(),
                Process64Bit = Environment.Is64BitProcess,
                ProcessorCount = Environment.ProcessorCount,
                ProcessorIdentifier = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"),
                ModelPath = modelPath,
                ModelSha256 = FileFingerprint.Sha256(modelPath),
                Results = new List<WorkerResult>()
            };

            foreach (var mode in options.Modes)
                report.Results.Add(RunWorker(modelPath, mode, options.TimeoutSeconds));

            return report;
        }

        private static WorkerResult RunWorker(
            string modelPath,
            string mode,
            int timeoutSeconds)
        {
            var executable = Assembly.GetEntryAssembly().Location;
            var startInfo = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = "--worker --model " + Quote(modelPath) + " --mode " + mode,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = Process.Start(startInfo))
            {
                var stdout = process.StandardOutput.ReadToEndAsync();
                var stderr = process.StandardError.ReadToEndAsync();
                var completed = process.WaitForExit(timeoutSeconds * 1000);

                if (!completed)
                {
                    process.Kill();
                    process.WaitForExit();
                    return new WorkerResult
                    {
                        Mode = mode,
                        Status = "timeout",
                        Error = "Inference exceeded " + timeoutSeconds + " seconds",
                        Progress = stderr.Result
                    };
                }

                var output = stdout.Result;
                var progress = stderr.Result;
                if (string.IsNullOrWhiteSpace(output))
                {
                    return new WorkerResult
                    {
                        Mode = mode,
                        Status = "error",
                        Error = "Worker returned no JSON; exitCode=" + process.ExitCode,
                        Progress = progress
                    };
                }

                var result = JsonOutput.Deserialize<WorkerResult>(output);
                result.Progress = progress;
                return result;
            }
        }

        private static string Quote(string value)
        {
            return "\"" + value.Replace("\"", "\\\"") + "\"";
        }
    }
}
