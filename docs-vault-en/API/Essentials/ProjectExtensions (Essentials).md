---
title: "ProjectExtensions (Essentials)"
tags: [api, Essentials]
generated: z3n7-docgen
---

# ProjectExtensions (Essentials)

`static class` · namespace `z3n7` · source [Essentials/ExternalCode.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/ExternalCode.cs#L9), [Essentials/Init.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Init.cs#L113), [Essentials/Logger.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L213), [Essentials/Time.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L147)

```csharp
public static class ProjectExtensions
```

Other parts of this type: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

*No description yet.*

## Methods

### Age

```csharp
public static T Age<T>(this IZennoPosterProjectModel project, string var = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L158)

### Deadline

```csharp
public static int Deadline(this IZennoPosterProjectModel project, int sec = 0, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L204)

### InitVariables

```csharp
public static void InitVariables(this IZennoPosterProjectModel project, Instance instance, string author = "w3bgr3p")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Init.cs#L117)

### log

```csharp
public static void log(this IZennoPosterProjectModel project, object toLog, [CallerMemberName] string caller = "", bool show = true, bool toZp = true)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L215)

### RunZp

```csharp
public static bool RunZp(this IZennoPosterProjectModel project, List<string> vars = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/ExternalCode.cs#L11)

```csharp
public static bool RunZp(this IZennoPosterProjectModel project, string path)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/ExternalCode.cs#L37)

### StartSession

```csharp
public static void StartSession(this IZennoPosterProjectModel project)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L224)

### TimeElapsed

```csharp
public static int TimeElapsed(this IZennoPosterProjectModel project, string varName = "varSessionId")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L149)

### TimeOut

```csharp
public static void TimeOut(this IZennoPosterProjectModel project, int min = 0)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L192)

### warn

```csharp
public static void warn(this IZennoPosterProjectModel project, string msg, bool thrw = false, bool show = true, [CallerMemberName] string caller = "")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L227)

```csharp
public static void warn(this IZennoPosterProjectModel project, Exception ex, bool thrw = false, bool withStack = false, bool toZp = true, [CallerMemberName] string caller = "")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L241)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
