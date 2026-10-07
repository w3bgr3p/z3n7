---
title: "ISAFU"
tags: [api, Essentials]
generated: z3n7-docgen
---

# ISAFU

`interface` · namespace `z3n7` · source [Essentials/Safu8.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L23)

```csharp
public interface ISAFU
```

Encryption used by SAFU (secure storage of account secrets).

## Methods

### Decode

```csharp
string Decode(IZennoPosterProjectModel project, string toDecrypt, string pin, string acc)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L28)

Decrypts text produced by `Encode` with the same machine, PIN and account.

### DecodeHWID

```csharp
string DecodeHWID(IZennoPosterProjectModel project, string toDecrypt)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L37)

Decrypts text produced by `EncodeHWID` on the same machine.

### Encode

```csharp
string Encode(IZennoPosterProjectModel project, string toEncrypt, string pin, string acc)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L26)

Encrypts text with a key bound to the machine, the PIN and the account.

### EncodeHWID

```csharp
string EncodeHWID(IZennoPosterProjectModel project, string toEncrypt)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L35)

Encrypts text with a key bound to the machine only.

### HWPass

```csharp
string HWPass(IZennoPosterProjectModel project, string pin, string acc)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L33)

Returns a password derived from the machine, the PIN and the account. The same inputs always give the same password.

