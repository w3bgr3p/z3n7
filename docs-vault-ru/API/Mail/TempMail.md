---
title: "TempMail"
tags: [api, Mail]
generated: z3n7-docgen
---

# TempMail

`class` · пространство имён `z3n7.Api` · исходник [Mail/TempMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L15)

```csharp
public class TempMail
```

*Описания пока нет.*

## Конструкторы

### TempMail

```csharp
public TempMail(IZennoPosterProjectModel project, string apikey, bool log = false, bool useNetHttp = false, string proxy = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L26)

## Методы

### CreateAddress

```csharp
public static string CreateAddress(string login, string domain)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L176)

### GetDomains

```csharp
public string[] GetDomains()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L57)

### GetHrefs

```csharp
public HashSet<string> GetHrefs(int deadline = 120)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L144)

### GetMail

```csharp
public string GetMail(int deadline = 120)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L106)

Ждать письмо. Возвращает JSON первого сообщения.

### GetMessages

```csharp
public string GetMessages()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L93)

Получить текущий список сообщений без ожидания.

### HashEmail

```csharp
public static string HashEmail(string email)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L186)

### Link

```csharp
public string Link(string urlPattern, int deadline = 120)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L167)

### NewMail

```csharp
public string[] NewMail(string login = null, string domain = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L69)

Создать временный email. Возвращает [md5, email].

### Otp

```csharp
public string Otp(int deadline = 120)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L126)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
