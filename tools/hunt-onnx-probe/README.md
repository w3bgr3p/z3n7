# Hunt ONNX Probe

Standalone `net48` diagnostic for `hunt-shape.onnx`. It does not reference
ZennoLab assemblies. Each ONNX mode runs in a child process so a native hang can
be terminated by the configured timeout.

## Build

```powershell
rtk dotnet build .\src\HuntOnnxProbe.csproj -c Release
```

Copy the complete `src\bin\Release\net48` directory to the target machine.
The directory includes the managed assemblies and native `onnxruntime.dll`.

## Run

```powershell
.\hunt-onnx-probe.exe `
  --model F:\onnx\hunt-shape.onnx `
  --mode both `
  --timeout-seconds 120
```

Arguments:

- `--model PATH` — required ONNX model path.
- `--mode default|single|both` — default is `both`.
- `--timeout-seconds N` — per-mode timeout; default is `120`.

`default` uses ordinary ONNX Runtime settings. `single` uses one intra-op
thread, one inter-op thread, and `ORT_SEQUENTIAL`.

The program writes one JSON document to stdout. Progress and errors go to
stderr. Exit code `0` means every requested mode completed; `2` means a mode
failed or timed out; `1` means invalid arguments or startup failure.

## Tests

```powershell
rtk dotnet run --project .\tests\HuntOnnxProbe.Tests.csproj
```
