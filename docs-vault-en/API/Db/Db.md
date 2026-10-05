---
title: "Db"
tags: [api, Db]
generated: z3n7-docgen
---

# Db

`class` · namespace `z3n7` · source [Db/Db.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L12), [Db/DbZenno.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbZenno.cs#L7)

```csharp
public class Db
```

*No description yet.*

## Constructors

### Db

```csharp
public Db(string dbMode = "pgSQL", string sqLitePath = null, string pgHost = "localhost", string pgPort = "5432", string pgDbName = "postgres", string pgUser = "postgres", string pgPass = "", string defaultTable = null, LogLevel logLevel = LogLevel.Off)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L29)

```csharp
public Db(IZennoPosterProjectModel project, string dbMode = null, string sqLitePath = null, string pgHost = null, string pgPort = null, string pgDbName = null, string pgUser = null, string pgPass = null, string defaultTable = null, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbZenno.cs#L10)

## Methods

### AddColumn

```csharp
public void AddColumn(string columnName, string tableName = null, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L831)

### AddColumns

```csharp
public void AddColumns(List<string> columns, string tableName = null, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L843)

```csharp
public void AddColumns(Dictionary<string, string> tableStructure, string tableName = null, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L851)

### AddRange

```csharp
public void AddRange(string tableName, int range, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L905)

### BridgeTable

```csharp
public void BridgeTable(string sourceTable, string targetDbPath, string targetTable, string targetMode = "SQLite", string schema = "public", bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1268)

Transfer table between two databases (auto-detect direction)

| Parameter | Description |
|---|---|
| `sourceTable` | Source table name |
| `targetDbPath` | Target database path (for SQLite) or connection string |
| `targetTable` | Target table name |
| `targetMode` | Target database mode (PostgreSQL or SQLite) |
| `schema` | Schema name for PostgreSQL |
| `log` | Enable logging |

### Clear

```csharp
public void Clear(string tableName = null, bool log = false, bool thrw = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L962)

Delete all rows from table (truncate)

### ClearLine

```csharp
public void ClearLine(int id, string tableName = null, bool log = false, bool thrw = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L983)

### ColumnExists

```csharp
public bool ColumnExists(string columnName, string tableName, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L814)

### CreateTable

```csharp
public void CreateTable(Dictionary<string, string> tableStructure, string tableName, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L751)

### DbToJson

```csharp
public string DbToJson(string tableName = null, bool log = false, bool thrw = false, object id = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L255)

### Del

```csharp
public void Del(string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L936)

Delete rows from table

### DropColumn

```csharp
public void DropColumn(string columnName, string tableName = null, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L868)

### Get

```csharp
public string Get(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L91)

### GetColumns

```csharp
public Dictionary<string, string> GetColumns(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L116)

### GetLine

```csharp
public string[] GetLine(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L137)

### GetLines

```csharp
public List<string> GetLines(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L142)

### GetRandom

```csharp
public string GetRandom(string column, string tableName = null, bool log = false, bool thrw = false, int maxId = 0, bool includeId = false, bool single = true, bool invertEmpty = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L147)

### GetTableColumns

```csharp
public List<string> GetTableColumns(string tableName, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L799)

### GetTables

```csharp
public List<string> GetTables(bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L787)

### InsertDic

```csharp
public void InsertDic(Dictionary<string, string> data, string tableName = null, bool log = false, bool thrw = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L222)

### JsonToDb

```csharp
public void JsonToDb(string json, string tableName = null, bool log = false, bool thrw = false, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L245)

### PgToSqlite

```csharp
public void PgToSqlite(string pgTable, string sqlitePath, string sqliteTable, string pgSchema = "public", bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1179)

Transfer table from PostgreSQL to SQLite

| Parameter | Description |
|---|---|
| `pgSchema` | PostgreSQL schema (default: public) |
| `pgTable` | PostgreSQL table name |
| `sqlitePath` | Path to SQLite database file |
| `sqliteTable` | SQLite table name |
| `log` | Enable logging |

### PrepareTable

```csharp
public void PrepareTable(Dictionary<string, string> tableStructure, string tableName = null, bool log = false, bool prune = false, bool rearrange = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L500)

```csharp
public void PrepareTable(List<string> columns, string tableName = null, string defaultType = "TEXT DEFAULT ''", string serial = "INTEGER", bool log = false, bool prune = false, bool rearrange = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L516)

### PruneColumns

```csharp
public void PruneColumns(Dictionary<string, string> tableStructure, string tableName = null, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L732)

### PruneEmptyColumns

```csharp
public void PruneEmptyColumns(string tableName = null, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L882)

### Query

```csharp
public string Query(string query, bool log = false, bool thrw = false, bool unSafe = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L46)

### RearrangeColumns

```csharp
public void RearrangeColumns(Dictionary<string, string> tableStructure, string tableName = null, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L535)

### SetDone

```csharp
public void SetDone(string taskColumn = "daily", int cooldownMin = 0, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L236)

### SqliteToPg

```csharp
public void SqliteToPg(string sqlitePath, string sqliteTable, string pgTable, string pgSchema = "public", bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1211)

Transfer table from SQLite to PostgreSQL

| Parameter | Description |
|---|---|
| `sqlitePath` | Path to SQLite database file |
| `sqliteTable` | SQLite table name |
| `pgTable` | PostgreSQL table name |
| `pgSchema` | PostgreSQL schema (default: public) |
| `log` | Enable logging |

### SqliteToSqlite

```csharp
public void SqliteToSqlite(string sourcePath, string sourceTable, string targetPath, string targetTable, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1239)

Transfer table from one SQLite database to another SQLite database

| Parameter | Description |
|---|---|
| `sourcePath` | Source SQLite database path |
| `sourceTable` | Source table name |
| `targetPath` | Target SQLite database path |
| `targetTable` | Target table name |
| `log` | Enable logging |

### SwapLines

```csharp
public void SwapLines(int id1, int id2, string tableName = null, bool log = false, bool thrw = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1005)

### TableExists

```csharp
public bool TableExists(string tableName, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L770)

### Upd

```csharp
public void Upd(string setClause, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L176)

### UpdFromDict

```csharp
public void UpdFromDict(Dictionary<string, string> data, string tableName = null, bool log = false, bool thrw = false, string where = "")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L199)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
