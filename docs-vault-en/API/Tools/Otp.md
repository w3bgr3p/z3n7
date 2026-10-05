---
title: "Otp"
tags: [api, Tools]
generated: z3n7-docgen
---

# Otp

`static class` · namespace `z3n7.Tools` · source [Tools/Otp.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Otp.cs#L8)

```csharp
public static class Otp
```

One-time codes.

## Methods

### FirstMail

```csharp
public static string FirstMail(IZennoPosterProjectModel project, string email)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Otp.cs#L39)

Code from the latest FirstMail message sent to `email` (see `z3n7.FirstMail.GetOTP`).

| Parameter | Description |
|---|---|
| `email` | Original recipient the message was sent to. |

### Offline

```csharp
public static string Offline(string keyString, int waitIfTimeLess = 5)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Otp.cs#L17)

Computes the current TOTP code from a Base32 secret. When the code expires within `waitIfTimeLess` seconds, waits for the next one.

| Parameter | Description |
|---|---|
| `keyString` | Base32 secret. |
| `waitIfTimeLess` | Seconds of validity below which the next code is awaited. |

**Returns:** The code. Throws for an empty secret.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
