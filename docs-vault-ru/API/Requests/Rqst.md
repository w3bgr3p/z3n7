---
title: "Rqst"
tags: [api, Requests]
generated: z3n7-docgen
---

# Rqst

`class` · пространство имён `z3n7` · исходник [Requests/Rqst.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L20)

```csharp
public class Rqst
```

HTTP client for ZennoPoster projects: proxy, headers and cookies are taken from the project when not given. Requests go through ZennoPoster's HTTP client by default, serialised by a lock shared by all `Rqst` instances. Every request is appended as a JSON line to the traffic file (`ZpTraffic`).

## Конструкторы

### Rqst

```csharp
public Rqst(IZennoPosterProjectModel project, bool log = false, bool mask = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L31)

Creates a client for the project.

| Параметр | Описание |
|---|---|
| `log` | Log responses and request errors. Without it, nothing is written to the log. |
| `mask` | Not used. |

## Методы

### DELETE

```csharp
public string DELETE(string url, string proxy = "", string[] headers = null, string cookies = null, bool log = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L226)

Sends a DELETE request.

| Параметр | Описание |
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
| `bodyOnly` | Ask ZennoPoster for the body only; response headers are then not recorded. |

**Возвращает:** The trimmed response body. For a non-2xx status: the body (or an exception with `thrw`). For a transport error: `Error: {message}` (or the exception with `thrw`).

### GET

```csharp
public string GET(string url, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L100)

Sends a GET request.

| Параметр | Описание |
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

**Возвращает:** The trimmed response body. For a non-2xx status: the body (or an exception with `thrw`). For a transport error: `Error: {message}` (or the exception with `thrw`).

### POST

```csharp
public string POST(string url, string body, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L142)

Sends a POST request.

| Параметр | Описание |
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

**Возвращает:** The trimmed response body. For a non-2xx status: the body (or an exception with `thrw`). For a transport error: `Error: {message}` (or the exception with `thrw`).

### PUT

```csharp
public string PUT(string url, string body, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L185)

Sends a PUT request.

| Параметр | Описание |
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

**Возвращает:** The trimmed response body. For a non-2xx status: the body (or an exception with `thrw`). For a transport error: `Error: {message}` (or the exception with `thrw`).

