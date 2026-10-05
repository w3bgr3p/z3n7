---
title: "Traffic"
tags: [api, Traffic]
generated: z3n7-docgen
---

# Traffic

`class` · пространство имён `z3n7` · исходник [Traffic/Traffic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L14)

```csharp
public class Traffic
```

*Описания пока нет.*

## Конструкторы

### Traffic

```csharp
public Traffic(Instance instance, string defaultFilter = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L19)

## Методы

### Find

```csharp
public TrafficElement Find(string url, bool strict = false, int timeoutSec = 15)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L41)

### FindAll

```csharp
public List<TrafficElement> FindAll(string url, bool strict = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L53)

### GetApiStructure

```csharp
public string GetApiStructure(string urlFilter = "api", bool includeHeaders = false, bool excludeFiles = true)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L58)

### SaveHeadersToVar

```csharp
public void SaveHeadersToVar(string url, string varName = "headers", bool strict = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L95)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
