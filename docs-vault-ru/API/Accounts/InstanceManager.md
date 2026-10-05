---
title: "InstanceManager"
tags: [api, Accounts]
generated: z3n7-docgen
---

# InstanceManager

`class` · пространство имён `z3n7` · исходник [Accounts/InstanceManager.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L19)

```csharp
public class InstanceManager
```

Starts the browser for the current account with its profile, proxy and cookies, and saves and cleans up at the end.

## Конструкторы

### InstanceManager

```csharp
public InstanceManager(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L30)

Creates the manager.

| Параметр | Описание |
|---|---|
| `log` | Logger for progress; `null` logs nothing. |

## Методы

### Cleanup

```csharp
public void Cleanup()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L507)

Frees the account (clears its global `acc{n}`, sets `state = 'idle'` in `_instance`), clears `acc0` and stops the instance.

### Initialize

```csharp
public void Initialize(string browserToLaunch = null, bool fixTimezone = false, bool useLegacy = true, bool useZpprofile = false, bool useFolder = true)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L56)

Launches the browser for `acc0` and prepares it. Stores the browser port and PID in `port`, `pid` and `instancePort`. Legacy path: for Chromium, applies WebGL from the database, the proxy and the cookies (database, else the cookie file); otherwise only the proxy; up to 4 attempts, then the account's global `acc{n}` is cleared and the error is thrown. Non-legacy path: restores the profile from the `folder_*` tables (`ProfileSync`) and sets the proxy.

| Параметр | Описание |
|---|---|
| `browserToLaunch` | `Chromium` or `WithoutBrowser`; default is the `cfgBrowser` variable. |
| `fixTimezone` | Fix the timezone through browserscan.net when its score mentions time. |
| `useLegacy` | Use the legacy setup path. |
| `useZpprofile` | Load the ZennoPoster profile file `{profileFolder}.zpprofile` when it exists. |
| `useFolder` | Launch Chromium with the account's profile folder. |

### ProxySet

```csharp
public bool ProxySet(string proxyString = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L390)

Checks a proxy and applies it to the instance: compares the IP seen by public echo services directly and through the proxy, and sets the proxy only when they differ.

| Параметр | Описание |
|---|---|
| `proxyString` | Proxy; default is the `proxy` column of the account's `_instance` row. |

**Возвращает:** `true`. Throws when the proxy is empty, does not answer, or shows the local IP.

### SaveProfile

```csharp
public void SaveProfile(bool saveCookies = true, bool saveProfile = true, string saveTo = "folder", bool saveZpProfile = true)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L462)

Saves the account's browser data when the browser is Chromium, `acc0` is set and `accRnd` is empty: profile, instance, cookies and WebGL to the tables of `saveTo` (`ProfileSync`), and the ZennoPoster profile to the profile folder. Errors are logged, not thrown.

| Параметр | Описание |
|---|---|
| `saveCookies` | Save cookies. |
| `saveProfile` | Save profile, instance and WebGL. |
| `saveTo` | Table prefix: `folder`, `zb` or `zpprofile`. |
| `saveZpProfile` | Also save the ZennoPoster profile to the profile folder. |

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
