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

Вид базы данных за соединением `Sql`.

## Значения

| | Описание |
|---|---|
| `Unknown` | Не SQLite и не PostgreSQL. |
| `SQLite` | SQLite через ODBC. |
| `PostgreSQL` | PostgreSQL через Npgsql. |

