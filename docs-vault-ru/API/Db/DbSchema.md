---
title: "DbSchema"
tags: [api, Db]
generated: z3n7-docgen
---

# DbSchema

`static class` · пространство имён `z3n7` · исходник [Db/DbSchema.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L17)

```csharp
public static class DbSchema
```

Names and layouts of the library's own tables.

## Свойства

### Instance

```csharp
public static string Instance { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L42)

Instance table, default `_instance`.

### Wlt

```csharp
public static string Wlt { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L39)

Wallet table, default `_wlt`.

## Поля

### Process

```csharp
public static readonly TableSchema Process;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L22)

Table `_processes`: one row per process with machine, name, RAM, uptime and command line.

