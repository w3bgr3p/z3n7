---
title: "Accountant.HtmlEncoder"
tags: [api, Reports]
generated: z3n7-docgen
---

# Accountant.HtmlEncoder

`static class` · namespace `z3n7.Utilities` · source [Reports/Accountant.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L220)

```csharp
public static class HtmlEncoder
```

HTML escaping helpers.

## Methods

### HtmlAttributeEncode

```csharp
public static string HtmlAttributeEncode(string text)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L237)

Escapes `& " ' < >` for HTML attribute values.

### HtmlEncode

```csharp
public static string HtmlEncode(string text)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L223)

Escapes `& < > " '` for HTML text.

