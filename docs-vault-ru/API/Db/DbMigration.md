---
title: "DbMigration"
tags: [api, Db]
generated: z3n7-docgen
---

# DbMigration

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1689)

```csharp
public static class DbMigration
```

Копирование таблиц внутри базы проекта и между PostgreSQL и SQLite.

## Методы

### MigrateAllTables

```csharp
public static void MigrateAllTables(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1719)

Копирует все пользовательские таблицы из текущей базы в базу другого вида: PostgreSQL → SQLite или SQLite → PostgreSQL (см. `Sql.MigrateAllTablesAsync`). Ошибки пишутся в лог как предупреждение.

**Примечания:** PostgreSQL — на localhost:5432, база и пользователь `postgres`, пароль из переменной `DBpstgrPass`; SQLite — по пути из переменной `DBsqltPath`.

### MigrateTable

```csharp
public static void MigrateTable(this IZennoPosterProjectModel project, string source, string dest)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1702)

Копирует `source` в новую таблицу `dest` в той же базе, затем переименовывает колонку `acc0` или `key` в `id`, если такая есть.

| Параметр | Описание |
|---|---|
| `source` | Существующая таблица. |
| `dest` | Таблица, которую нужно создать. |

**Примечания:** Вид базы определяется `dbSource`. PostgreSQL — на localhost:5432, база и пользователь `postgres`, пароль из переменной `DBpstgrPass`; SQLite — по пути из переменной `DBsqltPath`.

