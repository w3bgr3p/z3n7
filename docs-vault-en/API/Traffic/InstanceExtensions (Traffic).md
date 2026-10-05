---
title: "InstanceExtensions (Traffic)"
tags: [api, Traffic]
generated: z3n7-docgen
---

# InstanceExtensions (Traffic)

`static class` · namespace `z3n7` · source [Traffic/CdpHar.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L663), [Traffic/Traffic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L239)

```csharp
public static class InstanceExtensions
```

Other parts of this type: [[InstanceExtensions (Browser)]]

Extension methods on `Instance`: HAR recording over DevTools.

## Methods

### GrabTrafficList

```csharp
public static List<Traffic.TrafficElement> GrabTrafficList(this Instance instance, string url, bool strict = false)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L247)

Returns the requests recorded so far whose URL matches `url`. The same text is used as the traffic filter.

| Parameter | Description |
|---|---|
| `url` | Text to look for in the URL. |
| `strict` | Require an exact URL match. |

### SaveHar

```csharp
public static int SaveHar(this Instance instance, string path, string urlRegex = null)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L675)

Writes the traffic recorded since `StartHar` to a HAR file. See `CdpHar.Save`.

| Parameter | Description |
|---|---|
| `path` | Target file. |
| `urlRegex` | Case-insensitive regex the URL must match; `null` keeps everything. |

**Returns:** Number of entries written. Throws when the recorder was not started.

### StartHar

```csharp
public static CdpHar StartHar(this Instance instance)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L666)

Starts HAR recording over DevTools for this instance. Call BEFORE the traffic you need.

### StopHar

```csharp
public static void StopHar(this Instance instance)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L683)

Stops HAR recording for this instance.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
