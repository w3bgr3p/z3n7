---
title: "NetHttpAsync"
tags: [api, Requests]
generated: z3n7-docgen
---

# NetHttpAsync

`class` · namespace `z3n7` · source [Requests/NetHttp.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L22)

```csharp
public class NetHttpAsync
```

HTTP client on .NET `HttpClient` with async methods. Clients are shared: one for direct requests and one per proxy string (up to 100 are cached). Set-Cookie values of the response are written to the `debugCookies` variable.

## Constructors

### NetHttpAsync

```csharp
public NetHttpAsync(IZennoPosterProjectModel project, Logger log = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L43)

Creates a client. Sets the current thread culture to invariant.

| Parameter | Description |
|---|---|
| `log` | Logger for requests, responses and errors; `null` logs nothing. |

## Methods

### ClearProxyCache

```csharp
public static void ClearProxyCache()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L639)

Disposes and forgets all cached proxy clients.

### DeleteAsync

```csharp
public async Task<string> DeleteAsync(string url, string proxyString = "", Dictionary<string, string> headers = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L535)

Sends a DELETE request with a 30-second timeout.

| Parameter | Description |
|---|---|
| `url` | Request URL. |
| `proxyString` | Empty for a direct request. `+` — the `proxy` column of the account's `_instance` row; otherwise `[scheme://][user:pass@]host:port`. The proxy is always used as an HTTP proxy. |
| `headers` | Extra headers; the profile user agent is sent unless `User-Agent` is given. |

**Returns:** The trimmed body, or the error message. Never throws.

### GetAsync

```csharp
public async Task<string> GetAsync(string url, string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L174)

Sends a GET request.

| Parameter | Description |
|---|---|
| `url` | Request URL. |
| `proxyString` | Empty for a direct request. `+` — the `proxy` column of the account's `_instance` row; otherwise `[scheme://][user:pass@]host:port`. The proxy is always used as an HTTP proxy. |
| `headers` | Extra headers, sent together with the profile user agent; transport headers such as Host and Content-Length are skipped. |
| `parse` | Load the response body into `project.Json`. |
| `deadline` | Timeout in seconds; the client itself never waits longer than 30 s. |
| `throwOnFail` | Throw on a non-2xx status or an error instead of returning a message. |

**Returns:** The trimmed body. For a non-2xx status: `{code} !!! {reason}`; on timeout `Timeout: …`; on other errors `Error: …`.

### PostAsync

```csharp
public async Task<string> PostAsync(string url, string body, string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L310)

Sends a POST request with a JSON body (`application/json; charset=UTF-8`).

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

### PutAsync

```csharp
public async Task<string> PutAsync(string url, string body = "", string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L423)

Sends a PUT request; a non-empty body is sent as JSON.

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
