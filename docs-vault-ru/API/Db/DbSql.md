---
title: "DbSql"
tags: [api, Db]
generated: z3n7-docgen
---

# DbSql

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L885)

```csharp
public static class DbSql
```

Lower-level SELECT and UPDATE helpers behind the `Db*` methods.

## Методы

### SqlGet

```csharp
public static string SqlGet(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L900)

Selects columns from the row where `key` = `id`, or from the rows matching `where`.

| Параметр | Описание |
|---|---|
| `toGet` | Comma-separated column names; each is quoted. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`; default is the current account, `acc0`. Inserted into the SQL as written. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

**Возвращает:** Columns joined by `¦`, rows by `·`.

### SqlGetArrFromLine

```csharp
public static string[] SqlGetArrFromLine(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L968)

Like `SqlGet`, split into column values.

| Параметр | Описание |
|---|---|
| `toGet` | Comma-separated column names; each is quoted. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`; default is the current account, `acc0`. Inserted into the SQL as written. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

### SqlGetDicFromLine

```csharp
public static Dictionary<string, string> SqlGetDicFromLine(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "", bool set = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L937)

Like `SqlGet`, returning the first row as column → value.

| Параметр | Описание |
|---|---|
| `toGet` | Comma-separated column names; each is quoted. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`; default is the current account, `acc0`. Inserted into the SQL as written. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |
| `set` | Also set project variables named after the columns. |

### SqlGetListFromLines

```csharp
public static List<string> SqlGetListFromLines(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L983)

Like `SqlGet`, split into rows.

| Параметр | Описание |
|---|---|
| `toGet` | Comma-separated column names; each is quoted. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`; default is the current account, `acc0`. Inserted into the SQL as written. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

### SqlUpd

```csharp
public static string SqlUpd(this IZennoPosterProjectModel project, string toUpd, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1002)

Runs `UPDATE … SET toUpd` for the row where `key` = `id`, or for the rows matching `where`.

| Параметр | Описание |
|---|---|
| `toUpd` | Assignments; column names are quoted, values are taken as written. |
| `tableName` | Table; default is the project table (`projectTable` variable). |
| `log` | Write the query and its result to the project log. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`; default is the current account, `acc0`. Inserted into the SQL as written. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

**Возвращает:** Affected row count as text.

