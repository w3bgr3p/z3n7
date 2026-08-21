using System;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace HuntOnnxProbe
{
    public static class TensorFactory
    {
        public static DenseTensor<float> Create()
        {
            return new DenseTensor<float>(new[] { 1, 3, 640, 640 });
        }
    }
}
