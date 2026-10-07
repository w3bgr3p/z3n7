---
title: "PropertyManager"
tags: [api, Accounts]
generated: z3n7-docgen
---

# PropertyManager

`static class` · пространство имён `z3n7.Utilities` · исходник [Accounts/PropertyManager.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L9)

```csharp
public static class PropertyManager
```

Копирует простые свойства объектов в строки базы и обратно (через рефлексию).

## Методы

### GetTypeProperties

```csharp
public static List<string> GetTypeProperties(Type type, bool requireSetter = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L18)

Имена публичных читаемых свойств простых типов: примитивы, `string`, `decimal`, `DateTime`, перечисления.

| Параметр | Описание |
|---|---|
| `type` | Тип, который нужно разобрать. |
| `requireSetter` | Только свойства, у которых есть и публичный сеттер. |

```csharp
public static List<string> GetTypeProperties(object obj)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L42)

То же, что `GetTypeProperties(obj.GetType())`.

### GetValuesByProperty

```csharp
public static Dictionary<string, string> GetValuesByProperty(this IZennoPosterProjectModel project, object obj, List<string> propertyList = null, string tableToUpd = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L55)

Читает значения свойств текстом, одинарные кавычки удваиваются. Свойства, которые не удалось прочитать, пропускаются.

| Параметр | Описание |
|---|---|
| `obj` | Объект-источник. |
| `propertyList` | Какие свойства читать; по умолчанию `GetTypeProperties`. |
| `tableToUpd` | Если задано, значения также записываются в строку текущего аккаунта этой таблицы (`DicToDb`). |

**Возвращает:** Свойство → значение.

### SetValuesFromDb

```csharp
public static void SetValuesFromDb(this IZennoPosterProjectModel project, object obj, string table = "profile", List<string> propertyList = null, string key = "id", object id = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L97)

Задаёт записываемые свойства объекта из строки базы, приводя текст к типу свойства. Пустые значения и неудачные приведения пропускаются; остальные ошибки пишутся в лог как предупреждения.

| Параметр | Описание |
|---|---|
| `obj` | Целевой объект. |
| `table` | Таблица. |
| `propertyList` | Какие свойства задавать; по умолчанию `GetTypeProperties`. |
| `key` | Колонка, которая сверяется с `id`. |
| `id` | Строка; по умолчанию текущий аккаунт (`acc0`). |
| `where` | Сырое SQL-условие вместо `key`/`id`. |

