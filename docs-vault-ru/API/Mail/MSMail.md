---
title: "MSMail"
tags: [api, Mail]
generated: z3n7-docgen
---

# MSMail

`class` · пространство имён `z3n7.Api` · исходник [Mail/MSMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L17)

```csharp
public class MSMail
```

Microsoft mailbox access over Microsoft Graph with an OAuth refresh token. Credentials are kept in the `mail` table of a `FastDb` (created if missing): the row of the address in the `mail` project variable supplies `thunderbird_client_id` and `graph_refresh_token`.

## Конструкторы

### MSMail

```csharp
public MSMail(IZennoPosterProjectModel project, FastDb db, string proxy = "", bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L40)

Creates the client. When the `mail` variable is set, loads its credentials and gets an access token right away; throws when the table has no credentials for it or the token request fails.

| Параметр | Описание |
|---|---|
| `db` | Database with the `mail` table. |
| `proxy` | Proxy for Graph requests (`Rqst` format). The token request itself is sent directly. |
| `log` | Log requests and responses. |

## Методы

### CleanAll

```csharp
public void CleanAll(int batchSize = 50)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L242)

Deletes all messages, `batchSize` at a time.

| Параметр | Описание |
|---|---|
| `batchSize` | Messages fetched per round. |

### Delete

```csharp
public string Delete(string endpoint)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L122)

Sends a DELETE request to Microsoft Graph.

| Параметр | Описание |
|---|---|
| `endpoint` | Path under `https://graph.microsoft.com/v1.0/`, e.g. `me/messages`. |

**Возвращает:** Response body. Throws on a non-2xx status.

### DelLast

```csharp
public void DelLast()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L221)

Deletes the newest message, if there is one.

### Get

```csharp
public string Get(string endpoint)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L98)

Sends a GET request to Microsoft Graph.

| Параметр | Описание |
|---|---|
| `endpoint` | Path under `https://graph.microsoft.com/v1.0/`, e.g. `me/messages`. |

**Возвращает:** Response body. Throws on a non-2xx status.

### GetMessages

```csharp
public JArray GetMessages(int top = 10)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L132)

Mailbox messages, newest first.

| Параметр | Описание |
|---|---|
| `top` | How many. |

### ImportFromJson

```csharp
public void ImportFromJson(string json)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L291)

Adds mailboxes to the `mail` table from a JSON array; existing addresses are left as they are.

| Параметр | Описание |
|---|---|
| `json` | Array of objects with `email`, `password`, `access_token`, `refresh_token`, `thunderbird_client_id`, `graph_access_token`, `graph_refresh_token`. |

### Post

```csharp
public string Post(string endpoint, string jsonBody)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L110)

Sends a POST request with a JSON body to Microsoft Graph.

| Параметр | Описание |
|---|---|
| `endpoint` | Path under `https://graph.microsoft.com/v1.0/`, e.g. `me/messages`. |
| `jsonBody` | JSON body. |

**Возвращает:** Response body. Throws on a non-2xx status.

### SelfCheck

```csharp
public bool SelfCheck(int timeoutSeconds = 30, int checkIntervalSeconds = 3)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L170)

Sends a message with a unique subject to the mailbox itself and waits until it shows up among the latest 20 messages.

| Параметр | Описание |
|---|---|
| `timeoutSeconds` | How long to wait. |
| `checkIntervalSeconds` | Pause between checks. |

**Возвращает:** `true` when the message arrived in time.

### SendMail

```csharp
public void SendMail(string toEmail, string subject, string body)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L143)

Sends a plain-text message.

| Параметр | Описание |
|---|---|
| `toEmail` | Recipient. |
| `subject` | Subject. |
| `body` | Text. |

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
