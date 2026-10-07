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

Методы расширения для `IZennoPosterProjectModel`: отчёты о балансах.

## Методы

### GenerateNative

```csharp
public static void GenerateNative(this IZennoPosterProjectModel project, string chains, bool call = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L1496)

Пишет таблицу балансов заданных сетей (`Accountant.ShowBalanceTable` с добавлением `id`).

| Параметр | Описание |
|---|---|
| `chains` | Колонки `_native` через запятую. |
| `call` | Потом открыть файл программой по умолчанию. |

