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

Имена и раскладки собственных таблиц библиотеки.

## Свойства

### Instance

```csharp
public static string Instance { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L42)

Таблица инстансов, по умолчанию `_instance`.

### Wlt

```csharp
public static string Wlt { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L39)

Таблица кошельков, по умолчанию `_wlt`.

## Поля

### Process

```csharp
public static readonly TableSchema Process;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbSchema.cs#L22)

Таблица `_processes`: по строке на процесс с машиной, именем, RAM, временем работы и командной строкой.

