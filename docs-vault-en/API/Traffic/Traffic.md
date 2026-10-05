---
title: "Traffic"
tags: [api, Traffic]
generated: z3n7-docgen
---

# Traffic

`class` · namespace `z3n7` · source [Traffic/Traffic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L14)

```csharp
public class Traffic
```

*No description yet.*

## Constructors

### Traffic

```csharp
public Traffic(Instance instance, string defaultFilter = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L19)

## Methods

### Find

```csharp
public TrafficElement Find(string url, bool strict = false, int timeoutSec = 15)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L41)

### FindAll

```csharp
public List<TrafficElement> FindAll(string url, bool strict = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L53)

### GetApiStructure

```csharp
public string GetApiStructure(string urlFilter = "api", bool includeHeaders = false, bool excludeFiles = true)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L58)

### SaveHeadersToVar

```csharp
public void SaveHeadersToVar(string url, string varName = "headers", bool strict = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L95)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
