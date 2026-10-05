---
title: "Sql"
tags: [api, Db]
generated: z3n7-docgen
---

# Sql

`class` · namespace `z3n7` · source [Db/Sql.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L20)

```csharp
public class Sql : IDisposable
```

*No description yet.*

## Constructors

### Sql

```csharp
public Sql(string dbPath, string dbPass)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L25)

```csharp
public Sql(string hostname, string port, string database, string user, string password)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L32)

```csharp
public Sql(string connectionstring)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L38)

```csharp
public Sql(IDbConnection connection)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L44)

## Properties

### ConnectionType

```csharp
public DatabaseType ConnectionType { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L53)

## Methods

### AddRange

```csharp
public async Task AddRange(int range, string tableName = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L720)

### CopyTableAsync

```csharp
public async Task<int> CopyTableAsync(string sourceTable, string destinationTable)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L228)

### CreateParameter

```csharp
public IDbDataParameter CreateParameter(string name, object value)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L99)

### CreateParameters

```csharp
public IDbDataParameter[] CreateParameters(params (string name, object value)[] parameters)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L115)

### DbRead

```csharp
public string DbRead(string sql, string separator = "|")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L165)

### DbReadAsync

```csharp
public async Task<string> DbReadAsync(string sql, string columnSeparator = "|", string rawSepararor = "\r\n")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L125)

### DbWrite

```csharp
public int DbWrite(string sql, params IDbDataParameter[] parameters)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L223)

### DbWriteAsync

```csharp
public async Task<int> DbWriteAsync(string sql, params IDbDataParameter[] parameters)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L170)

### Dispose

```csharp
public void Dispose()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L76)

### Get

```csharp
public async Task<string> Get(string toGet, string id, string tableName = null, string where = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L665)

### MigrateAllTablesAsync

```csharp
public static async Task<int> MigrateAllTablesAsync(Sql sourceDb, Sql destinationDb)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L383)

### Upd

```csharp
public async Task<int> Upd(string toUpd, object id, string tableName = null, string where = null, bool last = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L612)

```csharp
public async Task Upd(List<string> toWrite, string tableName = null, string where = null, bool last = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L654)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
