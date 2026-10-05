---
title: "AccountRunner"
tags: [api, Accounts]
generated: z3n7-docgen
---

# AccountRunner

`static class` · namespace `z3n7` · source [Accounts/AccountRunner.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L12)

```csharp
public static class AccountRunner
```

*No description yet.*

## Methods

### ChooseAccountByCondition

```csharp
public static void ChooseAccountByCondition(this IZennoPosterProjectModel project, string condition, string sortByTaskAge = null, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, string tableName = null, bool debugLog = false, bool sqlNow = true)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L176)

```csharp
public static void ChooseAccountByCondition(this IZennoPosterProjectModel project, Dictionary<string, string> conditionsByTable, string sortByTaskAge = null, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, bool debugLog = false, bool sqlNow = true)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L218)

### ChooseAndRunByCondition

```csharp
public static void ChooseAndRunByCondition(this IZennoPosterProjectModel project, Instance instance, string condition, bool browser = false, string sortByTaskAge = null, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, string tableName = null, bool debugLog = false, bool sqlNow = true, bool useLegacy = true, bool useZpprofile = false, bool useFolder = true)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L358)

### QuantityByCondition

```csharp
public static int QuantityByCondition(this IZennoPosterProjectModel project, string condition, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, string tableName = null, bool debugLog = false, bool sqlNow = true)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L402)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
