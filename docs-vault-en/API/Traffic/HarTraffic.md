---
title: "HarTraffic"
tags: [api, Traffic]
generated: z3n7-docgen
---

# HarTraffic

`static class` · namespace `z3n7` · source [Traffic/Har.Rqst.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.Rqst.cs#L11), [Traffic/Har.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.cs#L18)

```csharp
public static class HarTraffic
```

Standalone HAR exporter for a ZennoPoster C# action.

## Methods

### ExportRqst

```csharp
public static string ExportRqst(IZennoPosterProjectModel project, string projectFilter = null, string taskId = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.Rqst.cs#L14)

Exports all complete saved Rqst transactions as a HAR 1.2 JSON document.

### FromRqstJsonl

```csharp
public static string FromRqstJsonl(string path, string projectFilter = null, string taskId = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.Rqst.cs#L17)

### Save

```csharp
public static int Save(Instance instance, string path, string filter = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.cs#L20)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
