---
title: "Sql"
tags: [api, Db]
generated: z3n7-docgen
---

# Sql

`class` · namespace `z3n7` · source [Db/Sql.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L29)

```csharp
public class Sql : IDisposable
```

One open connection to SQLite (through the SQLite3 ODBC driver) or PostgreSQL (Npgsql). Dispose it to close the connection.

**Remarks:** The connection is opened in the constructor.

## Constructors

### Sql

```csharp
public Sql(string dbPath, string dbPass)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L37)

Opens an SQLite database through the `SQLite3 ODBC Driver`.

| Parameter | Description |
|---|---|
| `dbPath` | Database file. |
| `dbPass` | Not used. |

```csharp
public Sql(string hostname, string port, string database, string user, string password)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L45)

Opens a PostgreSQL connection with pooling.

```csharp
public Sql(string connectionstring)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L52)

Opens a PostgreSQL connection from an Npgsql connection string.

```csharp
public Sql(IDbConnection connection)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L59)

Wraps an existing connection and opens it if it is closed.

## Properties

### ConnectionType

```csharp
public DatabaseType ConnectionType { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L69)

SQLite for an ODBC connection, PostgreSQL for Npgsql, otherwise Unknown.

## Methods

### AddRange

```csharp
public async Task AddRange(int range, string tableName = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L780)

Inserts rows with ids from the current maximum + 1 up to `range`, one statement per row.

### CopyTableAsync

```csharp
public async Task<int> CopyTableAsync(string sourceTable, string destinationTable)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L265)

Creates `destinationTable` with the columns and primary key of `sourceTable` and copies all rows into it, within the same database.

| Parameter | Description |
|---|---|
| `sourceTable` | Source; on PostgreSQL may be `schema.table`. |
| `destinationTable` | Table to create; must not exist. |

**Returns:** Number of copied rows.

### CreateParameter

```csharp
public IDbDataParameter CreateParameter(string name, object value)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L120)

Creates a command parameter of the right provider type; `null` becomes `DBNull`.

### CreateParameters

```csharp
public IDbDataParameter[] CreateParameters(params (string name, object value)[] parameters)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L137)

Creates several parameters, see `CreateParameter`.

### DbRead

```csharp
public string DbRead(string sql, string separator = "|")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L192)

Synchronous `DbReadAsync` with the default row separator.

### DbReadAsync

```csharp
public async Task<string> DbReadAsync(string sql, string columnSeparator = "|", string rawSepararor = "\r\n")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L151)

Runs a query and returns every row as text.

| Parameter | Description |
|---|---|
| `sql` | Query. |
| `columnSeparator` | Joins the columns of a row. |
| `rawSepararor` | Joins the rows. |

### DbWrite

```csharp
public int DbWrite(string sql, params IDbDataParameter[] parameters)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L253)

Synchronous `DbWriteAsync`.

### DbWriteAsync

```csharp
public async Task<int> DbWriteAsync(string sql, params IDbDataParameter[] parameters)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L199)

Executes a non-query statement.

**Returns:** The affected row count. Errors are rethrown with the SQL text appended.

### Dispose

```csharp
public void Dispose()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L93)

Closes and disposes the connection.

### Get

```csharp
public async Task<string> Get(string toGet, string id, string tableName = null, string where = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L722)

Reads the first column of the first matching row.

| Parameter | Description |
|---|---|
| `toGet` | Column list. |
| `id` | Row id, passed as a parameter. |
| `tableName` | Table; required. |
| `where` | Raw SQL condition; when set, `id` is ignored. |

**Returns:** The value as text, or `null`.

### MigrateAllTablesAsync

```csharp
public static async Task<int> MigrateAllTablesAsync(Sql sourceDb, Sql destinationDb)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L426)

Copies every user table from one database to another of the other kind (PostgreSQL ↔ SQLite). Tables that already exist in the target are not recreated, rows are still inserted. A table that fails is skipped; the error goes to the debug output only.

**Returns:** Total number of copied rows.

### Upd

```csharp
public async Task<int> Upd(string toUpd, object id, string tableName = null, string where = null, bool last = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L662)

Runs `UPDATE … SET toUpd` for the row `id` or the rows matching `where`.

| Parameter | Description |
|---|---|
| `toUpd` | Assignments; column names are quoted. |
| `id` | Row id, inserted as written. |
| `tableName` | Table; required. |
| `where` | Raw SQL condition; when set, `id` is ignored. |
| `last` | Also set the `last` column to the UTC time `MM-ddTHH:mm`. |

**Returns:** Affected row count.

```csharp
public async Task Upd(List<string> toWrite, string tableName = null, string where = null, bool last = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L705)

Runs `Upd` for each item, with ids 0, 1, 2 … in list order.

