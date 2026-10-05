---
title: "DbJson"
tags: [api, Db]
generated: z3n7-docgen
---

# DbJson

`static class` · namespace `z3n7` · source [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L252)

```csharp
public static class DbJson
```

*No description yet.*

## Methods

### DbToJson

```csharp
public static string DbToJson(this IZennoPosterProjectModel project, string tableName = null, bool log = false, bool thrw = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L303)

### JsonToDb

```csharp
public static void JsonToDb(this IZennoPosterProjectModel project, string json, string tableName = null, bool log = false, bool thrw = false, string where = "", bool saveStructure = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L254)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
