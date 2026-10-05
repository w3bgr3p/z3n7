---
title: "ProjectExtensions (Essentials)"
tags: [api, Essentials]
generated: z3n7-docgen
---

# ProjectExtensions (Essentials)

`static class` · пространство имён `z3n7` · исходник [Essentials/ExternalCode.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/ExternalCode.cs#L9), [Essentials/Init.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Init.cs#L113), [Essentials/Logger.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L213), [Essentials/Time.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L147)

```csharp
public static class ProjectExtensions
```

Другие части этого типа: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

*Описания пока нет.*

## Методы

### Age

```csharp
public static T Age<T>(this IZennoPosterProjectModel project, string var = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L158)

### Deadline

```csharp
public static int Deadline(this IZennoPosterProjectModel project, int sec = 0, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L204)

### InitVariables

```csharp
public static void InitVariables(this IZennoPosterProjectModel project, Instance instance, string author = "w3bgr3p")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Init.cs#L117)

### log

```csharp
public static void log(this IZennoPosterProjectModel project, object toLog, [CallerMemberName] string caller = "", bool show = true, bool toZp = true)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L215)

### RunZp

```csharp
public static bool RunZp(this IZennoPosterProjectModel project, List<string> vars = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/ExternalCode.cs#L11)

```csharp
public static bool RunZp(this IZennoPosterProjectModel project, string path)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/ExternalCode.cs#L37)

### StartSession

```csharp
public static void StartSession(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L224)

### TimeElapsed

```csharp
public static int TimeElapsed(this IZennoPosterProjectModel project, string varName = "varSessionId")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L149)

### TimeOut

```csharp
public static void TimeOut(this IZennoPosterProjectModel project, int min = 0)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L192)

### warn

```csharp
public static void warn(this IZennoPosterProjectModel project, string msg, bool thrw = false, bool show = true, [CallerMemberName] string caller = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L227)

```csharp
public static void warn(this IZennoPosterProjectModel project, Exception ex, bool thrw = false, bool withStack = false, bool toZp = true, [CallerMemberName] string caller = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L241)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
