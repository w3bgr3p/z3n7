---
title: "NetHttp"
tags: [api, Requests]
generated: z3n7-docgen
---

# NetHttp

`class` · пространство имён `z3n7` · исходник [Requests/NetHttp.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L597)

```csharp
public class NetHttp
```

СИНХРОННЫЕ ОБЕРТКИ для ZennoPoster Project (не поддерживает async) ⚠️ ВНИМАНИЕ: Используй NetHttpAsync если можешь работать с async/await Этот класс - только адаптер для legacy кода

## Конструкторы

### NetHttp

```csharp
public NetHttp(IZennoPosterProjectModel project, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L601)

## Методы

### DELETE

```csharp
public string DELETE(string url, string proxyString = "", Dictionary<string, string> headers = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L669)

Синхронная обертка для DELETE (для ZennoPoster) ⚠️ Блокирует поток! Используй NetHttpAsync.DeleteAsync() если возможно

### GET

```csharp
public string GET(string url, string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L610)

Синхронная обертка для GET (для ZennoPoster) ⚠️ Блокирует поток! Используй NetHttpAsync.GetAsync() если возможно

### POST

```csharp
public string POST(string url, string body, string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L629)

Синхронная обертка для POST (для ZennoPoster) ⚠️ Блокирует поток! Используй NetHttpAsync.PostAsync() если возможно

### PUT

```csharp
public string PUT(string url, string body = "", string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L649)

Синхронная обертка для PUT (для ZennoPoster) ⚠️ Блокирует поток! Используй NetHttpAsync.PutAsync() если возможно

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
