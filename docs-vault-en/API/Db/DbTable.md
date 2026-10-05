---
title: "DbTable"
tags: [api, Db]
generated: z3n7-docgen
---

# DbTable

`static class` · namespace `z3n7` · source [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1029)

```csharp
public static class DbTable
```

Creating and inspecting tables of the project database.

## Methods

### EnsureTable

```csharp
public static void EnsureTable(this IZennoPosterProjectModel project, TableSchema schema)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1034)

Creates the table described by `schema` if it does not exist.

| Parameter | Description |
|---|---|
| `schema` | Table name and columns, e.g. `DbSchema.Process`. |

### PrepareProjectTable

```csharp
public static void PrepareProjectTable(this IZennoPosterProjectModel project, string[] projectColumns, string tblName = null, bool log = false, bool prune = false, bool rearrange = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1172)

Array form of `PrepareProjectTable(List<string>, …)`.

```csharp
public static void PrepareProjectTable(this IZennoPosterProjectModel project, List<string> projectColumns = null, string tblName = null, bool log = false, bool prune = false, bool rearrange = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1186)

Creates the table with the `TblForProject` layout if needed and adds missing columns.

| Parameter | Description |
|---|---|
| `projectColumns` | Extra columns. |
| `tblName` | Table; default is the `projectTable` variable. |
| `log` | Write the query and its result to the project log. |
| `prune` | Also drop columns that are not in the layout. |
| `rearrange` | Also reorder columns to match the layout. |

```csharp
public static void PrepareProjectTable(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false, bool prune = false, bool rearrange = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1202)

Creates the table if needed and adds missing columns.

| Parameter | Description |
|---|---|
| `tableStructure` | Column → SQL type. |
| `tblName` | Table; default is the `projectTable` variable. |
| `log` | Write the query and its result to the project log. |
| `prune` | Also drop columns that are not in `tableStructure`. |
| `rearrange` | Also reorder columns to match `tableStructure`. |

### TblAdd

```csharp
public static void TblAdd(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1046)

Creates a table unless it exists. On PostgreSQL `INTEGER PRIMARY KEY AUTOINCREMENT` becomes `SERIAL PRIMARY KEY`.

| Parameter | Description |
|---|---|
| `tableStructure` | Column → SQL type. |
| `tblName` | Table; may be `schema.table`. |
| `log` | Write the query and its result to the project log. |

### TblColumns

```csharp
public static List<string> TblColumns(this IZennoPosterProjectModel project, string tblName, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1100)

Column names of a table.

| Parameter | Description |
|---|---|
| `tblName` | Table; on PostgreSQL may be `schema.table`. |
| `log` | Write the query and its result to the project log. |

### TblExist

```csharp
public static bool TblExist(this IZennoPosterProjectModel project, string tblName, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1064)

Checks whether a table exists.

| Parameter | Description |
|---|---|
| `tblName` | Table; on PostgreSQL may be `schema.table` (default schema `public`). |
| `log` | Write the query and its result to the project log. |

### TblForProject

```csharp
public static Dictionary<string, string> TblForProject(this IZennoPosterProjectModel project, List<string> projectColumns = null, string defaultType = "TEXT DEFAULT ''")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1121)

Builds a table layout: `id INTEGER PRIMARY KEY AUTOINCREMENT`, the given columns, and one column per item of the comma-separated `cfgToDo` variable.

| Parameter | Description |
|---|---|
| `projectColumns` | Extra columns. |
| `defaultType` | SQL type of every column except `id`. |

### TblList

```csharp
public static List<string> TblList(this IZennoPosterProjectModel project, bool log = false, string schema = "public")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1084)

Names of all tables, sorted.

| Parameter | Description |
|---|---|
| `log` | Write the query and its result to the project log. |
| `schema` | PostgreSQL schema. |

### TblPrepareDefault

```csharp
public static void TblPrepareDefault(this IZennoPosterProjectModel project, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1161)

Creates the project table (`projectTable` variable) with the `TblForProject` layout and adds missing columns.

| Parameter | Description |
|---|---|
| `log` | Write the query and its result to the project log. |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
