---
title: "InstanceExtensions (Traffic)"
tags: [api, Traffic]
generated: z3n7-docgen
---

# InstanceExtensions (Traffic)

`static class` · namespace `z3n7` · source [Traffic/CdpHar.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L639), [Traffic/Traffic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L192)

```csharp
public static class InstanceExtensions
```

Other parts of this type: [[InstanceExtensions (Browser)]]

*No description yet.*

## Methods

### GrabTrafficList

```csharp
public static List<Traffic.TrafficElement> GrabTrafficList(this Instance instance, string url, bool strict = false)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L194)

### SaveHar

```csharp
public static int SaveHar(this Instance instance, string path, string urlRegex = null)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L647)

### StartHar

```csharp
public static CdpHar StartHar(this Instance instance)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L642)

Starts HAR recording over DevTools for this instance. Call BEFORE the traffic you need.

### StopHar

```csharp
public static void StopHar(this Instance instance)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L654)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
