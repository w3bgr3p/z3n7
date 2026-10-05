---
title: "Time.Sleeper"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Time.Sleeper

`class` · namespace `z3n7` · source [Essentials/Time.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L45)

```csharp
public class Sleeper
```

Random pause within a fixed range.

## Constructors

### Sleeper

```csharp
public Sleeper(int min, int max)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L57)

Creates a sleeper for pauses of `min` … `max` milliseconds, both inclusive.

| Parameter | Description |
|---|---|
| `min` | Minimum, ms. Must not be negative. |
| `max` | Maximum, ms. Must not be less than `min`. |

## Methods

### Sleep

```csharp
public void Sleep(double multiplier = 1.0)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L74)

Blocks the thread for a random time within the range.

| Parameter | Description |
|---|---|
| `multiplier` | Scale factor for the pause, e.g. 2.0 waits twice as long. |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
