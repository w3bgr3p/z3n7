---
title: "OmniRoute"
tags: [api, Api]
generated: z3n7-docgen
---

# OmniRoute

`class` · namespace `z3n7.Api` · source [Api/OmniRoute.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L14)

```csharp
public sealed class OmniRoute
```

*No description yet.*

## Constructors

### OmniRoute

```csharp
public OmniRoute()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L22)

## Methods

### Check

```csharp
public bool Check()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L166)

### CheckAsync

```csharp
public async Task<bool> CheckAsync()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L171)

### Complete

```csharp
public string Complete(string model, string systemPrompt, string userPrompt, double temperature = 0.3, int maxTokens = 800, int timeoutSec = 90)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L26)

### CompleteAsync

```csharp
public async Task<string> CompleteAsync(string model, string systemPrompt, string userPrompt, double temperature = 0.3, int maxTokens = 800, int timeoutSec = 90)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L39)

### CompleteVision

```csharp
public string CompleteVision(string model, string systemPrompt, string userPrompt, IList<string> imagesBase64, double temperature = 0.1, int maxTokens = 800, int timeoutSec = 90)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L77)

### CompleteVisionAsync

```csharp
public async Task<string> CompleteVisionAsync(string model, string systemPrompt, string userPrompt, IList<string> imagesBase64, double temperature = 0.1, int maxTokens = 800, int timeoutSec = 90)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L98)

### GetModels

```csharp
public List<string> GetModels()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L132)

### GetModelsAsync

```csharp
public async Task<List<string>> GetModelsAsync()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L137)

### InvalidateModelsCache

```csharp
public static void InvalidateModelsCache()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L185)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
