---
title: "ListExtensions"
tags: [api, MethodExtensions]
generated: z3n7-docgen
---

# ListExtensions

`static class` · namespace `z3n7` · source [MethodExtensions/ListExtentions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L10)

```csharp
public static class ListExtensions
```

Extension methods on lists.

## Methods

### Rnd

```csharp
public static T Rnd<T>(this IList<T> list, bool remove = false)
```

Extension method for `IList<T>`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L20)

Returns a random item.

| Parameter | Description |
|---|---|
| `remove` | Also remove it from the list. |

**Returns:** The item. Throws when the list is empty.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
