---
title: "Sql"
tags: [api, Db]
generated: z3n7-docgen
---

# Sql

`class` · пространство имён `z3n7` · исходник [Db/Sql.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L20)

```csharp
public class Sql : IDisposable
```

*Описания пока нет.*

## Конструкторы

### Sql

```csharp
public Sql(string dbPath, string dbPass)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L25)

```csharp
public Sql(string hostname, string port, string database, string user, string password)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L32)

```csharp
public Sql(string connectionstring)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L38)

```csharp
public Sql(IDbConnection connection)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L44)

## Свойства

### ConnectionType

```csharp
public DatabaseType ConnectionType { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L53)

## Методы

### AddRange

```csharp
public async Task AddRange(int range, string tableName = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L720)

### CopyTableAsync

```csharp
public async Task<int> CopyTableAsync(string sourceTable, string destinationTable)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L228)

### CreateParameter

```csharp
public IDbDataParameter CreateParameter(string name, object value)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L99)

### CreateParameters

```csharp
public IDbDataParameter[] CreateParameters(params (string name, object value)[] parameters)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L115)

### DbRead

```csharp
public string DbRead(string sql, string separator = "|")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L165)

### DbReadAsync

```csharp
public async Task<string> DbReadAsync(string sql, string columnSeparator = "|", string rawSepararor = "\r\n")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L125)

### DbWrite

```csharp
public int DbWrite(string sql, params IDbDataParameter[] parameters)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L223)

### DbWriteAsync

```csharp
public async Task<int> DbWriteAsync(string sql, params IDbDataParameter[] parameters)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L170)

### Dispose

```csharp
public void Dispose()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L76)

### Get

```csharp
public async Task<string> Get(string toGet, string id, string tableName = null, string where = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L665)

### MigrateAllTablesAsync

```csharp
public static async Task<int> MigrateAllTablesAsync(Sql sourceDb, Sql destinationDb)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L383)

### Upd

```csharp
public async Task<int> Upd(string toUpd, object id, string tableName = null, string where = null, bool last = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L612)

```csharp
public async Task Upd(List<string> toWrite, string tableName = null, string where = null, bool last = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L654)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
