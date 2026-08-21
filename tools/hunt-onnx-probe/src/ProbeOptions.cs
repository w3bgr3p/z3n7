using System;
using System.Collections.Generic;

namespace HuntOnnxProbe
{
    public sealed class ProbeOptions
    {
        public string ModelPath { get; private set; }
        public string Mode { get; private set; }
        public int TimeoutSeconds { get; private set; }
        public IReadOnlyList<string> Modes
        {
            get
            {
                return Mode == "both"
                    ? new[] { "default", "single" }
                    : new[] { Mode };
            }
        }

        public static ProbeOptions Parse(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var modelPath = ReadValue(args, "--model");
            var mode = ReadValue(args, "--mode") ?? "both";
            var timeoutText = ReadValue(args, "--timeout-seconds") ?? "120";

            int timeoutSeconds;
            if (string.IsNullOrWhiteSpace(modelPath))
                throw new ArgumentException("--model is required");
            if (mode != "default" && mode != "single" && mode != "both")
                throw new ArgumentException("--mode must be default, single, or both");
            if (!int.TryParse(timeoutText, out timeoutSeconds) || timeoutSeconds < 1)
                throw new ArgumentException("--timeout-seconds must be a positive integer");

            return new ProbeOptions
            {
                ModelPath = modelPath,
                Mode = mode,
                TimeoutSeconds = timeoutSeconds
            };
        }

        private static string ReadValue(string[] args, string name)
        {
            for (var index = 0; index < args.Length; index++)
            {
                if (args[index] != name)
                    continue;
                if (index + 1 >= args.Length)
                    throw new ArgumentException(name + " requires a value");
                return args[index + 1];
            }

            return null;
        }
    }
}
