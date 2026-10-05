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

Reads the fingerprint report of `browserscan.net` in a browser instance.

## Конструкторы

### BrowserScan

```csharp
public BrowserScan(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L21)

Creates the reader.

| Параметр | Описание |
|---|---|
| `log` | Logger for progress; `null` logs nothing. |

## Методы

### FixTime

```csharp
public string FixTime()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L180)

Opens `browserscan.net` and waits for it to finish (checked every 3–5 seconds, up to 60 seconds). Sets the instance timezone to the page's IP-based offset (emulation mode) and IANA zone.

**Возвращает:** JSON `{ timezoneOffset, timezoneName }`.

### GetScore

```csharp
public string GetScore()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L134)

Opens `browserscan.net` and waits for it to finish (checked every 3–5 seconds, up to 60 seconds). Reads the overall score.

**Возвращает:** `[score] problems`; problems are listed when the score is not 100%.

### ParseStats

```csharp
public Dictionary<string, string> ParseStats()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L79)

Opens `browserscan.net` and waits for it to finish (checked every 3–5 seconds, up to 60 seconds). Reads WebGL, WebGL report, audio, client rects, WebGPU report, fonts and the IP-based timezone and time, and writes them to the current account's row of the `_browserscan` table (created and filled with account rows if needed).

**Возвращает:** Field → value as shown on the page.

### Problems

```csharp
public Dictionary<string, string> Problems()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L153)

Opens `browserscan.net` and waits for it to finish (checked every 3–5 seconds, up to 60 seconds). Reads the listed problems when the score is not 100%.

**Возвращает:** Problem → description; empty at 100%.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
