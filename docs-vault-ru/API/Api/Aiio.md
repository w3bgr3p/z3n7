---
title: "Aiio"
tags: [api, Api]
generated: z3n7-docgen
---

# Aiio

`class` · пространство имён `z3n7.Api` · исходник [Api/Aiio.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L13)

```csharp
public sealed class Aiio
```

*Описания пока нет.*

## Конструкторы

### Aiio

```csharp
public Aiio(IZennoPosterProjectModel project)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L23)

## Методы

### Complete

```csharp
public string Complete(string model, string systemPrompt, string userPrompt, double temperature = 0.8, int maxTokens = 800, int timeoutSec = 90)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L28)

### CompleteAsync

```csharp
public async Task<string> CompleteAsync(string model, string systemPrompt, string userPrompt, double temperature = 0.8, int maxTokens = 800, int timeoutSec = 90)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L41)

### GetModels

```csharp
public List<string> GetModels()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L80)

### GetModelsAsync

```csharp
public async Task<List<string>> GetModelsAsync()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L85)

### HasKey

```csharp
public bool HasKey()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L119)

### InvalidateModelsCache

```csharp
public static void InvalidateModelsCache()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L124)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
