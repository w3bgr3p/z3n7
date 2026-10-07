---
title: "ProfileSync"
tags: [api, Accounts]
generated: z3n7-docgen
---

# ProfileSync

`class` · namespace `z3n7.Utilities` · source [Accounts/ProfileSync.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L14)

```csharp
public class ProfileSync
```

Saves the ZennoPoster profile, instance settings, cookies and WebGL settings of the current account to database tables and restores them.

## Constructors

### ProfileSync

```csharp
public ProfileSync(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L22)

Creates the helper.

| Parameter | Description |
|---|---|
| `log` | Logger for progress; `null` logs nothing. |

## Methods

### AddStructureToDb

```csharp
public void AddStructureToDb(bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L169)

Creates the `folder_*`, `zpprofile_*` and `zb_*` tables with account rows and the profile and instance columns, unless `folder_profile` and `zb_profile` already exist.

| Parameter | Description |
|---|---|
| `log` | Not used. |

### RestoreProfile

```csharp
public void RestoreProfile(string restoreFrom, bool restoreProfile = true, bool restoreCookies = true, bool restoreInstance = true, bool restoreWebgl = true, bool rebuildWebgl = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L45)

Restores the current account's data from the tables of `restoreFrom`. Throws for any other source.

| Parameter | Description |
|---|---|
| `restoreFrom` | `folder`, `zb` or `zpprofile`: the prefix of the tables `{prefix}_profile`, `{prefix}_instance` and `{prefix}_webgl`. |
| `restoreProfile` | Profile properties. |
| `restoreCookies` | Cookies (Base64 in the `cookies` column). |
| `restoreInstance` | Instance properties. |
| `restoreWebgl` | WebGL settings (`_preferences` column). |
| `rebuildWebgl` | Rebuild the WebGL settings from the flattened JSON columns (`DbToJson`) instead of `_preferences`. |

### SaveProfile

```csharp
public void SaveProfile(string saveTo, bool saveProfile = true, bool saveInstance = true, bool saveCookies = true, bool saveWebgl = true)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L112)

Saves the current account's data to the tables of `saveTo`. Throws for any other target.

| Parameter | Description |
|---|---|
| `saveTo` | `folder`, `zb` or `zpprofile`: the prefix of the tables `{prefix}_profile`, `{prefix}_instance` and `{prefix}_webgl`. |
| `saveProfile` | Profile properties. |
| `saveInstance` | Instance properties. |
| `saveCookies` | All cookies (`SaveAllCookies`). |
| `saveWebgl` | WebGL settings, both as `_preferences` and flattened into columns. |

