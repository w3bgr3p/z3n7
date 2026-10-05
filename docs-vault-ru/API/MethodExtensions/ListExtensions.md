---
title: "ListExtensions"
tags: [api, MethodExtensions]
generated: z3n7-docgen
---

# ListExtensions

`static class` · пространство имён `z3n7` · исходник [MethodExtensions/ListExtentions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L10)

```csharp
public static class ListExtensions
```

Extension methods on lists.

## Методы

### Rnd

```csharp
public static T Rnd<T>(this IList<T> list, bool remove = false)
```

Метод расширения для `IList<T>`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L20)

Returns a random item.

| Параметр | Описание |
|---|---|
| `remove` | Also remove it from the list. |

**Возвращает:** The item. Throws when the list is empty.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
