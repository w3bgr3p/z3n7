---
title: "Vars"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Vars

`static class` · пространство имён `z3n7` · исходник [Essentials/Vars.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L13)

```csharp
public static class Vars
```

Short accessors for project variables: read, write, parse, count.

## Методы

### Bool

```csharp
public static bool Bool(this IZennoPosterProjectModel project, string var)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L96)

Returns `true` when the project variable equals `True` exactly.

### Decimal

```csharp
public static decimal Decimal(this IZennoPosterProjectModel project, string var)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L83)

Returns a project variable parsed as `decimal` (current culture), or 0 when it cannot be parsed.

### Int

```csharp
public static int Int(this IZennoPosterProjectModel project, string var)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L59)

Returns a project variable parsed as `int`, or 0 when it is empty or not a number.

```csharp
public static int Int(this IZennoPosterProjectModel project, string varName, int input)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L73)

Adds `input` to an integer project variable and stores the result.

**Возвращает:** The new value.

### MaxErr

```csharp
public static void MaxErr(this IZennoPosterProjectModel project, int maxAttempts, Exception ex = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L108)

Error counter for retry loops. Stores the error text in `err` and increments `maxErr`; once `maxErr` exceeds `maxAttempts`, writes a warning and throws.

| Параметр | Описание |
|---|---|
| `maxAttempts` | Number of errors tolerated. |
| `ex` | The error; when null, `project.LastErrorComment` is used. |

### Range

```csharp
public static List<string> Range(this IZennoPosterProjectModel project, string accRange = null, string output = null, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L258)

Parses an account range and stores it in `rangeStart`, `rangeEnd` and `range` (comma-separated list). Accepted forms: `5`, `1-10`, `1,4,7`. Anything after `:` is ignored.

| Параметр | Описание |
|---|---|
| `accRange` | Range text; when empty, the `cfgAccRange` variable is used. |
| `output` | Not used. |
| `log` | Not used. |

**Возвращает:** Account numbers as strings, or `null` (with a warning) when no range is given.

### Var

```csharp
public static string Var(this IZennoPosterProjectModel project, string var)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L22)

Returns the value of a project variable. A missing variable is reported to the log and an empty string is returned.

| Параметр | Описание |
|---|---|
| `var` | Variable name. |

```csharp
public static string Var(this IZennoPosterProjectModel project, string var, object value)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L45)

Sets a project variable to `value.ToString()`. `null` is ignored. A missing variable is reported to the log, nothing is thrown.

| Параметр | Описание |
|---|---|
| `var` | Variable name. |
| `value` | New value. |

**Возвращает:** Always an empty string.

### VarAdd

```csharp
public static bool VarAdd(this IZennoPosterProjectModel project, string name, string defaultValue = "", string comment = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L140)

Adds a variable to the project open in ProjectMaker through the local ZennoPoster API (`http://localhost:5299`). Development-time only: it edits ProjectMaker's in-memory copy of the project, so a task running in the runner is not affected. Save the project to keep the change. The API key is read from `ZENNO_API_KEY` in the `.env` next to `z3n7.dll`; the key tier must be T1 or higher.

| Параметр | Описание |
|---|---|
| `name` | Variable name. |
| `defaultValue` | Initial value. |
| `comment` | Variable comment. |

**Возвращает:** `true` when the API answered `RESULT_OK`; otherwise the answer is written to the log as a warning.

### VarCounter

```csharp
public static int VarCounter(this IZennoPosterProjectModel project, string varName, int input)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L188)

Adds `input` to an integer project variable and stores the result. Same as `Int(varName, input)`.

**Возвращает:** The new value.

### VarRnd

```csharp
public static string VarRnd(this IZennoPosterProjectModel project, string var)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L162)

Reads a project variable. A value like `10-20` returns a random integer from 10 (inclusive) to 20 (exclusive); any other value is returned trimmed.

| Параметр | Описание |
|---|---|
| `var` | Variable name. |

### VarsFromDict

```csharp
public static void VarsFromDict(this IZennoPosterProjectModel project, Dictionary<string, string> dict)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L232)

Sets a project variable for every key of the dictionary.

### VarsFromJson

```csharp
public static void VarsFromJson(this IZennoPosterProjectModel project, string json = "jVars")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L242)

Sets project variables from a flat JSON object of string values.

| Параметр | Описание |
|---|---|
| `json` | JSON text, or the default `jVars` to read the JSON from the `jVars` variable. |

### VarsMath

```csharp
public static decimal VarsMath(this IZennoPosterProjectModel project, string varA, string operation, string varB, string resultVar = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L203)

Applies `+`, `-`, `*` or `/` to two project variables parsed as `decimal` (invariant culture). Other operations throw.

| Параметр | Описание |
|---|---|
| `varA` | Left operand variable. |
| `operation` | One of `+ - * /`. |
| `varB` | Right operand variable. |
| `resultVar` | Variable that receives the result; empty to skip. |

**Возвращает:** The result.

