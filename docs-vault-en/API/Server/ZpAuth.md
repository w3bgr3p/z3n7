---
title: "ZpAuth"
tags: [api, Server]
generated: z3n7-docgen
---

# ZpAuth

`static class` · namespace `z3n7` · source [Server/ZpAuth.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L18)

```csharp
public static class ZpAuth
```

Токен доступа к ZpServer: хранение, выдача, проверка запроса. Секрет один на машину и лежит в ключе ZP_TOKEN файла .env рядом со сборкой (тот же файл, что читает Env.ReadEnv(global: true)). Токен печатается в строке узла, чтобы её можно было целиком вставить в панель DevDeck.

## Properties

### Token

```csharp
public static string Token { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L25)

Действующий токен. Пустая строка, пока не вызван Load.

## Methods

### Authorized

```csharp
public static bool Authorized(HttpListenerRequest req)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L62)

Токен запроса: заголовок Authorization: Bearer, иначе параметр ?token= — он нужен для ссылок на скачивание, куда заголовок не подставить.

### Load

```csharp
public static void Load(IZennoPosterProjectModel project, bool log)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L35)

Достаёт токен из .env, а при отсутствии — генерирует и пытается сохранить. Вызывать до захвата порта: проверка конфигурации дешёвая, и незачем занимать ресурс, если с ней что-то не так. Неудачная запись файла сервер не останавливает: узел остаётся управляемым, но токен живёт только в памяти процесса.

## Fields

### EnvKey

```csharp
public const string EnvKey = "ZP_TOKEN";
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L20)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
