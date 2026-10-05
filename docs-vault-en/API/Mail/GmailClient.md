---
title: "GmailClient"
tags: [api, Mail]
generated: z3n7-docgen
---

# GmailClient

`class` · namespace `z3n7.Api` · source [Mail/GMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L13)

```csharp
public class GmailClient
```

*No description yet.*

## Constructors

### GmailClient

```csharp
public GmailClient(IZennoPosterProjectModel project, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L27)

## Methods

### GetLink

```csharp
public string GetLink(string targetEmail)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L177)

Ищет ссылку в последнем письме адресованном на targetEmail.

### Otp

```csharp
public string Otp(string targetEmail, int maxResults = 10)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L149)

Ищет 6-значный OTP в последних письмах адресованных на targetEmail. Бросает Exception если не найден.

### SendMail

```csharp
public void SendMail(string to, string subject, string body)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L210)

Отправляет письмо из текущего ящика.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
