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

HTTP-сервер внутри ZennoPoster, который принимает команды от оркестратора напрямую, без базы. Запуск: `project.StartZpServer()`; остановка: `project.StopZpServer()`. Эндпойнты: `GET /state` (задачи и процессы этой машины), `POST /command` (`{ action, task_id, payload }`, выполняется сразу), `GET/POST /task/xml`, `GET /task/settings`, `GET /log` (файлы логов ZennoPoster), `GET /traffic` (трафик в JSONL: `tail=N` или страницы по смещению в байтах), `GET /traffic/har`, `GET /version` (версии z3n7, ZennoPoster и среды выполнения), `GET /debug/assemblies`. Каждый маршрут требует токен: заголовок `Authorization: Bearer {token}` или параметр `?token=` (для ссылок на скачивание). См. `ZpAuth`.

## Методы

### StartZpServer

```csharp
public static void StartZpServer(this IZennoPosterProjectModel project, int port = 22222, bool log = false, bool openFirewall = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpServer.cs#L54)

Загружает токен доступа, берёт первый свободный порт начиная с `port` (пробует до 20) и запускает сервер в фоновом потоке. Если сервер уже работает, только снова печатает строку узла. Занятый порт — не ошибка; если нет URL ACL (доступ запрещён), попытка останавливается с предупреждением, в котором указана команда `netsh http add urlacl`.

| Параметр | Описание |
|---|---|
| `port` | Первый порт, который пробуется. |
| `log` | Напечатать в лог строку регистрации узла (и подсказки к ней). |
| `openFirewall` | Создать входящее правило брандмауэра для порта, если его нет (нужны права администратора). |

### StopZpServer

```csharp
public static void StopZpServer(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpServer.cs#L78)

Останавливает сервер.

| Параметр | Описание |
|---|---|
| `log` | Написать строку в лог. |

