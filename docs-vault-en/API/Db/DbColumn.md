---
title: "DbColumn"
tags: [api, Db]
generated: z3n7-docgen
---

# DbColumn

`static class` · namespace `z3n7` · source [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1250)

```csharp
public static class DbColumn
```

Adding, dropping and reordering columns.

## Methods

### ClmnAdd

```csharp
public static void ClmnAdd(this IZennoPosterProjectModel project, string clmnName, string tblName = null, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1278)

Adds a column if the table does not have it.

| Parameter | Description |
|---|---|
| `clmnName` | Column. |
| `tblName` | Table; default is `project.ProjectTable()` (`__` + project name). |
| `log` | Write the query and its result to the project log. |
| `defaultValue` | SQL type of the new column. |

```csharp
public static void ClmnAdd(this IZennoPosterProjectModel project, List<string> columns, string tblName, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1293)

Adds each listed column that the table does not have.

| Parameter | Description |
|---|---|
| `columns` | Columns. |
| `tblName` | Table; default is `project.ProjectTable()` (`__` + project name). |
| `log` | Write the query and its result to the project log. |
| `defaultValue` | SQL type of the new columns. |

```csharp
public static void ClmnAdd(this IZennoPosterProjectModel project, string[] columns, string tblName, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1303)

Adds each listed column that the table does not have.

| Parameter | Description |
|---|---|
| `columns` | Columns. |
| `tblName` | Table; default is `project.ProjectTable()` (`__` + project name). |
| `log` | Write the query and its result to the project log. |
| `defaultValue` | SQL type of the new columns. |

```csharp
public static void ClmnAdd(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1313)

Adds each column of `tableStructure` that the table does not have, with its type.

| Parameter | Description |
|---|---|
| `tableStructure` | Column → SQL type. |
| `tblName` | Table; default is `project.ProjectTable()` (`__` + project name). |
| `log` | Write the query and its result to the project log. |

### ClmnDrop

```csharp
public static void ClmnDrop(this IZennoPosterProjectModel project, string clmnName, string tblName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1345)

Drops a column if it exists (`CASCADE` on PostgreSQL).

| Parameter | Description |
|---|---|
| `clmnName` | Column. |
| `tblName` | Table; default is `project.ProjectTable()` (`__` + project name). |
| `log` | Write the query and its result to the project log. |

```csharp
public static void ClmnDrop(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1363)

Drops each column named by a key of `tableStructure` that the table has.

| Parameter | Description |
|---|---|
| `tableStructure` | Column → type; only the keys are used. |
| `tblName` | Table; default is `project.ProjectTable()` (`__` + project name). |
| `log` | Write the query and its result to the project log. |

### ClmnExist

```csharp
public static bool ClmnExist(this IZennoPosterProjectModel project, string clmnName, string tblName, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1258)

Checks whether a column exists. Case-insensitive on PostgreSQL, case-sensitive on SQLite.

| Parameter | Description |
|---|---|
| `clmnName` | Column. |
| `tblName` | Table. |
| `log` | Write the query and its result to the project log. |

### ClmnList

```csharp
public static List<string> ClmnList(this IZennoPosterProjectModel project, string tableName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1331)

Column names of a table.

| Parameter | Description |
|---|---|
| `tableName` | Table; default is `project.ProjectTable()` (`__` + project name). |
| `log` | Write the query and its result to the project log. |

### ClmnPrune

```csharp
public static void ClmnPrune(this IZennoPosterProjectModel project, string tblName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1381)

Drops every column except `id` in which no row has a non-empty value.

| Parameter | Description |
|---|---|
| `tblName` | Table; default is `project.ProjectTable()` (`__` + project name). |
| `log` | Write the query and its result to the project log. |

```csharp
public static void ClmnPrune(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1411)

Drops every column that is not a key of `tableStructure` (including `id` if it is not listed).

| Parameter | Description |
|---|---|
| `tableStructure` | Columns to keep. |
| `tblName` | Table; default is `project.ProjectTable()` (`__` + project name). |
| `log` | Write the query and its result to the project log. |

### ClmnRearrange

```csharp
public static void ClmnRearrange(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1433)

Reorders columns: `id` first, then the columns of `tableStructure` that exist, then the rest. Copies the data into a new table, drops the old one and renames the new one. On failure the temporary table is dropped and an exception is thrown.

| Parameter | Description |
|---|---|
| `tableStructure` | Desired order. |
| `tblName` | Table; default is `project.ProjectTable()` (`__` + project name). |
| `log` | Write the query and its result to the project log. |

```csharp
public static void ClmnRearrange(this IZennoPosterProjectModel project, List<string> projectColumns, string tblName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1486)

Reorders columns to the `TblForProject(projectColumns)` layout.

