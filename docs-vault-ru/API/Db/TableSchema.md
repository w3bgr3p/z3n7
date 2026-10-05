---
title: "TableSchema"
tags: [api, Db]
generated: z3n7-docgen
---

# TableSchema

`class` · пространство имён `z3n7` · исходник [Db/DbSchema.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L6)

```csharp
public class TableSchema
```

Name and column definitions of a table.

## Свойства

### Columns

```csharp
public Dictionary<string, string> Columns { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L11)

Column name → SQL type.

### Name

```csharp
public string Name { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L9)

Table name.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
