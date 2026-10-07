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

Клиент чат-API io.net intelligence (`api.intelligence.io.solutions`). API-ключи берутся из колонки `api` таблицы `__aiio` (строки, у которых `expire` пусто или в будущем); на каждый запрос берётся случайный.

## Конструкторы

### Aiio

```csharp
public Aiio(IZennoPosterProjectModel project)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L29)

Создаёт клиент; проект даёт доступ к таблице ключей.

## Методы

### Complete

```csharp
public string Complete(string model, string systemPrompt, string userPrompt, double temperature = 0.8, int maxTokens = 800, int timeoutSec = 90)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L35)

Блокирующая версия `CompleteAsync`.

### CompleteAsync

```csharp
public async Task<string> CompleteAsync(string model, string systemPrompt, string userPrompt, double temperature = 0.8, int maxTokens = 800, int timeoutSec = 90)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L61)

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

### GetModels

```csharp
public List<string> GetModels()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L101)

Блокирующая версия `GetModelsAsync`.

### GetModelsAsync

```csharp
public async Task<List<string>> GetModelsAsync()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L109)

Доступные id моделей, по алфавиту. Список кешируется на время процесса; см. `InvalidateModelsCache`.

### HasKey

```csharp
public bool HasKey()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L144)

Есть ли в таблице ключей хотя бы один действующий ключ.

### InvalidateModelsCache

```csharp
public static void InvalidateModelsCache()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Aiio.cs#L150)

Забывает закешированный список моделей.

