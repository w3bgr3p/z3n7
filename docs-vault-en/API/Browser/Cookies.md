---
title: "Cookies"
tags: [api, Browser]
generated: z3n7-docgen
---

# Cookies

`static class` · namespace `z3n7` · source [Browser/Cookies.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L19)

```csharp
public static class Cookies
```

Browser cookies: read and write in an instance, store as Base64 in the account's database row, convert between JSON and Netscape formats.

## Methods

### AnalyzeCookies

```csharp
public static CookieInfo AnalyzeCookies(this IZennoPosterProjectModel project, string table = "_instance", string column = "cookies")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L44)

Reads the current account's stored cookies and summarises them.

| Parameter | Description |
|---|---|
| `table` | Table with the account's cookies. |
| `column` | Column with Base64 cookies (JSON or Netscape). |

**Returns:** The summary; zero counts when nothing is stored.

### CleanDomainInDb

```csharp
public static void CleanDomainInDb(this IZennoPosterProjectModel project, string domain, string table = "_instance", string column = "cookies")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L489)

Removes cookies of a domain (and, for cookies stored with a leading dot, its subdomains) from the current account's stored set.

| Parameter | Description |
|---|---|
| `domain` | Domain, e.g. `x.com`. |
| `table` | Table with the account's cookies. |
| `column` | Column with Base64 cookies (JSON or Netscape). |

### ConvertCookieFormat

```csharp
public static string ConvertCookieFormat(string input, string output = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L167)

Converts cookies between JSON (browser-extension format) and Netscape (tab-separated). Text starting with `[` or `{` is JSON; text with tabs is Netscape.

| Parameter | Description |
|---|---|
| `input` | Cookies. |
| `output` | `json`, `netscape`, or empty to convert to the other format. |

**Returns:** The converted text, or the input when it is already in the requested format. Throws on an unknown format.

### GetCookies

```csharp
public static string GetCookies(this Instance instance, string domainFilter = null, string format = "json")
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L311)

Reads the instance's cookies.

| Parameter | Description |
|---|---|
| `domainFilter` | Only cookies of this domain; `.` for the active tab's main domain; empty for all. |
| `format` | `json`, `netscape`, `base64Json` or `base64Netscape`. |

### GetCookiesByJs

```csharp
public static string GetCookiesByJs(this Instance instance)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L393)

Reads `document.cookie` of the active page as JSON (path `/`, no expiry; HttpOnly cookies are not visible to scripts).

### ParseJwt

```csharp
public static Dictionary<string, object> ParseJwt(string jwt)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L593)

Decodes a JWT without checking its signature.

**Returns:** `alg`, `typ`, `kid`, `iss`, `sub`, `aud`, `iat`/`exp` with dates, `ttl_seconds`, `is_expired`, raw header and payload JSON and the signature; or `error`.

### PrintCookieReport

```csharp
public static void PrintCookieReport(this IZennoPosterProjectModel project, string table = "_instance", string column = "cookies")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L132)

Writes the `AnalyzeCookies` summary to the log (top 10 domains, top 5 largest cookies).

| Parameter | Description |
|---|---|
| `table` | Table with the account's cookies. |
| `column` | Column with Base64 cookies (JSON or Netscape). |

### PruneAllCookies

```csharp
public static void PruneAllCookies(this IZennoPosterProjectModel project, bool removeExpired = true, bool removeOld = true, bool removeNonGoogle = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L114)

Runs `PruneCookies` on the `_instance` and `folder_profile` tables for every account from `rangeStart` to `rangeEnd`, then clears `acc0`.

| Parameter | Description |
|---|---|
| `removeExpired` | Remove expired cookies. |
| `removeOld` | Remove cookies that expired more than 6 months ago. |
| `removeNonGoogle` | Keep only cookies whose domain contains `google`. |

### PruneCookies

```csharp
public static void PruneCookies(this IZennoPosterProjectModel project, bool removeExpired = true, bool removeOld = true, bool removeNonGoogle = false, string table = "_instance", string column = "cookies")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L75)

Removes cookies from the current account's stored set and writes it back as Base64 JSON.

| Parameter | Description |
|---|---|
| `removeExpired` | Remove expired cookies. |
| `removeOld` | Remove cookies that expired more than 6 months ago. |
| `removeNonGoogle` | Keep only cookies whose domain contains `google`. |
| `table` | Table with the account's cookies. |
| `column` | Column with Base64 cookies (JSON or Netscape). |

### SaveAllCookies

```csharp
public static void SaveAllCookies(this IZennoPosterProjectModel project, Instance instance, string jsonPath = null, string table = "_instance", bool saveJsonToDb = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L337)

Writes all instance cookies as Base64 to the `cookies` column of the current account's row.

| Parameter | Description |
|---|---|
| `instance` | Browser instance. |
| `jsonPath` | Also write the cookies as JSON to this file. |
| `table` | Target table. |
| `saveJsonToDb` | Store JSON instead of Netscape. |

### SaveDomainCookies

```csharp
public static void SaveDomainCookies(this IZennoPosterProjectModel project, Instance instance, string domain = null, string jsonPath = null, string tableName = "_instance", bool saveJsonToDb = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L357)

Writes the instance cookies of one domain as Base64 to the `cookies` column of the current account's row.

| Parameter | Description |
|---|---|
| `instance` | Browser instance. |
| `domain` | Domain; default is the active tab's main domain. |
| `jsonPath` | Also write the cookies as JSON to this file. |
| `tableName` | Target table. |
| `saveJsonToDb` | Store JSON instead of Netscape. |

### SetCookiesByJs

```csharp
public static void SetCookiesByJs(this Instance instance, string cookiesJson)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L428)

Sets cookies of the active tab's domain through `document.cookie`, for the parent domain, Secure, expiring in a year when the stored date has passed.

| Parameter | Description |
|---|---|
| `cookiesJson` | JSON array of cookies; for duplicate domain + name the last one wins, other domains are skipped. |

