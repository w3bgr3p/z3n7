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

Секундомер, который бросает исключение при превышении предела времени.

## Конструкторы

### Deadline

```csharp
public Deadline()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L19)

Запускает секундомер.

## Методы

### Check

```csharp
public double Check(double limitSec)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L25)

Возвращает число секунд, прошедших со старта или с последнего `Reset`.

| Параметр | Описание |
|---|---|
| `limitSec` | Предел в секундах; при превышении бросается `TimeoutException`. |

### Reset

```csharp
public void Reset()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L37)

Перезапускает секундомер.

