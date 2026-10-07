---
title: "DbSql"
tags: [api, Db]
generated: z3n7-docgen
---

# DbSql

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L885)

```csharp
public static class DbSql
```

Низкоуровневые помощники SELECT и UPDATE, на которых построены методы `Db*`.

## Методы

### SqlGet

```csharp
public static string SqlGet(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L900)

Выбирает колонки из строки, где `key` = `id`, или из строк, подходящих под `where`.

| Параметр | Описание |
|---|---|
| `toGet` | Имена колонок через запятую; каждое берётся в кавычки. |
| `tableName` | Таблица; по умолчанию таблица проекта (переменная `projectTable`). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`; по умолчанию текущий аккаунт, `acc0`. Подставляется в SQL как написано. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

**Возвращает:** Колонки через `¦`, строки через `·`.

### SqlGetArrFromLine

```csharp
public static string[] SqlGetArrFromLine(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L968)

То же, что `SqlGet`, с разбивкой на значения колонок.

| Параметр | Описание |
|---|---|
| `toGet` | Имена колонок через запятую; каждое берётся в кавычки. |
| `tableName` | Таблица; по умолчанию таблица проекта (переменная `projectTable`). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`; по умолчанию текущий аккаунт, `acc0`. Подставляется в SQL как написано. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

### SqlGetDicFromLine

```csharp
public static Dictionary<string, string> SqlGetDicFromLine(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "", bool set = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L937)

То же, что `SqlGet`, но возвращает первую строку в виде колонка → значение.

| Параметр | Описание |
|---|---|
| `toGet` | Имена колонок через запятую; каждое берётся в кавычки. |
| `tableName` | Таблица; по умолчанию таблица проекта (переменная `projectTable`). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`; по умолчанию текущий аккаунт, `acc0`. Подставляется в SQL как написано. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |
| `set` | Заодно задать переменные проекта с именами колонок. |

### SqlGetListFromLines

```csharp
public static List<string> SqlGetListFromLines(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L983)

То же, что `SqlGet`, с разбивкой на строки.

| Параметр | Описание |
|---|---|
| `toGet` | Имена колонок через запятую; каждое берётся в кавычки. |
| `tableName` | Таблица; по умолчанию таблица проекта (переменная `projectTable`). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`; по умолчанию текущий аккаунт, `acc0`. Подставляется в SQL как написано. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

### SqlUpd

```csharp
public static string SqlUpd(this IZennoPosterProjectModel project, string toUpd, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1002)

Выполняет `UPDATE … SET toUpd` для строки, где `key` = `id`, или для строк, подходящих под `where`.

| Параметр | Описание |
|---|---|
| `toUpd` | Присваивания; имена колонок берутся в кавычки, значения — как написаны. |
| `tableName` | Таблица; по умолчанию таблица проекта (переменная `projectTable`). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`; по умолчанию текущий аккаунт, `acc0`. Подставляется в SQL как написано. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

**Возвращает:** Число затронутых строк, текстом.

