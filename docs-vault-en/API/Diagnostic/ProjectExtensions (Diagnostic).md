---
title: "ProjectExtensions (Diagnostic)"
tags: [api, Diagnostic]
generated: z3n7-docgen
---

# ProjectExtensions (Diagnostic)

`static class` · namespace `z3n7` · source [Diagnostic/Diagnostic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L18)

```csharp
public static class ProjectExtensions
```

Other parts of this type: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

*No description yet.*

## Methods

### CatchErrorFromTraffic

```csharp
public static void CatchErrorFromTraffic(this IZennoPosterProjectModel project, Instance instance, string url, string errField, int sleepBefore = 5000)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L115)

### SaveDebugScreenshot

```csharp
public static void SaveDebugScreenshot(this IZennoPosterProjectModel project, Instance instance, string watermark = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L20)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
