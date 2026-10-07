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

Помощники для времени: метки времени, дедлайны, случайные паузы.

## Методы

### Cd

```csharp
public static string Cd(object input = null, string o = "iso")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L105)

Возвращает момент времени, отсчитанный от текущего (UTC), для кулдаунов, хранящихся в базе.

| Параметр | Описание |
|---|---|
| `input` | `null` — сегодня 23:59:59; `"nextH"` — минута после начала следующего часа; `int`/`decimal` — столько минут от текущего момента (0 означает практически никогда); другой текст — `TimeSpan` вроде `"02:30:00"`, прибавленный к текущему моменту. |
| `o` | `iso` (`yyyy-MM-ddTHH:mm:ss.fffZ`) или `unix` (секунды). Всё остальное приводит к исключению. |

### Elapsed

```csharp
public static long Elapsed(long startTime = 0, bool useMs = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L140)

Время, прошедшее с `startTime`, или текущее Unix-время, если `startTime` равно 0.

| Параметр | Описание |
|---|---|
| `startTime` | Старт в Unix-времени в той же единице, которую задаёт `useMs`. |
| `useMs` | Миллисекунды вместо секунд. |

### Now

```csharp
public static string Now(string format = "unix")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L87)

Текущее время UTC текстом.

| Параметр | Описание |
|---|---|
| `format` | `unix` — миллисекунды с начала эпохи; `iso` — `yyyy-MM-ddTHH:mm:ss.fffZ`; `short` — `MM-ddTHH:mm`; `utcToId` — секунды с начала эпохи. Всё остальное приводит к исключению. |

### TillNextHour

```csharp
public static int TillNextHour()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L158)

Сколько секунд осталось до следующего полного часа, по местному времени.

