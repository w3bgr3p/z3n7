---
title: "Diagnostic"
tags: [api, Diagnostic]
generated: z3n7-docgen
---

# Diagnostic

`static class` · пространство имён `z3n7` · исходник [Diagnostic/Diagnostic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L182)

```csharp
public static class Diagnostic
```

Environment information for logs and diagnostics.

## Методы

### Info

```csharp
public static VersionInfo Info()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L188)

Reads library, ZennoPoster and runtime versions and the machine name. Each field is read separately and a failure leaves only that field empty; never throws.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
