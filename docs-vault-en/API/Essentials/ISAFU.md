---
title: "ISAFU"
tags: [api, Essentials]
generated: z3n7-docgen
---

# ISAFU

`interface` · namespace `z3n7` · source [Essentials/Safu8.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L17)

```csharp
public interface ISAFU
```

*No description yet.*

## Methods

### Decode

```csharp
string Decode(IZennoPosterProjectModel project, string toDecrypt, string pin, string acc)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L20)

### DecodeHWID

```csharp
string DecodeHWID(IZennoPosterProjectModel project, string toDecrypt)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L23)

### Encode

```csharp
string Encode(IZennoPosterProjectModel project, string toEncrypt, string pin, string acc)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L19)

### EncodeHWID

```csharp
string EncodeHWID(IZennoPosterProjectModel project, string toEncrypt)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L22)

### HWPass

```csharp
string HWPass(IZennoPosterProjectModel project, string pin, string acc)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L21)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
