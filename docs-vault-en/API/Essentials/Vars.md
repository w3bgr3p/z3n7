---
title: "Vars"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Vars

`static class` · namespace `z3n7` · source [Essentials/Vars.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L12)

```csharp
public static class Vars
```

*No description yet.*

## Methods

### Bool

```csharp
public static bool Bool(this IZennoPosterProjectModel project, string var)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L75)

### Decimal

```csharp
public static decimal Decimal(this IZennoPosterProjectModel project, string var)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L63)

### Int

```csharp
public static int Int(this IZennoPosterProjectModel project, string var)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L45)

```csharp
public static int Int(this IZennoPosterProjectModel project, string varName, int input)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L57)

### MaxErr

```csharp
public static void MaxErr(this IZennoPosterProjectModel project, int maxAttempts, Exception ex = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L81)

### Range

```csharp
public static List<string> Range(this IZennoPosterProjectModel project, string accRange = null, string output = null, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L195)

### Var

```csharp
public static string Var(this IZennoPosterProjectModel project, string var)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L16)

```csharp
public static string Var(this IZennoPosterProjectModel project, string var, object value)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L32)

### VarAdd

```csharp
public static bool VarAdd(this IZennoPosterProjectModel project, string name, string defaultValue = "", string comment = "")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L108)

Добавляет переменную в проект, открытый в ProjectMaker. Работает только во время разработки: у ILocalVariables нет метода добавления, поэтому правка идёт через PublicApi и касается копии проекта в памяти ProjectMaker. Из задачи в раннере вызывать бессмысленно — на себя это не подействует. Чтобы правка попала на диск, проект надо сохранить. Ключ берётся из ZENNO_API_KEY, тир ключа должен быть не ниже T1.

### VarCounter

```csharp
public static int VarCounter(this IZennoPosterProjectModel project, string varName, int input)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L146)

### VarRnd

```csharp
public static string VarRnd(this IZennoPosterProjectModel project, string var)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L125)

### VarsFromDict

```csharp
public static void VarsFromDict(this IZennoPosterProjectModel project, Dictionary<string, string> dict)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L180)

### VarsFromJson

```csharp
public static void VarsFromJson(this IZennoPosterProjectModel project, string json = "jVars")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L188)

### VarsMath

```csharp
public static decimal VarsMath(this IZennoPosterProjectModel project, string varA, string operation, string varB, string resultVar = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L152)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
