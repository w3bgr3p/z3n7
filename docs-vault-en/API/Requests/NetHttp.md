---
title: "NetHttp"
tags: [api, Requests]
generated: z3n7-docgen
---

# NetHttp

`class` · namespace `z3n7` · source [Requests/NetHttp.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L657)

```csharp
public class NetHttp
```

Blocking wrapper over `NetHttpAsync` for C# actions that cannot await.

## Constructors

### NetHttp

```csharp
public NetHttp(IZennoPosterProjectModel project, Logger log = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L663)

Creates a client.

| Parameter | Description |
|---|---|
| `log` | Logger for requests, responses and errors; `null` logs nothing. |

## Methods

### DELETE

```csharp
public string DELETE(string url, string proxyString = "", Dictionary<string, string> headers = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L772)

Sends a DELETE request and waits for it. See `NetHttpAsync.DeleteAsync`.

**Returns:** The trimmed body, or the error message.

### GET

```csharp
public string GET(string url, string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L685)

Sends a GET request and waits for it. See `NetHttpAsync.GetAsync`.

| Parameter | Description |
|---|---|
| `url` | Request URL. |
| `proxyString` | Empty for a direct request. `+` — the `proxy` column of the account's `_instance` row; otherwise `[scheme://][user:pass@]host:port`. The proxy is always used as an HTTP proxy. |
| `headers` | Extra headers, sent together with the profile user agent; transport headers such as Host and Content-Length are skipped. |
| `parse` | Load the response body into `project.Json`. |
| `deadline` | Timeout in seconds; the client itself never waits longer than 30 s. |
| `throwOnFail` | Throw on a non-2xx status or an error instead of returning a message. |

**Returns:** The trimmed body. For a non-2xx status: `{code} !!! {reason}`; on timeout `Timeout: …`; on other errors `Error: …`.

### POST

```csharp
public string POST(string url, string body, string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L720)

Sends a POST request with a JSON body and waits for it. See `NetHttpAsync.PostAsync`.

| Parameter | Description |
|---|---|
| `url` | Request URL. |
| `body` | JSON body. |
| `proxyString` | Empty for a direct request. `+` — the `proxy` column of the account's `_instance` row; otherwise `[scheme://][user:pass@]host:port`. The proxy is always used as an HTTP proxy. |
| `headers` | Extra headers, sent together with the profile user agent; transport headers such as Host and Content-Length are skipped. |
| `parse` | Load the response body into `project.Json`. |
| `deadline` | Timeout in seconds; the client itself never waits longer than 30 s. |
| `throwOnFail` | Throw on a non-2xx status or an error instead of returning a message. |

**Returns:** The trimmed body. For a non-2xx status: `{code} !!! {reason}`; on timeout `Timeout: …`; on other errors `Error: …`.

### PUT

```csharp
public string PUT(string url, string body = "", string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L754)

Sends a PUT request and waits for it. See `NetHttpAsync.PutAsync`.

| Parameter | Description |
|---|---|
| `url` | Request URL. |
| `body` | JSON body; may be empty. |
| `proxyString` | Empty for a direct request. `+` — the `proxy` column of the account's `_instance` row; otherwise `[scheme://][user:pass@]host:port`. The proxy is always used as an HTTP proxy. |
| `headers` | Extra headers, sent together with the profile user agent; transport headers such as Host and Content-Length are skipped. |
| `parse` | Load the response body into `project.Json`. |
| `deadline` | Timeout in seconds; the client itself never waits longer than 30 s. |
| `throwOnFail` | Throw on a non-2xx status or an error instead of returning a message. |

**Returns:** The trimmed body. For a non-2xx status: `{code} !!! {reason}`; on timeout `Timeout: …`; on other errors `Error: …`.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
