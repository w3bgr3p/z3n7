---
title: "Reporter"
tags: [api, Reports]
generated: z3n7-docgen
---

# Reporter

`class` · пространство имён `z3n7` · исходник [Reports/Reporter.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L18)

```csharp
public class Reporter
```

Builds run reports (error or success) and sends them to the log, Telegram and the account's database row. Telegram credentials come from the `_api` table, row `id = 'tg_logger'` (`apikey`, `extra` = `{chat}/{topic}`).

## Конструкторы

### Reporter

```csharp
public Reporter(IZennoPosterProjectModel project, Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L30)

Creates a reporter; remembers the current time and the session's elapsed seconds.

## Методы

### ReportError

```csharp
public string ReportError(bool toLog = true, bool toTelegram = false, bool toDb = true, bool screenshot = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L56)

Reports the project's last error: account, action id and comment, exception type, message, inner message, first stack-trace frame and the current URL.

| Параметр | Описание |
|---|---|
| `toLog` | Write it to the log as a warning. |
| `toTelegram` | Send it to Telegram (also stored in `failReport`). |
| `toDb` | Set `status = 'dropped'` and write the report to `last` in the current account's row. |
| `screenshot` | Save a screenshot with the report as a watermark to `{project.Path}/.failed/{projectName}/`, scaled to 50%. |

**Возвращает:** The log text; empty when there is no last error.

### ReportSuccess

```csharp
public string ReportSuccess(bool toLog = true, bool toTelegram = false, bool toDb = true, string customMessage = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L101)

Reports a successful run: account, the `lastQuery` variable, an optional message and the elapsed time.

| Параметр | Описание |
|---|---|
| `toLog` | Write it to the log. |
| `toTelegram` | Send it to Telegram. |
| `toDb` | Set `status = 'idle'` and write the report to `last` in the current account's row. |
| `customMessage` | Extra line. |

**Возвращает:** The log text.

