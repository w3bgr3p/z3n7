---
title: "ZpServer"
tags: [api, Server]
generated: z3n7-docgen
---

# ZpServer

`static class` · пространство имён `z3n7` · исходник [Server/ZpServer.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpServer.cs#L29)

```csharp
public static class ZpServer
```

HTTP server inside ZennoPoster that takes commands from an orchestrator directly, without the database. Start: `project.StartZpServer()`; stop: `project.StopZpServer()`. Endpoints: `GET /state` (tasks and processes of this machine), `POST /command` (`{ action, task_id, payload }`, executed immediately), `GET/POST /task/xml`, `GET /task/settings`, `GET /log` (ZennoPoster log files), `GET /traffic` (JSONL traffic: `tail=N` or pages by byte offset), `GET /traffic/har`, `GET /version` (z3n7, ZennoPoster and runtime versions), `GET /debug/assemblies`. Every route requires the token: header `Authorization: Bearer {token}` or parameter `?token=` (for download links). See `ZpAuth`.

## Методы

### StartZpServer

```csharp
public static void StartZpServer(this IZennoPosterProjectModel project, int port = 22222, bool log = false, bool openFirewall = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpServer.cs#L54)

Loads the access token, takes the first free port from `port` (up to 20 tried) and starts serving on a background thread. When the server is already running, only prints the node line again. A taken port is not an error; a missing URL ACL (access denied) stops the attempt with a warning that names the `netsh http add urlacl` command.

| Параметр | Описание |
|---|---|
| `port` | First port to try. |
| `log` | Print the node registration line (and its hints) to the log. |
| `openFirewall` | Create an inbound firewall rule for the port when none is found (needs administrator rights). |

### StopZpServer

```csharp
public static void StopZpServer(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpServer.cs#L78)

Stops the server.

| Параметр | Описание |
|---|---|
| `log` | Write a line to the log. |

