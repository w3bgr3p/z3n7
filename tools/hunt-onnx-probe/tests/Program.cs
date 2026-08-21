using System;
using HuntOnnxProbe;
using Microsoft.ML.OnnxRuntime;

internal static class Program
{
    private static int Main()
    {
        try
        {
            ParseOptions();
            ExpandModes();
            CreateTensor();
            CreateSingleThreadOptions();
            Console.WriteLine("PASS");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void ExpandModes()
    {
        var options = ProbeOptions.Parse(new[] { "--model", "model.onnx" });
        Equal(2, options.Modes.Count, "mode count");
        Equal("default", options.Modes[0], "first mode");
        Equal("single", options.Modes[1], "second mode");
    }

    private static void ParseOptions()
    {
        var options = ProbeOptions.Parse(new[]
        {
            "--model", "model.onnx",
            "--mode", "single",
            "--timeout-seconds", "17"
        });

        Equal("model.onnx", options.ModelPath, "model path");
        Equal("single", options.Mode, "mode");
        Equal(17, options.TimeoutSeconds, "timeout");
    }

    private static void CreateTensor()
    {
        var tensor = TensorFactory.Create();
        Equal(4, tensor.Dimensions.Length, "rank");
        Equal(1, tensor.Dimensions[0], "batch");
        Equal(3, tensor.Dimensions[1], "channels");
        Equal(640, tensor.Dimensions[2], "height");
        Equal(640, tensor.Dimensions[3], "width");
    }

    private static void CreateSingleThreadOptions()
    {
        using (var options = SessionFactory.CreateOptions("single"))
        {
            Equal(1, options.IntraOpNumThreads, "intra-op threads");
            Equal(1, options.InterOpNumThreads, "inter-op threads");
            Equal(ExecutionMode.ORT_SEQUENTIAL, options.ExecutionMode, "execution mode");
        }
    }

    private static void Equal<T>(T expected, T actual, string name)
    {
        if (!Equals(expected, actual))
            throw new Exception(name + ": expected " + expected + ", got " + actual);
    }
}
