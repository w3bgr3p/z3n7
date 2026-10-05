---
title: "GmailClient"
tags: [api, Mail]
generated: z3n7-docgen
---

# GmailClient

`class` · пространство имён `z3n7.Api` · исходник [Mail/GMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L18)

```csharp
public class GmailClient
```

Gmail access over the Gmail API with an OAuth refresh token. Credentials come from the `_api` table, row `id = 'gmail'`: `client_id`, `client_secret`, `refresh_token`. A new access token is requested before every operation.

## Конструкторы

### GmailClient

```csharp
public GmailClient(IZennoPosterProjectModel project, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L34)

Creates the client and reads the credentials from the database.

| Параметр | Описание |
|---|---|
| `log` | Log requests and responses. |

## Методы

### GetLink

```csharp
public string GetLink(string targetEmail)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L190)

Returns the first http(s) link in the plain-text body of the newest of the last 5 messages (within 5 minutes) sent to `targetEmail`.

| Параметр | Описание |
|---|---|
| `targetEmail` | Address the message must be sent to (matched against the `To` header). |

**Возвращает:** The link. Throws when none is found.

### Otp

```csharp
public string Otp(string targetEmail, int maxResults = 10)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L159)

Looks through messages of the last 5 minutes sent to `targetEmail` and returns the first 6-digit number of the subject, else of the plain-text body.

| Параметр | Описание |
|---|---|
| `targetEmail` | Address the message must be sent to (matched against the `To` header). |
| `maxResults` | How many recent messages to check. |

**Возвращает:** The code. Throws when none is found.

### SendMail

```csharp
public void SendMail(string to, string subject, string body)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L224)

Sends a plain-text message from this mailbox.

| Параметр | Описание |
|---|---|
| `to` | Recipient. |
| `subject` | Subject. |
| `body` | Text. |

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
