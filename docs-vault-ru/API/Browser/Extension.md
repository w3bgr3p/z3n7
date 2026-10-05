---
title: "Extension"
tags: [api, Browser]
generated: z3n7-docgen
---

# Extension

`class` · пространство имён `z3n7` · исходник [Browser/ChromeExt.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L19)

```csharp
public class Extension
```

Chrome extension management in a ZennoPoster instance: version, install, enable/disable, remove.

## Конструкторы

### Extension

```csharp
public Extension(IZennoPosterProjectModel project, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L33)

Creates a helper without an instance; only `GetVer` works.

| Параметр | Описание |
|---|---|
| `log` | Logger for progress; `null` logs nothing. |

```csharp
public Extension(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L42)

Creates a helper for an instance.

| Параметр | Описание |
|---|---|
| `log` | Logger for progress; `null` logs nothing. |

## Методы

### GetVer

```csharp
public string GetVer(string extId)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L54)

Reads an installed extension's version from `{pathProfileFolder}\Default\Secure Preferences`.

| Параметр | Описание |
|---|---|
| `extId` | Extension id. |

**Возвращает:** The version. Throws when the file has no such extension or version.

### InstallFromCrx

```csharp
public bool InstallFromCrx(string extId, string fileName, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L144)

Installs a CRX file unless an extension with this id is already installed.

| Параметр | Описание |
|---|---|
| `extId` | Extension id. |
| `fileName` | CRX file name in `{project.Path}.crx\`. |
| `log` | Not used. |

**Возвращает:** `true` when installed now. Throws when the file is missing.

### InstallFromStore

```csharp
public bool InstallFromStore(string url, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L97)

Opens the Chrome Web Store page and installs the extension, confirming the dialog with keystrokes. When it is already installed, clicks "Enable now" if shown.

| Параметр | Описание |
|---|---|
| `url` | Web Store page of the extension. |
| `log` | Not used. |

**Возвращает:** `true` when the install was started; `false` when it was already installed.

### Rm

```csharp
public void Rm(string[] ExtToRemove)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L250)

Uninstalls the extensions; failures are logged and skipped.

| Параметр | Описание |
|---|---|
| `ExtToRemove` | Extension ids. |

### Switch

```csharp
public bool Switch(string toUse = "", bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L178)

Enables the listed extensions and disables all others through the One-Click Extensions Manager page (installed first if missing). Mouse emulation is restored afterwards. Works for Chromium (manager from CRX) and ChromiumFromZB (manager from the Web Store) instances.

| Параметр | Описание |
|---|---|
| `toUse` | Names or ids of the extensions to keep enabled; matched as substrings of this text. |
| `log` | Not used. |

**Возвращает:** `true` when at least one listed extension is enabled.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
