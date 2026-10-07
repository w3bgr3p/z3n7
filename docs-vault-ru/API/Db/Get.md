---
title: "Get"
tags: [api, Db]
generated: z3n7-docgen
---

# Get

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L84)

```csharp
public static class Get
```

Чтение из базы проекта. По умолчанию строка текущего аккаунта (`acc0`).

## Методы

### DbGet

```csharp
public static string DbGet(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", string acc = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L95)

Читает колонки одной строки; то же, что `SqlGet`.

| Параметр | Описание |
|---|---|
| `toGet` | Имена колонок через запятую; каждое берётся в кавычки. |
| `tableName` | Таблица; по умолчанию таблица проекта (переменная `projectTable`). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `acc` | Значение `key`; по умолчанию `acc0`. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

**Возвращает:** Колонки через `¦`; несколько строк соединяются через `·`.

### DbGetColumns

```csharp
public static Dictionary<string, string> DbGetColumns(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L111)

Читает колонки одной строки в виде колонка → значение.

| Параметр | Описание |
|---|---|
| `toGet` | Имена колонок через запятую; каждое берётся в кавычки. |
| `tableName` | Таблица; по умолчанию таблица проекта (переменная `projectTable`). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`; по умолчанию текущий аккаунт, `acc0`. Подставляется в SQL как написано. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

**Возвращает:** Пустой словарь, если ничего не найдено.

### DbGetLine

```csharp
public static string[] DbGetLine(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L126)

Читает колонки одной строки в виде массива.

| Параметр | Описание |
|---|---|
| `toGet` | Имена колонок через запятую; каждое берётся в кавычки. |
| `tableName` | Таблица; по умолчанию таблица проекта (переменная `projectTable`). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`; по умолчанию текущий аккаунт, `acc0`. Подставляется в SQL как написано. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

### DbGetLines

```csharp
public static List<string> DbGetLines(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "", string toList = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L142)

Читает несколько строк; в каждом элементе колонки соединены через `¦`.

| Параметр | Описание |
|---|---|
| `toGet` | Имена колонок через запятую; каждое берётся в кавычки. |
| `tableName` | Таблица; по умолчанию таблица проекта (переменная `projectTable`). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`; по умолчанию текущий аккаунт, `acc0`. Подставляется в SQL как написано. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |
| `toList` | Если задано, результат также записывается в этот список ZennoPoster. |

### DbGetRandom

```csharp
public static string DbGetRandom(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool acc = false, bool thrw = false, int range = 0, bool single = true, bool invert = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L178)

Читает `toGet` из случайных строк, где оно не пустое, а `id` меньше `range`.

| Параметр | Описание |
|---|---|
| `toGet` | Колонка, которую нужно прочитать. |
| `tableName` | Таблица; по умолчанию `project.ProjectTable()` (`__` + имя проекта). |
| `log` | Писать запрос и его результат в лог проекта. |
| `acc` | Начинать результат с колонки `id`. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `range` | Верхняя граница id (не включая); 0 — последний аккаунт из `project.Range()`. |
| `single` | Одна строка вместо всех подходящих. |
| `invert` | Вместо этого выбирать строки, где колонка пустая. |

### DbKey

```csharp
public static string DbKey(this IZennoPosterProjectModel project, string chainType = "evm")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L210)

Читает ключ текущего аккаунта из таблицы кошельков (`DbSchema.Wlt`) и расшифровывает его через `SAFU.Decode`.

| Параметр | Описание |
|---|---|
| `chainType` | `evm` (колонка `secp256k1`), `sol` (`base58`) или `seed` (`bip39`). Всё остальное приводит к исключению. |

### DbToVars

```csharp
public static Dictionary<string, string> DbToVars(this IZennoPosterProjectModel project, string toGet, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L160)

Читает колонки одной строки и для каждой задаёт переменную проекта с тем же именем.

| Параметр | Описание |
|---|---|
| `toGet` | Имена колонок через запятую; каждое берётся в кавычки. |
| `tableName` | Таблица; по умолчанию таблица проекта (переменная `projectTable`). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`; по умолчанию текущий аккаунт, `acc0`. Подставляется в SQL как написано. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

**Возвращает:** Значения, которые были заданы.

### RndInvite

```csharp
public static string RndInvite(this IZennoPosterProjectModel project, object limit = null, string inviteColumn = "refcode", bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L243)

Возвращает переменную `cfgRefCode`; если она пуста, берёт случайное непустое значение `inviteColumn` из таблицы проекта и сохраняет его в `cfgRefCode`.

| Параметр | Описание |
|---|---|
| `limit` | Если задано — только строки с `id` до этого числа. Должно быть положительным целым, иначе бросается исключение. |
| `inviteColumn` | Колонка с инвайт-кодами. |
| `log` | Писать запрос и его результат в лог проекта. |

