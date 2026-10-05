---
title: "InstanceExtensions (Traffic)"
tags: [api, Traffic]
generated: z3n7-docgen
---

# InstanceExtensions (Traffic)

`static class` · пространство имён `z3n7` · исходник [Traffic/CdpHar.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L639), [Traffic/Traffic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L192)

```csharp
public static class InstanceExtensions
```

Другие части этого типа: [[InstanceExtensions (Browser)]]

*Описания пока нет.*

## Методы

### GrabTrafficList

```csharp
public static List<Traffic.TrafficElement> GrabTrafficList(this Instance instance, string url, bool strict = false)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L194)

### SaveHar

```csharp
public static int SaveHar(this Instance instance, string path, string urlRegex = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L647)

### StartHar

```csharp
public static CdpHar StartHar(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L642)

Starts HAR recording over DevTools for this instance. Call BEFORE the traffic you need.

### StopHar

```csharp
public static void StopHar(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L654)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
