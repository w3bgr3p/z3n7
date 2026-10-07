---
title: "FastDb"
tags: [api, Db]
generated: z3n7-docgen
---

# FastDb

`class` · пространство имён `z3n7` · исходник [Db/FastDb.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L23)

```csharp
public class FastDb
```

Доступ к SQLite через встроенный в ZennoPoster ODBC-исполнитель запросов, без открытия собственных соединений.

## Конструкторы

### FastDb

```csharp
public FastDb(IZennoPosterProjectModel project, string dbName = null, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L32)

Используется файл `{project.Path}{dbName}.sql`.

| Параметр | Описание |
|---|---|
| `dbName` | Имя файла без расширения; по умолчанию переменная `dbName` или `db`. |
| `log` | Писать в лог запросы и ответы на `SELECT`. |

## Методы

### dbList

```csharp
public List<string> dbList(string query)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L64)

Выполняет запрос и возвращает его строки.

### dbString

```csharp
public string dbString(string query)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L53)

Выполняет запрос.

**Возвращает:** Строки через перевод строки, колонки через `|`.

### ExportToCsv

```csharp
public void ExportToCsv(string tableName, string fileName)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L74)

Записывает всю таблицу в `{project.Path}{fileName}` в CSV со строкой заголовка (UTF-8).

```csharp
public void ExportToCsv(string tableName, string fileName, string columns = "*")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L98)

Записывает выбранные колонки в `{project.Path}{fileName}` в CSV, UTF-8 с BOM, чтобы Excel открыл его правильно.

| Параметр | Описание |
|---|---|
| `columns` | `*` — все колонки, или список через запятую, который также служит заголовком. |

