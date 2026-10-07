---
title: "Rnd"
tags: [api, Tools]
generated: z3n7-docgen
---

# Rnd

`static class` · namespace `z3n7` · source [Tools/Rnd.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L15)

```csharp
public static class Rnd
```

Random values: strings, nicknames, e-mail addresses, passwords, numbers from project variables, pauses.

## Methods

### Delay

```csharp
public static void Delay(int min = 1008, int max = 1337)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L488)

Sleeps for a random time.

| Parameter | Description |
|---|---|
| `min` | Shortest pause, ms. |
| `max` | Upper bound of the pause, ms (exclusive). |

### RndBool

```csharp
public static bool RndBool(this int truePercent)
```

Extension method for `int`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L275)

`true` with the given probability, in percent.

### RndDecimal

```csharp
public static decimal RndDecimal(this IZennoPosterProjectModel project, string Var)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L225)

Reads a project variable as a decimal; a value like `0.1-0.5` gives a random number in that range.

| Parameter | Description |
|---|---|
| `Var` | Variable name. |

### RndFile

```csharp
public static string RndFile(string directoryPath, string extension = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L286)

Random file from a folder and its subfolders.

| Parameter | Description |
|---|---|
| `directoryPath` | Folder. |
| `extension` | Only files with this extension; empty for all. |

**Returns:** The path, or `null` when there are no files. An I/O error (e.g. a missing folder) is retried twice and then thrown.

### RndHexString

```csharp
public static string RndHexString(int length)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L20)

Random lowercase hex string of `length` digits, prefixed with `0x`.

### RndInt

```csharp
public static int RndInt(this IZennoPosterProjectModel project, string Var)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L253)

Reads a project variable as an integer; a value like `10-20` gives a random integer from 10 (inclusive) to 20 (exclusive).

| Parameter | Description |
|---|---|
| `Var` | Variable name. |

### RndMail

```csharp
public static string RndMail(int minLength = 5, int maxLength = 10, string domain = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L349)

Random e-mail address: a random local part of letters and digits at a popular mail domain.

| Parameter | Description |
|---|---|
| `minLength` | Shortest local part. |
| `maxLength` | Longest local part. |
| `domain` | Domain; random when `null`. |

### RndMonth

```csharp
public static string RndMonth()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L364)

Random English month name.

### RndNickname

```csharp
public static string RndNickname(int min = 8, int max = 16)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L45)

Random nickname built from word lists (adjective, noun, suffix, numbers, separators), up to 100 tries to fit the length.

| Parameter | Description |
|---|---|
| `min` | Shortest length. |
| `max` | Longest length. |

### RndPass

```csharp
public static string RndPass(int minLength = 10, int maxLength = 14, bool upperCase = true, bool symbols = true)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L386)

Random password with at least one lowercase letter and digit, plus the selected groups. Passwords with three sequential characters (abc, 321, ZYX) are rejected and generated again.

| Parameter | Description |
|---|---|
| `minLength` | Shortest length. |
| `maxLength` | Longest length. |
| `upperCase` | Include uppercase letters. |
| `symbols` | Include `!@#$%?&`. |

### RndPercent

```csharp
public static double RndPercent(decimal input, double percent, double maxPercent)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L197)

Takes `percent`% of `input` and reduces it by a random 0…`maxPercent`%. A result that is not positive is replaced by a tiny positive value.

| Parameter | Description |
|---|---|
| `input` | Base amount. |
| `percent` | Share to take, 0–100. |
| `maxPercent` | Largest random reduction, 0–100. |

### RndProfileData

```csharp
public static void RndProfileData(this IZennoPosterProjectModel project, bool email = true, bool password = true)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L477)

Sets random profile data.

| Parameter | Description |
|---|---|
| `email` | Set `project.Profile.Email` to `RndMail()`. |
| `password` | Set `project.Profile.Password` to `RndPass()`. |

### RndString

```csharp
public static string RndString(int length)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L32)

Random string of Latin letters and digits.

