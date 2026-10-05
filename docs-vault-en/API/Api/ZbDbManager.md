---
title: "ZbDbManager"
tags: [api, Api]
generated: z3n7-docgen
---

# ZbDbManager

`static class` · namespace `z3n7` · source [Api/ZB.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L94)

```csharp
public static class ZbDbManager
```

Reading the ZennoBrowser profile database and parsing its profile lists.

## Methods

### ZBDbGet

```csharp
public static string ZBDbGet(this IZennoPosterProjectModel project, string query, string tableName = "ProfileInfos", bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L108)

Reads `query` columns of the profile whose id is in the `zb_id` variable. While reading, `DBmode`, `DBsqltPath` and `acc0` point at the ZennoBrowser database and profile; they are restored afterwards.

| Parameter | Description |
|---|---|
| `query` | Comma-separated column names. |
| `tableName` | Table. |
| `log` | Write the query to the log. |

**Remarks:** The read goes through `DbGetLines`/`DbGet` → `DbQ`, which picks the database by `dbSource` and does not consult `DBmode` or `DBsqltPath`.

### ZBIdDic

```csharp
public static Dictionary<string, string> ZBIdDic(this IZennoPosterProjectModel project, string json, string folder = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L145)

Maps profile names to ids from a JSON array of ZennoBrowser profiles (`Name`, `Id`, `FolderName`); for duplicate names the first wins.

| Parameter | Description |
|---|---|
| `json` | JSON array of profiles. |
| `folder` | Only profiles of this folder; empty for all. |

### ZBIdList

```csharp
public static List<string> ZBIdList(this IZennoPosterProjectModel project, string json, string folder = "Farm")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L163)

Profile ids of a folder from a JSON array of profiles (see `ZBIdDic`).

| Parameter | Description |
|---|---|
| `json` | JSON array of profiles. |
| `folder` | Folder name. |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
