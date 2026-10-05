---
title: "BestMailBox"
tags: [api, Mail]
generated: z3n7-docgen
---

# BestMailBox

`class` · namespace `z3n7.Api` · source [Mail/BestMailBox.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L12)

```csharp
public class BestMailBox
```

*No description yet.*

## Constructors

### BestMailBox

```csharp
public BestMailBox(IZennoPosterProjectModel project, string apikey = null, string baseUrl = null, bool useNetHttp = false, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L21)

## Methods

### DeleteMail

```csharp
public bool DeleteMail(string id = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L212)

Досрочно уничтожает ящик и все его письма.

### GetDomains

```csharp
public List<string> GetDomains()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L233)

Получает список доступных доменов.

### GetHrefs

```csharp
public HashSet<string> GetHrefs(int deadline = 60, string id = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L153)

Извлекает ссылки активации/подтверждения из полученного письма.

### GetMail

```csharp
public string GetMail(int deadline = 60, string id = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L125)

Ожидает письмо и возвращает полное тело последнего письма (HTML / текст).

| Parameter | Description |
|---|---|
| `deadline` | Таймаут ожидания в секундах (по умолчанию 60 сек). |
| `id` | ID ящика или email (если null, берется из переменной mailId). |

### NewMail

```csharp
public string[] NewMail(string domain = null, string prefix = null, int ttl = 1200)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L71)

Создает временный почтовый ящик. Возвращает [id, email].

| Parameter | Description |
|---|---|
| `domain` | Желаемый домен (например "autoz3n.xyz" или "z3nd3v.xyz"), если null — выбирается случайно. |
| `prefix` | Желаемый префикс (например "alex.miller"), если null — генерируется автоматически. |
| `ttl` | Время жизни ящика в секундах (по умолчанию 1200 = 20 минут). |

### Otp

```csharp
public string Otp(int deadline = 60, string id = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L95)

Быстрое получение OTP-кода (4-8 знаков). Опрашивает API до получения или таймаута.

| Parameter | Description |
|---|---|
| `deadline` | Таймаут ожидания в секундах (по умолчанию 60 сек). |
| `id` | ID ящика или email (если null, берется из переменной mailId). |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
