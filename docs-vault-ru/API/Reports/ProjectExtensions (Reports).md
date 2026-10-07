---
title: "ProjectExtensions (Reports)"
tags: [api, Reports]
generated: z3n7-docgen
---

# ProjectExtensions (Reports)

`static class` · пространство имён `z3n7` · исходник [Reports/Accountant.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L1488)

```csharp
public static class ProjectExtensions
```

Другие части этого типа: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

Extension methods on `IZennoPosterProjectModel`: balance reports.

## Методы

### GenerateNative

```csharp
public static void GenerateNative(this IZennoPosterProjectModel project, string chains, bool call = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L1496)

Writes the balance table of the given chains (`Accountant.ShowBalanceTable` with `id` added).

| Параметр | Описание |
|---|---|
| `chains` | Comma-separated columns of `_native`. |
| `call` | Open the file with the default program afterwards. |

