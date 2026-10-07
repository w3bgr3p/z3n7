---
title: "OmniRoute"
tags: [api, Api]
generated: z3n7-docgen
---

# OmniRoute

`class` · пространство имён `z3n7.Api` · исходник [Api/OmniRoute.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L15)

```csharp
public sealed class OmniRoute
```

Client of a local OpenAI-compatible router at `http://localhost:20128` (no API key).

## Конструкторы

### OmniRoute

```csharp
public OmniRoute()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L24)

Creates a client.

## Методы

### Check

```csharp
public bool Check()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L204)

Blocking version of `CheckAsync`.

### CheckAsync

```csharp
public async Task<bool> CheckAsync()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L210)

Whether the router answers the model list with 2xx within 3 seconds.

### Complete

```csharp
public string Complete(string model, string systemPrompt, string userPrompt, double temperature = 0.3, int maxTokens = 800, int timeoutSec = 90)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L29)

Blocking version of `CompleteAsync`.

### CompleteAsync

```csharp
public async Task<string> CompleteAsync(string model, string systemPrompt, string userPrompt, double temperature = 0.3, int maxTokens = 800, int timeoutSec = 90)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L55)

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

### CompleteVision

```csharp
public string CompleteVision(string model, string systemPrompt, string userPrompt, IList<string> imagesBase64, double temperature = 0.1, int maxTokens = 800, int timeoutSec = 90)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L94)

Blocking version of `CompleteVisionAsync`.

### CompleteVisionAsync

```csharp
public async Task<string> CompleteVisionAsync(string model, string systemPrompt, string userPrompt, IList<string> imagesBase64, double temperature = 0.1, int maxTokens = 800, int timeoutSec = 90)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L131)

Sends a prompt with images. The prompt is extended with the pixel size of the first image; a reply wrapped in a code fence is unwrapped, and when it is JSON with `canvas_width`/`canvas_height`, those are set to the first image's size.

| Параметр | Описание |
|---|---|
| `model` | Model id; required. |
| `systemPrompt` | System message. |
| `userPrompt` | User message. |
| `temperature` | Sampling temperature. |
| `maxTokens` | Reply length limit. |
| `timeoutSec` | Request timeout, seconds. |
| `imagesBase64` | Images as Base64 (optionally as data URLs); at least one. |

**Возвращает:** The reply text. Throws on a non-2xx status or an unexpected answer; the message includes the raw answer.

### GetModels

```csharp
public List<string> GetModels()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L166)

Blocking version of `GetModelsAsync`.

### GetModelsAsync

```csharp
public async Task<List<string>> GetModelsAsync()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L174)

Available model ids, sorted. The list is cached for the process; see `InvalidateModelsCache`.

### InvalidateModelsCache

```csharp
public static void InvalidateModelsCache()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L225)

Forgets the cached model list.

