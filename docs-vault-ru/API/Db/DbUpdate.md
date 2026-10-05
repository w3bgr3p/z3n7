---
title: "DbUpdate"
tags: [api, Db]
generated: z3n7-docgen
---

# DbUpdate

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L278)

```csharp
public static class DbUpdate
```

Writing to the project database.

## Методы

### DbDone

```csharp
public static void DbDone(this IZennoPosterProjectModel project, string task = "daily", int cooldownMin = 0, string tableName = null, bool log = false, bool thrw = false, string key = "id", object acc = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L375)

Writes a cooldown timestamp (`Time.Cd`, ISO UTC) to the `task` column of the current account's row (or the row selected by `key`/`acc`, or the rows matching `where`): end of today, or now plus `cooldownMin`.

| Параметр | Описание |
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

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L288)

Inserts one row. The `id` key is skipped; an empty dictionary inserts a row of defaults.

| Параметр | Описание |
|---|---|
| `dataDic` | Column → value. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |

**Возвращает:** Affected row count as text.

### DbUpd

```csharp
public static void DbUpd(this IZennoPosterProjectModel project, string toUpd, string tableName = null, bool log = false, bool thrw = false, string key = "id", object acc = null, string where = "", string saveToVar = "lastQuery")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L354)

Runs `UPDATE … SET toUpd` for the current account's row, or for the rows matching `where`.

| Параметр | Описание |
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

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L312)

Writes a dictionary to the current account's row (or the rows matching `where`), adding missing columns first. A key `id` is written to the column `_id`.

| Параметр | Описание |
|---|---|
| `dataDic` | Column → value. Modified in place when it has an `id` key. |
| `tableName` | Table; default is the `projectTable` variable. |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
