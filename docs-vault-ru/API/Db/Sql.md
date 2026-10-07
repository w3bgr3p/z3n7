---
title: "Sql"
tags: [api, Db]
generated: z3n7-docgen
---

# Sql

`class` · пространство имён `z3n7` · исходник [Db/Sql.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L29)

```csharp
public class Sql : IDisposable
```

Одно открытое соединение с SQLite (через ODBC-драйвер SQLite3) или PostgreSQL (Npgsql). Вызови Dispose, чтобы закрыть соединение.

**Примечания:** Соединение открывается в конструкторе.

## Конструкторы

### Sql

```csharp
public Sql(string dbPath, string dbPass)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L37)

Открывает базу SQLite через `SQLite3 ODBC Driver`.

| Параметр | Описание |
|---|---|
| `dbPath` | Файл базы данных. |
| `dbPass` | Не используется. |

```csharp
public Sql(string hostname, string port, string database, string user, string password)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L45)

Открывает соединение PostgreSQL с пулом.

```csharp
public Sql(string connectionstring)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L52)

Открывает соединение PostgreSQL по строке подключения Npgsql.

```csharp
public Sql(IDbConnection connection)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L59)

Оборачивает существующее соединение и открывает его, если оно закрыто.

## Свойства

### ConnectionType

```csharp
public DatabaseType ConnectionType { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L69)

SQLite для соединения ODBC, PostgreSQL для Npgsql, иначе Unknown.

## Методы

### AddRange

```csharp
public async Task AddRange(int range, string tableName = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L780)

Вставляет строки с id от текущего максимума + 1 до `range`, по одному запросу на строку.

### CopyTableAsync

```csharp
public async Task<int> CopyTableAsync(string sourceTable, string destinationTable)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L265)

Создаёт `destinationTable` с колонками и первичным ключом `sourceTable` и копирует в неё все строки, в пределах одной базы.

| Параметр | Описание |
|---|---|
| `sourceTable` | Источник; на PostgreSQL может быть `schema.table`. |
| `destinationTable` | Таблица, которую нужно создать; её не должно быть. |

**Возвращает:** Число скопированных строк.

### CreateParameter

```csharp
public IDbDataParameter CreateParameter(string name, object value)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L120)

Создаёт параметр команды нужного для провайдера типа; `null` превращается в `DBNull`.

### CreateParameters

```csharp
public IDbDataParameter[] CreateParameters(params (string name, object value)[] parameters)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L137)

Создаёт несколько параметров, см. `CreateParameter`.

### DbRead

```csharp
public string DbRead(string sql, string separator = "|")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L192)

Синхронный `DbReadAsync` с разделителем строк по умолчанию.

### DbReadAsync

```csharp
public async Task<string> DbReadAsync(string sql, string columnSeparator = "|", string rawSepararor = "\r\n")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L151)

Выполняет запрос и возвращает все строки текстом.

| Параметр | Описание |
|---|---|
| `sql` | Запрос. |
| `columnSeparator` | Чем соединять колонки строки. |
| `rawSepararor` | Чем соединять строки. |

### DbWrite

```csharp
public int DbWrite(string sql, params IDbDataParameter[] parameters)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L253)

Синхронный `DbWriteAsync`.

### DbWriteAsync

```csharp
public async Task<int> DbWriteAsync(string sql, params IDbDataParameter[] parameters)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L199)

Выполняет запрос, не возвращающий строк.

**Возвращает:** Число затронутых строк. Ошибки пробрасываются с добавленным текстом SQL.

### Dispose

```csharp
public void Dispose()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L93)

Закрывает соединение и освобождает его.

### Get

```csharp
public async Task<string> Get(string toGet, string id, string tableName = null, string where = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L722)

Читает первую колонку первой подходящей строки.

| Параметр | Описание |
|---|---|
| `toGet` | Список колонок. |
| `id` | Id строки, передаётся параметром. |
| `tableName` | Таблица; обязательна. |
| `where` | Сырое SQL-условие; если задано, `id` игнорируется. |

**Возвращает:** Значение текстом или `null`.

### MigrateAllTablesAsync

```csharp
public static async Task<int> MigrateAllTablesAsync(Sql sourceDb, Sql destinationDb)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L426)

Копирует все пользовательские таблицы из одной базы в базу другого вида (PostgreSQL ↔ SQLite). Таблицы, которые уже есть в целевой базе, не пересоздаются, но строки всё равно вставляются. Таблица с ошибкой пропускается; ошибка выводится только в отладочный вывод.

**Возвращает:** Общее число скопированных строк.

### Upd

```csharp
public async Task<int> Upd(string toUpd, object id, string tableName = null, string where = null, bool last = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L662)

Выполняет `UPDATE … SET toUpd` для строки `id` или для строк, подходящих под `where`.

| Параметр | Описание |
|---|---|
| `toUpd` | Присваивания; имена колонок берутся в кавычки. |
| `id` | Id строки, подставляется как написан. |
| `tableName` | Таблица; обязательна. |
| `where` | Сырое SQL-условие; если задано, `id` игнорируется. |
| `last` | Заодно записать в колонку `last` время UTC в формате `MM-ddTHH:mm`. |

**Возвращает:** Число затронутых строк.

```csharp
public async Task Upd(List<string> toWrite, string tableName = null, string where = null, bool last = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L705)

Выполняет `Upd` для каждого элемента с id 0, 1, 2 … в порядке списка.

