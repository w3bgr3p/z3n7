---
title: "Z3n8SAFU"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Z3n8SAFU

`class` · namespace `z3n7` · source [Essentials/Safu8.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L48)

```csharp
public class Z3n8SAFU : ISAFU
```

SAFU implementation: AES-256-CBC with an HMAC-SHA256 tag, keys derived with PBKDF2-SHA256 (100 000 iterations). The salt comes from a 32-byte key file. The machine ID is a SHA-256 of the processor ID, the motherboard serial and the system disk serial (WMI). When the `jVars` blob contains `serverHwid`, that value is used instead of the local machine ID for `Encode`, `Decode` and `HWPass`.

## Constructors

### Z3n8SAFU

```csharp
public Z3n8SAFU(string keyFilePath)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L58)

Creates the implementation. The key file is read on first use and must be exactly 32 bytes.

| Parameter | Description |
|---|---|
| `keyFilePath` | Path to the key file. |

## Methods

### Decode

```csharp
public string Decode(IZennoPosterProjectModel project, string toDecrypt, string pin, string acc)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L288)

Decrypts `toDecrypt`.

**Returns:** The plaintext, or an empty string when the input is empty, malformed, or the HMAC does not match.

### DecodeHWID

```csharp
public string DecodeHWID(IZennoPosterProjectModel project, string toDecrypt)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L348)

Decrypts with a key derived from the local machine ID only.

**Returns:** The plaintext, or an empty string when decryption fails.

### Encode

```csharp
public string Encode(IZennoPosterProjectModel project, string toEncrypt, string pin, string acc)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L276)

Encrypts `toEncrypt`. An empty PIN is replaced by a fixed placeholder.

**Returns:** Base64 of IV + ciphertext + HMAC, or an empty string for empty input.

### EncodeHWID

```csharp
public string EncodeHWID(IZennoPosterProjectModel project, string toEncrypt)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L339)

Encrypts with a key derived from the local machine ID only.

**Returns:** Base64 of IV + ciphertext + HMAC, or an empty string for empty input.

### HWPass

```csharp
public string HWPass(IZennoPosterProjectModel project, string pin, string acc)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L300)

Returns a deterministic 24-character password with at least one lowercase letter, uppercase letter, digit and symbol.

