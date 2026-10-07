---
title: "SAFU"
tags: [api, Essentials]
generated: z3n7-docgen
---

# SAFU

`static class` · namespace `z3n7` · source [Essentials/Safu8.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L362)

```csharp
public static class SAFU
```

Entry point to secure storage. Call `InitZ3n8` once (done by `InitVariables`) before the other methods.

## Methods

### Decode

```csharp
public static string Decode(IZennoPosterProjectModel project, string toDecrypt)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L406)

Decrypts with the key of the current account (`acc0`) and the PIN from the `cfgPin` secure variable.

### DecryptHWID

```csharp
public static string DecryptHWID(IZennoPosterProjectModel project, string toDecrypt)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L385)

Decrypts with the machine-bound key. Returns an empty string for empty input.

### Encode

```csharp
public static string Encode(IZennoPosterProjectModel project, string toEncrypt)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L420)

Encrypts with the key of the current account (`acc0`) and the PIN from the `cfgPin` secure variable.

### EncryptHWID

```csharp
public static string EncryptHWID(IZennoPosterProjectModel project, string toEncrypt)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L394)

Encrypts with the machine-bound key. Returns an empty string for empty input.

### HWPass

```csharp
public static string HWPass(this IZennoPosterProjectModel project)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L434)

Returns the deterministic password of the current account (`acc0`), using the PIN from the `cfgPin` secure variable.

### InitZ3n8

```csharp
public static void InitZ3n8(IZennoPosterProjectModel project, string keyFilePath)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L366)

Registers `Z3n8SAFU` in `FunctionStorage` and writes the key path to the log.

| Parameter | Description |
|---|---|
| `keyFilePath` | Path to the 32-byte key file. |

