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

Клиент локального OpenAI-совместимого роутера на `http://localhost:20128` (без API-ключа).

## Конструкторы

### OmniRoute

```csharp
public OmniRoute()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L24)

Создаёт клиент.

## Методы

### Check

```csharp
public bool Check()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L204)

Блокирующая версия `CheckAsync`.

### CheckAsync

```csharp
public async Task<bool> CheckAsync()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L210)

Отвечает ли роутер на запрос списка моделей статусом 2xx за 3 секунды.

### Complete

```csharp
public string Complete(string model, string systemPrompt, string userPrompt, double temperature = 0.3, int maxTokens = 800, int timeoutSec = 90)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L29)

Блокирующая версия `CompleteAsync`.

### CompleteAsync

```csharp
public async Task<string> CompleteAsync(string model, string systemPrompt, string userPrompt, double temperature = 0.3, int maxTokens = 800, int timeoutSec = 90)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L55)

Отправляет одно системное и одно пользовательское сообщение и возвращает ответ (`top_p` 0.9, без стриминга).

| Параметр | Описание |
|---|---|
| `model` | Id модели; обязателен. |
| `systemPrompt` | Системное сообщение. |
| `userPrompt` | Сообщение пользователя. |
| `temperature` | Температура сэмплирования. |
| `maxTokens` | Ограничение длины ответа. |
| `timeoutSec` | Таймаут запроса, секунды. |

**Возвращает:** Текст ответа. Бросает исключение при статусе не 2xx или неожиданном ответе; в сообщении есть сырой ответ.

### CompleteVision

```csharp
public string CompleteVision(string model, string systemPrompt, string userPrompt, IList<string> imagesBase64, double temperature = 0.1, int maxTokens = 800, int timeoutSec = 90)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L94)

Блокирующая версия `CompleteVisionAsync`.

### CompleteVisionAsync

```csharp
public async Task<string> CompleteVisionAsync(string model, string systemPrompt, string userPrompt, IList<string> imagesBase64, double temperature = 0.1, int maxTokens = 800, int timeoutSec = 90)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L131)

Отправляет промпт с картинками. К промпту добавляется размер первой картинки в пикселях; ответ, обёрнутый в блок кода, разворачивается, а если это JSON с `canvas_width`/`canvas_height`, им ставится размер первой картинки.

| Параметр | Описание |
|---|---|
| `model` | Id модели; обязателен. |
| `systemPrompt` | Системное сообщение. |
| `userPrompt` | Сообщение пользователя. |
| `temperature` | Температура сэмплирования. |
| `maxTokens` | Ограничение длины ответа. |
| `timeoutSec` | Таймаут запроса, секунды. |
| `imagesBase64` | Картинки в Base64 (по желанию в виде data URL); хотя бы одна. |

**Возвращает:** Текст ответа. Бросает исключение при статусе не 2xx или неожиданном ответе; в сообщении есть сырой ответ.

### GetModels

```csharp
public List<string> GetModels()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L166)

Блокирующая версия `GetModelsAsync`.

### GetModelsAsync

```csharp
public async Task<List<string>> GetModelsAsync()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L174)

Доступные id моделей, по алфавиту. Список кешируется на время процесса; см. `InvalidateModelsCache`.

### InvalidateModelsCache

```csharp
public static void InvalidateModelsCache()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/OmniRoute.cs#L225)

Забывает закешированный список моделей.

