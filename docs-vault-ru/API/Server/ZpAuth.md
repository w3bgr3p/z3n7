---
title: "ZpAuth"
tags: [api, Server]
generated: z3n7-docgen
---

# ZpAuth

`static class` · пространство имён `z3n7` · исходник [Server/ZpAuth.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L16)

```csharp
public static class ZpAuth
```

Токен доступа `ZpServer`: хранение, выдача, проверка запросов. Секрет один на машину, хранится под `ZP_TOKEN` в `.env` рядом со сборкой (тот же файл, что читает `Env.ReadEnv(global: true)`). Токен печатается в строке узла, чтобы строку можно было целиком вставить в панель DevDeck.

## Свойства

### Token

```csharp
public static string Token { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L24)

Текущий токен. Пустая строка, пока не вызван `Load`.

## Методы

### Authorized

```csharp
public static bool Authorized(HttpListenerRequest req)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L61)

Проверяет токен запроса: заголовок `Authorization: Bearer`, иначе параметр `?token=` — он нужен для ссылок на скачивание, где заголовок не задать.

**Возвращает:** `false`, если `Load` не вызывался или токен не совпадает.

### Load

```csharp
public static void Load(IZennoPosterProjectModel project, bool log)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L34)

Читает токен из `.env`; если его нет, генерирует (32 случайных байта в hex) и пытается сохранить. Вызывай до того, как занимать порт: проверка конфигурации дешёвая, и держать ресурс, если с ней что-то не так, незачем. Неудачная запись не останавливает сервер: узлом по-прежнему можно управлять, но токен живёт только в памяти процесса.

| Параметр | Описание |
|---|---|
| `log` | Записать в лог путь к сохранённому токену. |

## Поля

### EnvKey

```csharp
public const string EnvKey = "ZP_TOKEN";
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L19)

Ключ токена в `.env`.

