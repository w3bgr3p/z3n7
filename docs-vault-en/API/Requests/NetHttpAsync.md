---
title: "NetHttpAsync"
tags: [api, Requests]
generated: z3n7-docgen
---

# NetHttpAsync

`class` · namespace `z3n7` · source [Requests/NetHttp.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L22)

```csharp
public class NetHttpAsync
```

ИСПРАВЛЕНО: Основной класс для HTTP запросов с ASYNC методами ✅ Использует singleton HttpClient для предотвращения socket exhaustion ✅ Кеширует клиенты с proxy для переиспользования

## Constructors

### NetHttpAsync

```csharp
public NetHttpAsync(IZennoPosterProjectModel project, Logger log = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L41)

## Methods

### ClearProxyCache

```csharp
public static void ClearProxyCache()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L575)

### DeleteAsync

```csharp
public async Task<string> DeleteAsync(string url, string proxyString = "", Dictionary<string, string> headers = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L472)

### GetAsync

```csharp
public async Task<string> GetAsync(string url, string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L155)

### PostAsync

```csharp
public async Task<string> PostAsync(string url, string body, string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L273)

### PutAsync

```csharp
public async Task<string> PutAsync(string url, string body = "", string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L368)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
