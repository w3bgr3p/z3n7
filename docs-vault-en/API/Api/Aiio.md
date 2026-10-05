---
title: "Aiio"
tags: [api, Api]
generated: z3n7-docgen
---

# Aiio

`class` · namespace `z3n7.Api` · source [Api/Aiio.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L13)

```csharp
public sealed class Aiio
```

*No description yet.*

## Constructors

### Aiio

```csharp
public Aiio(IZennoPosterProjectModel project)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L23)

## Methods

### Complete

```csharp
public string Complete(string model, string systemPrompt, string userPrompt, double temperature = 0.8, int maxTokens = 800, int timeoutSec = 90)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L28)

### CompleteAsync

```csharp
public async Task<string> CompleteAsync(string model, string systemPrompt, string userPrompt, double temperature = 0.8, int maxTokens = 800, int timeoutSec = 90)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L41)

### GetModels

```csharp
public List<string> GetModels()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L80)

### GetModelsAsync

```csharp
public async Task<List<string>> GetModelsAsync()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L85)

### HasKey

```csharp
public bool HasKey()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L119)

### InvalidateModelsCache

```csharp
public static void InvalidateModelsCache()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L124)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
