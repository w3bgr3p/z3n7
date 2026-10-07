---
title: "AccountRunner"
tags: [api, Accounts]
generated: z3n7-docgen
---

# AccountRunner

`static class` · namespace `z3n7` · source [Accounts/AccountRunner.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L16)

```csharp
public static class AccountRunner
```

Picking the next account to work on from the database by condition, range priorities and social-account filters.

## Methods

### ChooseAccountByCondition

```csharp
public static void ChooseAccountByCondition(this IZennoPosterProjectModel project, string condition, string sortByTaskAge = null, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, string tableName = null, bool debugLog = false, bool sqlNow = true)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L204)

Picks an account and stores it in `acc0`; the candidates stay in the `accs` list and its row gets `status = 'working...'`. When `acc0Forced` is set, it is used as is. When `acc0` is already set, the candidate search is skipped. Throws when no account matches.

| Parameter | Description |
|---|---|
| `condition` | SQL condition on the account table. The word `NOW` is replaced by the current time as `yyyy-MM-ddTHH:mm:ss` when `sqlNow` is true. |
| `useRange` | Limit to `cfgAccRange`. Groups separated by `:` are priorities: the first group with matching accounts is used. |
| `filterTwitter` | Keep only accounts whose row in the `_twitter` table has `status = 'ok'`. |
| `filterDiscord` | Keep only accounts whose row in the `_discord` table has `status = 'ok'`. |
| `filterGithub` | Keep only accounts whose row in the `_github` table has `status = 'ok'`. |
| `tableName` | Account table; default `__` + project name. |
| `debugLog` | Write the queries to the log. |
| `sqlNow` | Replace `NOW` in the condition. |
| `sortByTaskAge` | Column with ISO timestamps: take the account with the oldest value (empty first). When empty, a random account is taken. |

```csharp
public static void ChooseAccountByCondition(this IZennoPosterProjectModel project, Dictionary<string, string> conditionsByTable, string sortByTaskAge = null, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, bool debugLog = false, bool sqlNow = true)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L261)

Like the single-condition overload, but takes the accounts that match every condition in its own table (intersection).

| Parameter | Description |
|---|---|
| `conditionsByTable` | Condition → table. |
| `sortByTaskAge` | When set, the first remaining account is taken instead of a random one; no sorting is done. |
| `useRange` | Limit to `cfgAccRange`. Groups separated by `:` are priorities: the first group with matching accounts is used. |
| `filterTwitter` | Keep only accounts whose row in the `_twitter` table has `status = 'ok'`. |
| `filterDiscord` | Keep only accounts whose row in the `_discord` table has `status = 'ok'`. |
| `filterGithub` | Keep only accounts whose row in the `_github` table has `status = 'ok'`. |
| `debugLog` | Write the queries to the log; also makes an empty intersection throw. |
| `sqlNow` | Replace `NOW` in the conditions. |

### ChooseAndRunByCondition

```csharp
public static void ChooseAndRunByCondition(this IZennoPosterProjectModel project, Instance instance, string condition, bool browser = false, string sortByTaskAge = null, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, string tableName = null, bool debugLog = false, bool sqlNow = true, bool useLegacy = true, bool useZpprofile = false, bool useFolder = true)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L429)

Picks an account (`ChooseAccountByCondition`) and starts the browser for it (`RunBrowser`). When the browser cannot be started in the profile folder, the next account is tried; any other error is thrown.

| Parameter | Description |
|---|---|
| `condition` | SQL condition on the account table. The word `NOW` is replaced by the current time as `yyyy-MM-ddTHH:mm:ss` when `sqlNow` is true. |
| `useRange` | Limit to `cfgAccRange`. Groups separated by `:` are priorities: the first group with matching accounts is used. |
| `filterTwitter` | Keep only accounts whose row in the `_twitter` table has `status = 'ok'`. |
| `filterDiscord` | Keep only accounts whose row in the `_discord` table has `status = 'ok'`. |
| `filterGithub` | Keep only accounts whose row in the `_github` table has `status = 'ok'`. |
| `tableName` | Account table; default `__` + project name. |
| `debugLog` | Write the queries to the log. |
| `sqlNow` | Replace `NOW` in the condition. |
| `instance` | Browser instance. |
| `browser` | Start Chromium; otherwise run without a browser. |
| `sortByTaskAge` | Column with ISO timestamps: take the account with the oldest value (empty first). When empty, a random account is taken. |
| `useLegacy` | Passed to `RunBrowser`. |
| `useZpprofile` | Passed to `RunBrowser`. |
| `useFolder` | Passed to `RunBrowser`. |

### QuantityByCondition

```csharp
public static int QuantityByCondition(this IZennoPosterProjectModel project, string condition, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, string tableName = null, bool debugLog = false, bool sqlNow = true)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L490)

Counts the accounts matching the condition over all range groups, after the social filters.

| Parameter | Description |
|---|---|
| `condition` | SQL condition on the account table. The word `NOW` is replaced by the current time as `yyyy-MM-ddTHH:mm:ss` when `sqlNow` is true. |
| `useRange` | Limit to `cfgAccRange`. Groups separated by `:` are priorities: the first group with matching accounts is used. |
| `filterTwitter` | Keep only accounts whose row in the `_twitter` table has `status = 'ok'`. |
| `filterDiscord` | Keep only accounts whose row in the `_discord` table has `status = 'ok'`. |
| `filterGithub` | Keep only accounts whose row in the `_github` table has `status = 'ok'`. |
| `tableName` | Account table; default `__` + project name. |
| `debugLog` | Write the queries to the log. |
| `sqlNow` | Replace `NOW` in the condition. |

