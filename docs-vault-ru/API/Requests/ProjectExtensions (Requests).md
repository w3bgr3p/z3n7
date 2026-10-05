---
title: "ProjectExtensions (Requests)"
tags: [api, Requests]
generated: z3n7-docgen
---

# ProjectExtensions (Requests)

`static class` · пространство имён `z3n7` · исходник [Requests/NetHttp.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L686)

```csharp
public static class ProjectExtensions
```

Другие части этого типа: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Traffic)]]

Extension методы для удобного вызова из Project Остаются синхронными для совместимости с ZennoPoster

## Методы

### NetGet

```csharp
public static string NetGet(this IZennoPosterProjectModel project, string url, string proxyString = "", string[] headers = null, bool parse = false, int deadline = 15, bool thrw = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L723)

Extension метод для GET из ZennoPoster Project

### NetPost

```csharp
public static string NetPost(this IZennoPosterProjectModel project, string url, string body, string proxyString = "", string[] headers = null, bool parse = false, int deadline = 15, bool thrw = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L737)

Extension метод для POST из ZennoPoster Project

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
