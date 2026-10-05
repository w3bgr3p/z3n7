---
title: "DbRange"
tags: [api, Db]
generated: z3n7-docgen
---

# DbRange

`static class` · namespace `z3n7` · source [Db/DbExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1640)

```csharp
public static class DbRange
```

Filling a table with account rows.

## Methods

### AddRange

```csharp
public static void AddRange(this IZennoPosterProjectModel project, string tblName, int range = 0, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/DbExtencions.cs#L1651)

Inserts rows with ids from the current maximum + 1 up to `range`, in batches of 500. Existing ids are skipped.

| Parameter | Description |
|---|---|
| `tblName` | Table with an `id` column. |
| `range` | Highest id; 0 reads `rangeEnd`, and 10 is used with a warning when that is not a number. |
| `log` | Write the query and its result to the project log. |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
