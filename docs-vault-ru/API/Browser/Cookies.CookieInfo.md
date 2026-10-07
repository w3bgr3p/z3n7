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

Сводка по сохранённому набору кук (см. `AnalyzeCookies`).

## Свойства

### ByDomain

```csharp
public Dictionary<string, int> ByDomain { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L35)

Число кук по доменам.

### ExpiredCookies

```csharp
public int ExpiredCookies { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L31)

Уже истёкшие куки.

### GoogleCookies

```csharp
public int GoogleCookies { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L29)

Куки, у которых домен содержит `google`.

### LargestCookies

```csharp
public List<dynamic> LargestCookies { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L37)

10 кук с самыми длинными значениями.

### OldCookies

```csharp
public int OldCookies { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L33)

Куки, истёкшие больше 6 месяцев назад.

### TotalCount

```csharp
public int TotalCount { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L25)

Число кук.

### TotalSizeBytes

```csharp
public long TotalSizeBytes { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L27)

Размер JSON с куками, байты.

