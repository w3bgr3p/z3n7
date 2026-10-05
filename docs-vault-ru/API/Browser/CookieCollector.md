---
title: "CookieCollector"
tags: [api, Browser]
generated: z3n7-docgen
---

# CookieCollector

`class` · пространство имён `z3n7` · исходник [Browser/CookieCollector.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L11)

```csharp
public class CookieCollector
```

*Описания пока нет.*

## Свойства

### Accept

```csharp
public string Accept { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L28)

### AcceptLanguage

```csharp
public string AcceptLanguage { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L31)

### AllowRedirects

```csharp
public bool AllowRedirects { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L18)

### Log

```csharp
public Action<string> Log { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L34)

### MaxCookieAgeDays

```csharp
public int MaxCookieAgeDays { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L20)

### MaxLastAccessAgeDays

```csharp
public int MaxLastAccessAgeDays { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L21)

### MinCookieAgeDays

```csharp
public int MinCookieAgeDays { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L19)

### TimeoutSeconds

```csharp
public int TimeoutSeconds { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L17)

### UserAgent

```csharp
public string UserAgent { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L23)

## Методы

### Run

```csharp
public string Run(IEnumerable<string> services, string cookiesJson, string proxy = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L36)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
