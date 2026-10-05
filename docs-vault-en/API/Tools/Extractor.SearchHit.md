---
title: "Extractor.SearchHit"
tags: [api, Tools]
generated: z3n7-docgen
---

# Extractor.SearchHit

`class` · namespace `z3n7.Tools` · source [Tools/Extractor.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L215)

```csharp
public class SearchHit
```

One match of `SearchInZp`.

## Methods

### ToString

```csharp
public override string ToString()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L224)

`ZpPath`, `StepId` and `Context`, tab-separated.

## Fields

### Context

```csharp
public string Context;
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L222)

The match with surrounding text; line breaks collapsed to spaces.

### StepId

```csharp
public string StepId;
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L220)

Id of the action (step) containing the match.

### ZpPath

```csharp
public string ZpPath;
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L218)

Project file.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
