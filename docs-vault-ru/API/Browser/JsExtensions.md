---
title: "JsExtensions"
tags: [api, Browser]
generated: z3n7-docgen
---

# JsExtensions

`static class` · пространство имён `z3n7` · исходник [Browser/js.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L15)

```csharp
public static class JsExtensions
```

*Описания пока нет.*

## Методы

### CenterMouse

```csharp
public static void CenterMouse(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L21)

### JsClick

```csharp
public static string JsClick(this Instance instance, string selector, double delay = 1.0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L29)

```csharp
public static void JsClick(this Instance instance, int x, int y)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L93)

```csharp
public static void JsClick(this Instance instance, int[] pos)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L126)

### JsGet

```csharp
public static string JsGet(this Instance instance, string jsSelector, string property)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L190)

### JsPost

```csharp
public static string JsPost(this Instance instance, string script, int delay = 0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L229)

### JsSet

```csharp
public static string JsSet(this Instance instance, string selector, string value, double delay = 1.0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L132)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
