---
title: "ZpAuth"
tags: [api, Server]
generated: z3n7-docgen
---

# ZpAuth

`static class` · namespace `z3n7` · source [Server/ZpAuth.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L16)

```csharp
public static class ZpAuth
```

Access token of `ZpServer`: storing, issuing, checking requests. There is one secret per machine, kept under `ZP_TOKEN` in the `.env` next to the assembly (the same file `Env.ReadEnv(global: true)` reads). The token is printed in the node line so that the line can be pasted into the DevDeck panel as a whole.

## Properties

### Token

```csharp
public static string Token { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L24)

The current token. An empty string until `Load` is called.

## Methods

### Authorized

```csharp
public static bool Authorized(HttpListenerRequest req)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L61)

Checks the request token: the `Authorization: Bearer` header, otherwise the `?token=` parameter, needed for download links where a header cannot be set.

**Returns:** `false` when `Load` has not run or the token does not match.

### Load

```csharp
public static void Load(IZennoPosterProjectModel project, bool log)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L34)

Reads the token from `.env`; when there is none, generates one (32 random bytes as hex) and tries to save it. Call it before taking the port: the configuration check is cheap, and there is no point holding the resource if something is wrong with it. A failed write does not stop the server: the node stays manageable, but the token lives only in the process memory.

| Parameter | Description |
|---|---|
| `log` | Write the path of the saved token to the log. |

## Fields

### EnvKey

```csharp
public const string EnvKey = "ZP_TOKEN";
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Server/ZpAuth.cs#L19)

Key of the token in `.env`.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
