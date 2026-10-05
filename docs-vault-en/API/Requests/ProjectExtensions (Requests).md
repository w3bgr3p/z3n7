---
title: "ProjectExtensions (Requests)"
tags: [api, Requests]
generated: z3n7-docgen
---

# ProjectExtensions (Requests)

`static class` · namespace `z3n7` · source [Requests/NetHttp.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L686)

```csharp
public static class ProjectExtensions
```

Other parts of this type: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Traffic)]]

Extension методы для удобного вызова из Project Остаются синхронными для совместимости с ZennoPoster

## Methods

### NetGet

```csharp
public static string NetGet(this IZennoPosterProjectModel project, string url, string proxyString = "", string[] headers = null, bool parse = false, int deadline = 15, bool thrw = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L723)

Extension метод для GET из ZennoPoster Project

### NetPost

```csharp
public static string NetPost(this IZennoPosterProjectModel project, string url, string body, string proxyString = "", string[] headers = null, bool parse = false, int deadline = 15, bool thrw = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L737)

Extension метод для POST из ZennoPoster Project

> This page is generated from the source code. Do not edit it: changes will be overwritten.
