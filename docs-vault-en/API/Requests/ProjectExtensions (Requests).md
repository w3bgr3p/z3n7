---
title: "ProjectExtensions (Requests)"
tags: [api, Requests]
generated: z3n7-docgen
---

# ProjectExtensions (Requests)

`static class` · namespace `z3n7` · source [Requests/NetHttp.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L786)

```csharp
public static class ProjectExtensions
```

Other parts of this type: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Traffic)]]

Project shortcuts for `NetHttp` requests.

## Methods

### NetGet

```csharp
public static string NetGet(this IZennoPosterProjectModel project, string url, string proxyString = "", string[] headers = null, bool parse = false, int deadline = 15, bool thrw = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L834)

Sends a GET request through `NetHttp` without logging.

| Parameter | Description |
|---|---|
| `url` | Request URL. |
| `proxyString` | Empty for a direct request. `+` — the `proxy` column of the account's `_instance` row; otherwise `[scheme://][user:pass@]host:port`. The proxy is always used as an HTTP proxy. |
| `headers` | `Name: value` lines. |
| `parse` | Load the response body into `project.Json`. |
| `deadline` | Timeout in seconds; the client itself never waits longer than 30 s. |
| `thrw` | Throw on a non-2xx status or an error instead of returning a message. |

**Returns:** The trimmed body. For a non-2xx status: `{code} !!! {reason}`; on timeout `Timeout: …`; on other errors `Error: …`.

### NetPost

```csharp
public static string NetPost(this IZennoPosterProjectModel project, string url, string body, string proxyString = "", string[] headers = null, bool parse = false, int deadline = 15, bool thrw = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L860)

Sends a POST request with a JSON body through `NetHttp` without logging.

| Parameter | Description |
|---|---|
| `url` | Request URL. |
| `body` | JSON body. |
| `proxyString` | Empty for a direct request. `+` — the `proxy` column of the account's `_instance` row; otherwise `[scheme://][user:pass@]host:port`. The proxy is always used as an HTTP proxy. |
| `headers` | `Name: value` lines. |
| `parse` | Load the response body into `project.Json`. |
| `deadline` | Timeout in seconds; the client itself never waits longer than 30 s. |
| `thrw` | Throw on a non-2xx status or an error instead of returning a message. |

**Returns:** The trimmed body. For a non-2xx status: `{code} !!! {reason}`; on timeout `Timeout: …`; on other errors `Error: …`.

