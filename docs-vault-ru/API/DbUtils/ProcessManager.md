---
title: "ProcessManager"
tags: [api, DbUtils]
generated: z3n7-docgen
---

# ProcessManager

`static class` · пространство имён `z3n7.DbUtils` · исходник [DbUtils/ProcessManager.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L13)

```csharp
public static class ProcessManager
```

Keeps the `_processes` table up to date with this machine's ZennoPoster and `zbe1` processes.

## Методы

### CollectAndSave

```csharp
public static void CollectAndSave(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L42)

Writes one row per running ZennoPoster and `zbe1` process of this machine (id `{pid}|{machine}`, name, RAM in MB, uptime in minutes, command line, time) and deletes this machine's rows of processes that no longer run. PostgreSQL upsert syntax is used when `dbSource` is a PostgreSQL connection string (the same rule as `DbQ`); otherwise SQLite's `INSERT OR REPLACE`.

| Параметр | Описание |
|---|---|
| `log` | Write the queries to the log. |

### EnsureProcessTable

```csharp
public static void EnsureProcessTable(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L27)

Creates the `_processes` table if it does not exist.

| Параметр | Описание |
|---|---|
| `log` | Not used. |

### GetAllMachines

```csharp
public static List<string> GetAllMachines(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L89)

Distinct machine names in the `_processes` table.

| Параметр | Описание |
|---|---|
| `log` | Write the queries to the log. |

### KillByUptime

```csharp
public static void KillByUptime(this IZennoPosterProjectModel project, int maxUptimeMinutes, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L107)

Kills `zbe1` processes running longer than `maxUptimeMinutes` and, when any were killed, refreshes the table.

| Параметр | Описание |
|---|---|
| `maxUptimeMinutes` | Uptime limit, minutes. |
| `log` | Write each kill to the log. |

### ZennoProcesses

```csharp
public static List<string[]> ZennoProcesses()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L138)

Running ZennoPoster and `zbe1` processes of this machine.

**Возвращает:** Items `[name, ramMb, uptimeMinutes, pid]`.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
