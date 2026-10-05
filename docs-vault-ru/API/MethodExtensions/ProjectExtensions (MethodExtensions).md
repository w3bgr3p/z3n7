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

Extension methods: dictionaries and ZennoPoster lists.

## Методы

### DicToVars

```csharp
public static void DicToVars(this Dictionary<string, string> dict, IZennoPosterProjectModel project)
```

Метод расширения для `Dictionary<string, string>`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/DictionaryExtensions.cs#L12)

Sets a project variable for every key of the dictionary.

### ListFromFile

```csharp
public static List<string> ListFromFile(this IZennoPosterProjectModel project, string listName, string fileName)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L78)

Replaces the content of a ZennoPoster list with the lines of a file.

| Параметр | Описание |
|---|---|
| `listName` | Project list name. |
| `fileName` | File to read. |

**Возвращает:** The lines.

### ListSync

```csharp
public static List<string> ListSync(this IZennoPosterProjectModel project, string listName)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L49)

Copies a ZennoPoster list into a new `List<string>`.

```csharp
public static List<string> ListSync(this IZennoPosterProjectModel project, string listName, List<string> localList)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L62)

Replaces the content of a ZennoPoster list with `localList`.

**Возвращает:** `localList`.

### RndFromList

```csharp
public static string RndFromList(this IZennoPosterProjectModel project, string listName, bool remove = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L38)

Returns a random line of a ZennoPoster list.

| Параметр | Описание |
|---|---|
| `listName` | Project list name. |
| `remove` | Also remove it from the project list. |

### ToJson

```csharp
public static void ToJson(this IZennoPosterProjectModel project, string json, bool thrw = false, int objIndex = 1)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L530)

Loads JSON into `project.Json`. When the text is not JSON, takes the line starting with `{objIndex}:` and loads the rest of it. Failures are logged as warnings.

| Параметр | Описание |
|---|---|
| `json` | JSON text, or numbered lines of JSON. |
| `thrw` | Throw when the second attempt also fails. |
| `objIndex` | Line number prefix to look for. |

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
