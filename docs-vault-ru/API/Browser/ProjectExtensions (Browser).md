---
title: "ProjectExtensions (Browser)"
tags: [api, Browser]
generated: z3n7-docgen
---

# ProjectExtensions (Browser)

`static class` · пространство имён `z3n7` · исходник [Browser/BrowserScan.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L228), [Browser/GpuSpoof.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L425)

```csharp
public static class ProjectExtensions
```

Другие части этого типа: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

Extension methods on `IZennoPosterProjectModel`: WebGL spoofing.

## Методы

### FixTime

```csharp
public static void FixTime(this IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L236)

Runs `BrowserScan.FixTime`; an error is written to the log as a warning and does not stop the project.

| Параметр | Описание |
|---|---|
| `instance` | Browser instance. |
| `log` | Logger for progress. |

### SpoofGpu

```csharp
public static void SpoofGpu(this IZennoPosterProjectModel project, Instance instance)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L434)

Takes a random WebGL profile (Base64 JSON per line) from `{project.Path}/resourses/webgl.txt`, replaces its unmasked vendor and renderer with `RandomAngleString` (GPU list cached in `resourses/gpu.json`), stores it in `webgl` and loads it into the instance. Does nothing when `webgl.txt` is missing.

| Параметр | Описание |
|---|---|
| `instance` | Browser instance. |

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
