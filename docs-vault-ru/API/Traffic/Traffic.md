---
title: "Traffic"
tags: [api, Traffic]
generated: z3n7-docgen
---

# Traffic

`class` · пространство имён `z3n7` · исходник [Traffic/Traffic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L19)

```csharp
public class Traffic
```

Читает трафик, записанный активной вкладкой инстанса ZennoPoster (`ActiveTab.GetTraffic`). Запросы `OPTIONS` пропускаются; тела ответов в gzip распаковываются.

## Конструкторы

### Traffic

```csharp
public Traffic(Instance instance, string defaultFilter = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L26)

Включает мониторинг трафика для инстанса.

| Параметр | Описание |
|---|---|
| `defaultFilter` | Фильтр, который передаётся в `GetTraffic`; по умолчанию домен активной вкладки. |

## Методы

### Find

```csharp
public TrafficElement Find(string url, bool strict = false, int timeoutSec = 15)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L53)

Ждёт запрос с подходящим URL, проверяя раз в секунду.

| Параметр | Описание |
|---|---|
| `url` | Текст, который ищется в URL, или весь URL при `strict`. |
| `strict` | Требовать точного совпадения URL. |
| `timeoutSec` | Сколько ждать. |

**Возвращает:** Первый подходящий запрос. Бросает `TimeoutException`, если вовремя ничего не появилось.

### FindAll

```csharp
public List<TrafficElement> FindAll(string url, bool strict = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L68)

Возвращает все записанные к этому моменту запросы, URL которых подходит.

| Параметр | Описание |
|---|---|
| `url` | Текст, который ищется в URL, или весь URL при `strict`. |
| `strict` | Требовать точного совпадения URL. |

### GetApiStructure

```csharp
public string GetApiStructure(string urlFilter = "api", bool includeHeaders = false, bool excludeFiles = true)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L81)

Сводит записанные вызовы API в JSON с отступами: `total` и по массиву на каждый метод (`getEndpoints`, `postEndpoints`, …). Каждая пара метод + URL попадает один раз; тела включены и, где возможно, разобраны как JSON.

| Параметр | Описание |
|---|---|
| `urlFilter` | Текст, который должен содержаться в URL. |
| `includeHeaders` | Добавлять заголовки запроса и ответа. |
| `excludeFiles` | Пропускать URL, у которых в последнем сегменте пути есть расширение файла. |

### SaveHeadersToVar

```csharp
public void SaveHeadersToVar(string url, string varName = "headers", bool strict = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L129)

Ждёт запрос (см. `Find`) и собирает его заголовки запроса без псевдозаголовков с `:`.

| Параметр | Описание |
|---|---|
| `url` | Фильтр URL. |
| `varName` | Не используется. |
| `strict` | Требовать точного совпадения URL. |

**Примечания:** Собранные заголовки никуда не сохраняются: строка, которая записывала их в переменную, закомментирована.

