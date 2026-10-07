---
title: "FastDb"
tags: [api, Db]
generated: z3n7-docgen
---

# FastDb

`class` · пространство имён `z3n7` · исходник [Db/FastDb.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L23)

```csharp
public class FastDb
```

SQLite access through ZennoPoster's built-in ODBC query runner, without opening own connections.

## Конструкторы

### FastDb

```csharp
public FastDb(IZennoPosterProjectModel project, string dbName = null, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L32)

Uses the file `{project.Path}{dbName}.sql`.

| Параметр | Описание |
|---|---|
| `dbName` | File name without extension; default is the `dbName` variable, or `db`. |
| `log` | Write queries and `SELECT` answers to the log. |

## Методы

### dbList

```csharp
public List<string> dbList(string query)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L64)

Executes a query and returns its rows.

### dbString

```csharp
public string dbString(string query)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L53)

Executes a query.

**Возвращает:** Rows joined by line breaks, columns by `|`.

### ExportToCsv

```csharp
public void ExportToCsv(string tableName, string fileName)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L74)

Writes the whole table to `{project.Path}{fileName}` as CSV with a header row (UTF-8).

```csharp
public void ExportToCsv(string tableName, string fileName, string columns = "*")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L98)

Writes the selected columns to `{project.Path}{fileName}` as CSV, UTF-8 with BOM so that Excel opens it correctly.

| Параметр | Описание |
|---|---|
| `columns` | `*` for all columns, or a comma-separated list that is also used as the header. |

