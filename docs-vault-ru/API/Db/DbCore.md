---
title: "DbCore"
tags: [api, Db]
generated: z3n7-docgen
---

# DbCore

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1784)

```csharp
public static class DbCore
```

Single entry point that runs SQL against the project database.

## Методы

### DbQ

```csharp
public static string DbQ(this IZennoPosterProjectModel project, string query, bool log = false, string sqLitePath = null, bool thrw = false, bool unSafe = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1824)

Executes one SQL statement against the database named by `dbSource` (project variable, else global variable). A `dbSource` starting with `Host=` is a PostgreSQL connection string; anything else selects SQLite. On SQLite a "database is locked" error is retried up to 10 times with a growing pause.

| Параметр | Описание |
|---|---|
| `query` | SQL text. |
| `log` | Write the query and its result to the project log. |
| `sqLitePath` | SQLite database file. Only this argument is used as the SQLite path. |
| `thrw` | Throw on a database error instead of logging a warning and returning an empty result. |
| `unSafe` | Not used. |

**Возвращает:** For `SELECT`: rows joined by `·`, columns by `¦`. Otherwise the affected row count as text. An empty string after an error when `thrw` is false.

**Примечания:** Throws when `dbSource` is not set.

