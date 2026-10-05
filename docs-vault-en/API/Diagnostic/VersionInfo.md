---
title: "VersionInfo"
tags: [api, Diagnostic]
generated: z3n7-docgen
---

# VersionInfo

`class` · namespace `z3n7` · source [Diagnostic/Diagnostic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L165)

```csharp
public sealed class VersionInfo
```

Versions of the node's environment. An empty string means the value could not be read.

## Properties

### framework

```csharp
public string framework { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L176)

.NET runtime description.

### machine

```csharp
public string machine { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L178)

Machine name.

### process

```csharp
public string process { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L174)

File name of the host process.

### product

```csharp
public string product { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L172)

Product name of the host process.

### z3n7

```csharp
public string z3n7 { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L168)

Version of `z3n7.dll`.

### zennoposter

```csharp
public string zennoposter { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L170)

Product version of the host process (ZennoPoster).

> This page is generated from the source code. Do not edit it: changes will be overwritten.
