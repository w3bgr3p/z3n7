---
title: "HarTraffic"
tags: [api, Traffic]
generated: z3n7-docgen
---

# HarTraffic

`static class` · пространство имён `z3n7` · исходник [Traffic/Har.Rqst.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.Rqst.cs#L11), [Traffic/Har.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.cs#L16)

```csharp
public static class HarTraffic
```

HAR 1.2 export of browser traffic (`GetTraffic`) and of saved `Rqst` traffic.

## Методы

### ExportRqst

```csharp
public static string ExportRqst(IZennoPosterProjectModel project, string projectFilter = null, string taskId = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.Rqst.cs#L14)

Exports all complete saved Rqst transactions as a HAR 1.2 JSON document.

### FromRqstJsonl

```csharp
public static string FromRqstJsonl(string path, string projectFilter = null, string taskId = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.Rqst.cs#L25)

Converts the `Rqst` traffic file at `path` to a HAR 1.2 JSON document. Only what `Rqst` recorded is available: timings hold the total duration only, binary bodies are text.

| Параметр | Описание |
|---|---|
| `path` | Traffic file written by `ZpTraffic`. |
| `projectFilter` | Keep only records of this project. |
| `taskId` | Keep only records of this task. |

### Save

```csharp
public static int Save(Instance instance, string path, string filter = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.cs#L28)

Writes the active tab's traffic to a HAR 1.2 file, replacing an existing file. Response bodies are decoded (gzip, deflate; brotli and zstd when Brotli.Core or ZstdNet are loaded); text types are stored as text, others as Base64. Items that cannot be converted are skipped. Timings and start times are approximate: ZennoPoster gives only the total time.

| Параметр | Описание |
|---|---|
| `instance` | Browser instance. |
| `path` | Target file; must end with `.har`. Missing folders are created. |
| `filter` | URL filter for `GetTraffic`; default is the active tab's domain. |

**Возвращает:** Number of entries written.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
