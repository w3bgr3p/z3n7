---
title: "ProcessManager"
tags: [api, DbUtils]
generated: z3n7-docgen
---

# ProcessManager

`static class` · пространство имён `z3n7.DbUtils` · исходник [DbUtils/ProcessManager.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L9)

```csharp
public static class ProcessManager
```

*Описания пока нет.*

## Методы

### CollectAndSave

```csharp
public static void CollectAndSave(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L28)

### EnsureProcessTable

```csharp
public static void EnsureProcessTable(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L21)

### GetAllMachines

```csharp
public static List<string> GetAllMachines(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L73)

### KillByUptime

```csharp
public static void KillByUptime(this IZennoPosterProjectModel project, int maxUptimeMinutes, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L85)

### ZennoProcesses

```csharp
public static List<string[]> ZennoProcesses()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L114)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
