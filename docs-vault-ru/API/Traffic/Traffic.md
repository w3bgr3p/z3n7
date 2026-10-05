---
title: "Traffic"
tags: [api, Traffic]
generated: z3n7-docgen
---

# Traffic

`class` · пространство имён `z3n7` · исходник [Traffic/Traffic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L19)

```csharp
public class Traffic
```

Reads the traffic recorded by the active tab of a ZennoPoster instance (`ActiveTab.GetTraffic`). `OPTIONS` requests are skipped; gzip response bodies are decompressed.

## Конструкторы

### Traffic

```csharp
public Traffic(Instance instance, string defaultFilter = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L26)

Turns on traffic monitoring for the instance.

| Параметр | Описание |
|---|---|
| `defaultFilter` | Filter passed to `GetTraffic`; default is the active tab's domain. |

## Методы

### Find

```csharp
public TrafficElement Find(string url, bool strict = false, int timeoutSec = 15)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L53)

Waits for a request whose URL matches, polling once a second.

| Параметр | Описание |
|---|---|
| `url` | Text to look for in the URL, or the whole URL with `strict`. |
| `strict` | Require an exact URL match. |
| `timeoutSec` | How long to wait. |

**Возвращает:** The first matching request. Throws `TimeoutException` when none appears in time.

### FindAll

```csharp
public List<TrafficElement> FindAll(string url, bool strict = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L68)

Returns all requests recorded so far whose URL matches.

| Параметр | Описание |
|---|---|
| `url` | Text to look for in the URL, or the whole URL with `strict`. |
| `strict` | Require an exact URL match. |

### GetApiStructure

```csharp
public string GetApiStructure(string urlFilter = "api", bool includeHeaders = false, bool excludeFiles = true)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L81)

Summarises the recorded API calls as indented JSON: `total` and one array per method (`getEndpoints`, `postEndpoints`, …). Each method + URL pair appears once; bodies are included, parsed as JSON when possible.

| Параметр | Описание |
|---|---|
| `urlFilter` | Text the URL must contain. |
| `includeHeaders` | Add request and response headers. |
| `excludeFiles` | Skip URLs whose last path segment has a file extension. |

### SaveHeadersToVar

```csharp
public void SaveHeadersToVar(string url, string varName = "headers", bool strict = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L129)

Waits for a request (see `Find`) and collects its request headers without `:`-pseudo-headers.

| Параметр | Описание |
|---|---|
| `url` | URL filter. |
| `varName` | Not used. |
| `strict` | Require an exact URL match. |

**Примечания:** The collected headers are not stored anywhere: the line that wrote them to a variable is commented out.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
