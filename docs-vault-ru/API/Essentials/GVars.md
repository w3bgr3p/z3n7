---
title: "GVars"
tags: [api, Essentials]
generated: z3n7-docgen
---

# GVars

`static class` · пространство имён `z3n7` · исходник [Essentials/Vars.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L307)

```csharp
public static class GVars
```

Global ZennoPoster variables, kept in a namespace named after the current Windows user.

## Методы

### GClean

```csharp
public static List<int> GClean(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L450)

Clears the global variables `acc1` … `acc{rangeEnd}`.

**Возвращает:** Numbers of the variables that were cleared.

### GGetBusyList

```csharp
public static List<string> GGetBusyList(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L354)

Lists accounts taken by running threads: every non-empty global variable `acc1` … `acc{rangeEnd}`.

| Параметр | Описание |
|---|---|
| `log` | Write the list to the log. |

**Возвращает:** Entries in the form `number:value`.

### GSetAcc

```csharp
public static bool GSetAcc(this IZennoPosterProjectModel project, string input = null, bool force = false, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L400)

Marks the current account (`acc0`) as taken by writing `input` to the global variable `acc{acc0}`.

| Параметр | Описание |
|---|---|
| `input` | Value to store; default is the `projectName` variable. |
| `force` | Overwrite even if the account is already taken. |
| `log` | Write the outcome to the log. |

**Возвращает:** `false` when the account is already taken and `force` is false.

### GVar

```csharp
public static string GVar(this IZennoPosterProjectModel project, string var)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L311)

Returns a global variable, or an empty string when it does not exist.

```csharp
public static string GVar(this IZennoPosterProjectModel project, string var, object value)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L327)

Sets a global variable, creating it when it does not exist. Errors are swallowed.

**Возвращает:** Always an empty string.

