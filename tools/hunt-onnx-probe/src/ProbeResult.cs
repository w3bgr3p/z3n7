using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HuntOnnxProbe
{
    [DataContract]
    public sealed class ProbeReport
    {
        [DataMember(Order = 1)] public string Machine { get; set; }
        [DataMember(Order = 2)] public string Os { get; set; }
        [DataMember(Order = 3)] public string Clr { get; set; }
        [DataMember(Order = 4)] public bool Process64Bit { get; set; }
        [DataMember(Order = 5)] public int ProcessorCount { get; set; }
        [DataMember(Order = 6)] public string ProcessorIdentifier { get; set; }
        [DataMember(Order = 7)] public string ModelPath { get; set; }
        [DataMember(Order = 8)] public string ModelSha256 { get; set; }
        [DataMember(Order = 9)] public List<WorkerResult> Results { get; set; }
    }

    [DataContract]
    public sealed class WorkerResult
    {
        [DataMember(Order = 1)] public string Mode { get; set; }
        [DataMember(Order = 2)] public string Status { get; set; }
        [DataMember(Order = 3)] public long SessionMilliseconds { get; set; }
        [DataMember(Order = 4)] public long RunMilliseconds { get; set; }
        [DataMember(Order = 5)] public string InputName { get; set; }
        [DataMember(Order = 6)] public string OutputDimensions { get; set; }
        [DataMember(Order = 7)] public string ManagedAssembly { get; set; }
        [DataMember(Order = 8)] public string NativeLibrary { get; set; }
        [DataMember(Order = 9)] public string NativeSha256 { get; set; }
        [DataMember(Order = 10)] public string Error { get; set; }
        [DataMember(Order = 11)] public string Progress { get; set; }
    }
}
