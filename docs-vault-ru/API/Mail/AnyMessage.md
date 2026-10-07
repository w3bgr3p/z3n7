---
title: "AnyMessage"
tags: [api, Mail]
generated: z3n7-docgen
---

# AnyMessage

`class` · пространство имён `z3n7.Api` · исходник [Mail/AnyMessage.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L21)

```csharp
public class AnyMessage
```

Client of the AnyMessage mailbox service (`api.anymessage.shop`): short-term and long-term mailboxes. State is kept in project variables: `anyMailId` for the current short-term order, `anyLLId` for the long-term one. A response whose `status` is not `success` throws with the service's message.

## Конструкторы

### AnyMessage

```csharp
public AnyMessage(IZennoPosterProjectModel project, string apikey, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L30)

Creates a client.

| Параметр | Описание |
|---|---|
| `apikey` | AnyMessage API token. |
| `log` | Log requests and responses. |

## Свойства

### LastDomain

```csharp
public string LastDomain { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L55)

Domain of the last ordered mailbox; may differ from the requested one after a fallback.

## Методы

### Balance

```csharp
public string Balance()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L421)

Account balance as returned by the service.

### Cancel

```csharp
public void Cancel()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L372)

Cancels the order in `anyMailId`.

### CheapestDomains

```csharp
public List<string> CheapestDomains(string site, params string[] exclude)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L90)

Domains with mailboxes available, cheapest first; at equal price the one with more mailboxes first.

| Параметр | Описание |
|---|---|
| `site` | Target site. |
| `exclude` | Domains to leave out. |

### GetHrefs

```csharp
public List<string> GetHrefs(int deadline = 60)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L252)

Waits for a message and collects its unique links, skipping anchors, `mailto:`, `tel:`, `javascript:`, `data:` and links to images, styles, scripts and fonts.

| Параметр | Описание |
|---|---|
| `deadline` | Seconds to wait for the message. |

### GetLastMessages

```csharp
public string GetLastMessages(string subject = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L408)

Recent messages of the long-term mailbox in `anyLLId`.

| Параметр | Описание |
|---|---|
| `subject` | When set, only messages with this subject. |

**Возвращает:** The raw JSON answer.

### GetMail

```csharp
public string GetMail(int deadline = 120)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L175)

Waits for a message to the mailbox in `anyMailId`, polling every 5 seconds.

| Параметр | Описание |
|---|---|
| `deadline` | Seconds to wait; then `TimeoutException`. |

**Возвращает:** The message body (HTML).

### Href

```csharp
public string Href(int hrefIndex = 0, int deadline = 60)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L235)

Returns one link from the message (see `GetHrefs`).

| Параметр | Описание |
|---|---|
| `hrefIndex` | Index of the link; -1 writes all links to the log and returns an empty string. |
| `deadline` | Seconds to wait for the message. |

### LinkByRegex

```csharp
public string LinkByRegex(string urlPattern)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L344)

Waits for a message and returns the first match of `urlPattern` in its HTML. Throws when there is none.

| Параметр | Описание |
|---|---|
| `urlPattern` | Regular expression. |

### NewMail

```csharp
public string[] NewMail(string site, string domain = "outlook.com", bool fallback = true, int maxFallback = 5)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L131)

Orders a short-term mailbox. Stores the id in `anyMailId` and the address in `email` and `project.Profile.Email`.

| Параметр | Описание |
|---|---|
| `site` | Target site, e.g. `instagram.com`. |
| `domain` | Mailbox domain. |
| `fallback` | When the service answers "no emails", try the cheapest available domains instead. The domain used ends up in `LastDomain`. |
| `maxFallback` | How many fallback domains to try. |

**Возвращает:** `[id, email]`. Throws with every attempt's error when nothing could be ordered.

### OrderLongLive

```csharp
public string[] OrderLongLive(string site, string domain)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L388)

Buys a long-term mailbox. Stores the id in `anyLLId` and the address in `anyLLEmail`.

| Параметр | Описание |
|---|---|
| `site` | Target site, e.g. `instagram.com`. |
| `domain` | Mailbox domain, e.g. `hotmail.com`. |

**Возвращает:** `[id, email, imapPassword, imapHost, imapPort]` of the first mailbox in the answer.

### Otp

```csharp
public string Otp(int matchIndex = 0)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L203)

Waits for a message and extracts a 6-digit code; stores it in `mailOtp`.

| Параметр | Описание |
|---|---|
| `matchIndex` | Which 6-digit number of the message to take; -1 writes all of them to the log and returns an empty string. |

**Возвращает:** The code. Throws when no code is found after 10 attempts.

### Quantity

```csharp
public Dictionary<string, (int Count, double Price)> Quantity(string site)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L60)

Available domains for a site (`/email/quantity`).

| Параметр | Описание |
|---|---|
| `site` | Target site, e.g. `instagram.com`. |

**Возвращает:** Domain → (mailboxes available, price). Domains without a price are left out.

### Reorder

```csharp
public string[] Reorder()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L357)

Orders the mailbox in `anyMailId` again under a new id; updates `anyMailId` and `email`.

**Возвращает:** `[id, email]`.

