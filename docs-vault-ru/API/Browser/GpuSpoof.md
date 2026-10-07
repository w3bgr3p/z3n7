---
title: "GpuSpoof"
tags: [api, Browser]
generated: z3n7-docgen
---

# GpuSpoof

`static class` · пространство имён `z3n7` · исходник [Browser/GpuSpoof.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L51)

```csharp
public static class GpuSpoof
```

Picks a plausible WebGL vendor/renderer pair of the same GPU architecture as the machine's card, from the public PCI ID list.

## Методы

### BuildGpuJson

```csharp
public static string BuildGpuJson(string pciIdsUrl = "https://pci-ids.ucw.cz/v2.2/pci.ids")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L148)

Downloads the PCI ID list and groups the GPU models of NVIDIA, AMD and Intel by vendor and architecture (only GeForce/Quadro/Tesla/RTX, Radeon/Navi/Vega/Polaris and HD Graphics/UHD/Iris/Xe/Arc entries).

| Параметр | Описание |
|---|---|
| `pciIdsUrl` | URL of `pci.ids`. |

**Возвращает:** JSON `[{ Vendor, Archs: [{ Arch, Models: [{ DeviceId, Name, Chip }] }] }]`.

### DetectCurrentArch

```csharp
public static string DetectCurrentArch(string gpuJson, string cardName = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L328)

Architecture of the card in `gpuJson`: the first model of the card's vendor whose name contains the card name or is contained in it.

| Параметр | Описание |
|---|---|
| `gpuJson` | JSON from `BuildGpuJson`. |
| `cardName` | Card name; default is the current card (`GetCurrentCardName`). |

**Возвращает:** Architecture name, or an empty string.

### EnsureGpuJson

```csharp
public static string EnsureGpuJson(string path)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L406)

Reads the GPU JSON from `path`; when the file does not exist, builds it with `BuildGpuJson` and saves it (one builder at a time).

| Параметр | Описание |
|---|---|
| `path` | Cache file. |

### GetCurrentCardName

```csharp
public static string GetCurrentCardName()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L307)

Full name of the machine's video card from WMI (the second card when there are several); empty on error.

### GetCurrentVendor

```csharp
public static string GetCurrentVendor()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L281)

Vendor of the machine's video card from WMI `Win32_VideoController`; with several cards the second one is used.

**Возвращает:** `NVIDIA`, `AMD`, `Intel`, the first word of another name, or an empty string.

### LoadGpuJson

```csharp
public static string LoadGpuJson(string path)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L398)

Reads the GPU JSON from a file.

### RandomAngleString

```csharp
public static string[] RandomAngleString(string gpuJson, string cardName = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L362)

Random model of the same vendor and architecture as the card, formatted as Chrome's ANGLE strings.

| Параметр | Описание |
|---|---|
| `gpuJson` | JSON from `BuildGpuJson`. |
| `cardName` | Card name; default is the current card (`GetCurrentCardName`). |

**Возвращает:** `["Google Inc. (NVIDIA)", "ANGLE (NVIDIA, NVIDIA {model} (0x0000XXXX) Direct3D11 vs_5_0 ps_5_0, D3D11)"]`; two empty strings when the architecture is unknown.

### SaveGpuJson

```csharp
public static void SaveGpuJson(string json, string path)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L394)

Writes the GPU JSON to a file (UTF-8).

