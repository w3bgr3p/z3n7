---
title: "Time"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Time

`class` · пространство имён `z3n7` · исходник [Essentials/Time.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L12)

```csharp
public class Time
```

Time helpers: timestamps, deadlines, random pauses.

## Методы

### Cd

```csharp
public static string Cd(object input = null, string o = "iso")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L105)

Returns a point in time counted from now (UTC), for cooldowns stored in the database.

| Параметр | Описание |
|---|---|
| `input` | `null` — today 23:59:59; `"nextH"` — one minute past the next hour; `int`/`decimal` — that many minutes from now (0 means practically never); other text — a `TimeSpan` such as `"02:30:00"` added to now. |
| `o` | `iso` (`yyyy-MM-ddTHH:mm:ss.fffZ`) or `unix` (seconds). Anything else throws. |

### Elapsed

```csharp
public static long Elapsed(long startTime = 0, bool useMs = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L140)

Time since `startTime`, or the current Unix time when `startTime` is 0.

| Параметр | Описание |
|---|---|
| `startTime` | Start as Unix time in the same unit as `useMs` selects. |
| `useMs` | Milliseconds instead of seconds. |

### Now

```csharp
public static string Now(string format = "unix")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L87)

Current UTC time as text.

| Параметр | Описание |
|---|---|
| `format` | `unix` — milliseconds since epoch; `iso` — `yyyy-MM-ddTHH:mm:ss.fffZ`; `short` — `MM-ddTHH:mm`; `utcToId` — seconds since epoch. Anything else throws. |

### TillNextHour

```csharp
public static int TillNextHour()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L158)

Seconds left until the next full hour, local time.

