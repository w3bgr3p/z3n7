---
title: "Accountant.HtmlEncoder"
tags: [api, Reports]
generated: z3n7-docgen
---

# Accountant.HtmlEncoder

`static class` · пространство имён `z3n7.Utilities` · исходник [Reports/Accountant.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L220)

```csharp
public static class HtmlEncoder
```

Помощники для экранирования HTML.

## Методы

### HtmlAttributeEncode

```csharp
public static string HtmlAttributeEncode(string text)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L237)

Экранирует `& " ' < >` для значений HTML-атрибутов.

### HtmlEncode

```csharp
public static string HtmlEncode(string text)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L223)

Экранирует `& < > " '` для текста HTML.

