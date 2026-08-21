using System;
using System.Linq;

namespace HuntOnnxProbe
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                var options = ProbeOptions.Parse(args);
                if (args.Contains("--worker"))
                {
                    var result = ProbeWorker.Run(options.ModelPath, options.Mode);
                    Console.Out.Write(JsonOutput.Serialize(result));
                    return result.Status == "success" ? 0 : 2;
                }

                var report = ProbeCoordinator.Run(options);
                Console.Out.Write(JsonOutput.Serialize(report));
                return report.Results.All(result => result.Status == "success")
                    ? 0
                    : 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return 1;
            }
        }
    }
}
