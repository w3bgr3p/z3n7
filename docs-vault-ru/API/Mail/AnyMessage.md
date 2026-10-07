---
title: "AnyMessage"
tags: [api, Mail]
generated: z3n7-docgen
---

# AnyMessage

`class` · пространство имён `z3n7.Api` · исходник [Mail/AnyMessage.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L21)

```csharp
public class AnyMessage
```

Клиент почтового сервиса AnyMessage (`api.anymessage.shop`): краткосрочные и долгосрочные ящики. Состояние хранится в переменных проекта: `anyMailId` — текущий краткосрочный заказ, `anyLLId` — долгосрочный. Ответ, у которого `status` не `success`, приводит к исключению с сообщением сервиса.

## Конструкторы

### AnyMessage

```csharp
public AnyMessage(IZennoPosterProjectModel project, string apikey, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L30)

Создаёт клиент.

| Параметр | Описание |
|---|---|
| `apikey` | API-токен AnyMessage. |
| `log` | Писать запросы и ответы в лог. |

## Свойства

### LastDomain

```csharp
public string LastDomain { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L55)

Домен последнего заказанного ящика; после замены может отличаться от запрошенного.

## Методы

### Balance

```csharp
public string Balance()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L421)

Баланс аккаунта в том виде, в каком его вернул сервис.

### Cancel

```csharp
public void Cancel()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L372)

Отменяет заказ из `anyMailId`.

### CheapestDomains

```csharp
public List<string> CheapestDomains(string site, params string[] exclude)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L90)

Домены, где есть ящики, сначала самые дешёвые; при равной цене — тот, где ящиков больше.

| Параметр | Описание |
|---|---|
| `site` | Целевой сайт. |
| `exclude` | Какие домены исключить. |

### GetHrefs

```csharp
public List<string> GetHrefs(int deadline = 60)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L252)

Ждёт письмо и собирает его уникальные ссылки, пропуская якоря, `mailto:`, `tel:`, `javascript:`, `data:` и ссылки на картинки, стили, скрипты и шрифты.

| Параметр | Описание |
|---|---|
| `deadline` | Сколько секунд ждать письма. |

### GetLastMessages

```csharp
public string GetLastMessages(string subject = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L408)

Последние письма долгосрочного ящика из `anyLLId`.

| Параметр | Описание |
|---|---|
| `subject` | Если задано — только письма с этой темой. |

**Возвращает:** Сырой ответ в JSON.

### GetMail

```csharp
public string GetMail(int deadline = 120)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L175)

Ждёт письмо в ящик из `anyMailId`, проверяя каждые 5 секунд.

| Параметр | Описание |
|---|---|
| `deadline` | Сколько секунд ждать; потом `TimeoutException`. |

**Возвращает:** Тело письма (HTML).

### Href

```csharp
public string Href(int hrefIndex = 0, int deadline = 60)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L235)

Возвращает одну ссылку из письма (см. `GetHrefs`).

| Параметр | Описание |
|---|---|
| `hrefIndex` | Индекс ссылки; -1 пишет все ссылки в лог и возвращает пустую строку. |
| `deadline` | Сколько секунд ждать письма. |

### LinkByRegex

```csharp
public string LinkByRegex(string urlPattern)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L344)

Ждёт письмо и возвращает первое совпадение `urlPattern` в его HTML. Если совпадения нет, бросает исключение.

| Параметр | Описание |
|---|---|
| `urlPattern` | Регулярное выражение. |

### NewMail

```csharp
public string[] NewMail(string site, string domain = "outlook.com", bool fallback = true, int maxFallback = 5)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L131)

Заказывает краткосрочный ящик. Сохраняет id в `anyMailId`, адрес — в `email` и `project.Profile.Email`.

| Параметр | Описание |
|---|---|
| `site` | Целевой сайт, например `instagram.com`. |
| `domain` | Домен ящика. |
| `fallback` | Если сервис отвечает «no emails», пробовать самые дешёвые доступные домены. Использованный домен попадает в `LastDomain`. |
| `maxFallback` | Сколько запасных доменов пробовать. |

**Возвращает:** `[id, email]`. Если ничего заказать не удалось, бросает исключение с ошибками всех попыток.

### OrderLongLive

```csharp
public string[] OrderLongLive(string site, string domain)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L388)

Покупает долгосрочный ящик. Сохраняет id в `anyLLId`, адрес — в `anyLLEmail`.

| Параметр | Описание |
|---|---|
| `site` | Целевой сайт, например `instagram.com`. |
| `domain` | Домен ящика, например `hotmail.com`. |

**Возвращает:** `[id, email, imapPassword, imapHost, imapPort]` первого ящика в ответе.

### Otp

```csharp
public string Otp(int matchIndex = 0)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L203)

Ждёт письмо и достаёт 6-значный код; сохраняет его в `mailOtp`.

| Параметр | Описание |
|---|---|
| `matchIndex` | Какое по счёту 6-значное число из письма брать; -1 пишет их все в лог и возвращает пустую строку. |

**Возвращает:** Код. Бросает исключение, если за 10 попыток код не найден.

### Quantity

```csharp
public Dictionary<string, (int Count, double Price)> Quantity(string site)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L60)

Доступные домены для сайта (`/email/quantity`).

| Параметр | Описание |
|---|---|
| `site` | Целевой сайт, например `instagram.com`. |

**Возвращает:** Домен → (доступно ящиков, цена). Домены без цены не попадают.

### Reorder

```csharp
public string[] Reorder()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/AnyMessage.cs#L357)

Заново заказывает ящик из `anyMailId` под новым id; обновляет `anyMailId` и `email`.

**Возвращает:** `[id, email]`.

