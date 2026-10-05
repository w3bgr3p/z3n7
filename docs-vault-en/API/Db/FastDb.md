---
title: "FastDb"
tags: [api, Db]
generated: z3n7-docgen
---

# FastDb

`class` · namespace `z3n7` · source [Db/FastDb.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L18)

```csharp
public class FastDb
```

*No description yet.*

## Constructors

### FastDb

```csharp
public FastDb(IZennoPosterProjectModel project, string dbName = null, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L24)

## Methods

### dbList

```csharp
public List<string> dbList(string query)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L53)

### dbString

```csharp
public string dbString(string query)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L43)

### ExportToCsv

```csharp
public void ExportToCsv(string tableName, string fileName)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L60)

```csharp
public void ExportToCsv(string tableName, string fileName, string columns = "*")
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L79)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
