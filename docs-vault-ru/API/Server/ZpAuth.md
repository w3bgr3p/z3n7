---
title: "ZpAuth"
tags: [api, Server]
generated: z3n7-docgen
---

# ZpAuth

`static class` · пространство имён `z3n7` · исходник [Server/ZpAuth.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L18)

```csharp
public static class ZpAuth
```

Токен доступа к ZpServer: хранение, выдача, проверка запроса. Секрет один на машину и лежит в ключе ZP_TOKEN файла .env рядом со сборкой (тот же файл, что читает Env.ReadEnv(global: true)). Токен печатается в строке узла, чтобы её можно было целиком вставить в панель DevDeck.

## Свойства

### Token

```csharp
public static string Token { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L25)

Действующий токен. Пустая строка, пока не вызван Load.

## Методы

### Authorized

```csharp
public static bool Authorized(HttpListenerRequest req)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L62)

Токен запроса: заголовок Authorization: Bearer, иначе параметр ?token= — он нужен для ссылок на скачивание, куда заголовок не подставить.

### Load

```csharp
public static void Load(IZennoPosterProjectModel project, bool log)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L35)

Достаёт токен из .env, а при отсутствии — генерирует и пытается сохранить. Вызывать до захвата порта: проверка конфигурации дешёвая, и незачем занимать ресурс, если с ней что-то не так. Неудачная запись файла сервер не останавливает: узел остаётся управляемым, но токен живёт только в памяти процесса.

## Поля

### EnvKey

```csharp
public const string EnvKey = "ZP_TOKEN";
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L20)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
