---
title: "Db"
tags: [api, Db]
generated: z3n7-docgen
---

# Db

`class` · пространство имён `z3n7` · исходник [Db/Db.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L22), [Db/DbZenno.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbZenno.cs#L7)

```csharp
public class Db
```

Помощник SQL поверх PostgreSQL или SQLite с одним API для обоих. Каждый запрос открывает своё соединение. `SELECT` возвращает строки через `·` и колонки через `¦`; остальные запросы возвращают число затронутых строк. На SQLite ошибка «database is locked» повторяется до 10 раз с нарастающей паузой.

**Примечания:** SQLite подключается через ODBC-драйвер SQLite3, он должен быть установлен. Значения, переданные как `id` или `where`, подставляются в SQL как написаны.

## Конструкторы

### Db

```csharp
public Db(string dbMode = "pgSQL", string sqLitePath = null, string pgHost = "localhost", string pgPort = "5432", string pgDbName = "postgres", string pgUser = "postgres", string pgPass = "", string defaultTable = null, LogLevel logLevel = LogLevel.Off)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L49)

Создаёт помощника базы данных с явно заданными настройками подключения.

| Параметр | Описание |
|---|---|
| `dbMode` | `pgSQL` — PostgreSQL; любое другое значение — SQLite. |
| `sqLitePath` | Файл базы SQLite. |
| `pgHost` | Хост PostgreSQL. |
| `pgPort` | Порт PostgreSQL. |
| `pgDbName` | База PostgreSQL. |
| `pgUser` | Пользователь PostgreSQL. |
| `pgPass` | Пароль PostgreSQL. |
| `defaultTable` | Таблица, которая используется, если методу не передано имя таблицы. |
| `logLevel` | Для вывода не используется: без проекта логгеру некуда писать. |

```csharp
public Db(IZennoPosterProjectModel project, string dbMode = null, string sqLitePath = null, string pgHost = null, string pgPort = null, string pgDbName = null, string pgUser = null, string pgPass = null, string defaultTable = null, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbZenno.cs#L18)

Создаёт помощника базы данных по настройкам проекта. Каждый аргумент, оставленный `null`, читается так: `dbMode` — переменная `DBmode`; `sqLitePath` — переменная `DBsqltPath`; хост, порт, база, пользователь и пароль PostgreSQL — глобальные переменные `sqlPgHost`, `sqlPgPort`, `sqlPgName`, `sqlPgUser`, `sqlPgPass`; `defaultTable` — `project.ProjectTable()` (`__` + имя проекта).

| Параметр | Описание |
|---|---|
| `log` | Писать запросы и результаты в лог на уровне `Info`. |

## Методы

### AddColumn

```csharp
public void AddColumn(string columnName, string tableName = null, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1094)

Добавляет колонку, если её ещё нет в таблице.

| Параметр | Описание |
|---|---|
| `columnName` | Имя колонки. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `defaultValue` | Тип SQL новой колонки. |

### AddColumns

```csharp
public void AddColumns(List<string> columns, string tableName = null, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1114)

Добавляет каждую из перечисленных колонок, которой ещё нет в таблице.

| Параметр | Описание |
|---|---|
| `columns` | Имена колонок. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `defaultValue` | Тип SQL новых колонок. |

```csharp
public void AddColumns(Dictionary<string, string> tableStructure, string tableName = null, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1131)

Добавляет каждую колонку из `tableStructure`, которой ещё нет в таблице, с её типом.

| Параметр | Описание |
|---|---|
| `tableStructure` | Колонка → тип SQL. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

### AddRange

```csharp
public void AddRange(string tableName, int range, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1208)

Вставляет строки с id от текущего максимума + 1 до `range` порциями по 500. Существующие id пропускаются.

| Параметр | Описание |
|---|---|
| `tableName` | Таблица с колонкой `id`. |
| `range` | Наибольший id, который должен быть. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

### BridgeTable

```csharp
public void BridgeTable(string sourceTable, string targetDbPath, string targetTable, string targetMode = "SQLite", string schema = "public", bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1622)

Копирует таблицу из этой базы в другую: PostgreSQL → SQLite, SQLite → PostgreSQL или SQLite → SQLite. Целевая таблица удаляется и создаётся заново.

| Параметр | Описание |
|---|---|
| `sourceTable` | Таблица в этой базе. |
| `targetDbPath` | Целевой файл SQLite или строка подключения Npgsql к целевой базе для `pgSQL`. |
| `targetTable` | Целевая таблица. |
| `targetMode` | `SQLite` или `pgSQL`. |
| `schema` | Схема PostgreSQL. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

### Clear

```csharp
public void Clear(string tableName = null, bool log = false, bool thrw = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1279)

Удаляет все строки и сбрасывает счётчик id (`TRUNCATE … RESTART IDENTITY CASCADE` на PostgreSQL, запись в `sqlite_sequence` на SQLite).

| Параметр | Описание |
|---|---|
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |

### ClearLine

```csharp
public void ClearLine(int id, string tableName = null, bool log = false, bool thrw = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1308)

Записывает пустую строку во все колонки одной строки, кроме `id`.

| Параметр | Описание |
|---|---|
| `id` | Id строки. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |

### ColumnExists

```csharp
public bool ColumnExists(string columnName, string tableName, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1069)

Проверяет, есть ли колонка. На PostgreSQL без учёта регистра, на SQLite с учётом.

| Параметр | Описание |
|---|---|
| `columnName` | Имя колонки. |
| `tableName` | Имя таблицы. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

### CreateTable

```csharp
public void CreateTable(Dictionary<string, string> tableStructure, string tableName, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L980)

Создаёт таблицу, если её нет. На PostgreSQL `AUTOINCREMENT` в типе заменяется на `SERIAL`.

| Параметр | Описание |
|---|---|
| `tableStructure` | Колонка → тип SQL. |
| `tableName` | Таблица, которую нужно создать. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

### DbToJson

```csharp
public string DbToJson(string tableName = null, bool log = false, bool thrw = false, object id = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L430)

Восстанавливает JSON, сохранённый `JsonToDb`, из строки с заданным `id`. Колонки, начинающиеся с `_`, и `id` не попадают.

| Параметр | Описание |
|---|---|
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |
| `id` | Значение `key`, подставляется в SQL как написано: текстовые значения бери в кавычки сам. |

**Возвращает:** Текст JSON или `{}`, если в таблице нет колонки `_json_structure` или её не удалось разобрать.

### Del

```csharp
public void Del(string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1246)

Удаляет строку, где `key` = `id`, или строки, подходящие под `where`.

| Параметр | Описание |
|---|---|
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`, подставляется в SQL как написано: текстовые значения бери в кавычки сам. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

### DropColumn

```csharp
public void DropColumn(string columnName, string tableName = null, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1155)

Удаляет колонку, если она есть (на PostgreSQL с `CASCADE`).

| Параметр | Описание |
|---|---|
| `columnName` | Имя колонки. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

### Get

```csharp
public string Get(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L137)

Выбирает колонки из строки, где `key` = `id`, или из строк, подходящих под `where`.

| Параметр | Описание |
|---|---|
| `columns` | Имена колонок через запятую; каждое берётся в кавычки. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`, подставляется в SQL как написано: текстовые значения бери в кавычки сам. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

**Возвращает:** Сырой результат в формате `Query`.

### GetColumns

```csharp
public Dictionary<string, string> GetColumns(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L174)

То же, что `Get`, но возвращает первую строку в виде колонка → значение.

| Параметр | Описание |
|---|---|
| `columns` | Имена колонок через запятую; каждое берётся в кавычки. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`, подставляется в SQL как написано: текстовые значения бери в кавычки сам. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

**Возвращает:** Пустой словарь, если ничего не найдено.

### GetLine

```csharp
public string[] GetLine(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L206)

То же, что `Get`, с разбивкой на значения колонок.

| Параметр | Описание |
|---|---|
| `columns` | Имена колонок через запятую; каждое берётся в кавычки. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`, подставляется в SQL как написано: текстовые значения бери в кавычки сам. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

### GetLines

```csharp
public List<string> GetLines(string columns, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L222)

То же, что `Get`, с разбивкой на строки. Колонки в каждой строке по-прежнему соединены через `¦`.

| Параметр | Описание |
|---|---|
| `columns` | Имена колонок через запятую; каждое берётся в кавычки. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`, подставляется в SQL как написано: текстовые значения бери в кавычки сам. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

### GetRandom

```csharp
public string GetRandom(string column, string tableName = null, bool log = false, bool thrw = false, int maxId = 0, bool includeId = false, bool single = true, bool invertEmpty = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L239)

Выбирает `column` из случайных строк, где она не пустая.

| Параметр | Описание |
|---|---|
| `column` | Колонка, которую нужно прочитать. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |
| `maxId` | Если больше 0 — только строки с `id` меньше этого числа. |
| `includeId` | Начинать результат с колонки `id`. |
| `single` | Вернуть одну строку вместо всех подходящих в случайном порядке. |
| `invertEmpty` | Вместо этого выбирать строки, где колонка пустая. |

### GetTableColumns

```csharp
public List<string> GetTableColumns(string tableName, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1045)

Имена колонок таблицы.

| Параметр | Описание |
|---|---|
| `tableName` | Имя таблицы. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

### GetTables

```csharp
public List<string> GetTables(bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1027)

Имена всех таблиц по алфавиту (PostgreSQL: базовые таблицы схемы `public`).

| Параметр | Описание |
|---|---|
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

### InsertDic

```csharp
public void InsertDic(Dictionary<string, string> data, string tableName = null, bool log = false, bool thrw = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L354)

Вставляет одну строку из словаря. На PostgreSQL конфликтующая строка пропускается (`ON CONFLICT DO NOTHING`).

| Параметр | Описание |
|---|---|
| `data` | Колонка → значение. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |

### JsonToDb

```csharp
public void JsonToDb(string json, string tableName = null, bool log = false, bool thrw = false, string where = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L405)

Разворачивает JSON-объект в колонки и записывает через `UpdFromDict`. Вложенные ключи соединяются через `_` (`a_b_0`). Исходная форма сохраняется в колонке `_json_structure`, чтобы `DbToJson` мог её восстановить.

| Параметр | Описание |
|---|---|
| `json` | JSON-объект. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |
| `where` | Сырое SQL-условие, выбирающее строку. |

### PgToSqlite

```csharp
public void PgToSqlite(string pgTable, string sqlitePath, string sqliteTable, string pgSchema = "public", bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1518)

Копирует таблицу PostgreSQL в файл SQLite. Целевая таблица удаляется и создаётся заново; типы PostgreSQL переводятся в INTEGER, REAL, TEXT или BLOB.

| Параметр | Описание |
|---|---|
| `pgTable` | Таблица-источник. |
| `sqlitePath` | Файл базы SQLite. |
| `sqliteTable` | Целевая таблица. |
| `pgSchema` | Схема-источник. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

**Примечания:** Этот экземпляр должен быть в режиме `pgSQL`.

### PrepareTable

```csharp
public void PrepareTable(Dictionary<string, string> tableStructure, string tableName = null, bool log = false, bool prune = false, bool rearrange = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L684)

Создаёт таблицу, если её нет, и добавляет недостающие колонки.

| Параметр | Описание |
|---|---|
| `tableStructure` | Колонка → тип SQL, например `{"id", "INTEGER PRIMARY KEY"}`. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `prune` | Заодно удалить колонки, которых нет в `tableStructure` (`PruneColumns`). |
| `rearrange` | Заодно переставить колонки в порядке `tableStructure` (`RearrangeColumns`). |

```csharp
public void PrepareTable(List<string> columns, string tableName = null, string defaultType = "TEXT DEFAULT ''", string serial = "INTEGER", bool log = false, bool prune = false, bool rearrange = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L716)

То же, что перегрузка со словарём, с первичным ключом `id` и одним типом для всех остальных колонок.

| Параметр | Описание |
|---|---|
| `columns` | Имена колонок; `id` и повторы пропускаются. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `defaultType` | Тип SQL для всех колонок. |
| `serial` | Тип `id`. `INTEGER` превращается в `INTEGER PRIMARY KEY AUTOINCREMENT` (на PostgreSQL `AUTOINCREMENT` заменяется на `SERIAL`). |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `prune` | Удалить колонки, которых нет в списке. |
| `rearrange` | Переставить колонки в порядке списка. |

### PruneColumns

```csharp
public void PruneColumns(Dictionary<string, string> tableStructure, string tableName = null, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L951)

Удаляет все колонки, кроме `id`, которых нет среди ключей `tableStructure`.

| Параметр | Описание |
|---|---|
| `tableStructure` | Какие колонки оставить. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

### PruneEmptyColumns

```csharp
public void PruneEmptyColumns(string tableName = null, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1175)

Удаляет все колонки, кроме `id`, в которых ни в одной строке нет непустого значения.

| Параметр | Описание |
|---|---|
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

### Query

```csharp
public string Query(string query, bool log = false, bool thrw = false, bool unSafe = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L78)

Выполняет один SQL-запрос.

| Параметр | Описание |
|---|---|
| `query` | Текст SQL. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |
| `unSafe` | Не используется. |

**Возвращает:** Для `SELECT`: строки через `·`, колонки через `¦`. Иначе число затронутых строк текстом. Пустая строка после ошибки, если `thrw` равно false.

### RearrangeColumns

```csharp
public void RearrangeColumns(Dictionary<string, string> tableStructure, string tableName = null, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L747)

Переставляет колонки таблицы: сначала `id`, затем существующие колонки из `tableStructure`, затем остальные. Для этого копирует данные в новую таблицу, удаляет старую и переименовывает новую. При ошибке временная таблица удаляется и бросается исключение.

| Параметр | Описание |
|---|---|
| `tableStructure` | Нужный порядок (колонка → тип). |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

### SetDone

```csharp
public void SetDone(string taskColumn = "daily", int cooldownMin = 0, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L383)

Записывает в `taskColumn` местное время `yyyy-MM-dd HH:mm:ss`: текущее или текущее плюс `cooldownMin`.

| Параметр | Описание |
|---|---|
| `taskColumn` | Колонка, в которую нужно записать. |
| `cooldownMin` | Сколько минут прибавить; 0 — записать текущее время. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`, подставляется в SQL как написано: текстовые значения бери в кавычки сам. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

### SqliteToPg

```csharp
public void SqliteToPg(string sqlitePath, string sqliteTable, string pgTable, string pgSchema = "public", bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1555)

Копирует таблицу SQLite в PostgreSQL. Целевая таблица удаляется и создаётся заново; типы SQLite переводятся в bigint, double precision, text или bytea.

| Параметр | Описание |
|---|---|
| `sqlitePath` | Файл базы SQLite. |
| `sqliteTable` | Таблица-источник. |
| `pgTable` | Целевая таблица. |
| `pgSchema` | Целевая схема. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

**Примечания:** Этот экземпляр должен быть в режиме `pgSQL`; он и есть цель.

### SqliteToSqlite

```csharp
public void SqliteToSqlite(string sourcePath, string sourceTable, string targetPath, string targetTable, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1589)

Копирует таблицу между двумя файлами SQLite. Целевая таблица удаляется и создаётся заново.

| Параметр | Описание |
|---|---|
| `sourcePath` | Файл базы-источника. |
| `sourceTable` | Таблица-источник. |
| `targetPath` | Файл целевой базы. |
| `targetTable` | Целевая таблица. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

### SwapLines

```csharp
public void SwapLines(int id1, int id2, string tableName = null, bool log = false, bool thrw = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1339)

Меняет местами значения всех колонок, кроме `id`, у двух строк.

| Параметр | Описание |
|---|---|
| `id1` | Id первой строки. |
| `id2` | Id второй строки. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Бросать исключение, если строка не найдена; иначе это пишется в лог и ничего не меняется. |

### TableExists

```csharp
public bool TableExists(string tableName, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L1005)

Проверяет, есть ли таблица (на PostgreSQL — в схеме `public`).

| Параметр | Описание |
|---|---|
| `tableName` | Имя таблицы; двойные кавычки игнорируются. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |

### Upd

```csharp
public void Upd(string setClause, string tableName = null, bool log = false, bool thrw = false, string key = "id", object id = null, string where = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L285)

Выполняет `UPDATE … SET setClause` для строки, где `key` = `id`, или для строк, подходящих под `where`.

| Параметр | Описание |
|---|---|
| `setClause` | Присваивания вида `status = 'ok', note = ''`; имена колонок берутся в кавычки, значения — как написаны. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Значение `key`, подставляется в SQL как написано: текстовые значения бери в кавычки сам. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |

### UpdFromDict

```csharp
public void UpdFromDict(Dictionary<string, string> data, string tableName = null, bool log = false, bool thrw = false, string where = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/Db.cs#L320)

Обновляет строки, подходящие под `where`, из словаря, сначала добавив недостающие колонки. Ключ `id` пишется в колонку `_id`. Одинарные кавычки из значений удаляются.

| Параметр | Описание |
|---|---|
| `data` | Колонка → значение. |
| `tableName` | Таблица; по умолчанию та, что передана в конструктор. |
| `log` | Писать запрос и его результат в лог проекта, даже если уровень логгера `Off`. `Db`, созданный без проекта, ничего не пишет. |
| `thrw` | Пробрасывать ошибку базы, а не возвращать пустой результат. |
| `where` | Сырое SQL-условие; обязательно. |

