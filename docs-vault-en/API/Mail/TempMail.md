---
title: "TempMail"
tags: [api, Mail]
generated: z3n7-docgen
---

# TempMail

`class` · namespace `z3n7.Api` · source [Mail/TempMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L19)

```csharp
public class TempMail
```

Client of the Temp Mail service (Privatix) on RapidAPI. The mailbox id is the MD5 of the address; it is kept in `tempMailId`.

## Constructors

### TempMail

```csharp
public TempMail(IZennoPosterProjectModel project, string apikey, bool log = false, bool useNetHttp = false, string proxy = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L35)

Creates a client.

| Parameter | Description |
|---|---|
| `apikey` | RapidAPI key; required. |
| `log` | Log requests and responses. |
| `useNetHttp` | Send requests through `NetHttp` instead of ZennoPoster's HTTP client. |
| `proxy` | Proxy in `Rqst` format; empty for none. |

## Methods

### CreateAddress

```csharp
public static string CreateAddress(string login, string domain)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L216)

Joins login and domain into an address.

### GetDomains

```csharp
public string[] GetDomains()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L68)

Domains the service offers.

**Returns:** Throws when the list is empty.

### GetHrefs

```csharp
public HashSet<string> GetHrefs(int deadline = 120)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L177)

Waits for a message and collects the links of its HTML, skipping anchors, `mailto:`, `tel:`, `javascript:`, `data:` and links to images, styles, scripts and fonts.

| Parameter | Description |
|---|---|
| `deadline` | Seconds to wait; then `TimeoutException`. |

### GetMail

```csharp
public string GetMail(int deadline = 120)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L129)

Waits for a message, checking every 5 seconds.

| Parameter | Description |
|---|---|
| `deadline` | Seconds to wait; then `TimeoutException`. |

**Returns:** JSON of the first message.

### GetMessages

```csharp
public string GetMessages()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L114)

Current messages of the mailbox in `tempMailId` (else `mailId`), without waiting.

**Returns:** The raw JSON answer. Throws when no mailbox was created.

### HashEmail

```csharp
public static string HashEmail(string email)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L227)

MD5 of the lower-cased address, as hex: the mailbox id used by the service.

### Link

```csharp
public string Link(string urlPattern, int deadline = 120)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L206)

Waits for a message and returns the first match of `urlPattern` in its JSON, HTML-decoded.

| Parameter | Description |
|---|---|
| `urlPattern` | Regular expression. |
| `deadline` | Seconds to wait; then `TimeoutException`. |

**Returns:** The match. Throws when there is none.

### NewMail

```csharp
public string[] NewMail(string login = null, string domain = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L87)

Builds an address on a service domain; no request creates it, the service accepts mail for any login. Stores the id in `tempMailId` and `mailId`, the address in `email` and `project.Profile.Email`.

| Parameter | Description |
|---|---|
| `login` | Local part; random 10 hex characters when empty. |
| `domain` | Domain; a random service domain when empty. |

**Returns:** `[md5, email]`.

### Otp

```csharp
public string Otp(int deadline = 120)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L154)

Waits for a message and returns the first 6-digit number of its subject, else of its text.

| Parameter | Description |
|---|---|
| `deadline` | Seconds to wait; then `TimeoutException`. |

**Returns:** The code. Throws when there is none.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
