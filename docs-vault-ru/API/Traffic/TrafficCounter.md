---
title: "TrafficCounter"
tags: [api, Traffic]
generated: z3n7-docgen
---

# TrafficCounter

`static class` · пространство имён `z3n7` · исходник [Traffic/TrafficCounter.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/TrafficCounter.cs#L15)

```csharp
public static class TrafficCounter
```

Считает трафик по помеченным шагам прогона проекта и выдаёт отчёт в JSON. Шаги хранятся в `project.Context`.

## Методы

### Add

```csharp
public static void Add(IZennoPosterProjectModel project, string label, string responseText)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/TrafficCounter.cs#L88)

Добавляет шаг для трафика вне браузера; считается как размер `responseText` в UTF-8.

| Параметр | Описание |
|---|---|
| `label` | Имя шага. |
| `responseText` | Текст ответа. |

### Checkpoint

```csharp
public static long Checkpoint(Instance instance, IZennoPosterProjectModel project, string label)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/TrafficCounter.cs#L51)

Добавляет шаг: суммарный размер тел запросов и ответов из трафика, который вернул `ActiveTab.GetTraffic()` (заблокированные запросы пропускаются). Ошибки пишутся в лог как предупреждения.

| Параметр | Описание |
|---|---|
| `label` | Имя шага. |

**Возвращает:** Число байт, учтённых для этого шага.

### Init

```csharp
public static void Init(Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/TrafficCounter.cs#L20)

Включает мониторинг трафика и один раз читает записанный к этому моменту трафик.

### MergeAndReport

```csharp
public static string MergeAndReport(IZennoPosterProjectModel project, string existingJson)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/TrafficCounter.cs#L107)

Объединяет шаги прежнего отчёта с текущими, сортирует по времени и строит новый отчёт.

| Параметр | Описание |
|---|---|
| `existingJson` | Отчёт из `ReportJson` или из этого метода; пустой или нечитаемый игнорируется. |

**Возвращает:** JSON `{ total_kb, steps: [{ t, label, kb }] }`; `t` — секунды с 2020-01-01 UTC.

### ReportJson

```csharp
public static string ReportJson(IZennoPosterProjectModel project)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/TrafficCounter.cs#L150)

Собирает отчёт из текущих шагов.

**Возвращает:** JSON `{ total_kb, steps: [{ t, label, kb }] }`; `t` — секунды с 2020-01-01 UTC.

