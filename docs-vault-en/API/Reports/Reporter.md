---
title: "Reporter"
tags: [api, Reports]
generated: z3n7-docgen
---

# Reporter

`class` · namespace `z3n7` · source [Reports/Reporter.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L16)

```csharp
public class Reporter
```

Отвечает за создание, форматирование и отправку отчетов

## Constructors

### Reporter

```csharp
public Reporter(IZennoPosterProjectModel project, Instance instance)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L27)

## Methods

### ReportError

```csharp
public string ReportError(bool toLog = true, bool toTelegram = false, bool toDb = true, bool screenshot = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L42)

Создает и отправляет отчет об ошибке

### ReportSuccess

```csharp
public string ReportSuccess(bool toLog = true, bool toTelegram = false, bool toDb = true, string customMessage = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L81)

Создает и отправляет отчет об успехе

> This page is generated from the source code. Do not edit it: changes will be overwritten.
