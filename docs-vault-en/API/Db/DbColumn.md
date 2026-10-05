---
title: "DbColumn"
tags: [api, Db]
generated: z3n7-docgen
---

# DbColumn

`static class` · namespace `z3n7` · source [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L979)

```csharp
public static class DbColumn
```

*No description yet.*

## Methods

### ClmnAdd

```csharp
public static void ClmnAdd(this IZennoPosterProjectModel project, string clmnName, string tblName = null, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L996)

```csharp
public static void ClmnAdd(this IZennoPosterProjectModel project, List<string> columns, string tblName, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1006)

```csharp
public static void ClmnAdd(this IZennoPosterProjectModel project, string[] columns, string tblName, bool log = false, string defaultValue = "TEXT DEFAULT ''")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1011)

```csharp
public static void ClmnAdd(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1017)

### ClmnDrop

```csharp
public static void ClmnDrop(this IZennoPosterProjectModel project, string clmnName, string tblName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1042)

```csharp
public static void ClmnDrop(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1056)

### ClmnExist

```csharp
public static bool ClmnExist(this IZennoPosterProjectModel project, string clmnName, string tblName, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L981)

### ClmnList

```csharp
public static List<string> ClmnList(this IZennoPosterProjectModel project, string tableName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1032)

### ClmnPrune

```csharp
public static void ClmnPrune(this IZennoPosterProjectModel project, string tblName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1071)

```csharp
public static void ClmnPrune(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1094)

### ClmnRearrange

```csharp
public static void ClmnRearrange(this IZennoPosterProjectModel project, Dictionary<string, string> tableStructure, string tblName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1107)

```csharp
public static void ClmnRearrange(this IZennoPosterProjectModel project, List<string> projectColumns, string tblName = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1159)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
