---
title: "Get"
tags: [api, Db]
generated: z3n7-docgen
---

# Get

`static class` · namespace `z3n7` · source [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L84)

```csharp
public static class Get
```

Reading from the project database. The row defaults to the current account (`acc0`).

## Methods

### DbGet

```csharp
public static string DbGet(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", string acc = null, string where = "")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L95)

Reads columns of one row; same as `SqlGet`.

| Parameter | Description |
|---|---|
| `toGet` | Comma-separated column names; each is quoted. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `key` | Column matched against `id`. |
| `acc` | Value of `key`; default `acc0`. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

**Returns:** Columns joined by `¦`; several rows are joined by `·`.

### DbGetColumns

```csharp
public static Dictionary<string, string> DbGetColumns(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L111)

Reads columns of one row as column → value.

| Parameter | Description |
|---|---|
| `toGet` | Comma-separated column names; each is quoted. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`; default is the current account, `acc0`. Inserted into the SQL as written. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

**Returns:** An empty dictionary when nothing was found.

### DbGetLine

```csharp
public static string[] DbGetLine(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L126)

Reads columns of one row as an array.

| Parameter | Description |
|---|---|
| `toGet` | Comma-separated column names; each is quoted. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`; default is the current account, `acc0`. Inserted into the SQL as written. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

### DbGetLines

```csharp
public static List<string> DbGetLines(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "", string toList = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L142)

Reads several rows; each item keeps its columns joined by `¦`.

| Parameter | Description |
|---|---|
| `toGet` | Comma-separated column names; each is quoted. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`; default is the current account, `acc0`. Inserted into the SQL as written. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |
| `toList` | When set, the result is also written to this ZennoPoster list. |

### DbGetRandom

```csharp
public static string DbGetRandom(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool acc = false, bool thrw = false, int range = 0, bool single = true, bool invert = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L178)

Reads `toGet` from random rows where it is not empty and `id` is below `range`.

| Parameter | Description |
|---|---|
| `toGet` | Column to read. |
| `tableName` | Table; default is `project.ProjectTable()` (`__` + project name). |
| `log` | Write the query and its result to the project log. |
| `acc` | Prefix the result with the `id` column. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `range` | Upper id bound (exclusive); 0 uses the last account of `project.Range()`. |
| `single` | One row instead of all matching rows. |
| `invert` | Select rows where the column is empty instead. |

### DbKey

```csharp
public static string DbKey(this IZennoPosterProjectModel project, string chainType = "evm")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L210)

Reads the current account's key from the wallet table (`DbSchema.Wlt`) and decrypts it with `SAFU.Decode`.

| Parameter | Description |
|---|---|
| `chainType` | `evm` (column `secp256k1`), `sol` (`base58`) or `seed` (`bip39`). Anything else throws. |

### DbToVars

```csharp
public static Dictionary<string, string> DbToVars(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L160)

Reads columns of one row and sets a project variable of the same name for each.

| Parameter | Description |
|---|---|
| `toGet` | Comma-separated column names; each is quoted. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`; default is the current account, `acc0`. Inserted into the SQL as written. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

**Returns:** The values that were set.

### RndInvite

```csharp
public static string RndInvite(this IZennoPosterProjectModel project, object limit = null, string inviteColumn = "refcode", bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L243)

Returns the `cfgRefCode` variable; when it is empty, picks a random non-empty `inviteColumn` from the project table and stores it in `cfgRefCode`.

| Parameter | Description |
|---|---|
| `limit` | When set, only rows with `id` up to this number. Must be a positive integer, otherwise it throws. |
| `inviteColumn` | Column with invite codes. |
| `log` | Write the query and its result to the project log. |

