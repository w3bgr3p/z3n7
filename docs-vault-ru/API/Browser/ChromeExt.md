---
title: "ChromeExt"
tags: [api, Browser]
generated: z3n7-docgen
---

# ChromeExt

`class` · пространство имён `z3n7` · исходник [Browser/ChromeExt.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L279)

```csharp
public class ChromeExt
```

Older variant of `Extension`: Chromium instances only, manager installed from CRX.

## Конструкторы

### ChromeExt

```csharp
public ChromeExt(IZennoPosterProjectModel project, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L292)

Creates a helper without an instance; only `GetVer` works.

| Параметр | Описание |
|---|---|
| `log` | Not used. |

```csharp
public ChromeExt(IZennoPosterProjectModel project, Instance instance, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L301)

Creates a helper for an instance.

| Параметр | Описание |
|---|---|
| `log` | Not used. |

## Методы

### GetVer

```csharp
public string GetVer(string extId)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L315)

Reads an installed extension's version from `{pathProfileFolder}\Default\Secure Preferences`.

| Параметр | Описание |
|---|---|
| `extId` | Extension id. |

**Возвращает:** The version. Throws when the file has no such extension or version.

### Install

```csharp
public bool Install(string extId, string fileName, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L346)

Installs a CRX file unless an extension with this id is already installed.

| Параметр | Описание |
|---|---|
| `extId` | Extension id. |
| `fileName` | CRX file name in `{project.Path}.crx\`. |
| `log` | Not used. |

**Возвращает:** `true` when installed now. Throws when the file is missing.

### Rm

```csharp
public void Rm(string[] ExtToRemove)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L424)

Uninstalls the extensions; failures are ignored.

| Параметр | Описание |
|---|---|
| `ExtToRemove` | Extension ids. |

### Switch

```csharp
public bool Switch(string toUse = "", bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L373)

Enables the listed extensions and disables all others through the One-Click Extensions Manager page (installed first if missing). Mouse emulation is restored afterwards. Chromium instances only.

| Параметр | Описание |
|---|---|
| `toUse` | Names or ids of the extensions to keep enabled; matched as substrings of this text. |
| `log` | Not used. |

**Возвращает:** `true` when a listed extension was switched on.

