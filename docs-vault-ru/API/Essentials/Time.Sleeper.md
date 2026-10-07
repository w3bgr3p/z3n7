---
title: "Time.Sleeper"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Time.Sleeper

`class` · пространство имён `z3n7` · исходник [Essentials/Time.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L45)

```csharp
public class Sleeper
```

Random pause within a fixed range.

## Конструкторы

### Sleeper

```csharp
public Sleeper(int min, int max)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L57)

Creates a sleeper for pauses of `min` … `max` milliseconds, both inclusive.

| Параметр | Описание |
|---|---|
| `min` | Minimum, ms. Must not be negative. |
| `max` | Maximum, ms. Must not be less than `min`. |

## Методы

### Sleep

```csharp
public void Sleep(double multiplier = 1.0)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L74)

Blocks the thread for a random time within the range.

| Параметр | Описание |
|---|---|
| `multiplier` | Scale factor for the pause, e.g. 2.0 waits twice as long. |

