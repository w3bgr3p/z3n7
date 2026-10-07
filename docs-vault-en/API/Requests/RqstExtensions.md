---
title: "RqstExtensions"
tags: [api, Requests]
generated: z3n7-docgen
---

# RqstExtensions

`static class` · namespace `z3n7` · source [Requests/Rqst.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L799)

```csharp
public static class RqstExtensions
```

Shortcuts that create an `Rqst` for one request.

## Methods

### DELETE

```csharp
public static string DELETE(this IZennoPosterProjectModel project, string url, string proxy = "", string[] headers = null, string cookies = null, bool log = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L962)

Sends a DELETE request with a new `Rqst`; `log` also enables its logging.

| Parameter | Description |
|---|---|
| `url` | Request URL. |
| `proxy` | Empty for none. `+` — the `proxy` variable, else the `proxy` column of the account's `_instance` row; `z` — the `z_proxy` column of that row; otherwise `[scheme://][user:pass@]host:port` (scheme defaults to http). |
| `headers` | `Name: value` lines. When empty, the lines of the `headers` variable are used. User-Agent and Content-Type lines set those values (defaults: the profile user agent and `application/json`); Host, Connection, Content-Length and similar transport headers are dropped. |
| `cookies` | `-` — no cookies. Any other text is sent as the Cookie header. Empty — cookies for the URL's domain from the `cookies` variable (JSON array); when it is empty and `acc0` and `dbSource` are set, from the Base64 `cookies` column of the `_instance` row (also stored into the variable); when nothing is found, the profile cookie container. |
| `log` | Write the response body to the log. Effective only when the `Rqst` was created with `log: true`. |
| `deadline` | Timeout in seconds. |
| `thrw` | Throw on a non-2xx status or a transport error. |
| `useNetHttp` | Send through `NetHttp` (.NET HttpClient) instead of ZennoPoster's HTTP client. |
| `returnSuccessWithStatus` | Return `{status}\r\n\r\n{body}` for any status, without the non-2xx handling. |

**Returns:** The trimmed response body. For a non-2xx status: the body (or an exception with `thrw`). For a transport error: `Error: {message}` (or the exception with `thrw`).

### GET

```csharp
public static string GET(this IZennoPosterProjectModel project, string url, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L834)

Sends a GET request with a new `Rqst`; `log` also enables its logging.

| Parameter | Description |
|---|---|
| `url` | Request URL. |
| `proxy` | Empty for none. `+` — the `proxy` variable, else the `proxy` column of the account's `_instance` row; `z` — the `z_proxy` column of that row; otherwise `[scheme://][user:pass@]host:port` (scheme defaults to http). |
| `headers` | `Name: value` lines. When empty, the lines of the `headers` variable are used. User-Agent and Content-Type lines set those values (defaults: the profile user agent and `application/json`); Host, Connection, Content-Length and similar transport headers are dropped. |
| `cookies` | `-` — no cookies. Any other text is sent as the Cookie header. Empty — cookies for the URL's domain from the `cookies` variable (JSON array); when it is empty and `acc0` and `dbSource` are set, from the Base64 `cookies` column of the `_instance` row (also stored into the variable); when nothing is found, the profile cookie container. |
| `log` | Write the response body to the log. Effective only when the `Rqst` was created with `log: true`. |
| `parse` | Load the response body into `project.Json`. |
| `deadline` | Timeout in seconds. |
| `thrw` | Throw on a non-2xx status or a transport error. |
| `useNetHttp` | Send through `NetHttp` (.NET HttpClient) instead of ZennoPoster's HTTP client. |
| `returnSuccessWithStatus` | Return `{status}\r\n\r\n{body}` for any status, without the non-2xx handling. |
| `bodyOnly` | Ask ZennoPoster for the body only; response headers are then not recorded. |

**Returns:** The trimmed response body. For a non-2xx status: the body (or an exception with `thrw`). For a transport error: `Error: {message}` (or the exception with `thrw`).

### POST

```csharp
public static string POST(this IZennoPosterProjectModel project, string url, string body, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L878)

Sends a POST request with a new `Rqst`; `log` also enables its logging.

| Parameter | Description |
|---|---|
| `url` | Request URL. |
| `body` | Request body. |
| `proxy` | Empty for none. `+` — the `proxy` variable, else the `proxy` column of the account's `_instance` row; `z` — the `z_proxy` column of that row; otherwise `[scheme://][user:pass@]host:port` (scheme defaults to http). |
| `headers` | `Name: value` lines. When empty, the lines of the `headers` variable are used. User-Agent and Content-Type lines set those values (defaults: the profile user agent and `application/json`); Host, Connection, Content-Length and similar transport headers are dropped. |
| `cookies` | `-` — no cookies. Any other text is sent as the Cookie header. Empty — cookies for the URL's domain from the `cookies` variable (JSON array); when it is empty and `acc0` and `dbSource` are set, from the Base64 `cookies` column of the `_instance` row (also stored into the variable); when nothing is found, the profile cookie container. |
| `log` | Write the response body to the log. Effective only when the `Rqst` was created with `log: true`. |
| `parse` | Load the response body into `project.Json`. |
| `deadline` | Timeout in seconds. |
| `thrw` | Throw on a non-2xx status or a transport error. |
| `useNetHttp` | Send through `NetHttp` (.NET HttpClient) instead of ZennoPoster's HTTP client. |
| `returnSuccessWithStatus` | Return `{status}\r\n\r\n{body}` for any status, without the non-2xx handling. |
| `bodyOnly` | Ask ZennoPoster for the body only; response headers are then not recorded. |

**Returns:** The trimmed response body. For a non-2xx status: the body (or an exception with `thrw`). For a transport error: `Error: {message}` (or the exception with `thrw`).

### PUT

```csharp
public static string PUT(this IZennoPosterProjectModel project, string url, string body, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L921)

Sends a PUT request with a new `Rqst`; `log` also enables its logging.

| Parameter | Description |
|---|---|
| `url` | Request URL. |
| `body` | Request body. |
| `proxy` | Empty for none. `+` — the `proxy` variable, else the `proxy` column of the account's `_instance` row; `z` — the `z_proxy` column of that row; otherwise `[scheme://][user:pass@]host:port` (scheme defaults to http). |
| `headers` | `Name: value` lines. When empty, the lines of the `headers` variable are used. User-Agent and Content-Type lines set those values (defaults: the profile user agent and `application/json`); Host, Connection, Content-Length and similar transport headers are dropped. |
| `cookies` | `-` — no cookies. Any other text is sent as the Cookie header. Empty — cookies for the URL's domain from the `cookies` variable (JSON array); when it is empty and `acc0` and `dbSource` are set, from the Base64 `cookies` column of the `_instance` row (also stored into the variable); when nothing is found, the profile cookie container. |
| `log` | Write the response body to the log. Effective only when the `Rqst` was created with `log: true`. |
| `parse` | Load the response body into `project.Json`. |
| `deadline` | Timeout in seconds. |
| `thrw` | Throw on a non-2xx status or a transport error. |
| `useNetHttp` | Send through `NetHttp` (.NET HttpClient) instead of ZennoPoster's HTTP client. |
| `returnSuccessWithStatus` | Return `{status}\r\n\r\n{body}` for any status, without the non-2xx handling. |

**Returns:** The trimmed response body. For a non-2xx status: the body (or an exception with `thrw`). For a transport error: `Error: {message}` (or the exception with `thrw`).

