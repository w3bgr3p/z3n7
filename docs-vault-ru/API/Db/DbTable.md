---
title: "DbTable"
tags: [api, Db]
generated: z3n7-docgen
---

# DbTable

`static class` · пространство имён `z3n7` · исходник [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L803)

```csharp
public static class DbTable
```

*Описания пока нет.*

## Методы

### EnsureTable

```csharp
public static void EnsureTable(this IZennoPosterProjectModel project, TableSchema schema)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L806)

### PrepareProjectTable

```csharp
public static void PrepareProjectTable(this IZennoPosterProjectModel project, string[] projectColumns, string tblName = null, bool log = false, bool prune = false, bool rearrange = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L916)

```csharp
public static void PrepareProjectTable(this IZennoPosterProjectModel project, List<string> projectColumns = null, string tblName = null, bool log = false, bool prune = false, bool rearrange = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L922)

```csharp
public static void PrepareProjectTable(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false, bool prune = false, bool rearrange = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L932)

### TblAdd

```csharp
public static void TblAdd(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L811)

### TblColumns

```csharp
public static List<string> TblColumns(this IZennoPosterProjectModel project, string tblName, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L856)

### TblExist

```csharp
public static bool TblExist(this IZennoPosterProjectModel project, string tblName, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L826)

### TblForProject

```csharp
public static Dictionary<string, string> TblForProject(this IZennoPosterProjectModel project, List<string> projectColumns = null, string defaultType = "TEXT DEFAULT ''")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L871)

### TblList

```csharp
public static List<string> TblList(this IZennoPosterProjectModel project, bool log = false, string schema = "public")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L843)

### TblPrepareDefault

```csharp
public static void TblPrepareDefault(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L906)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
