---
title: "CookieCollector"
tags: [api, Browser]
generated: z3n7-docgen
---

# CookieCollector

`class` · namespace `z3n7` · source [Browser/CookieCollector.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L15)

```csharp
public class CookieCollector
```

Collects cookies by visiting sites over plain HTTP (not the browser), starting from an existing cookie set, and returns them as browser-extension style JSON.

## Properties

### Accept

```csharp
public string Accept { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L39)

Accept header of the requests.

### AcceptLanguage

```csharp
public string AcceptLanguage { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L43)

Accept-Language header of the requests.

### AllowRedirects

```csharp
public bool AllowRedirects { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L24)

Follow redirects (up to 10).

### Log

```csharp
public Action<string> Log { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L47)

Receives `COOKIES={count}` at the end of `Run`; `null` for none.

### MaxCookieAgeDays

```csharp
public int MaxCookieAgeDays { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L28)

Upper bound of the made-up age of new cookies, days.

### MaxLastAccessAgeDays

```csharp
public int MaxLastAccessAgeDays { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L30)

Made-up last-access dates of new cookies fall within this many days before now.

### MinCookieAgeDays

```csharp
public int MinCookieAgeDays { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L26)

Lower bound of the made-up age of new cookies, days.

### TimeoutSeconds

```csharp
public int TimeoutSeconds { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L22)

Timeout of each request, seconds.

### UserAgent

```csharp
public string UserAgent { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L33)

User-Agent header of the requests.

## Methods

### Run

```csharp
public string Run(IEnumerable<string> services, string cookiesJson, string proxy = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L63)

Loads `cookiesJson`, sends a GET to each service and saves every cookie of the involved domains. A service that times out or fails is skipped. Cookies without `creationDate` get random creation and last-access dates within the configured ages.

| Parameter | Description |
|---|---|
| `services` | URLs or host names; `https://` is added when missing. |
| `cookiesJson` | Starting cookies: JSON array with `name`, `value`, `domain`, `path`, `secure`, `httpOnly`, `expirationDate` and optional `creationDate`/`lastAccessDate`. |
| `proxy` | `[scheme://][user:pass@]host:port`; empty for none. |

**Returns:** JSON array of cookies in the same format.

