---
title: "ProjectExtensions (MethodExtensions)"
tags: [api, MethodExtensions]
generated: z3n7-docgen
---

# ProjectExtensions (MethodExtensions)

`static class` · пространство имён `z3n7` · исходник [MethodExtensions/DictionaryExtensions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/DictionaryExtensions.cs#L8), [MethodExtensions/ListExtentions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L32), [MethodExtensions/StringExtentions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L521)

```csharp
public static class ProjectExtensions
```

Другие части этого типа: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

Методы расширения: словари и списки ZennoPoster.

## Методы

### DicToVars

```csharp
public static void DicToVars(this Dictionary<string, string> dict, IZennoPosterProjectModel project)
```

Метод расширения для `Dictionary<string, string>`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/DictionaryExtensions.cs#L12)

Задаёт переменную проекта для каждого ключа словаря.

### ListFromFile

```csharp
public static List<string> ListFromFile(this IZennoPosterProjectModel project, string listName, string fileName)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L78)

Заменяет содержимое списка ZennoPoster строками файла.

| Параметр | Описание |
|---|---|
| `listName` | Имя списка проекта. |
| `fileName` | Файл, который нужно прочитать. |

**Возвращает:** Строки.

### ListSync

```csharp
public static List<string> ListSync(this IZennoPosterProjectModel project, string listName)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L49)

Копирует список ZennoPoster в новый `List<string>`.

```csharp
public static List<string> ListSync(this IZennoPosterProjectModel project, string listName, List<string> localList)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L62)

Заменяет содержимое списка ZennoPoster на `localList`.

**Возвращает:** `localList`.

### RndFromList

```csharp
public static string RndFromList(this IZennoPosterProjectModel project, string listName, bool remove = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L38)

Возвращает случайную строку списка ZennoPoster.

| Параметр | Описание |
|---|---|
| `listName` | Имя списка проекта. |
| `remove` | Заодно удалить его из списка проекта. |

### ToJson

```csharp
public static void ToJson(this IZennoPosterProjectModel project, string json, bool thrw = false, int objIndex = 1)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L530)

Загружает JSON в `project.Json`. Если текст не JSON, берёт строку, начинающуюся с `{objIndex}:`, и загружает её остаток. Ошибки пишутся в лог как предупреждения.

| Параметр | Описание |
|---|---|
| `json` | Текст JSON или пронумерованные строки JSON. |
| `thrw` | Бросать исключение, если и вторая попытка не удалась. |
| `objIndex` | Префикс с номером строки, который нужно искать. |

