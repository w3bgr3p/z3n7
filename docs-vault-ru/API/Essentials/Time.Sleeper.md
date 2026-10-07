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

Случайная пауза в фиксированном диапазоне.

## Конструкторы

### Sleeper

```csharp
public Sleeper(int min, int max)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L57)

Создаёт объект пауз длиной от `min` до `max` миллисекунд, обе границы включительно.

| Параметр | Описание |
|---|---|
| `min` | Минимум, мс. Не может быть отрицательным. |
| `max` | Максимум, мс. Не меньше `min`. |

## Методы

### Sleep

```csharp
public void Sleep(double multiplier = 1.0)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L74)

Блокирует поток на случайное время в пределах диапазона.

| Параметр | Описание |
|---|---|
| `multiplier` | Множитель паузы, например 2.0 ждёт вдвое дольше. |

