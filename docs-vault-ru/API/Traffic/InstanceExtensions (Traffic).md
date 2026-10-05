---
title: "InstanceExtensions (Traffic)"
tags: [api, Traffic]
generated: z3n7-docgen
---

# InstanceExtensions (Traffic)

`static class` · пространство имён `z3n7` · исходник [Traffic/CdpHar.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L663), [Traffic/Traffic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L239)

```csharp
public static class InstanceExtensions
```

Другие части этого типа: [[InstanceExtensions (Browser)]]

Extension methods on `Instance`: HAR recording over DevTools.

## Методы

### GrabTrafficList

```csharp
public static List<Traffic.TrafficElement> GrabTrafficList(this Instance instance, string url, bool strict = false)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L247)

Returns the requests recorded so far whose URL matches `url`. The same text is used as the traffic filter.

| Параметр | Описание |
|---|---|
| `url` | Text to look for in the URL. |
| `strict` | Require an exact URL match. |

### SaveHar

```csharp
public static int SaveHar(this Instance instance, string path, string urlRegex = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L675)

Writes the traffic recorded since `StartHar` to a HAR file. See `CdpHar.Save`.

| Параметр | Описание |
|---|---|
| `path` | Target file. |
| `urlRegex` | Case-insensitive regex the URL must match; `null` keeps everything. |

**Возвращает:** Number of entries written. Throws when the recorder was not started.

### StartHar

```csharp
public static CdpHar StartHar(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L666)

Starts HAR recording over DevTools for this instance. Call BEFORE the traffic you need.

### StopHar

```csharp
public static void StopHar(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L683)

Stops HAR recording for this instance.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
