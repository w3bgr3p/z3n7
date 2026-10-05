---
title: "Webshare"
tags: [api, Api]
generated: z3n7-docgen
---

# Webshare

`class` · пространство имён `z3n7.Api` · исходник [Api/Webshare.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L11)

```csharp
public class Webshare : IDisposable
```

Client of the Webshare proxy API. Dispose it to release the HTTP client.

## Конструкторы

### Webshare

```csharp
public Webshare(string apiKey)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L19)

Creates a client.

| Параметр | Описание |
|---|---|
| `apiKey` | Value of the `Authorization` header; required. |

## Методы

### Dispose

```csharp
public void Dispose()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L71)

Disposes the HTTP client.

### GetProxyList

```csharp
public List<string> GetProxyList()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L65)

Blocking version of `GetProxyListAsync`.

### GetProxyListAsync

```csharp
public async Task<List<string>> GetProxyListAsync()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L34)

Downloads the proxy list of the account's first plan (direct connection, username authentication).

**Возвращает:** One proxy per item, as returned by Webshare. Throws when the plan or the download token cannot be obtained.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
