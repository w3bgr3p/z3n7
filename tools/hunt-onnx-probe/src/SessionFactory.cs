using System;
using Microsoft.ML.OnnxRuntime;

namespace HuntOnnxProbe
{
    public static class SessionFactory
    {
        public static SessionOptions CreateOptions(string mode)
        {
            var options = new SessionOptions();
            if (mode == "single")
            {
                options.IntraOpNumThreads = 1;
                options.InterOpNumThreads = 1;
                options.ExecutionMode = ExecutionMode.ORT_SEQUENTIAL;
            }
            else if (mode != "default")
            {
                options.Dispose();
                throw new ArgumentException("mode must be default or single", nameof(mode));
            }

            return options;
        }
    }
}
