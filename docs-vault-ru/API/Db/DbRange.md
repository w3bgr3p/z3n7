---
title: "DbRange"
tags: [api, Db]
generated: z3n7-docgen
---

# DbRange

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1640)

```csharp
public static class DbRange
```

Заполнение таблицы строками аккаунтов.

## Методы

### AddRange

```csharp
public static void AddRange(this IZennoPosterProjectModel project, string tblName, int range = 0, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1651)

Вставляет строки с id от текущего максимума + 1 до `range` порциями по 500. Существующие id пропускаются.

| Параметр | Описание |
|---|---|
| `tblName` | Таблица с колонкой `id`. |
| `range` | Наибольший id; 0 — читать `rangeEnd`, а если там не число, берётся 10 с предупреждением. |
| `log` | Писать запрос и его результат в лог проекта. |

