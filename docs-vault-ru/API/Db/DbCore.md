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

Единая точка входа, через которую выполняется SQL к базе проекта.

## Методы

### DbQ

```csharp
public static string DbQ(this IZennoPosterProjectModel project, string query, bool log = false, string sqLitePath = null, bool thrw = false, bool unSafe = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1824)

Выполняет один SQL-запрос к базе, заданной `dbSource` (переменная проекта, иначе глобальная переменная). `dbSource`, начинающийся с `Host=`, — строка подключения PostgreSQL; всё остальное означает SQLite. На SQLite ошибка «database is locked» повторяется до 10 раз с нарастающей паузой.

| Параметр | Описание |
|---|---|
| `query` | Текст SQL. |
| `log` | Писать запрос и его результат в лог проекта. |
| `sqLitePath` | Файл базы SQLite. Путём к SQLite служит только этот аргумент. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `unSafe` | Не используется. |

**Возвращает:** Для `SELECT`: строки через `·`, колонки через `¦`. Иначе число затронутых строк текстом. Пустая строка после ошибки, если `thrw` равно false.

**Примечания:** Бросает исключение, если `dbSource` не задан.

