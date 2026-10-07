---
title: "DbLock"
tags: [api, Db]
generated: z3n7-docgen
---

# DbLock

`static class` · пространство имён `z3n7` · исходник [Db/FastDb.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L14)

```csharp
public static class DbLock
```

Общий объект блокировки для кода, который не должен обращаться к базе одновременно.

## Поля

### lockObj

```csharp
public static readonly object lockObj;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L17)

Объект блокировки.

