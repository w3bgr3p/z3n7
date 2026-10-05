---
title: "Rnd"
tags: [api, Tools]
generated: z3n7-docgen
---

# Rnd

`static class` · namespace `z3n7` · source [Tools/Rnd.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L11)

```csharp
public static class Rnd
```

*No description yet.*

## Methods

### Delay

```csharp
public static void Delay(int min = 1008, int max = 1337)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L427)

### RndBool

```csharp
public static bool RndBool(this int truePercent)
```

Extension method for `int`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L245)

### RndDecimal

```csharp
public static decimal RndDecimal(this IZennoPosterProjectModel project, string Var)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L201)

### RndFile

```csharp
public static string RndFile(string directoryPath, string extension = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L249)

### RndHexString

```csharp
public static string RndHexString(int length)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L15)

### RndInt

```csharp
public static int RndInt(this IZennoPosterProjectModel project, string Var)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L224)

### RndMail

```csharp
public static string RndMail(int minLength = 5, int maxLength = 10, string domain = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L303)

### RndMonth

```csharp
public static string RndMonth()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L317)

### RndNickname

```csharp
public static string RndNickname(int min = 8, int max = 16)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L33)

### RndPass

```csharp
public static string RndPass(int minLength = 10, int maxLength = 14, bool upperCase = true, bool symbols = true)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L331)

### RndPercent

```csharp
public static double RndPercent(decimal input, double percent, double maxPercent)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L178)

### RndProfileData

```csharp
public static void RndProfileData(this IZennoPosterProjectModel project, bool email = true, bool password = true)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L419)

### RndString

```csharp
public static string RndString(int length)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L26)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
