---
title: "GpuSpoof"
tags: [api, Browser]
generated: z3n7-docgen
---

# GpuSpoof

`static class` · namespace `z3n7` · source [Browser/GpuSpoof.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L37)

```csharp
public static class GpuSpoof
```

*No description yet.*

## Methods

### BuildGpuJson

```csharp
public static string BuildGpuJson(string pciIdsUrl = "https://pci-ids.ucw.cz/v2.2/pci.ids")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L132)

Скачивает pci.ids и возвращает JSON вида: [ { Vendor, Archs: [ { Arch, Models: [ { DeviceId, Name, Chip } ] } ] } ] Фильтр: только GPU-строки (GeForce / Radeon / UHD / Iris / Xe / Arc)

### DetectCurrentArch

```csharp
public static string DetectCurrentArch(string gpuJson, string cardName = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L305)

По имени карты (из Win32) ищет в gpuJson её архитектуру. Поиск: частичное вхождение modelName в Name записи JSON.

### EnsureGpuJson

```csharp
public static string EnsureGpuJson(string path)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L377)

Если файл не существует — скачивает и создаёт, с локом на случай параллельных потоков. Возвращает содержимое JSON.

### GetCurrentCardName

```csharp
public static string GetCurrentCardName()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L287)

Возвращает полное имя текущей карты из Win32_VideoController

### GetCurrentVendor

```csharp
public static string GetCurrentVendor()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L262)

Возвращает "NVIDIA" / "AMD" / "Intel" / "" Использует Win32_VideoController, предпочитает дискретную (cards[1] если есть).

### LoadGpuJson

```csharp
public static string LoadGpuJson(string path)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L370)

### RandomAngleString

```csharp
public static string[] RandomAngleString(string gpuJson, string cardName = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L336)

Возвращает массив из двух строк: [0] "Google Inc. (NVIDIA)" [1] "ANGLE (NVIDIA, NVIDIA GeForce RTX 3060 Ti (0x00002486) Direct3D11 vs_5_0 ps_5_0, D3D11)" для той же архитектуры что у текущей карты.

### SaveGpuJson

```csharp
public static void SaveGpuJson(string json, string path)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L367)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
