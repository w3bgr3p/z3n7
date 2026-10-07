---
title: "MSMail"
tags: [api, Mail]
generated: z3n7-docgen
---

# MSMail

`class` · namespace `z3n7.Api` · source [Mail/MSMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L17)

```csharp
public class MSMail
```

Microsoft mailbox access over Microsoft Graph with an OAuth refresh token. Credentials are kept in the `mail` table of a `FastDb` (created if missing): the row of the address in the `mail` project variable supplies `thunderbird_client_id` and `graph_refresh_token`.

## Constructors

### MSMail

```csharp
public MSMail(IZennoPosterProjectModel project, FastDb db, string proxy = "", bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L40)

Creates the client. When the `mail` variable is set, loads its credentials and gets an access token right away; throws when the table has no credentials for it or the token request fails.

| Parameter | Description |
|---|---|
| `db` | Database with the `mail` table. |
| `proxy` | Proxy for Graph requests (`Rqst` format). The token request itself is sent directly. |
| `log` | Log requests and responses. |

## Methods

### CleanAll

```csharp
public void CleanAll(int batchSize = 50)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L242)

Deletes all messages, `batchSize` at a time.

| Parameter | Description |
|---|---|
| `batchSize` | Messages fetched per round. |

### Delete

```csharp
public string Delete(string endpoint)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L122)

Sends a DELETE request to Microsoft Graph.

| Parameter | Description |
|---|---|
| `endpoint` | Path under `https://graph.microsoft.com/v1.0/`, e.g. `me/messages`. |

**Returns:** Response body. Throws on a non-2xx status.

### DelLast

```csharp
public void DelLast()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L221)

Deletes the newest message, if there is one.

### Get

```csharp
public string Get(string endpoint)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L98)

Sends a GET request to Microsoft Graph.

| Parameter | Description |
|---|---|
| `endpoint` | Path under `https://graph.microsoft.com/v1.0/`, e.g. `me/messages`. |

**Returns:** Response body. Throws on a non-2xx status.

### GetMessages

```csharp
public JArray GetMessages(int top = 10)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L132)

Mailbox messages, newest first.

| Parameter | Description |
|---|---|
| `top` | How many. |

### ImportFromJson

```csharp
public void ImportFromJson(string json)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L291)

Adds mailboxes to the `mail` table from a JSON array; existing addresses are left as they are.

| Parameter | Description |
|---|---|
| `json` | Array of objects with `email`, `password`, `access_token`, `refresh_token`, `thunderbird_client_id`, `graph_access_token`, `graph_refresh_token`. |

### Post

```csharp
public string Post(string endpoint, string jsonBody)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L110)

Sends a POST request with a JSON body to Microsoft Graph.

| Parameter | Description |
|---|---|
| `endpoint` | Path under `https://graph.microsoft.com/v1.0/`, e.g. `me/messages`. |
| `jsonBody` | JSON body. |

**Returns:** Response body. Throws on a non-2xx status.

### SelfCheck

```csharp
public bool SelfCheck(int timeoutSeconds = 30, int checkIntervalSeconds = 3)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L170)

Sends a message with a unique subject to the mailbox itself and waits until it shows up among the latest 20 messages.

| Parameter | Description |
|---|---|
| `timeoutSeconds` | How long to wait. |
| `checkIntervalSeconds` | Pause between checks. |

**Returns:** `true` when the message arrived in time.

### SendMail

```csharp
public void SendMail(string toEmail, string subject, string body)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L143)

Sends a plain-text message.

| Parameter | Description |
|---|---|
| `toEmail` | Recipient. |
| `subject` | Subject. |
| `body` | Text. |

