---
title: "BetterBrowser"
tags: [api, Browser]
generated: z3n7-docgen
---

# BetterBrowser

`static class` · namespace `z3n7` · source [Browser/BetterBrowser.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BetterBrowser.cs#L15)

```csharp
public static class BetterBrowser
```

Preparing a browser instance for a session: cookies, profile data and a browser profile matching the proxy's exit point.

## Methods

### ImproveBrowser

```csharp
public static void ImproveBrowser(this IZennoPosterProjectModel project, Instance instance)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BetterBrowser.cs#L52)

Applies a browser profile matching the proxy's exit point, then checks the result. 1. Opens `check.z3n.pro/api/ip` in the browser to learn the exit IP and Chrome version. 2. Requests a profile for them and the `proxy_iso` country from `check.z3n.pro/api/profile` (directly, without the proxy). 3. Applies it to `project.Profile.BrowserProfile` and the instance. 4. Sets timezone and canvas emulation with the profile's canvas seed and window size. 5. Appends a diagnostic line to `{project.Path}/diag/z3n-diag.jsonl`. 6. Opens the `check.z3n.pro` fingerprint check, waits up to 60 seconds and writes each finding to the log as a warning.

### PrepareSession

```csharp
public static void PrepareSession(this IZennoPosterProjectModel project, Instance instance)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BetterBrowser.cs#L23)

Loads the account's cookies (`instance.GetCookies`), sets the profile email to `{NickName}@outlook.com` and a random 12-character password, turns on traffic monitoring, sets the window to 1280×720, stores `Time.Now()` in `ts0` and runs `ImproveBrowser`. An error is logged and rethrown.

