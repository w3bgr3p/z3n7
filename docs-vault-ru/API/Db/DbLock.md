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

Shared lock object for code that must not access the database concurrently.

## Поля

### lockObj

```csharp
public static readonly object lockObj;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L17)

The lock object.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
