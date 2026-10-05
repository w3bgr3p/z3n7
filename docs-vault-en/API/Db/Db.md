---
title: "Db"
tags: [api, Db]
generated: z3n7-docgen
---

# Db

`class` · namespace `z3n7` · source [Db/Db.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L22), [Db/DbZenno.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbZenno.cs#L7)

```csharp
public class Db
```

SQL helper over PostgreSQL or SQLite with one API for both. Every statement opens its own connection. A `SELECT` returns rows joined by `·` and columns joined by `¦`; other statements return the number of affected rows. On SQLite a "database is locked" error is retried up to 10 times with a growing pause.

**Remarks:** SQLite is reached through the SQLite3 ODBC driver, which must be installed. Values passed as `id` or `where` are inserted into SQL as written.

## Constructors

### Db

```csharp
public Db(string dbMode = "pgSQL", string sqLitePath = null, string pgHost = "localhost", string pgPort = "5432", string pgDbName = "postgres", string pgUser = "postgres", string pgPass = "", string defaultTable = null, LogLevel logLevel = LogLevel.Off)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L49)

Creates a database helper with explicit connection settings.

| Parameter | Description |
|---|---|
| `dbMode` | `pgSQL` for PostgreSQL; any other value uses SQLite. |
| `sqLitePath` | SQLite database file. |
| `pgHost` | PostgreSQL host. |
| `pgPort` | PostgreSQL port. |
| `pgDbName` | PostgreSQL database. |
| `pgUser` | PostgreSQL user. |
| `pgPass` | PostgreSQL password. |
| `defaultTable` | Table used when a method gets no table name. |
| `logLevel` | Not used for output: without a project the logger has nowhere to write. |

```csharp
public Db(IZennoPosterProjectModel project, string dbMode = null, string sqLitePath = null, string pgHost = null, string pgPort = null, string pgDbName = null, string pgUser = null, string pgPass = null, string defaultTable = null, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbZenno.cs#L18)

Creates a database helper from project settings. Each argument left `null` is read from: `dbMode` — variable `DBmode`; `sqLitePath` — variable `DBsqltPath`; PostgreSQL host, port, database, user and password — global variables `sqlPgHost`, `sqlPgPort`, `sqlPgName`, `sqlPgUser`, `sqlPgPass`; `defaultTable` — `project.ProjectTable()` (`__` + project name).

| Parameter | Description |
|---|---|
| `log` | Log queries and results at `Info` level. |

## Methods

### AddColumn

```csharp
public void AddColumn(string columnName, string tableName = null, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1094)

Adds a column if the table does not have it yet.

| Parameter | Description |
|---|---|
| `columnName` | Column name. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `defaultValue` | SQL type of the new column. |

### AddColumns

```csharp
public void AddColumns(List<string> columns, string tableName = null, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1114)

Adds each listed column that the table does not have yet.

| Parameter | Description |
|---|---|
| `columns` | Column names. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `defaultValue` | SQL type of the new columns. |

```csharp
public void AddColumns(Dictionary<string, string> tableStructure, string tableName = null, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1131)

Adds each column of `tableStructure` that the table does not have yet, with its type.

| Parameter | Description |
|---|---|
| `tableStructure` | Column → SQL type. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

### AddRange

```csharp
public void AddRange(string tableName, int range, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1208)

Inserts rows with ids from the current maximum + 1 up to `range`, in batches of 500. Existing ids are skipped.

| Parameter | Description |
|---|---|
| `tableName` | Table with an `id` column. |
| `range` | Highest id to have. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

### BridgeTable

```csharp
public void BridgeTable(string sourceTable, string targetDbPath, string targetTable, string targetMode = "SQLite", string schema = "public", bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1622)

Copies a table from this database to another one: PostgreSQL → SQLite, SQLite → PostgreSQL or SQLite → SQLite. The target table is dropped and recreated.

| Parameter | Description |
|---|---|
| `sourceTable` | Table in this database. |
| `targetDbPath` | Target SQLite file, or the Npgsql connection string of the target database for `pgSQL`. |
| `targetTable` | Target table. |
| `targetMode` | `SQLite` or `pgSQL`. |
| `schema` | PostgreSQL schema. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

### Clear

```csharp
public void Clear(string tableName = null, bool log = false, bool thrw = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1279)

Deletes all rows and resets the id counter (`TRUNCATE … RESTART IDENTITY CASCADE` on PostgreSQL, the `sqlite_sequence` entry on SQLite).

| Parameter | Description |
|---|---|
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |

### ClearLine

```csharp
public void ClearLine(int id, string tableName = null, bool log = false, bool thrw = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1308)

Sets every column except `id` to an empty string in one row.

| Parameter | Description |
|---|---|
| `id` | Row id. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |

### ColumnExists

```csharp
public bool ColumnExists(string columnName, string tableName, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1069)

Checks whether a column exists. Case-insensitive on PostgreSQL, case-sensitive on SQLite.

| Parameter | Description |
|---|---|
| `columnName` | Column name. |
| `tableName` | Table name. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

### CreateTable

```csharp
public void CreateTable(Dictionary<string, string> tableStructure, string tableName, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L980)

Creates the table unless it exists. On PostgreSQL `AUTOINCREMENT` in a type is replaced by `SERIAL`.

| Parameter | Description |
|---|---|
| `tableStructure` | Column → SQL type. |
| `tableName` | Table to create. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

### DbToJson

```csharp
public string DbToJson(string tableName = null, bool log = false, bool thrw = false, object id = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L430)

Rebuilds the JSON stored by `JsonToDb` from the row with the given `id`. Columns starting with `_` and `id` are left out.

| Parameter | Description |
|---|---|
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |
| `id` | Value of `key`, inserted into the SQL as written: quote text values yourself. |

**Returns:** The JSON text, or `{}` when the table has no `_json_structure` column or it cannot be parsed.

### Del

```csharp
public void Del(string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1246)

Deletes the row where `key` = `id`, or the rows matching `where`.

| Parameter | Description |
|---|---|
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`, inserted into the SQL as written: quote text values yourself. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

### DropColumn

```csharp
public void DropColumn(string columnName, string tableName = null, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1155)

Drops a column if it exists (`CASCADE` on PostgreSQL).

| Parameter | Description |
|---|---|
| `columnName` | Column name. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

### Get

```csharp
public string Get(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L137)

Selects columns from the row where `key` = `id`, or from the rows matching `where`.

| Parameter | Description |
|---|---|
| `columns` | Comma-separated column names; each is quoted. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`, inserted into the SQL as written: quote text values yourself. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

**Returns:** Raw result in the `Query` format.

### GetColumns

```csharp
public Dictionary<string, string> GetColumns(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L174)

Like `Get`, but returns the first row as column → value.

| Parameter | Description |
|---|---|
| `columns` | Comma-separated column names; each is quoted. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`, inserted into the SQL as written: quote text values yourself. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

**Returns:** An empty dictionary when nothing was found.

### GetLine

```csharp
public string[] GetLine(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L206)

Like `Get`, split into column values.

| Parameter | Description |
|---|---|
| `columns` | Comma-separated column names; each is quoted. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`, inserted into the SQL as written: quote text values yourself. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

### GetLines

```csharp
public List<string> GetLines(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L222)

Like `Get`, split into rows. Each row still has its columns joined by `¦`.

| Parameter | Description |
|---|---|
| `columns` | Comma-separated column names; each is quoted. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`, inserted into the SQL as written: quote text values yourself. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

### GetRandom

```csharp
public string GetRandom(string column, string tableName = null, bool log = false, bool thrw = false, int maxId = 0, bool includeId = false, bool single = true, bool invertEmpty = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L239)

Selects `column` from random rows where it is not empty.

| Parameter | Description |
|---|---|
| `column` | Column to read. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |
| `maxId` | When above 0, only rows with `id` below it. |
| `includeId` | Prefix the result with the `id` column. |
| `single` | Return one row instead of all matching rows in random order. |
| `invertEmpty` | Select rows where the column is empty instead. |

### GetTableColumns

```csharp
public List<string> GetTableColumns(string tableName, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1045)

Column names of a table.

| Parameter | Description |
|---|---|
| `tableName` | Table name. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

### GetTables

```csharp
public List<string> GetTables(bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1027)

Names of all tables, sorted (PostgreSQL: base tables of the `public` schema).

| Parameter | Description |
|---|---|
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

### InsertDic

```csharp
public void InsertDic(Dictionary<string, string> data, string tableName = null, bool log = false, bool thrw = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L354)

Inserts one row from a dictionary. On PostgreSQL a conflicting row is skipped (`ON CONFLICT DO NOTHING`).

| Parameter | Description |
|---|---|
| `data` | Column → value. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |

### JsonToDb

```csharp
public void JsonToDb(string json, string tableName = null, bool log = false, bool thrw = false, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L405)

Flattens a JSON object into columns and writes it with `UpdFromDict`. Nested keys are joined with `_` (`a_b_0`). The original shape is saved in the `_json_structure` column so that `DbToJson` can rebuild it.

| Parameter | Description |
|---|---|
| `json` | JSON object. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |
| `where` | Raw SQL condition selecting the row. |

### PgToSqlite

```csharp
public void PgToSqlite(string pgTable, string sqlitePath, string sqliteTable, string pgSchema = "public", bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1518)

Copies a PostgreSQL table into an SQLite file. The target table is dropped and recreated; PostgreSQL types are mapped to INTEGER, REAL, TEXT or BLOB.

| Parameter | Description |
|---|---|
| `pgTable` | Source table. |
| `sqlitePath` | SQLite database file. |
| `sqliteTable` | Target table. |
| `pgSchema` | Source schema. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

**Remarks:** This instance must be in `pgSQL` mode.

### PrepareTable

```csharp
public void PrepareTable(Dictionary<string, string> tableStructure, string tableName = null, bool log = false, bool prune = false, bool rearrange = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L684)

Creates the table if it does not exist and adds missing columns.

| Parameter | Description |
|---|---|
| `tableStructure` | Column → SQL type, e.g. `{"id", "INTEGER PRIMARY KEY"}`. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `prune` | Also drop columns that are not in `tableStructure` (`PruneColumns`). |
| `rearrange` | Also reorder columns to match `tableStructure` (`RearrangeColumns`). |

```csharp
public void PrepareTable(List<string> columns, string tableName = null, string defaultType = "TEXT DEFAULT ''", string serial = "INTEGER", bool log = false, bool prune = false, bool rearrange = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L716)

Same as the dictionary overload, with an `id` primary key and one type for every other column.

| Parameter | Description |
|---|---|
| `columns` | Column names; `id` and duplicates are skipped. |
| `tableName` | Table; default is the table given to the constructor. |
| `defaultType` | SQL type of every column. |
| `serial` | Type of `id`. `INTEGER` becomes `INTEGER PRIMARY KEY AUTOINCREMENT` (`AUTOINCREMENT` is replaced by `SERIAL` on PostgreSQL). |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `prune` | Drop columns not in the list. |
| `rearrange` | Reorder columns to match the list. |

### PruneColumns

```csharp
public void PruneColumns(Dictionary<string, string> tableStructure, string tableName = null, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L951)

Drops every column except `id` that is not a key of `tableStructure`.

| Parameter | Description |
|---|---|
| `tableStructure` | Columns to keep. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

### PruneEmptyColumns

```csharp
public void PruneEmptyColumns(string tableName = null, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1175)

Drops every column except `id` in which no row has a non-empty value.

| Parameter | Description |
|---|---|
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

### Query

```csharp
public string Query(string query, bool log = false, bool thrw = false, bool unSafe = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L78)

Executes one SQL statement.

| Parameter | Description |
|---|---|
| `query` | SQL text. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |
| `unSafe` | Not used. |

**Returns:** For `SELECT`: rows joined by `·`, columns by `¦`. Otherwise the affected row count as text. An empty string after an error when `thrw` is false.

### RearrangeColumns

```csharp
public void RearrangeColumns(Dictionary<string, string> tableStructure, string tableName = null, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L747)

Reorders the table's columns: `id` first, then the columns of `tableStructure` that exist, then the rest. Works by copying the data into a new table, dropping the old one and renaming the new one. On failure the temporary table is dropped and an exception is thrown.

| Parameter | Description |
|---|---|
| `tableStructure` | Desired order (column → type). |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

### SetDone

```csharp
public void SetDone(string taskColumn = "daily", int cooldownMin = 0, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L383)

Writes a local timestamp `yyyy-MM-dd HH:mm:ss` to `taskColumn`: now, or now plus `cooldownMin`.

| Parameter | Description |
|---|---|
| `taskColumn` | Column to write. |
| `cooldownMin` | Minutes to add; 0 writes the current time. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`, inserted into the SQL as written: quote text values yourself. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

### SqliteToPg

```csharp
public void SqliteToPg(string sqlitePath, string sqliteTable, string pgTable, string pgSchema = "public", bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1555)

Copies an SQLite table into PostgreSQL. The target table is dropped and recreated; SQLite types are mapped to bigint, double precision, text or bytea.

| Parameter | Description |
|---|---|
| `sqlitePath` | SQLite database file. |
| `sqliteTable` | Source table. |
| `pgTable` | Target table. |
| `pgSchema` | Target schema. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

**Remarks:** This instance must be in `pgSQL` mode; it is the target.

### SqliteToSqlite

```csharp
public void SqliteToSqlite(string sourcePath, string sourceTable, string targetPath, string targetTable, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1589)

Copies a table between two SQLite files. The target table is dropped and recreated.

| Parameter | Description |
|---|---|
| `sourcePath` | Source database file. |
| `sourceTable` | Source table. |
| `targetPath` | Target database file. |
| `targetTable` | Target table. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

### SwapLines

```csharp
public void SwapLines(int id1, int id2, string tableName = null, bool log = false, bool thrw = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1339)

Exchanges the values of all columns except `id` between two rows.

| Parameter | Description |
|---|---|
| `id1` | First row id. |
| `id2` | Second row id. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Throw when a row is not found; otherwise it is logged and nothing changes. |

### TableExists

```csharp
public bool TableExists(string tableName, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1005)

Checks whether the table exists (in the `public` schema on PostgreSQL).

| Parameter | Description |
|---|---|
| `tableName` | Table name; double quotes are ignored. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |

### Upd

```csharp
public void Upd(string setClause, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L285)

Runs `UPDATE … SET setClause` for the row where `key` = `id`, or for the rows matching `where`.

| Parameter | Description |
|---|---|
| `setClause` | Assignments such as `status = 'ok', note = ''`; column names are quoted, values are taken as written. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |
| `key` | Column matched against `id`. |
| `id` | Value of `key`, inserted into the SQL as written: quote text values yourself. |
| `where` | Raw SQL condition. When set, `key` and `id` are ignored. |

### UpdFromDict

```csharp
public void UpdFromDict(Dictionary<string, string> data, string tableName = null, bool log = false, bool thrw = false, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L320)

Updates the rows matching `where` from a dictionary, adding missing columns first. A key `id` is written to the column `_id`. Single quotes are removed from values.

| Parameter | Description |
|---|---|
| `data` | Column → value. |
| `tableName` | Table; default is the table given to the constructor. |
| `log` | Write the query and its result to the project log even when the logger level is `Off`. A `Db` created without a project writes nothing. |
| `thrw` | Rethrow a database error instead of returning an empty result. |
| `where` | Raw SQL condition; required. |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
