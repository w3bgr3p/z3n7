---
title: "DatabaseType"
tags: [api, Db]
generated: z3n7-docgen
---

# DatabaseType

`enum` · пространство имён `z3n7` · исходник [Db/Sql.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Sql.cs#L14)

```csharp
public enum DatabaseType
```

Kind of database behind an `Sql` connection.

## Значения

| | Описание |
|---|---|
| `Unknown` | Not SQLite or PostgreSQL. |
| `SQLite` | SQLite through ODBC. |
| `PostgreSQL` | PostgreSQL through Npgsql. |

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
