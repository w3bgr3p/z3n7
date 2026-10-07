---
title: "ProjectExtensions (Accounts)"
tags: [api, Accounts]
generated: z3n7-docgen
---

# ProjectExtensions (Accounts)

`static class` · namespace `z3n7` · source [Accounts/InstanceManager.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L559)

```csharp
public static class ProjectExtensions
```

Other parts of this type: [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

Extension methods on `IZennoPosterProjectModel`: browser start and finish for an account.

## Methods

### Finish

```csharp
public static void Finish(this IZennoPosterProjectModel project, Instance instance)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L588)

Ends the account session: `Disposer.FinishSession`.

### ProxySet

```csharp
public static bool ProxySet(this IZennoPosterProjectModel project, Instance instance, string proxyString = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L624)

Checks a proxy and applies it to the instance: compares the IP seen by public echo services directly and through the proxy, and sets the proxy only when they differ.

| Parameter | Description |
|---|---|
| `proxyString` | Proxy; default is the `proxy` column of the account's `_instance` row. |
| `instance` | Browser instance. |

**Returns:** `true`. Throws when the proxy is empty, does not answer, or shows the local IP.

### ReportError

```csharp
public static string ReportError(this IZennoPosterProjectModel project, Instance instance, bool toLog = true, bool toTelegram = false, bool toDb = false, bool screenshot = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L599)

Writes an error report (`Reporter.ReportError`).

| Parameter | Description |
|---|---|
| `instance` | Browser instance. |
| `toLog` | Write it to the log. |
| `toTelegram` | Send it to Telegram. |
| `toDb` | Write it to the account's row. |
| `screenshot` | Save a screenshot. |

### ReportSuccess

```csharp
public static string ReportSuccess(this IZennoPosterProjectModel project, Instance instance, bool toLog = true, bool toTelegram = false, bool toDb = false, string customMessage = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L611)

Writes a success report (`Reporter.ReportSuccess`).

| Parameter | Description |
|---|---|
| `instance` | Browser instance. |
| `toLog` | Write it to the log. |
| `toTelegram` | Send it to Telegram. |
| `toDb` | Write it to the account's row. |
| `customMessage` | Extra line. |

### RunBrowser

```csharp
public static void RunBrowser(this IZennoPosterProjectModel project, Instance instance, string browserToLaunch = "Chromium", bool debug = false, bool fixTimezone = false, bool useLegacy = true, bool useZpprofile = false, bool useFolder = true)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L573)

Starts the browser for the current account (`InstanceManager.Initialize`) and sets `state = 'busy'` in `_instance`. Does nothing when a Chromium browser is already running in the instance.

| Parameter | Description |
|---|---|
| `instance` | Browser instance. |
| `browserToLaunch` | `Chromium` or `WithoutBrowser`. |
| `debug` | Log progress. |
| `fixTimezone` | See `Initialize`. |
| `useLegacy` | See `Initialize`. |
| `useZpprofile` | See `Initialize`. |
| `useFolder` | See `Initialize`. |

### SaveProfile

```csharp
public static void SaveProfile(this IZennoPosterProjectModel project, Instance instance)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L684)

Exports the profile and instance properties, WebGL settings and cookies (Base64) to `{project.Directory}/profiles/zenno_profile_{yyyyMMdd_HHmmss}_{id}.json`.

| Parameter | Description |
|---|---|
| `instance` | Browser instance. |

