---
title: "ProjectExtensions (MethodExtensions)"
tags: [api, MethodExtensions]
generated: z3n7-docgen
---

# ProjectExtensions (MethodExtensions)

`static class` · namespace `z3n7` · source [MethodExtensions/DictionaryExtensions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/DictionaryExtensions.cs#L7), [MethodExtensions/ListExtentions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L28), [MethodExtensions/StringExtentions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L468)

```csharp
public static class ProjectExtensions
```

Other parts of this type: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

*No description yet.*

## Methods

### DicToVars

```csharp
public static void DicToVars(this Dictionary<string, string> dict, IZennoPosterProjectModel project)
```

Extension method for `Dictionary<string, string>`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/DictionaryExtensions.cs#L10)

### ListFromFile

```csharp
public static List<string> ListFromFile(this IZennoPosterProjectModel project, string listName, string fileName)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L64)

### ListSync

```csharp
public static List<string> ListSync(this IZennoPosterProjectModel project, string listName)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L41)

```csharp
public static List<string> ListSync(this IZennoPosterProjectModel project, string listName, List<string> localList)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L52)

### RndFromList

```csharp
public static string RndFromList(this IZennoPosterProjectModel project, string listName, bool remove = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L31)

### ToJson

```csharp
public static void ToJson(this IZennoPosterProjectModel project, string json, bool thrw = false, int objIndex = 1)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L470)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
