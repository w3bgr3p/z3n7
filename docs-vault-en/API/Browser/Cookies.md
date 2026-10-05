---
title: "Cookies"
tags: [api, Browser]
generated: z3n7-docgen
---

# Cookies

`static class` · namespace `z3n7` · source [Browser/Cookies.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L15)

```csharp
public static class Cookies
```

*No description yet.*

## Methods

### AnalyzeCookies

```csharp
public static CookieInfo AnalyzeCookies(this IZennoPosterProjectModel project, string table = "_instance", string column = "cookies")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L28)

### CleanDomainInDb

```csharp
public static void CleanDomainInDb(this IZennoPosterProjectModel project, string domain, string table = "_instance", string column = "cookies")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L415)

Очистить cookies для конкретного домена в БД

| Parameter | Description |
|---|---|
| `project` | Project model |
| `domain` | Домен для очистки (например, "x.com" или "twitter.com") |
| `table` | Таблица БД (по умолчанию "_instance") |
| `column` | Колонка с cookies (по умолчанию "cookies") |

### ConvertCookieFormat

```csharp
public static string ConvertCookieFormat(string input, string output = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L123)

### GetCookies

```csharp
public static string GetCookies(this Instance instance, string domainFilter = null, string format = "json")
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L264)

### GetCookiesByJs

```csharp
public static string GetCookiesByJs(this Instance instance)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L326)

### ParseJwt

```csharp
public static Dictionary<string, object> ParseJwt(string jwt)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L512)

### PrintCookieReport

```csharp
public static void PrintCookieReport(this IZennoPosterProjectModel project, string table = "_instance", string column = "cookies")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L98)

### PruneAllCookies

```csharp
public static void PruneAllCookies(this IZennoPosterProjectModel project, bool removeExpired = true, bool removeOld = true, bool removeNonGoogle = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L85)

### PruneCookies

```csharp
public static void PruneCookies(this IZennoPosterProjectModel project, bool removeExpired = true, bool removeOld = true, bool removeNonGoogle = false, string table = "_instance", string column = "cookies")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L53)

### SaveAllCookies

```csharp
public static void SaveAllCookies(this IZennoPosterProjectModel project, Instance instance, string jsonPath = null, string table = "_instance", bool saveJsonToDb = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L283)

### SaveDomainCookies

```csharp
public static void SaveDomainCookies(this IZennoPosterProjectModel project, Instance instance, string domain = null, string jsonPath = null, string tableName = "_instance", bool saveJsonToDb = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L294)

### SetCookiesByJs

```csharp
public static void SetCookiesByJs(this Instance instance, string cookiesJson)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L354)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
