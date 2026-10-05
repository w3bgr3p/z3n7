---
title: "GmailClient"
tags: [api, Mail]
generated: z3n7-docgen
---

# GmailClient

`class` · пространство имён `z3n7.Api` · исходник [Mail/GMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L13)

```csharp
public class GmailClient
```

*Описания пока нет.*

## Конструкторы

### GmailClient

```csharp
public GmailClient(IZennoPosterProjectModel project, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L27)

## Методы

### GetLink

```csharp
public string GetLink(string targetEmail)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L177)

Ищет ссылку в последнем письме адресованном на targetEmail.

### Otp

```csharp
public string Otp(string targetEmail, int maxResults = 10)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L149)

Ищет 6-значный OTP в последних письмах адресованных на targetEmail. Бросает Exception если не найден.

### SendMail

```csharp
public void SendMail(string to, string subject, string body)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L210)

Отправляет письмо из текущего ящика.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
