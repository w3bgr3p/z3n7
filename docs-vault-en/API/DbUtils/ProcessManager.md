---
title: "ProcessManager"
tags: [api, DbUtils]
generated: z3n7-docgen
---

# ProcessManager

`static class` · namespace `z3n7.DbUtils` · source [DbUtils/ProcessManager.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L9)

```csharp
public static class ProcessManager
```

*No description yet.*

## Methods

### CollectAndSave

```csharp
public static void CollectAndSave(this IZennoPosterProjectModel project, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L28)

### EnsureProcessTable

```csharp
public static void EnsureProcessTable(this IZennoPosterProjectModel project, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L21)

### GetAllMachines

```csharp
public static List<string> GetAllMachines(this IZennoPosterProjectModel project, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L73)

### KillByUptime

```csharp
public static void KillByUptime(this IZennoPosterProjectModel project, int maxUptimeMinutes, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L85)

### ZennoProcesses

```csharp
public static List<string[]> ZennoProcesses()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L114)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
