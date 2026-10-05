---
title: "TempMail"
tags: [api, Mail]
generated: z3n7-docgen
---

# TempMail

`class` · namespace `z3n7.Api` · source [Mail/TempMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L15)

```csharp
public class TempMail
```

*No description yet.*

## Constructors

### TempMail

```csharp
public TempMail(IZennoPosterProjectModel project, string apikey, bool log = false, bool useNetHttp = false, string proxy = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L26)

## Methods

### CreateAddress

```csharp
public static string CreateAddress(string login, string domain)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L176)

### GetDomains

```csharp
public string[] GetDomains()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L57)

### GetHrefs

```csharp
public HashSet<string> GetHrefs(int deadline = 120)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L144)

### GetMail

```csharp
public string GetMail(int deadline = 120)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L106)

Ждать письмо. Возвращает JSON первого сообщения.

### GetMessages

```csharp
public string GetMessages()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L93)

Получить текущий список сообщений без ожидания.

### HashEmail

```csharp
public static string HashEmail(string email)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L186)

### Link

```csharp
public string Link(string urlPattern, int deadline = 120)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L167)

### NewMail

```csharp
public string[] NewMail(string login = null, string domain = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L69)

Создать временный email. Возвращает [md5, email].

### Otp

```csharp
public string Otp(int deadline = 120)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L126)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
