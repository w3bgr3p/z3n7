---
title: "AnyMessage"
tags: [api, Mail]
generated: z3n7-docgen
---

# AnyMessage

`class` · пространство имён `z3n7.Api` · исходник [Mail/AnyMessage.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L14)

```csharp
public class AnyMessage
```

*Описания пока нет.*

## Конструкторы

### AnyMessage

```csharp
public AnyMessage(IZennoPosterProjectModel project, string apikey, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L20)

## Свойства

### LastDomain

```csharp
public string LastDomain { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L45)

Домен, на котором заказан последний email (может отличаться от запрошенного при откате).

## Методы

### Balance

```csharp
public string Balance()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L376)

### Cancel

```csharp
public void Cancel()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L330)

Отменить активацию.

### CheapestDomains

```csharp
public List<string> CheapestDomains(string site, params string[] exclude)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L77)

Домены с count &gt; 0: по цене, при равной цене — у кого больше ящиков.

### GetHrefs

```csharp
public List<string> GetHrefs(int deadline = 60)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L218)

### GetLastMessages

```csharp
public string GetLastMessages(string subject = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L364)

Получить последние сообщения (за 40 мин) для долгосрочного ящика.

### GetMail

```csharp
public string GetMail(int deadline = 120)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L154)

Ждать письмо. Возвращает HTML тела.

### Href

```csharp
public string Href(int hrefIndex = 0, int deadline = 60)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L206)

### LinkByRegex

```csharp
public string LinkByRegex(string urlPattern)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L306)

Извлечь ссылку из письма по паттерну.

### NewMail

```csharp
public string[] NewMail(string site, string domain = "outlook.com", bool fallback = true, int maxFallback = 5)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L112)

Заказать временный email. Возвращает [id, email].

| Параметр | Описание |
|---|---|
| `site` | Сайт, например "instagram.com" |
| `domain` | Домен: "mailcom", "gmx", "hotmail", "outlook" (или через запятую) |
| `fallback` | На ответ "no emails" взять список доменов (/email/quantity) и заказать на самом дешёвом из доступных (count &gt; 0). Итоговый домен — LastDomain. |
| `maxFallback` | Сколько доменов из списка пробовать. |

### OrderLongLive

```csharp
public string[] OrderLongLive(string site, string domain)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L346)

Купить долгосрочный почтовый ящик. Возвращает первый email из списка: [id, email, imapPass, imapHost, imapPort].

| Параметр | Описание |
|---|---|
| `site` | Сайт, например "instagram.com" |
| `domain` | Домен, например "hotmail.com" |

### Otp

```csharp
public string Otp(int matchIndex = 0)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L177)

Получить OTP (6 цифр) из письма.

### Quantity

```csharp
public Dictionary<string, (int Count, double Price)> Quantity(string site)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L51)

Доступные домены для сайта: domain -&gt; (count, price). Ответ /email/quantity: {"status":"success","data":{"gmx.com":{"count":..,"price":..},...}}.

### Reorder

```csharp
public string[] Reorder()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L315)

Перезаказать тот же email (новый id).

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
