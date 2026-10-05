---
title: "DbUpdate"
tags: [api, Db]
generated: z3n7-docgen
---

# DbUpdate

`static class` · namespace `z3n7` · source [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L278)

```csharp
public static class DbUpdate
```

Writing to the project database.

## Methods

### DbDone

```csharp
public static void DbDone(this IZennoPosterProjectModel project, string task = "daily", int cooldownMin = 0, string tableName = null, bool log = false, bool thrw = false, string key = "id", object acc = null, string where = "")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L375)

Writes a cooldown timestamp (`Time.Cd`, ISO UTC) to the `task` column of the current account's row (or the row selected by `key`/`acc`, or the rows matching `where`): end of today, or now plus `cooldownMin`.

| Parameter | Description |
|---|---|
| `task` | Column to write. |
| `cooldownMin` | Minutes from now; 0 means today 23:59:59 UTC. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `key` | Column matched against `id`. |
| `acc` | Value of `key`; default is the current account, `acc0`. Inserted into the SQL as written. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

### DbInsert

```csharp
public static string DbInsert(this IZennoPosterProjectModel project, Dictionary<string, string> dataDic, string tableName = null, bool log = false, bool thrw = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L288)

Inserts one row. The `id` key is skipped; an empty dictionary inserts a row of defaults.

| Parameter | Description |
|---|---|
| `dataDic` | Column → value. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |

**Returns:** Affected row count as text.

### DbUpd

```csharp
public static void DbUpd(this IZennoPosterProjectModel project, string toUpd, string tableName = null, bool log = false, bool thrw = false, string key = "id", object acc = null, string where = "", string saveToVar = "lastQuery")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L354)

Runs `UPDATE … SET toUpd` for the current account's row, or for the rows matching `where`.

| Parameter | Description |
|---|---|
| `toUpd` | Assignments such as `status = 'ok'`; column names are quoted, values are taken as written. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `key` | Column matched against `id`. |
| `acc` | Value of `key`; default is the current account, `acc0`. Inserted into the SQL as written. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |
| `saveToVar` | Variable that receives `toUpd` before the update; empty to skip. |

### DicToDb

```csharp
public static void DicToDb(this IZennoPosterProjectModel project, Dictionary<string, string> dataDic, string tableName = null, bool log = false, bool thrw = false, string where = "")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L312)

Writes a dictionary to the current account's row (or the rows matching `where`), adding missing columns first. A key `id` is written to the column `_id`.

| Parameter | Description |
|---|---|
| `dataDic` | Column → value. Modified in place when it has an `id` key. |
| `tableName` | Table; default is the `projectTable` variable. |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
