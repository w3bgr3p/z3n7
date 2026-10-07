---
title: "z3nmail"
tags: [api, Mail]
generated: z3n7-docgen
---

# z3nmail

`class` · namespace `z3n7.Api` · source [Mail/z3nmail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L16)

```csharp
public class z3nmail
```

Client of the temporary mailbox service (default `https://mail.autoz3n.xyz`); the same API as `BestMailBox`. A response whose `success` is not true throws with the service's error.

## Constructors

### z3nmail

```csharp
public z3nmail(IZennoPosterProjectModel project, string apikey = null, string baseUrl = null, bool useNetHttp = false, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L30)

Creates a client.

| Parameter | Description |
|---|---|
| `apikey` | API key; default `Z3NMAIL_API_KEY` from the project's `.env`. |
| `baseUrl` | Service URL; default `https://mail.autoz3n.xyz`. |
| `useNetHttp` | Send requests through `NetHttp` instead of ZennoPoster's HTTP client. |
| `log` | Log requests and responses. |

## Methods

### DeleteMail

```csharp
public bool DeleteMail(string id = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L238)

Deletes the mailbox and its messages.

| Parameter | Description |
|---|---|
| `id` | Mailbox id or address; default is the `mailId` variable, then `bestMailId`, then `email`. |

**Returns:** `true` when the service confirmed; `false` on any error or when there is no id.

### GetDomains

```csharp
public List<string> GetDomains()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L259)

Domains the service offers.

### GetHrefs

```csharp
public HashSet<string> GetHrefs(int deadline = 60, string id = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L170)

Polls every 2.5 seconds for the latest message and returns its links: the service's verification links, else all links of the HTML body. Anchors, `mailto:`, `tel:`, `javascript:`, `data:` and links to images, styles, scripts and fonts are skipped.

| Parameter | Description |
|---|---|
| `deadline` | Seconds to wait; then `TimeoutException`. |
| `id` | Mailbox id or address; default is the `mailId` variable, then `bestMailId`, then `email`. |

### GetMail

```csharp
public string GetMail(int deadline = 60, string id = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L133)

Polls every 2.5 seconds for the latest message.

| Parameter | Description |
|---|---|
| `deadline` | Seconds to wait; then `TimeoutException`. |
| `id` | Mailbox id or address; default is the `mailId` variable, then `bestMailId`, then `email`. |

**Returns:** The HTML body, or the text body when there is no HTML.

### NewMail

```csharp
public string[] NewMail(string domain = null, string prefix = null, int ttl = 1200)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L70)

Creates a mailbox. Stores the id in `mailId`, the address in `email` and `project.Profile.Email`.

| Parameter | Description |
|---|---|
| `domain` | Mailbox domain; random when `null`. |
| `prefix` | Local part of the address; generated when `null`. |
| `ttl` | Mailbox lifetime in seconds. |

**Returns:** `[id, email]`.

### Otp

```csharp
public string Otp(int deadline = 60, string id = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L99)

Polls the service every 2.5 seconds for a one-time code found by the service in the mailbox.

| Parameter | Description |
|---|---|
| `deadline` | Seconds to wait; then `TimeoutException`. |
| `id` | Mailbox id or address; default is the `mailId` variable, then `bestMailId`, then `email`. |

**Returns:** The code.

