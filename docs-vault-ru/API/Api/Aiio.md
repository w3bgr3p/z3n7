---
title: "Aiio"
tags: [api, Api]
generated: z3n7-docgen
---

# Aiio

`class` · пространство имён `z3n7.Api` · исходник [Api/Aiio.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L18)

```csharp
public sealed class Aiio
```

Client of the io.net intelligence chat API (`api.intelligence.io.solutions`). API keys come from the `api` column of the `__aiio` table (rows whose `expire` is empty or in the future); a random one is used per request.

## Конструкторы

### Aiio

```csharp
public Aiio(IZennoPosterProjectModel project)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L29)

Creates a client; the project gives access to the key table.

## Методы

### Complete

```csharp
public string Complete(string model, string systemPrompt, string userPrompt, double temperature = 0.8, int maxTokens = 800, int timeoutSec = 90)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L35)

Blocking version of `CompleteAsync`.

### CompleteAsync

```csharp
public async Task<string> CompleteAsync(string model, string systemPrompt, string userPrompt, double temperature = 0.8, int maxTokens = 800, int timeoutSec = 90)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L61)

Sends one system and one user message and returns the reply (`top_p` 0.9, no streaming).

| Параметр | Описание |
|---|---|
| `model` | Model id; required. |
| `systemPrompt` | System message. |
| `userPrompt` | User message. |
| `temperature` | Sampling temperature. |
| `maxTokens` | Reply length limit. |
| `timeoutSec` | Request timeout, seconds. |

**Возвращает:** The reply text. Throws on a non-2xx status or an unexpected answer; the message includes the raw answer.

### GetModels

```csharp
public List<string> GetModels()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L101)

Blocking version of `GetModelsAsync`.

### GetModelsAsync

```csharp
public async Task<List<string>> GetModelsAsync()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L109)

Available model ids, sorted. The list is cached for the process; see `InvalidateModelsCache`.

### HasKey

```csharp
public bool HasKey()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L144)

Whether the key table has at least one valid key.

### InvalidateModelsCache

```csharp
public static void InvalidateModelsCache()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L150)

Forgets the cached model list.

