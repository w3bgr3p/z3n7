---
title: "DbTable"
tags: [api, Db]
generated: z3n7-docgen
---

# DbTable

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1029)

```csharp
public static class DbTable
```

Создание и просмотр таблиц базы проекта.

## Методы

### EnsureTable

```csharp
public static void EnsureTable(this IZennoPosterProjectModel project, TableSchema schema)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1034)

Создаёт таблицу, описанную в `schema`, если её нет.

| Параметр | Описание |
|---|---|
| `schema` | Имя таблицы и колонки, например `DbSchema.Process`. |

### PrepareProjectTable

```csharp
public static void PrepareProjectTable(this IZennoPosterProjectModel project, string[] projectColumns, string tblName = null, bool log = false, bool prune = false, bool rearrange = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1172)

Вариант `PrepareProjectTable(List<string>, …)` для массива.

```csharp
public static void PrepareProjectTable(this IZennoPosterProjectModel project, List<string> projectColumns = null, string tblName = null, bool log = false, bool prune = false, bool rearrange = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1186)

Создаёт таблицу с раскладкой `TblForProject` при необходимости и добавляет недостающие колонки.

| Параметр | Описание |
|---|---|
| `projectColumns` | Дополнительные колонки. |
| `tblName` | Таблица; по умолчанию переменная `projectTable`. |
| `log` | Писать запрос и его результат в лог проекта. |
| `prune` | Заодно удалить колонки, которых нет в раскладке. |
| `rearrange` | Заодно переставить колонки в порядке раскладки. |

```csharp
public static void PrepareProjectTable(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false, bool prune = false, bool rearrange = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1202)

Создаёт таблицу при необходимости и добавляет недостающие колонки.

| Параметр | Описание |
|---|---|
| `tableStructure` | Колонка → тип SQL. |
| `tblName` | Таблица; по умолчанию переменная `projectTable`. |
| `log` | Писать запрос и его результат в лог проекта. |
| `prune` | Заодно удалить колонки, которых нет в `tableStructure`. |
| `rearrange` | Заодно переставить колонки в порядке `tableStructure`. |

### TblAdd

```csharp
public static void TblAdd(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1046)

Создаёт таблицу, если её нет. На PostgreSQL `INTEGER PRIMARY KEY AUTOINCREMENT` превращается в `SERIAL PRIMARY KEY`.

| Параметр | Описание |
|---|---|
| `tableStructure` | Колонка → тип SQL. |
| `tblName` | Таблица; может быть `schema.table`. |
| `log` | Писать запрос и его результат в лог проекта. |

### TblColumns

```csharp
public static List<string> TblColumns(this IZennoPosterProjectModel project, string tblName, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1100)

Имена колонок таблицы.

| Параметр | Описание |
|---|---|
| `tblName` | Таблица; на PostgreSQL может быть `schema.table`. |
| `log` | Писать запрос и его результат в лог проекта. |

### TblExist

```csharp
public static bool TblExist(this IZennoPosterProjectModel project, string tblName, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1064)

Проверяет, есть ли таблица.

| Параметр | Описание |
|---|---|
| `tblName` | Таблица; на PostgreSQL может быть `schema.table` (схема по умолчанию `public`). |
| `log` | Писать запрос и его результат в лог проекта. |

### TblForProject

```csharp
public static Dictionary<string, string> TblForProject(this IZennoPosterProjectModel project, List<string> projectColumns = null, string defaultType = "TEXT DEFAULT ''")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1121)

Строит раскладку таблицы: `id INTEGER PRIMARY KEY AUTOINCREMENT`, заданные колонки и по колонке на каждый элемент переменной `cfgToDo` (через запятую).

| Параметр | Описание |
|---|---|
| `projectColumns` | Дополнительные колонки. |
| `defaultType` | Тип SQL для всех колонок, кроме `id`. |

### TblList

```csharp
public static List<string> TblList(this IZennoPosterProjectModel project, bool log = false, string schema = "public")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1084)

Имена всех таблиц по алфавиту.

| Параметр | Описание |
|---|---|
| `log` | Писать запрос и его результат в лог проекта. |
| `schema` | Схема PostgreSQL. |

### TblPrepareDefault

```csharp
public static void TblPrepareDefault(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1161)

Создаёт таблицу проекта (переменная `projectTable`) с раскладкой `TblForProject` и добавляет недостающие колонки.

| Параметр | Описание |
|---|---|
| `log` | Писать запрос и его результат в лог проекта. |

