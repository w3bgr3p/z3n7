---
title: "DbUpdate"
tags: [api, Db]
generated: z3n7-docgen
---

# DbUpdate

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L195)

```csharp
public static class DbUpdate
```

*Описания пока нет.*

## Методы

### DbDone

```csharp
public static void DbDone(this IZennoPosterProjectModel project, string task = "daily", int cooldownMin = 0, string tableName = null, bool log = false, bool thrw = false, string key = "id", object acc = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L244)

### DbInsert

```csharp
public static string DbInsert(this IZennoPosterProjectModel project, Dictionary<string, string> dataDic, string tableName = null, bool log = false, bool thrw = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L197)

### DbUpd

```csharp
public static void DbUpd(this IZennoPosterProjectModel project, string toUpd, string tableName = null, bool log = false, bool thrw = false, string key = "id", object acc = null, string where = "", string saveToVar = "lastQuery")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L238)

### DicToDb

```csharp
public static void DicToDb(this IZennoPosterProjectModel project, Dictionary<string, string> dataDic, string tableName = null, bool log = false, bool thrw = false, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L211)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
