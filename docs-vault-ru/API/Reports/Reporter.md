---
title: "Reporter"
tags: [api, Reports]
generated: z3n7-docgen
---

# Reporter

`class` · пространство имён `z3n7` · исходник [Reports/Reporter.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L16)

```csharp
public class Reporter
```

Отвечает за создание, форматирование и отправку отчетов

## Конструкторы

### Reporter

```csharp
public Reporter(IZennoPosterProjectModel project, Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L27)

## Методы

### ReportError

```csharp
public string ReportError(bool toLog = true, bool toTelegram = false, bool toDb = true, bool screenshot = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L42)

Создает и отправляет отчет об ошибке

### ReportSuccess

```csharp
public string ReportSuccess(bool toLog = true, bool toTelegram = false, bool toDb = true, string customMessage = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L81)

Создает и отправляет отчет об успехе

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
