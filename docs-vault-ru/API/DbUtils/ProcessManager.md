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

Поддерживает актуальность таблицы `_processes` по процессам ZennoPoster и `zbe1` этой машины.

## Методы

### CollectAndSave

```csharp
public static void CollectAndSave(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L42)

Записывает по строке на каждый работающий процесс ZennoPoster и `zbe1` этой машины (id `{pid}|{machine}`, имя, RAM в МБ, время работы в минутах, командная строка, время) и удаляет строки этой машины для процессов, которые уже не работают. Синтаксис upsert PostgreSQL используется, если `dbSource` — строка подключения PostgreSQL (то же правило, что у `DbQ`); иначе `INSERT OR REPLACE` из SQLite.

| Параметр | Описание |
|---|---|
| `log` | Писать запросы в лог. |

### EnsureProcessTable

```csharp
public static void EnsureProcessTable(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L27)

Создаёт таблицу `_processes`, если её нет.

| Параметр | Описание |
|---|---|
| `log` | Не используется. |

### GetAllMachines

```csharp
public static List<string> GetAllMachines(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L89)

Уникальные имена машин из таблицы `_processes`.

| Параметр | Описание |
|---|---|
| `log` | Писать запросы в лог. |

### KillByUptime

```csharp
public static void KillByUptime(this IZennoPosterProjectModel project, int maxUptimeMinutes, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L107)

Убивает процессы `zbe1`, работающие дольше `maxUptimeMinutes`, и, если кого-то убил, обновляет таблицу.

| Параметр | Описание |
|---|---|
| `maxUptimeMinutes` | Предел времени работы, минуты. |
| `log` | Писать в лог каждое убийство процесса. |

### ZennoProcesses

```csharp
public static List<string[]> ZennoProcesses()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/DbUtils/ProcessManager.cs#L138)

Работающие процессы ZennoPoster и `zbe1` этой машины.

**Возвращает:** Элементы `[name, ramMb, uptimeMinutes, pid]`.

