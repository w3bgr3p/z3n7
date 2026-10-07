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

Методы расширения для списков.

## Методы

### Rnd

```csharp
public static T Rnd<T>(this IList<T> list, bool remove = false)
```

Метод расширения для `IList<T>`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/ListExtentions.cs#L20)

Возвращает случайный элемент.

| Параметр | Описание |
|---|---|
| `remove` | Заодно удалить его из списка. |

**Возвращает:** Элемент. Бросает исключение, если список пуст.

