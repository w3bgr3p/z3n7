---
title: "InstanceExtensions (Traffic)"
tags: [api, Traffic]
generated: z3n7-docgen
---

# InstanceExtensions (Traffic)

`static class` · пространство имён `z3n7` · исходник [Traffic/CdpHar.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L663), [Traffic/Traffic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L239)

```csharp
public static class InstanceExtensions
```

Другие части этого типа: [[InstanceExtensions (Browser)]]

Методы расширения для `Instance`: запись HAR через DevTools.

## Методы

### GrabTrafficList

```csharp
public static List<Traffic.TrafficElement> GrabTrafficList(this Instance instance, string url, bool strict = false)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L247)

Возвращает записанные к этому моменту запросы, URL которых подходит под `url`. Тот же текст используется как фильтр трафика.

| Параметр | Описание |
|---|---|
| `url` | Текст, который ищется в URL. |
| `strict` | Требовать точного совпадения URL. |

### SaveHar

```csharp
public static int SaveHar(this Instance instance, string path, string urlRegex = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L675)

Записывает трафик с момента `StartHar` в файл HAR. См. `CdpHar.Save`.

| Параметр | Описание |
|---|---|
| `path` | Целевой файл. |
| `urlRegex` | Регулярное выражение без учёта регистра, которому должен соответствовать URL; `null` оставляет всё. |

**Возвращает:** Число записанных записей. Бросает исключение, если запись не была запущена.

### StartHar

```csharp
public static CdpHar StartHar(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L666)

Запускает запись HAR через DevTools для этого инстанса. Вызывай ДО нужного трафика.

### StopHar

```csharp
public static void StopHar(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L683)

Останавливает запись HAR для этого инстанса.

