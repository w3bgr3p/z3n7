---
title: "DbLock"
tags: [api, Db]
generated: z3n7-docgen
---

# DbLock

`static class` · namespace `z3n7` · source [Db/FastDb.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L14)

```csharp
public static class DbLock
```

Shared lock object for code that must not access the database concurrently.

## Fields

### lockObj

```csharp
public static readonly object lockObj;
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L17)

The lock object.

