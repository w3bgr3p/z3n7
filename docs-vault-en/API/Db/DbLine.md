---
title: "DbLine"
tags: [api, Db]
generated: z3n7-docgen
---

# DbLine

`static class` · namespace `z3n7` · source [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L802)

```csharp
public static class DbLine
```

Operations on whole rows.

## Methods

### DbClearLine

```csharp
public static void DbClearLine(this IZennoPosterProjectModel project, int id, string tableName = null, bool log = false, bool thrw = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L809)

Sets every column except `id` to an empty string in one row.

| Parameter | Description |
|---|---|
| `id` | Row id. |
| `tableName` | Table; default is `project.ProjectTable()` (`__` + project name). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |

### DbSwapLines

```csharp
public static void DbSwapLines(this IZennoPosterProjectModel project, int id1, int id2, string tableName = null, bool log = false, bool thrw = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L837)

Exchanges the values of all columns except `id` between two rows.

| Parameter | Description |
|---|---|
| `id1` | First row id. |
| `id2` | Second row id. |
| `tableName` | Table; default is `project.ProjectTable()` (`__` + project name). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw when a row is not found; otherwise nothing changes. |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
