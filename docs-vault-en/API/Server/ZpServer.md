---
title: "ZpServer"
tags: [api, Server]
generated: z3n7-docgen
---

# ZpServer

`static class` · namespace `z3n7` · source [Server/ZpServer.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpServer.cs#L33)

```csharp
public static class ZpServer
```

HTTP-сервер внутри ZennoPoster. Принимает команды от оркестратора напрямую, минуя БД. Запуск: project.StartZpServer() Остановка: project.StopZpServer() Endpoints: GET /state — tasks + processes текущей машины GET /traffic — JSONL traffic: tail=N либо страницы по байтовому оффсету POST /command — { action, task_id, payload } → исполняет немедленно GET /version — версии z3n7, ZennoPoster, рантайма Все роуты требуют токен: заголовок Authorization: Bearer &lt;token&gt; либо параметр ?token= (для ссылок на скачивание). См. ZpAuth.

## Methods

### StartZpServer

```csharp
public static void StartZpServer(this IZennoPosterProjectModel project, int port = 22222, bool log = false, bool openFirewall = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpServer.cs#L47)

### StopZpServer

```csharp
public static void StopZpServer(this IZennoPosterProjectModel project, bool log = false)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpServer.cs#L69)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
