---
title: "Time.Deadline"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Time.Deadline

`class` · namespace `z3n7` · source [Essentials/Time.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L15)

```csharp
public class Deadline
```

Stopwatch that throws once a time limit is exceeded.

## Constructors

### Deadline

```csharp
public Deadline()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L19)

Starts the stopwatch.

## Methods

### Check

```csharp
public double Check(double limitSec)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L25)

Returns the seconds elapsed since start or the last `Reset`.

| Parameter | Description |
|---|---|
| `limitSec` | Limit in seconds; exceeding it throws `TimeoutException`. |

### Reset

```csharp
public void Reset()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L37)

Restarts the stopwatch.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
