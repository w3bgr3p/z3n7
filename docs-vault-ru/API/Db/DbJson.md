---
title: "DbJson"
tags: [api, Db]
generated: z3n7-docgen
---

# DbJson

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L252)

```csharp
public static class DbJson
```

*Описания пока нет.*

## Методы

### DbToJson

```csharp
public static string DbToJson(this IZennoPosterProjectModel project, string tableName = null, bool log = false, bool thrw = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L303)

### JsonToDb

```csharp
public static void JsonToDb(this IZennoPosterProjectModel project, string json, string tableName = null, bool log = false, bool thrw = false, string where = "", bool saveStructure = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L254)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
