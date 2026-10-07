---
title: "LogDisabler"
tags: [api, Essentials]
generated: z3n7-docgen
---

# LogDisabler

`class` · namespace `z3n7` · source [Essentials/LogDisabler.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/LogDisabler.cs#L10)

```csharp
public class LogDisabler
```

Stops ZennoPoster from writing its own log files to the `Logs` folder next to the running executable.

## Methods

### DisableLogs

```csharp
public static void DisableLogs(bool aggressive = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/LogDisabler.cs#L23)

Replaces the `Logs` folder with a directory link to `NUL`. Does nothing if the folder is already a link, a file, or has a `Logs.lock` marker next to it. If that fails, the folder is replaced with a hidden read-only file named `Logs` and a `Logs.lock` marker is written.

| Parameter | Description |
|---|---|
| `aggressive` | Use `rd /s /q` to remove the folder and retry up to 3 times. |

**Remarks:** Deletes the existing `Logs` folder with its contents.

