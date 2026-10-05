---
title: "ZbDbManager"
tags: [api, Api]
generated: z3n7-docgen
---

# ZbDbManager

`static class` · пространство имён `z3n7` · исходник [Api/ZB.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L84)

```csharp
public static class ZbDbManager
```

Reading the ZennoBrowser profile database and parsing its profile lists.

## Методы

### ZBDbGet

```csharp
public static string ZBDbGet(this IZennoPosterProjectModel project, string query, string tableName = "ProfileInfos", bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L94)

Reads `query` columns of the profile whose id is in the `zb_id` variable, directly from `%LOCALAPPDATA%\ZennoLab\ZP8\.zp8\ProfileManagement.db`.

| Параметр | Описание |
|---|---|
| `query` | Comma-separated column names. |
| `tableName` | Table. |
| `log` | Write the query and its result to the log. |

**Возвращает:** Columns joined by `¦`; empty when there is no such profile.

### ZBIdDic

```csharp
public static Dictionary<string, string> ZBIdDic(this IZennoPosterProjectModel project, string json, string folder = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L110)

Maps profile names to ids from a JSON array of ZennoBrowser profiles (`Name`, `Id`, `FolderName`); for duplicate names the first wins.

| Параметр | Описание |
|---|---|
| `json` | JSON array of profiles. |
| `folder` | Only profiles of this folder; empty for all. |

### ZBIdList

```csharp
public static List<string> ZBIdList(this IZennoPosterProjectModel project, string json, string folder = "Farm")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L128)

Profile ids of a folder from a JSON array of profiles (see `ZBIdDic`).

| Параметр | Описание |
|---|---|
| `json` | JSON array of profiles. |
| `folder` | Folder name. |

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
