---
title: "ProjectExtensions (Accounts)"
tags: [api, Accounts]
generated: z3n7-docgen
---

# ProjectExtensions (Accounts)

`static class` · пространство имён `z3n7` · исходник [Accounts/InstanceManager.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L518)

```csharp
public static class ProjectExtensions
```

Другие части этого типа: [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

*Описания пока нет.*

## Методы

### Finish

```csharp
public static void Finish(this IZennoPosterProjectModel project, Instance instance)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L534)

### ProxySet

```csharp
public static bool ProxySet(this IZennoPosterProjectModel project, Instance instance, string proxyString = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L551)

### ReportError

```csharp
public static string ReportError(this IZennoPosterProjectModel project, Instance instance, bool toLog = true, bool toTelegram = false, bool toDb = false, bool screenshot = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L539)

### ReportSuccess

```csharp
public static string ReportSuccess(this IZennoPosterProjectModel project, Instance instance, bool toLog = true, bool toTelegram = false, bool toDb = false, string customMessage = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L545)

### RunBrowser

```csharp
public static void RunBrowser(this IZennoPosterProjectModel project, Instance instance, string browserToLaunch = "Chromium", bool debug = false, bool fixTimezone = false, bool useLegacy = true, bool useZpprofile = false, bool useFolder = true)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L520)

### SaveProfile

```csharp
public static void SaveProfile(this IZennoPosterProjectModel project, Instance instance)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L606)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
