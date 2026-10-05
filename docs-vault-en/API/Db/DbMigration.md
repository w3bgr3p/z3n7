---
title: "DbMigration"
tags: [api, Db]
generated: z3n7-docgen
---

# DbMigration

`static class` · namespace `z3n7` · source [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1689)

```csharp
public static class DbMigration
```

Copying tables inside the project database and between PostgreSQL and SQLite.

## Methods

### MigrateAllTables

```csharp
public static void MigrateAllTables(this IZennoPosterProjectModel project)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1719)

Copies every user table from the current database to the other kind: PostgreSQL → SQLite or SQLite → PostgreSQL (see `Sql.MigrateAllTablesAsync`). Errors are written to the log as a warning.

**Remarks:** PostgreSQL is reached at localhost:5432, database and user `postgres`, password from the `DBpstgrPass` variable; SQLite at the `DBsqltPath` variable.

### MigrateTable

```csharp
public static void MigrateTable(this IZennoPosterProjectModel project, string source, string dest)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1702)

Copies `source` to a new table `dest` in the same database, then renames a column `acc0` or `key` to `id` if there is one.

| Parameter | Description |
|---|---|
| `source` | Existing table. |
| `dest` | Table to create. |

**Remarks:** The database kind follows `dbSource`. PostgreSQL is reached at localhost:5432, database and user `postgres`, password from the `DBpstgrPass` variable; SQLite at the `DBsqltPath` variable.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
