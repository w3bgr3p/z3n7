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

Реестр делегатов по имени на весь процесс. SAFU регистрирует здесь свою реализацию, чтобы её могли вызывать другие сборки.

## Поля

### Functions

```csharp
public static ConcurrentDictionary<string, object> Functions;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L19)

Зарегистрированные делегаты по имени (например, `SAFU_Encode`).

