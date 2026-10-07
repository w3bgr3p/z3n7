---
title: "Time.Deadline"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Time.Deadline

`class` · пространство имён `z3n7` · исходник [Essentials/Time.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L15)

```csharp
public class Deadline
```

Stopwatch that throws once a time limit is exceeded.

## Конструкторы

### Deadline

```csharp
public Deadline()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L19)

Starts the stopwatch.

## Методы

### Check

```csharp
public double Check(double limitSec)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L25)

Returns the seconds elapsed since start or the last `Reset`.

| Параметр | Описание |
|---|---|
| `limitSec` | Limit in seconds; exceeding it throws `TimeoutException`. |

### Reset

```csharp
public void Reset()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L37)

Restarts the stopwatch.

