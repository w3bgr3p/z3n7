---
title: "BrowserScan"
tags: [api, Browser]
generated: z3n7-docgen
---

# BrowserScan

`class` · пространство имён `z3n7` · исходник [Browser/BrowserScan.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L12)

```csharp
public class BrowserScan
```

Читает отчёт об отпечатке `browserscan.net` в инстансе браузера.

## Конструкторы

### BrowserScan

```csharp
public BrowserScan(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L21)

Создаёт читателя.

| Параметр | Описание |
|---|---|
| `log` | Логгер для хода работы; `null` — ничего не писать. |

## Методы

### FixTime

```csharp
public string FixTime()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L180)

Открывает `browserscan.net` и ждёт окончания проверки (каждые 3–5 секунд, до 60 секунд). Ставит инстансу часовой пояс по смещению IP со страницы (режим эмуляции) и зону IANA.

**Возвращает:** JSON `{ timezoneOffset, timezoneName }`.

### GetScore

```csharp
public string GetScore()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L134)

Открывает `browserscan.net` и ждёт окончания проверки (каждые 3–5 секунд, до 60 секунд). Считывает общую оценку.

**Возвращает:** `[score] problems`; проблемы перечисляются, если оценка не 100%.

### ParseStats

```csharp
public Dictionary<string, string> ParseStats()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L79)

Открывает `browserscan.net` и ждёт окончания проверки (каждые 3–5 секунд, до 60 секунд). Считывает WebGL, отчёт WebGL, аудио, client rects, отчёт WebGPU, шрифты и часовой пояс и время по IP и записывает их в строку текущего аккаунта таблицы `_browserscan` (при необходимости таблица создаётся и заполняется строками аккаунтов).

**Возвращает:** Поле → значение, как оно показано на странице.

### Problems

```csharp
public Dictionary<string, string> Problems()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L153)

Открывает `browserscan.net` и ждёт окончания проверки (каждые 3–5 секунд, до 60 секунд). Считывает перечисленные проблемы, если оценка не 100%.

**Возвращает:** Проблема → описание; при 100% пусто.

