---
title: "ProjectExtensions (Diagnostic)"
tags: [api, Diagnostic]
generated: z3n7-docgen
---

# ProjectExtensions (Diagnostic)

`static class` · пространство имён `z3n7` · исходник [Diagnostic/Diagnostic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L18)

```csharp
public static class ProjectExtensions
```

Другие части этого типа: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

*Описания пока нет.*

## Методы

### CatchErrorFromTraffic

```csharp
public static void CatchErrorFromTraffic(this IZennoPosterProjectModel project, Instance instance, string url, string errField, int sleepBefore = 5000)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L115)

### SaveDebugScreenshot

```csharp
public static void SaveDebugScreenshot(this IZennoPosterProjectModel project, Instance instance, string watermark = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L20)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
