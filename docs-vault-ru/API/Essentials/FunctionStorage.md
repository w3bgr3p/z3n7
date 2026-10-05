---
title: "FunctionStorage"
tags: [api, Essentials]
generated: z3n7-docgen
---

# FunctionStorage

`static class` · пространство имён `z3n7` · исходник [Essentials/Safu8.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L16)

```csharp
public static class FunctionStorage
```

Process-wide registry of delegates by name. SAFU registers its implementation here so that other assemblies can call it.

## Поля

### Functions

```csharp
public static ConcurrentDictionary<string, object> Functions;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L19)

Registered delegates, keyed by name (e.g. `SAFU_Encode`).

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
