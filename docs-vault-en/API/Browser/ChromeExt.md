---
title: "ChromeExt"
tags: [api, Browser]
generated: z3n7-docgen
---

# ChromeExt

`class` · namespace `z3n7` · source [Browser/ChromeExt.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L277)

```csharp
public class ChromeExt
```

Older variant of `Extension`: Chromium instances only, manager installed from CRX.

## Constructors

### ChromeExt

```csharp
public ChromeExt(IZennoPosterProjectModel project, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L287)

Creates a helper without an instance; only `GetVer` works.

| Parameter | Description |
|---|---|
| `log` | Not used. |

```csharp
public ChromeExt(IZennoPosterProjectModel project, Instance instance, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L296)

Creates a helper for an instance.

| Parameter | Description |
|---|---|
| `log` | Not used. |

## Methods

### GetVer

```csharp
public string GetVer(string extId)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L310)

Reads an installed extension's version from `{pathProfileFolder}\Default\Secure Preferences`.

| Parameter | Description |
|---|---|
| `extId` | Extension id. |

**Returns:** The version. Throws when the file has no such extension or version.

### Install

```csharp
public bool Install(string extId, string fileName, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L341)

Installs a CRX file unless an extension with this id is already installed.

| Parameter | Description |
|---|---|
| `extId` | Extension id. |
| `fileName` | CRX file name in `{project.Path}.crx\`. |
| `log` | Not used. |

**Returns:** `true` when installed now. Throws when the file is missing.

### Rm

```csharp
public void Rm(string[] ExtToRemove)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L419)

Uninstalls the extensions; failures are ignored.

| Parameter | Description |
|---|---|
| `ExtToRemove` | Extension ids. |

### Switch

```csharp
public bool Switch(string toUse = "", bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L368)

Enables the listed extensions and disables all others through the One-Click Extensions Manager page (installed first if missing). Mouse emulation is restored afterwards. Chromium instances only.

| Parameter | Description |
|---|---|
| `toUse` | Names or ids of the extensions to keep enabled; matched as substrings of this text. |
| `log` | Not used. |

**Returns:** `true` when a listed extension was switched on.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
