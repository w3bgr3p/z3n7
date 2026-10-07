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

Одноразовые коды.

## Методы

### FirstMail

```csharp
public static string FirstMail(IZennoPosterProjectModel project, string email)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Otp.cs#L39)

Код из последнего письма FirstMail, отправленного на `email` (см. `z3n7.FirstMail.GetOTP`).

| Параметр | Описание |
|---|---|
| `email` | Исходный получатель, на которого было отправлено письмо. |

### Offline

```csharp
public static string Offline(string keyString, int waitIfTimeLess = 5)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Otp.cs#L17)

Вычисляет текущий TOTP-код по секрету в Base32. Если до истечения кода осталось меньше `waitIfTimeLess` секунд, ждёт следующий.

| Параметр | Описание |
|---|---|
| `keyString` | Секрет в Base32. |
| `waitIfTimeLess` | Если код действителен меньше этого числа секунд, ждётся следующий. |

**Возвращает:** Код. Для пустого секрета бросает исключение.

