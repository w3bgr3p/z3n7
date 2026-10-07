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

Имя таблицы и определения её колонок.

## Свойства

### Columns

```csharp
public Dictionary<string, string> Columns { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L11)

Имя колонки → тип SQL.

### Name

```csharp
public string Name { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L9)

Имя таблицы.

