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

Access token of `ZpServer`: storing, issuing, checking requests. There is one secret per machine, kept under `ZP_TOKEN` in the `.env` next to the assembly (the same file `Env.ReadEnv(global: true)` reads). The token is printed in the node line so that the line can be pasted into the DevDeck panel as a whole.

## Свойства

### Token

```csharp
public static string Token { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L24)

The current token. An empty string until `Load` is called.

## Методы

### Authorized

```csharp
public static bool Authorized(HttpListenerRequest req)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L61)

Checks the request token: the `Authorization: Bearer` header, otherwise the `?token=` parameter, needed for download links where a header cannot be set.

**Возвращает:** `false` when `Load` has not run or the token does not match.

### Load

```csharp
public static void Load(IZennoPosterProjectModel project, bool log)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L34)

Reads the token from `.env`; when there is none, generates one (32 random bytes as hex) and tries to save it. Call it before taking the port: the configuration check is cheap, and there is no point holding the resource if something is wrong with it. A failed write does not stop the server: the node stays manageable, but the token lives only in the process memory.

| Параметр | Описание |
|---|---|
| `log` | Write the path of the saved token to the log. |

## Поля

### EnvKey

```csharp
public const string EnvKey = "ZP_TOKEN";
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L19)

Key of the token in `.env`.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
