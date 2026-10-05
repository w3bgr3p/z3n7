---
title: "Logger"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Logger

`class` · namespace `z3n7` · source [Essentials/Logger.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L28)

```csharp
public class Logger
```

Writes messages to the ZennoPoster log and, optionally, as JSON to an HTTP log collector. Header fields are switched on by substrings of the `cfgLog` project variable: `acc` (account), `time` (project age), `port` (instance port), `caller` (calling member), `wrap` (print the header at all), `http` (send to the collector), `force` (ignore the level filter).

## Constructors

### Logger

```csharp
public Logger(IZennoPosterProjectModel project, Instance instance = null, LogLevel logLevel = LogLevel.Info, string logHost = null, bool http = true, int timezoneOffset = -5, string classEmoji = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L82)

Creates a logger bound to a ZennoPoster project. The minimum level is taken from the `logLevel` project variable when it parses as `LogLevel`; otherwise `Debug` when `debug` is `True`; otherwise the `logLevel` argument. The collector address is `logHost`, then the `logHost` global variable, then `http://localhost:10993/log`.

| Parameter | Description |
|---|---|
| `instance` | Optional browser instance; its port and PID are read from the window title and sent to the collector. |
| `logLevel` | Minimum level when the project variables do not set one. |
| `logHost` | URL of the HTTP log collector. |
| `http` | Allow sending to the collector. It is sent only when `cfgLog` also contains `http`. |
| `timezoneOffset` | Hours added to UTC for the collector timestamp. |
| `classEmoji` | Value of `Emoji`. |

```csharp
public Logger(LogLevel logLevel = LogLevel.Info, string logHost = null, bool http = true, int timezoneOffset = -5, string classEmoji = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L130)

Creates a logger without a ZennoPoster project: messages go only to the HTTP collector. The caller name is always included.

| Parameter | Description |
|---|---|
| `logLevel` | Minimum level. |
| `logHost` | URL of the HTTP log collector; default `http://localhost:10993/log`. |
| `http` | Send messages to the collector. |
| `timezoneOffset` | Hours added to UTC for the collector timestamp. |
| `classEmoji` | Value of `Emoji`. |

## Properties

### Emoji

```csharp
public string Emoji { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L56)

Prefix shown in brackets before every message, e.g. the class marker.

## Methods

### ClearCache

```csharp
public static void ClearCache(IZennoPosterProjectModel project)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L35)

Does nothing. Kept for compatibility: the logger no longer caches instances.

### Debug

```csharp
public void Debug(object msg, [CallerMemberName] string caller = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L203)

Writes a message at `Debug` level.

### Error

```csharp
public void Error(object msg, [CallerMemberName] string caller = "", bool thrw = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L218)

Writes an error. Errors are always written regardless of the minimum level.

| Parameter | Description |
|---|---|
| `thrw` | Throw an `Exception` with the message after writing. |

### Get

```csharp
public static Logger Get(IZennoPosterProjectModel project, Instance instance = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L31)

Creates a logger for the project with default settings.

### Info

```csharp
public void Info(object msg, [CallerMemberName] string caller = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L207)

Writes a message at `Info` level.

### Send

```csharp
public void Send(object toLog, [CallerMemberName] string caller = "", bool show = false, bool thrw = false, bool toZp = true, int cut = 0, LogLevel level = LogLevel.Info, LogType type = LogType.Info, LogColor color = LogColor.Default)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L170)

Writes a message. Messages below the minimum level are dropped unless `show` is true or `cfgLog` contains `force`. The ZennoPoster log type follows the level; text containing `!W` or `!E` is logged as a warning or an error.

| Parameter | Description |
|---|---|
| `toLog` | Message; `ToString()` is used, `null` is written as "null". |
| `caller` | Filled in by the compiler with the calling member name. |
| `show` | Write even if below the minimum level. |
| `thrw` | After writing to the ZennoPoster log, throw an `Exception` with the message. Only when the logger has a project and `toZp` is true. |
| `toZp` | Write to the ZennoPoster log. |
| `cut` | When the message has more than this many line breaks, join it into one line. 0 keeps it as is. |
| `level` | Severity used for filtering and for the collector. |
| `type` | ZennoPoster log type; overridden by `level` Warning/Error and by the `!W`/`!E` markers. |
| `color` | ZennoPoster log color. |

### Warn

```csharp
public void Warn(object msg, [CallerMemberName] string caller = "", bool show = false, bool thrw = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L213)

Writes a warning.

| Parameter | Description |
|---|---|
| `show` | Write even if below the minimum level. |
| `thrw` | Throw an `Exception` with the message after writing. |

### WithInstance

```csharp
public Logger WithInstance(Instance instance)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L43)

Returns a copy of this logger bound to `instance` (its port and PID are sent to the collector).

> This page is generated from the source code. Do not edit it: changes will be overwritten.
