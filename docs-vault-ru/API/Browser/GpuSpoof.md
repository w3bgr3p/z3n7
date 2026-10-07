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

Подбирает правдоподобную пару WebGL vendor/renderer той же архитектуры GPU, что и карта машины, по публичному списку PCI ID.

## Методы

### BuildGpuJson

```csharp
public static string BuildGpuJson(string pciIdsUrl = "https://pci-ids.ucw.cz/v2.2/pci.ids")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L148)

Скачивает список PCI ID и группирует модели GPU NVIDIA, AMD и Intel по производителю и архитектуре (только записи GeForce/Quadro/Tesla/RTX, Radeon/Navi/Vega/Polaris и HD Graphics/UHD/Iris/Xe/Arc).

| Параметр | Описание |
|---|---|
| `pciIdsUrl` | URL файла `pci.ids`. |

**Возвращает:** JSON `[{ Vendor, Archs: [{ Arch, Models: [{ DeviceId, Name, Chip }] }] }]`.

### DetectCurrentArch

```csharp
public static string DetectCurrentArch(string gpuJson, string cardName = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L328)

Архитектура карты из `gpuJson`: первая модель производителя карты, название которой содержит название карты или содержится в нём.

| Параметр | Описание |
|---|---|
| `gpuJson` | JSON из `BuildGpuJson`. |
| `cardName` | Название карты; по умолчанию текущая карта (`GetCurrentCardName`). |

**Возвращает:** Название архитектуры или пустая строка.

### EnsureGpuJson

```csharp
public static string EnsureGpuJson(string path)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L406)

Читает JSON с GPU из `path`; если файла нет, строит его через `BuildGpuJson` и сохраняет (строит одновременно только один).

| Параметр | Описание |
|---|---|
| `path` | Файл кеша. |

### GetCurrentCardName

```csharp
public static string GetCurrentCardName()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L307)

Полное название видеокарты машины из WMI (вторая карта, если их несколько); пусто при ошибке.

### GetCurrentVendor

```csharp
public static string GetCurrentVendor()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L281)

Производитель видеокарты машины из WMI `Win32_VideoController`; если карт несколько, берётся вторая.

**Возвращает:** `NVIDIA`, `AMD`, `Intel`, первое слово другого названия или пустая строка.

### LoadGpuJson

```csharp
public static string LoadGpuJson(string path)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L398)

Читает JSON с GPU из файла.

### RandomAngleString

```csharp
public static string[] RandomAngleString(string gpuJson, string cardName = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L362)

Случайная модель того же производителя и архитектуры, что и карта, в формате строк ANGLE из Chrome.

| Параметр | Описание |
|---|---|
| `gpuJson` | JSON из `BuildGpuJson`. |
| `cardName` | Название карты; по умолчанию текущая карта (`GetCurrentCardName`). |

**Возвращает:** `["Google Inc. (NVIDIA)", "ANGLE (NVIDIA, NVIDIA {model} (0x0000XXXX) Direct3D11 vs_5_0 ps_5_0, D3D11)"]`; две пустые строки, если архитектура неизвестна.

### SaveGpuJson

```csharp
public static void SaveGpuJson(string json, string path)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L394)

Записывает JSON с GPU в файл (UTF-8).

