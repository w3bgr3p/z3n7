---
title: "DbSchema"
tags: [api, Db]
generated: z3n7-docgen
---

# DbSchema

`static class` · namespace `z3n7` · source [Db/DbSchema.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L17)

```csharp
public static class DbSchema
```

Names and layouts of the library's own tables.

## Properties

### Instance

```csharp
public static string Instance { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L42)

Instance table, default `_instance`.

### Wlt

```csharp
public static string Wlt { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L39)

Wallet table, default `_wlt`.

## Fields

### Process

```csharp
public static readonly TableSchema Process;
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L22)

Table `_processes`: one row per process with machine, name, RAM, uptime and command line.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
