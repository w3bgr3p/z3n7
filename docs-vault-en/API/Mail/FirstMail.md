---
title: "FirstMail"
tags: [api, Mail]
generated: z3n7-docgen
---

# FirstMail

`class` · namespace `z3n7` · source [Mail/FirstMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L19)

```csharp
public class FirstMail
```

Client of the FirstMail mailbox API (`firstmail.ltd`). API key, default login, password and proxy come from the `_api` table, row `id = 'firstmail'` (`apikey`, `apisecret`, `passphrase`, `proxy`). Responses are loaded into `project.Json`. The client signs in to the mailbox that receives forwarded mail. An `email` argument is the original recipient: the address the message was sent to, which forwarded it to this mailbox.

## Constructors

### FirstMail

```csharp
public FirstMail(IZennoPosterProjectModel project, Logger log = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L44)

Creates a client for the mailbox stored in the database.

| Parameter | Description |
|---|---|
| `log` | Not used. |

```csharp
public FirstMail(IZennoPosterProjectModel project, string mail, string password, Logger log = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L56)

Creates a client for the given mailbox; the API key and proxy still come from the database.

| Parameter | Description |
|---|---|
| `mail` | Mailbox address. |
| `password` | Mailbox password. |
| `log` | Not used. |

## Methods

### Delete

```csharp
public string Delete(string email, bool seen = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L81)

Calls `/v1/mail/delete` for the client's mailbox.

| Parameter | Description |
|---|---|
| `email` | Original recipient; not used. |
| `seen` | Append `seen=true` to the request URL. |

### Get

```csharp
public string Get(int limit = 5)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L110)

Latest INBOX messages of the client's mailbox (`/api/v1/email/messages`).

| Parameter | Description |
|---|---|
| `limit` | How many. |

**Returns:** JSON array of messages.

### GetAll

```csharp
public string GetAll(string email)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L100)

Messages of the client's mailbox (`/v1/get/messages`).

| Parameter | Description |
|---|---|
| `email` | Original recipient; not used. |

### GetLink

```csharp
public string GetLink(string email)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L164)

Takes the latest message; when its first recipient contains `email`, returns the first http(s) link of its text.

| Parameter | Description |
|---|---|
| `email` | Original recipient the message was sent to. |

**Returns:** The link. Throws when there is none.

### GetOne

```csharp
public string GetOne(string email)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L91)

Latest message of the client's mailbox (`/v1/mail/one`).

| Parameter | Description |
|---|---|
| `email` | Original recipient; not used. |

### GetOTP

```csharp
public string GetOTP(string email)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L129)

Takes the latest message; when its first recipient contains `email`, returns the first 6-digit number of the subject, text or HTML.

| Parameter | Description |
|---|---|
| `email` | Original recipient the message was sent to. |

**Returns:** The code. Throws when the latest message is for another address or has no code.

### Otp

```csharp
public string Otp(string email)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L193)

Looks through the latest 5 INBOX messages for one sent to `email` and returns the first 6-digit number of its subject, text or HTML.

| Parameter | Description |
|---|---|
| `email` | Original recipient the message was sent to. |

**Returns:** The code. Throws when none is found.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
