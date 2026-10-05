---
title: "TableSchema"
tags: [api, Db]
generated: z3n7-docgen
---

# TableSchema

`class` · namespace `z3n7` · source [Db/DbSchema.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L6)

```csharp
public class TableSchema
```

Name and column definitions of a table.

## Properties

### Columns

```csharp
public Dictionary<string, string> Columns { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L11)

Column name → SQL type.

### Name

```csharp
public string Name { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L9)

Table name.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
