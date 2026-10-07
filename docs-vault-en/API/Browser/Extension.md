---
title: "Extension"
tags: [api, Browser]
generated: z3n7-docgen
---

# Extension

`class` · namespace `z3n7` · source [Browser/ChromeExt.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L19)

```csharp
public class Extension
```

Chrome extension management in a ZennoPoster instance: version, install, enable/disable, remove.

## Constructors

### Extension

```csharp
public Extension(IZennoPosterProjectModel project, Logger log = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L35)

Creates a helper without an instance; only `GetVer` works.

| Parameter | Description |
|---|---|
| `log` | Logger for progress; `null` logs nothing. |

```csharp
public Extension(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L44)

Creates a helper for an instance.

| Parameter | Description |
|---|---|
| `log` | Logger for progress; `null` logs nothing. |

## Methods

### GetVer

```csharp
public string GetVer(string extId)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L56)

Reads an installed extension's version from `{pathProfileFolder}\Default\Secure Preferences`.

| Parameter | Description |
|---|---|
| `extId` | Extension id. |

**Returns:** The version. Throws when the file has no such extension or version.

### InstallFromCrx

```csharp
public bool InstallFromCrx(string extId, string fileName, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L146)

Installs a CRX file unless an extension with this id is already installed.

| Parameter | Description |
|---|---|
| `extId` | Extension id. |
| `fileName` | CRX file name in `{project.Path}.crx\`. |
| `log` | Not used. |

**Returns:** `true` when installed now. Throws when the file is missing.

### InstallFromStore

```csharp
public bool InstallFromStore(string url, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L99)

Opens the Chrome Web Store page and installs the extension, confirming the dialog with keystrokes. When it is already installed, clicks "Enable now" if shown.

| Parameter | Description |
|---|---|
| `url` | Web Store page of the extension. |
| `log` | Not used. |

**Returns:** `true` when the install was started; `false` when it was already installed.

### Rm

```csharp
public void Rm(string[] ExtToRemove)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L252)

Uninstalls the extensions; failures are logged and skipped.

| Parameter | Description |
|---|---|
| `ExtToRemove` | Extension ids. |

### Switch

```csharp
public bool Switch(string toUse = "", bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L180)

Enables the listed extensions and disables all others through the One-Click Extensions Manager page (installed first if missing). Mouse emulation is restored afterwards. Works for Chromium (manager from CRX) and ChromiumFromZB (manager from the Web Store) instances.

| Parameter | Description |
|---|---|
| `toUse` | Names or ids of the extensions to keep enabled; matched as substrings of this text. |
| `log` | Not used. |

**Returns:** `true` when at least one listed extension is enabled.

