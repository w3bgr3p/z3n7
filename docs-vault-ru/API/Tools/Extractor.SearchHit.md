---
title: "Extractor.SearchHit"
tags: [api, Tools]
generated: z3n7-docgen
---

# Extractor.SearchHit

`class` · пространство имён `z3n7.Tools` · исходник [Tools/Extractor.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L215)

```csharp
public class SearchHit
```

One match of `SearchInZp`.

## Методы

### ToString

```csharp
public override string ToString()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L224)

`ZpPath`, `StepId` and `Context`, tab-separated.

## Поля

### Context

```csharp
public string Context;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L222)

The match with surrounding text; line breaks collapsed to spaces.

### StepId

```csharp
public string StepId;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L220)

Id of the action (step) containing the match.

### ZpPath

```csharp
public string ZpPath;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L218)

Project file.

