---
title: "DbColumn"
tags: [api, Db]
generated: z3n7-docgen
---

# DbColumn

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1250)

```csharp
public static class DbColumn
```

Добавление, удаление и перестановка колонок.

## Методы

### ClmnAdd

```csharp
public static void ClmnAdd(this IZennoPosterProjectModel project, string clmnName, string tblName = null, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1278)

Добавляет колонку, если её нет в таблице.

| Параметр | Описание |
|---|---|
| `clmnName` | Колонка. |
| `tblName` | Таблица; по умолчанию `project.ProjectTable()` (`__` + имя проекта). |
| `log` | Писать запрос и его результат в лог проекта. |
| `defaultValue` | Тип SQL новой колонки. |

```csharp
public static void ClmnAdd(this IZennoPosterProjectModel project, List<string> columns, string tblName, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1293)

Добавляет каждую из перечисленных колонок, которой нет в таблице.

| Параметр | Описание |
|---|---|
| `columns` | Колонки. |
| `tblName` | Таблица; по умолчанию `project.ProjectTable()` (`__` + имя проекта). |
| `log` | Писать запрос и его результат в лог проекта. |
| `defaultValue` | Тип SQL новых колонок. |

```csharp
public static void ClmnAdd(this IZennoPosterProjectModel project, string[] columns, string tblName, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1303)

Добавляет каждую из перечисленных колонок, которой нет в таблице.

| Параметр | Описание |
|---|---|
| `columns` | Колонки. |
| `tblName` | Таблица; по умолчанию `project.ProjectTable()` (`__` + имя проекта). |
| `log` | Писать запрос и его результат в лог проекта. |
| `defaultValue` | Тип SQL новых колонок. |

```csharp
public static void ClmnAdd(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1313)

Добавляет каждую колонку из `tableStructure`, которой нет в таблице, с её типом.

| Параметр | Описание |
|---|---|
| `tableStructure` | Колонка → тип SQL. |
| `tblName` | Таблица; по умолчанию `project.ProjectTable()` (`__` + имя проекта). |
| `log` | Писать запрос и его результат в лог проекта. |

### ClmnDrop

```csharp
public static void ClmnDrop(this IZennoPosterProjectModel project, string clmnName, string tblName = null, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1345)

Удаляет колонку, если она есть (на PostgreSQL с `CASCADE`).

| Параметр | Описание |
|---|---|
| `clmnName` | Колонка. |
| `tblName` | Таблица; по умолчанию `project.ProjectTable()` (`__` + имя проекта). |
| `log` | Писать запрос и его результат в лог проекта. |

```csharp
public static void ClmnDrop(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1363)

Удаляет каждую колонку, имя которой есть среди ключей `tableStructure` и которая есть в таблице.

| Параметр | Описание |
|---|---|
| `tableStructure` | Колонка → тип; используются только ключи. |
| `tblName` | Таблица; по умолчанию `project.ProjectTable()` (`__` + имя проекта). |
| `log` | Писать запрос и его результат в лог проекта. |

### ClmnExist

```csharp
public static bool ClmnExist(this IZennoPosterProjectModel project, string clmnName, string tblName, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1258)

Проверяет, есть ли колонка. На PostgreSQL без учёта регистра, на SQLite с учётом.

| Параметр | Описание |
|---|---|
| `clmnName` | Колонка. |
| `tblName` | Таблица. |
| `log` | Писать запрос и его результат в лог проекта. |

### ClmnList

```csharp
public static List<string> ClmnList(this IZennoPosterProjectModel project, string tableName = null, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1331)

Имена колонок таблицы.

| Параметр | Описание |
|---|---|
| `tableName` | Таблица; по умолчанию `project.ProjectTable()` (`__` + имя проекта). |
| `log` | Писать запрос и его результат в лог проекта. |

### ClmnPrune

```csharp
public static void ClmnPrune(this IZennoPosterProjectModel project, string tblName = null, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1381)

Удаляет все колонки, кроме `id`, в которых ни в одной строке нет непустого значения.

| Параметр | Описание |
|---|---|
| `tblName` | Таблица; по умолчанию `project.ProjectTable()` (`__` + имя проекта). |
| `log` | Писать запрос и его результат в лог проекта. |

```csharp
public static void ClmnPrune(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1411)

Удаляет все колонки, которых нет среди ключей `tableStructure` (включая `id`, если его нет в списке).

| Параметр | Описание |
|---|---|
| `tableStructure` | Какие колонки оставить. |
| `tblName` | Таблица; по умолчанию `project.ProjectTable()` (`__` + имя проекта). |
| `log` | Писать запрос и его результат в лог проекта. |

### ClmnRearrange

```csharp
public static void ClmnRearrange(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1433)

Переставляет колонки: сначала `id`, затем существующие колонки из `tableStructure`, затем остальные. Копирует данные в новую таблицу, удаляет старую и переименовывает новую. При ошибке временная таблица удаляется и бросается исключение.

| Параметр | Описание |
|---|---|
| `tableStructure` | Нужный порядок. |
| `tblName` | Таблица; по умолчанию `project.ProjectTable()` (`__` + имя проекта). |
| `log` | Писать запрос и его результат в лог проекта. |

```csharp
public static void ClmnRearrange(this IZennoPosterProjectModel project, List<string> projectColumns, string tblName = null, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1486)

Переставляет колонки по раскладке `TblForProject(projectColumns)`.

