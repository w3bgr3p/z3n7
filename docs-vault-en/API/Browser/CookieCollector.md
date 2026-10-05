---
title: "CookieCollector"
tags: [api, Browser]
generated: z3n7-docgen
---

# CookieCollector

`class` · namespace `z3n7` · source [Browser/CookieCollector.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L11)

```csharp
public class CookieCollector
```

*No description yet.*

## Properties

### Accept

```csharp
public string Accept { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L28)

### AcceptLanguage

```csharp
public string AcceptLanguage { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L31)

### AllowRedirects

```csharp
public bool AllowRedirects { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L18)

### Log

```csharp
public Action<string> Log { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L34)

### MaxCookieAgeDays

```csharp
public int MaxCookieAgeDays { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L20)

### MaxLastAccessAgeDays

```csharp
public int MaxLastAccessAgeDays { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L21)

### MinCookieAgeDays

```csharp
public int MinCookieAgeDays { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L19)

### TimeoutSeconds

```csharp
public int TimeoutSeconds { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L17)

### UserAgent

```csharp
public string UserAgent { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L23)

## Methods

### Run

```csharp
public string Run(IEnumerable<string> services, string cookiesJson, string proxy = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L36)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
