---
title: "Logger"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Logger

`class` · пространство имён `z3n7` · исходник [Essentials/Logger.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L24)

```csharp
public class Logger
```

Writes messages to the ZennoPoster log. Header fields are switched on by substrings of the `cfgLog` project variable: `acc` (account), `time` (project age), `port` (instance port), `caller` (calling member), `wrap` (print the header at all), `force` (ignore the level filter).

## Конструкторы

### Logger

```csharp
public Logger(IZennoPosterProjectModel project, Instance instance = null, LogLevel logLevel = LogLevel.Info, string classEmoji = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L60)

Creates a logger bound to a ZennoPoster project. The minimum level is taken from the `logLevel` project variable when it parses as `LogLevel`; otherwise `Debug` when `debug` is `True`; otherwise the `logLevel` argument.

| Параметр | Описание |
|---|---|
| `instance` | Not used; kept for compatibility. |
| `logLevel` | Minimum level when the project variables do not set one. |
| `classEmoji` | Value of `Emoji`. |

```csharp
public Logger(LogLevel logLevel = LogLevel.Info, string classEmoji = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L89)

Creates a logger without a ZennoPoster project. It has nowhere to write, so every message is dropped; `thrw` does not throw either.

| Параметр | Описание |
|---|---|
| `logLevel` | Minimum level. |
| `classEmoji` | Value of `Emoji`. |

## Свойства

### Emoji

```csharp
public string Emoji { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L45)

Prefix shown in brackets before every message, e.g. the class marker.

## Методы

### ClearCache

```csharp
public static void ClearCache(IZennoPosterProjectModel project)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L31)

Does nothing. Kept for compatibility: the logger no longer caches instances.

### Debug

```csharp
public void Debug(object msg, [CallerMemberName] string caller = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L154)

Writes a message at `Debug` level.

### Error

```csharp
public void Error(object msg, [CallerMemberName] string caller = "", bool thrw = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L169)

Writes an error. Errors are always written regardless of the minimum level.

| Параметр | Описание |
|---|---|
| `thrw` | Throw an `Exception` with the message after writing. |

### Get

```csharp
public static Logger Get(IZennoPosterProjectModel project, Instance instance = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L27)

Creates a logger for the project with default settings.

### Info

```csharp
public void Info(object msg, [CallerMemberName] string caller = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L158)

Writes a message at `Info` level.

### Send

```csharp
public void Send(object toLog, [CallerMemberName] string caller = "", bool show = false, bool thrw = false, bool toZp = true, int cut = 0, LogLevel level = LogLevel.Info, LogType type = LogType.Info, LogColor color = LogColor.Default)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L123)

Writes a message. Messages below the minimum level are dropped unless `show` is true or `cfgLog` contains `force`. The ZennoPoster log type follows the level; text containing `!W` or `!E` is logged as a warning or an error.

| Параметр | Описание |
|---|---|
| `toLog` | Message; `ToString()` is used, `null` is written as "null". |
| `caller` | Filled in by the compiler with the calling member name. |
| `show` | Write even if below the minimum level. |
| `thrw` | After writing to the ZennoPoster log, throw an `Exception` with the message. Only when the logger has a project and `toZp` is true. |
| `toZp` | Write to the ZennoPoster log. |
| `cut` | When the message has more than this many line breaks, join it into one line. 0 keeps it as is. |
| `level` | Severity used for filtering. |
| `type` | ZennoPoster log type; overridden by `level` Warning/Error and by the `!W`/`!E` markers. |
| `color` | ZennoPoster log color. |

### Warn

```csharp
public void Warn(object msg, [CallerMemberName] string caller = "", bool show = false, bool thrw = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L164)

Writes a warning.

| Параметр | Описание |
|---|---|
| `show` | Write even if below the minimum level. |
| `thrw` | Throw an `Exception` with the message after writing. |

### WithInstance

```csharp
public Logger WithInstance(Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L37)

Returns a copy of this logger. Kept for compatibility: the instance is not used.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
