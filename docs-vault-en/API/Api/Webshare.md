---
title: "Webshare"
tags: [api, Api]
generated: z3n7-docgen
---

# Webshare

`class` · namespace `z3n7.Api` · source [Api/Webshare.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L11)

```csharp
public class Webshare : IDisposable
```

Client of the Webshare proxy API. Dispose it to release the HTTP client.

## Constructors

### Webshare

```csharp
public Webshare(string apiKey)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L19)

Creates a client.

| Parameter | Description |
|---|---|
| `apiKey` | Value of the `Authorization` header; required. |

## Methods

### Dispose

```csharp
public void Dispose()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L71)

Disposes the HTTP client.

### GetProxyList

```csharp
public List<string> GetProxyList()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L65)

Blocking version of `GetProxyListAsync`.

### GetProxyListAsync

```csharp
public async Task<List<string>> GetProxyListAsync()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L34)

Downloads the proxy list of the account's first plan (direct connection, username authentication).

**Returns:** One proxy per item, as returned by Webshare. Throws when the plan or the download token cannot be obtained.

