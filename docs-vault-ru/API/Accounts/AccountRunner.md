---
title: "AccountRunner"
tags: [api, Accounts]
generated: z3n7-docgen
---

# AccountRunner

`static class` · пространство имён `z3n7` · исходник [Accounts/AccountRunner.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L12)

```csharp
public static class AccountRunner
```

*Описания пока нет.*

## Методы

### ChooseAccountByCondition

```csharp
public static void ChooseAccountByCondition(this IZennoPosterProjectModel project, string condition, string sortByTaskAge = null, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, string tableName = null, bool debugLog = false, bool sqlNow = true)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L176)

```csharp
public static void ChooseAccountByCondition(this IZennoPosterProjectModel project, Dictionary<string, string> conditionsByTable, string sortByTaskAge = null, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, bool debugLog = false, bool sqlNow = true)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L218)

### ChooseAndRunByCondition

```csharp
public static void ChooseAndRunByCondition(this IZennoPosterProjectModel project, Instance instance, string condition, bool browser = false, string sortByTaskAge = null, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, string tableName = null, bool debugLog = false, bool sqlNow = true, bool useLegacy = true, bool useZpprofile = false, bool useFolder = true)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L358)

### QuantityByCondition

```csharp
public static int QuantityByCondition(this IZennoPosterProjectModel project, string condition, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, string tableName = null, bool debugLog = false, bool sqlNow = true)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L402)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
