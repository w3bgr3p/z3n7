---
title: "DbJson"
tags: [api, Db]
generated: z3n7-docgen
---

# DbJson

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L384)

```csharp
public static class DbJson
```

Хранение JSON-объекта в виде колонок таблицы и его восстановление.

## Методы

### DbToJson

```csharp
public static string DbToJson(this IZennoPosterProjectModel project, string tableName = null, bool log = false, bool thrw = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L457)

Восстанавливает JSON, сохранённый `JsonToDb` с `saveStructure`, из строки текущего аккаунта. Пишет подробный ход работы в лог проекта.

| Параметр | Описание |
|---|---|
| `tableName` | Таблица; по умолчанию `project.ProjectTable()` (`__` + имя проекта). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |

**Возвращает:** Текст JSON или `{}`, если `_json_structure` нет или её не удалось разобрать.

### JsonToDb

```csharp
public static void JsonToDb(this IZennoPosterProjectModel project, string json, string tableName = null, bool log = false, bool thrw = false, string where = "", bool saveStructure = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L398)

Разворачивает JSON-объект в колонки (вложенные ключи соединяются через `_`) и записывает их через `DicToDb`.

| Параметр | Описание |
|---|---|
| `json` | JSON-объект. |
| `tableName` | Таблица; по умолчанию таблица проекта (переменная `projectTable`). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |
| `where` | Сырое SQL-условие. Если задано, `key` и `id` игнорируются. |
| `saveStructure` | Заодно сохранить форму в `_json_structure`, чтобы `DbToJson` мог восстановить объект. |

