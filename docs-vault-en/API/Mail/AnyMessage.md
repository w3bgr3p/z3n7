---
title: "AnyMessage"
tags: [api, Mail]
generated: z3n7-docgen
---

# AnyMessage

`class` · namespace `z3n7.Api` · source [Mail/AnyMessage.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L14)

```csharp
public class AnyMessage
```

*No description yet.*

## Constructors

### AnyMessage

```csharp
public AnyMessage(IZennoPosterProjectModel project, string apikey, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L20)

## Properties

### LastDomain

```csharp
public string LastDomain { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L45)

Домен, на котором заказан последний email (может отличаться от запрошенного при откате).

## Methods

### Balance

```csharp
public string Balance()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L376)

### Cancel

```csharp
public void Cancel()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L330)

Отменить активацию.

### CheapestDomains

```csharp
public List<string> CheapestDomains(string site, params string[] exclude)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L77)

Домены с count &gt; 0: по цене, при равной цене — у кого больше ящиков.

### GetHrefs

```csharp
public List<string> GetHrefs(int deadline = 60)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L218)

### GetLastMessages

```csharp
public string GetLastMessages(string subject = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L364)

Получить последние сообщения (за 40 мин) для долгосрочного ящика.

### GetMail

```csharp
public string GetMail(int deadline = 120)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L154)

Ждать письмо. Возвращает HTML тела.

### Href

```csharp
public string Href(int hrefIndex = 0, int deadline = 60)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L206)

### LinkByRegex

```csharp
public string LinkByRegex(string urlPattern)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L306)

Извлечь ссылку из письма по паттерну.

### NewMail

```csharp
public string[] NewMail(string site, string domain = "outlook.com", bool fallback = true, int maxFallback = 5)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L112)

Заказать временный email. Возвращает [id, email].

| Parameter | Description |
|---|---|
| `site` | Сайт, например "instagram.com" |
| `domain` | Домен: "mailcom", "gmx", "hotmail", "outlook" (или через запятую) |
| `fallback` | На ответ "no emails" взять список доменов (/email/quantity) и заказать на самом дешёвом из доступных (count &gt; 0). Итоговый домен — LastDomain. |
| `maxFallback` | Сколько доменов из списка пробовать. |

### OrderLongLive

```csharp
public string[] OrderLongLive(string site, string domain)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L346)

Купить долгосрочный почтовый ящик. Возвращает первый email из списка: [id, email, imapPass, imapHost, imapPort].

| Parameter | Description |
|---|---|
| `site` | Сайт, например "instagram.com" |
| `domain` | Домен, например "hotmail.com" |

### Otp

```csharp
public string Otp(int matchIndex = 0)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L177)

Получить OTP (6 цифр) из письма.

### Quantity

```csharp
public Dictionary<string, (int Count, double Price)> Quantity(string site)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L51)

Доступные домены для сайта: domain -&gt; (count, price). Ответ /email/quantity: {"status":"success","data":{"gmx.com":{"count":..,"price":..},...}}.

### Reorder

```csharp
public string[] Reorder()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L315)

Перезаказать тот же email (новый id).

> This page is generated from the source code. Do not edit it: changes will be overwritten.
