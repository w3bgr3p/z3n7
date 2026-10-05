---
title: "DbJson"
tags: [api, Db]
generated: z3n7-docgen
---

# DbJson

`static class` · namespace `z3n7` · source [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L384)

```csharp
public static class DbJson
```

Storing a JSON object as table columns and rebuilding it.

## Methods

### DbToJson

```csharp
public static string DbToJson(this IZennoPosterProjectModel project, string tableName = null, bool log = false, bool thrw = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L457)

Rebuilds the JSON saved by `JsonToDb` with `saveStructure` from the current account's row. Writes detailed progress to the project log.

| Parameter | Description |
|---|---|
| `tableName` | Table; default is `project.ProjectTable()` (`__` + project name). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |

**Returns:** The JSON text, or `{}` when there is no `_json_structure` or it cannot be parsed.

### JsonToDb

```csharp
public static void JsonToDb(this IZennoPosterProjectModel project, string json, string tableName = null, bool log = false, bool thrw = false, string where = "", bool saveStructure = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L398)

Flattens a JSON object into columns (nested keys joined with `_`) and writes them with `DicToDb`.

| Parameter | Description |
|---|---|
| `json` | JSON object. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |
| `saveStructure` | Also save the shape in `_json_structure` so that `DbToJson` can rebuild the object. |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
