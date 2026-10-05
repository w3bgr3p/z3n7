---
title: "Extension"
tags: [api, Browser]
generated: z3n7-docgen
---

# Extension

`class` · namespace `z3n7` · source [Browser/ChromeExt.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L16)

```csharp
public class Extension
```

*No description yet.*

## Constructors

### Extension

```csharp
public Extension(IZennoPosterProjectModel project, Logger log = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L28)

```csharp
public Extension(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L35)

## Methods

### GetVer

```csharp
public string GetVer(string extId)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L42)

### InstallFromCrx

```csharp
public bool InstallFromCrx(string extId, string fileName, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L120)

### InstallFromStore

```csharp
public bool InstallFromStore(string url, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L78)

### Rm

```csharp
public void Rm(string[] ExtToRemove)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L216)

### Switch

```csharp
public bool Switch(string toUse = "", bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L146)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
