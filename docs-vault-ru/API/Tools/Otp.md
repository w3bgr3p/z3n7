---
title: "Otp"
tags: [api, Tools]
generated: z3n7-docgen
---

# Otp

`static class` · пространство имён `z3n7.Tools` · исходник [Tools/Otp.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Otp.cs#L8)

```csharp
public static class Otp
```

One-time codes.

## Методы

### FirstMail

```csharp
public static string FirstMail(IZennoPosterProjectModel project, string email)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Otp.cs#L39)

Code from the latest FirstMail message sent to `email` (see `z3n7.FirstMail.GetOTP`).

| Параметр | Описание |
|---|---|
| `email` | Original recipient the message was sent to. |

### Offline

```csharp
public static string Offline(string keyString, int waitIfTimeLess = 5)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Otp.cs#L17)

Computes the current TOTP code from a Base32 secret. When the code expires within `waitIfTimeLess` seconds, waits for the next one.

| Параметр | Описание |
|---|---|
| `keyString` | Base32 secret. |
| `waitIfTimeLess` | Seconds of validity below which the next code is awaited. |

**Возвращает:** The code. Throws for an empty secret.

