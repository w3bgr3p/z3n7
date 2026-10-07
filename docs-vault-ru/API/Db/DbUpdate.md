---
title: "DbUpdate"
tags: [api, Db]
generated: z3n7-docgen
---

# DbUpdate

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L278)

```csharp
public static class DbUpdate
```

Запись в базу проекта.

## Методы

### DbDone

```csharp
public static void DbDone(this IZennoPosterProjectModel project, string task = "daily", int cooldownMin = 0, string tableName = null, bool log = false, bool thrw = false, string key = "id", object acc = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L375)

Записывает метку кулдауна (`Time.Cd`, ISO UTC) в колонку `task` строки текущего аккаунта (или строки, выбранной `key`/`acc`, или строк, подходящих под `where`): конец сегодняшнего дня или текущий момент плюс `cooldownMin`.

| Параметр | Описание |
|---|---|
| `task` | Колонка, в которую нужно записать. |
| `cooldownMin` | Минуты от текущего момента; 0 — сегодня 23:59:59 UTC. |
| `tableName` | Таблица; по умолчанию таблица проекта (переменная `projectTable`). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `acc` | Значение `key`; по умолчанию текущий аккаунт, `acc0`. Подставляется в SQL как написано. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

### DbInsert

```csharp
public static string DbInsert(this IZennoPosterProjectModel project, Dictionary<string, string> dataDic, string tableName = null, bool log = false, bool thrw = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L288)

Вставляет одну строку. Ключ `id` пропускается; пустой словарь вставляет строку со значениями по умолчанию.

| Параметр | Описание |
|---|---|
| `dataDic` | Колонка → значение. |
| `tableName` | Таблица; по умолчанию таблица проекта (переменная `projectTable`). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |

**Возвращает:** Число затронутых строк, текстом.

### DbUpd

```csharp
public static void DbUpd(this IZennoPosterProjectModel project, string toUpd, string tableName = null, bool log = false, bool thrw = false, string key = "id", object acc = null, string where = "", string saveToVar = "lastQuery")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L354)

Выполняет `UPDATE … SET toUpd` для строки текущего аккаунта или для строк, подходящих под `where`.

| Параметр | Описание |
|---|---|
| `toUpd` | Присваивания вида `status = 'ok'`; имена колонок берутся в кавычки, значения — как написаны. |
| `tableName` | Таблица; по умолчанию таблица проекта (переменная `projectTable`). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `acc` | Значение `key`; по умолчанию текущий аккаунт, `acc0`. Подставляется в SQL как написано. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |
| `saveToVar` | Переменная, в которую перед обновлением записывается `toUpd`; пусто — не записывать. |

### DicToDb

```csharp
public static void DicToDb(this IZennoPosterProjectModel project, Dictionary<string, string> dataDic, string tableName = null, bool log = false, bool thrw = false, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L312)

Записывает словарь в строку текущего аккаунта (или в строки, подходящие под `where`), сначала добавив недостающие колонки. Ключ `id` пишется в колонку `_id`.

| Параметр | Описание |
|---|---|
| `dataDic` | Колонка → значение. Изменяется на месте, если есть ключ `id`. |
| `tableName` | Таблица; по умолчанию переменная `projectTable`. |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

