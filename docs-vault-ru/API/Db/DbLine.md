---
title: "DbLine"
tags: [api, Db]
generated: z3n7-docgen
---

# DbLine

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L802)

```csharp
public static class DbLine
```

Операции над строками целиком.

## Методы

### DbClearLine

```csharp
public static void DbClearLine(this IZennoPosterProjectModel project, int id, string tableName = null, bool log = false, bool thrw = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L809)

Записывает пустую строку во все колонки одной строки, кроме `id`.

| Параметр | Описание |
|---|---|
| `id` | Id строки. |
| `tableName` | Таблица; по умолчанию `project.ProjectTable()` (`__` + имя проекта). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение при ошибке базы, а не писать предупреждение и возвращать пустой результат. |

### DbSwapLines

```csharp
public static void DbSwapLines(this IZennoPosterProjectModel project, int id1, int id2, string tableName = null, bool log = false, bool thrw = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L837)

Меняет местами значения всех колонок, кроме `id`, у двух строк.

| Параметр | Описание |
|---|---|
| `id1` | Id первой строки. |
| `id2` | Id второй строки. |
| `tableName` | Таблица; по умолчанию `project.ProjectTable()` (`__` + имя проекта). |
| `log` | Писать запрос и его результат в лог проекта. |
| `thrw` | Бросать исключение, если строка не найдена; иначе ничего не меняется. |

