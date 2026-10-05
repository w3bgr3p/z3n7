---
title: "ProjectExtensions (Essentials)"
tags: [api, Essentials]
generated: z3n7-docgen
---

# ProjectExtensions (Essentials)

`static class` · namespace `z3n7` · source [Essentials/ExternalCode.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/ExternalCode.cs#L9), [Essentials/Init.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Init.cs#L134), [Essentials/Logger.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L286), [Essentials/Time.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L174)

```csharp
public static class ProjectExtensions
```

Other parts of this type: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

Extension methods on `IZennoPosterProjectModel`: start-up, logging, timing and running other projects.

## Methods

### Age

```csharp
public static T Age<T>(this IZennoPosterProjectModel project, string var = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L194)

Age of the session: time since the Unix milliseconds stored in `var`. When the variable does not hold a number, it is set to now first. `string` returns `TimeSpan.ToString()`, `TimeSpan` returns the span, any other type receives whole seconds converted with `Convert.ChangeType`.

| Parameter | Description |
|---|---|
| `var` | Variable with the start time; default `varSessionId`. |

### Deadline

```csharp
public static int Deadline(this IZennoPosterProjectModel project, int sec = 0, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L252)

Two-call deadline based on the `t0` variable. With `sec` = 0 it stores the current time and returns 0; with `sec` &gt; 0 it returns the seconds since then and throws when they exceed `sec`.

| Parameter | Description |
|---|---|
| `sec` | Limit in seconds, or 0 to start. |
| `log` | Write the elapsed seconds to the log. |

### InitVariables

```csharp
public static void InitVariables(this IZennoPosterProjectModel project, Instance instance, string author = "w3bgr3p")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Init.cs#L144)

Runs `Init.InitVariables` and then starts the embedded server (`StartZpServer`). A server start failure is written to the log as a warning and does not stop the project.

| Parameter | Description |
|---|---|
| `instance` | Browser instance of the project. |
| `author` | Script author shown in the start banner. |

### log

```csharp
public static void log(this IZennoPosterProjectModel project, object toLog, [CallerMemberName] string caller = "", bool show = true, bool toZp = true)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L295)

Writes a message to the project log through a default `Logger`. When called directly from a C# action, the generated action name is replaced by the project name.

| Parameter | Description |
|---|---|
| `toLog` | Message. |
| `show` | Write even if below the minimum level. |
| `toZp` | Write to the ZennoPoster log. |

### RunZp

```csharp
public static bool RunZp(this IZennoPosterProjectModel project, List<string> vars = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/ExternalCode.cs#L19)

Runs the project whose path is stored in the `projectScript` variable, via `ExecuteProject`. Each name in `vars` is mapped to the variable of the same name in the called project. An exception is written to the log as a warning and rethrown.

| Parameter | Description |
|---|---|
| `vars` | Variable names to pass to the called project. |

**Returns:** The result of `ExecuteProject`.

```csharp
public static bool RunZp(this IZennoPosterProjectModel project, string path)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/ExternalCode.cs#L52)

Runs the project at `path` via `ExecuteProject`, passing a fixed set of variables by name: `acc0`, `cfgLog`, `cfgPin`, `DBmode`, `DBpstgrPass`, `DBpstgrUser`, `DBsqltPath`, `instancePort`, `lastQuery`, `varSessionId`, `wkMode`.

| Parameter | Description |
|---|---|
| `path` | Path to the .zp file. |

**Returns:** The result of `ExecuteProject`.

### StartSession

```csharp
public static void StartSession(this IZennoPosterProjectModel project)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L273)

Waits a random 0–1 s and stores the current Unix milliseconds in `varSessionId`.

### TimeElapsed

```csharp
public static int TimeElapsed(this IZennoPosterProjectModel project, string varName = "varSessionId")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L178)

Seconds since the time stored (as Unix milliseconds) in a project variable.

| Parameter | Description |
|---|---|
| `varName` | Variable with the start time; default is the session start, `varSessionId`. |

### TimeOut

```csharp
public static void TimeOut(this IZennoPosterProjectModel project, int min = 0)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L233)

Throws once the session (`varSessionId`) is older than `min` minutes. The message names the last executed action.

| Parameter | Description |
|---|---|
| `min` | Limit in minutes; 0 reads it from the `timeOut` variable. |

### warn

```csharp
public static void warn(this IZennoPosterProjectModel project, string msg, bool thrw = false, bool show = true, [CallerMemberName] string caller = "")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L311)

Writes a warning to the project log.

| Parameter | Description |
|---|---|
| `msg` | Message. |
| `thrw` | Also store the message in the `err` variable and throw an `Exception`. |
| `show` | Write even if below the minimum level. |

```csharp
public static void warn(this IZennoPosterProjectModel project, Exception ex, bool thrw = false, bool withStack = false, bool toZp = true, [CallerMemberName] string caller = "")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L330)

Writes an exception message as a warning and stores it in the `err` variable.

| Parameter | Description |
|---|---|
| `ex` | Exception to report. |
| `thrw` | Throw an `Exception` with the message after writing. |
| `withStack` | Append the stack trace. |
| `toZp` | Not used. |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
