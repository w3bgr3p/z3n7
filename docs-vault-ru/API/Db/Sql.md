---
title: "Sql"
tags: [api, Db]
generated: z3n7-docgen
---

# Sql

`class` · пространство имён `z3n7` · исходник [Db/Sql.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L26)

```csharp
public class Sql : IDisposable
```

One open connection to SQLite (through the SQLite3 ODBC driver) or PostgreSQL (Npgsql). Dispose it to close the connection.

**Примечания:** The connection is opened in the constructor.

## Конструкторы

### Sql

```csharp
public Sql(string dbPath, string dbPass)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L34)

Opens an SQLite database through the `SQLite3 ODBC Driver`.

| Параметр | Описание |
|---|---|
| `dbPath` | Database file. |
| `dbPass` | Not used. |

```csharp
public Sql(string hostname, string port, string database, string user, string password)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L42)

Opens a PostgreSQL connection with pooling.

```csharp
public Sql(string connectionstring)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L49)

Opens a PostgreSQL connection from an Npgsql connection string.

```csharp
public Sql(IDbConnection connection)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L56)

Wraps an existing connection and opens it if it is closed.

## Свойства

### ConnectionType

```csharp
public DatabaseType ConnectionType { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L66)

SQLite for an ODBC connection, PostgreSQL for Npgsql, otherwise Unknown.

## Методы

### AddRange

```csharp
public async Task AddRange(int range, string tableName = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L776)

Inserts rows with ids from the current maximum + 1 up to `range`, one statement per row.

### CopyTableAsync

```csharp
public async Task<int> CopyTableAsync(string sourceTable, string destinationTable)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L261)

Creates `destinationTable` with the columns and primary key of `sourceTable` and copies all rows into it, within the same database.

| Параметр | Описание |
|---|---|
| `sourceTable` | Source; on PostgreSQL may be `schema.table`. |
| `destinationTable` | Table to create; must not exist. |

**Возвращает:** Number of copied rows.

### CreateParameter

```csharp
public IDbDataParameter CreateParameter(string name, object value)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L116)

Creates a command parameter of the right provider type; `null` becomes `DBNull`.

### CreateParameters

```csharp
public IDbDataParameter[] CreateParameters(params (string name, object value)[] parameters)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L133)

Creates several parameters, see `CreateParameter`.

### DbRead

```csharp
public string DbRead(string sql, string separator = "|")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L188)

Synchronous `DbReadAsync` with the default row separator.

### DbReadAsync

```csharp
public async Task<string> DbReadAsync(string sql, string columnSeparator = "|", string rawSepararor = "\r\n")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L147)

Runs a query and returns every row as text.

| Параметр | Описание |
|---|---|
| `sql` | Query. |
| `columnSeparator` | Joins the columns of a row. |
| `rawSepararor` | Joins the rows. |

### DbWrite

```csharp
public int DbWrite(string sql, params IDbDataParameter[] parameters)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L249)

Synchronous `DbWriteAsync`.

### DbWriteAsync

```csharp
public async Task<int> DbWriteAsync(string sql, params IDbDataParameter[] parameters)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L195)

Executes a non-query statement.

**Возвращает:** The affected row count. Errors are rethrown with the SQL text appended.

### Dispose

```csharp
public void Dispose()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L90)

Closes and disposes the connection.

### Get

```csharp
public async Task<string> Get(string toGet, string id, string tableName = null, string where = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L718)

Reads the first column of the first matching row.

| Параметр | Описание |
|---|---|
| `toGet` | Column list. |
| `id` | Row id, passed as a parameter. |
| `tableName` | Table; required. |
| `where` | Raw SQL condition; when set, `id` is ignored. |

**Возвращает:** The value as text, or `null`.

### MigrateAllTablesAsync

```csharp
public static async Task<int> MigrateAllTablesAsync(Sql sourceDb, Sql destinationDb)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L422)

Copies every user table from one database to another of the other kind (PostgreSQL ↔ SQLite). Tables that already exist in the target are not recreated, rows are still inserted. A table that fails is skipped; the error goes to the debug output only.

**Возвращает:** Total number of copied rows.

### Upd

```csharp
public async Task<int> Upd(string toUpd, object id, string tableName = null, string where = null, bool last = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L658)

Runs `UPDATE … SET toUpd` for the row `id` or the rows matching `where`.

| Параметр | Описание |
|---|---|
| `toUpd` | Assignments; column names are quoted. |
| `id` | Row id, inserted as written. |
| `tableName` | Table; required. |
| `where` | Raw SQL condition; when set, `id` is ignored. |
| `last` | Also set the `last` column to the UTC time `MM-ddTHH:mm`. |

**Возвращает:** Affected row count.

```csharp
public async Task Upd(List<string> toWrite, string tableName = null, string where = null, bool last = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L701)

Runs `Upd` for each item, with ids 0, 1, 2 … in list order.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
