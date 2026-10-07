---
title: "Cookies.CookieInfo"
tags: [api, Browser]
generated: z3n7-docgen
---

# Cookies.CookieInfo

`class` · пространство имён `z3n7` · исходник [Browser/Cookies.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L22)

```csharp
public class CookieInfo
```

Summary of a stored cookie set (see `AnalyzeCookies`).

## Свойства

### ByDomain

```csharp
public Dictionary<string, int> ByDomain { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L35)

Cookie count per domain.

### ExpiredCookies

```csharp
public int ExpiredCookies { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L31)

Cookies already expired.

### GoogleCookies

```csharp
public int GoogleCookies { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L29)

Cookies whose domain contains `google`.

### LargestCookies

```csharp
public List<dynamic> LargestCookies { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L37)

The 10 cookies with the longest values.

### OldCookies

```csharp
public int OldCookies { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L33)

Cookies that expired more than 6 months ago.

### TotalCount

```csharp
public int TotalCount { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L25)

Number of cookies.

### TotalSizeBytes

```csharp
public long TotalSizeBytes { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L27)

Size of the cookie JSON, bytes.

