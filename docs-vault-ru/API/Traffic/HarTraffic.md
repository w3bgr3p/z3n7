---
title: "HarTraffic"
tags: [api, Traffic]
generated: z3n7-docgen
---

# HarTraffic

`static class` · пространство имён `z3n7` · исходник [Traffic/Har.Rqst.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.Rqst.cs#L11), [Traffic/Har.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.cs#L16)

```csharp
public static class HarTraffic
```

Выгрузка в HAR 1.2 трафика браузера (`GetTraffic`) и сохранённого трафика `Rqst`.

## Методы

### ExportRqst

```csharp
public static string ExportRqst(IZennoPosterProjectModel project, string projectFilter = null, string taskId = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.Rqst.cs#L14)

Выгружает все завершённые сохранённые транзакции Rqst в JSON-документ HAR 1.2.

### FromRqstJsonl

```csharp
public static string FromRqstJsonl(string path, string projectFilter = null, string taskId = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.Rqst.cs#L25)

Переводит файл трафика `Rqst` по пути `path` в JSON-документ HAR 1.2. Доступно только то, что записал `Rqst`: в таймингах только общая длительность, бинарные тела — текстом.

| Параметр | Описание |
|---|---|
| `path` | Файл трафика, который пишет `ZpTraffic`. |
| `projectFilter` | Оставить только записи этого проекта. |
| `taskId` | Оставить только записи этой задачи. |

### Save

```csharp
public static int Save(Instance instance, string path, string filter = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.cs#L28)

Записывает трафик активной вкладки в файл HAR 1.2, заменяя существующий. Тела ответов декодируются (gzip, deflate; brotli и zstd — если загружены Brotli.Core или ZstdNet); текстовые типы хранятся текстом, остальные — в Base64. Элементы, которые не удалось преобразовать, пропускаются. Тайминги и время начала приблизительные: ZennoPoster даёт только общее время.

| Параметр | Описание |
|---|---|
| `instance` | Инстанс браузера. |
| `path` | Целевой файл; должен оканчиваться на `.har`. Недостающие папки создаются. |
| `filter` | Фильтр URL для `GetTraffic`; по умолчанию домен активной вкладки. |

**Возвращает:** Число записанных записей.

