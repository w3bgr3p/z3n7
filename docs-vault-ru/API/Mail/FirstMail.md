---
title: "FirstMail"
tags: [api, Mail]
generated: z3n7-docgen
---

# FirstMail

`class` · пространство имён `z3n7` · исходник [Mail/FirstMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L19)

```csharp
public class FirstMail
```

Клиент API почты FirstMail (`firstmail.ltd`). API-ключ, логин по умолчанию, пароль и прокси берутся из таблицы `_api`, строка `id = 'firstmail'` (`apikey`, `apisecret`, `passphrase`, `proxy`). Ответы загружаются в `project.Json`. Клиент входит в ящик, на который пересылается почта. Аргумент `email` — исходный получатель: адрес, на который было отправлено письмо и который переслал его в этот ящик.

## Конструкторы

### FirstMail

```csharp
public FirstMail(IZennoPosterProjectModel project, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L44)

Создаёт клиент для ящика, сохранённого в базе.

| Параметр | Описание |
|---|---|
| `log` | Не используется. |

```csharp
public FirstMail(IZennoPosterProjectModel project, string mail, string password, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L56)

Создаёт клиент для заданного ящика; API-ключ и прокси всё равно берутся из базы.

| Параметр | Описание |
|---|---|
| `mail` | Адрес ящика. |
| `password` | Пароль ящика. |
| `log` | Не используется. |

## Методы

### Delete

```csharp
public string Delete(string email, bool seen = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L81)

Вызывает `/v1/mail/delete` для ящика клиента.

| Параметр | Описание |
|---|---|
| `email` | Исходный получатель; не используется. |
| `seen` | Добавить `seen=true` к URL запроса. |

### Get

```csharp
public string Get(int limit = 5)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L110)

Последние письма INBOX ящика клиента (`/api/v1/email/messages`).

| Параметр | Описание |
|---|---|
| `limit` | Сколько. |

**Возвращает:** JSON-массив писем.

### GetAll

```csharp
public string GetAll(string email)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L100)

Письма ящика клиента (`/v1/get/messages`).

| Параметр | Описание |
|---|---|
| `email` | Исходный получатель; не используется. |

### GetLink

```csharp
public string GetLink(string email)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L164)

Берёт последнее письмо; если его первый получатель содержит `email`, возвращает первую ссылку http(s) из его текста.

| Параметр | Описание |
|---|---|
| `email` | Исходный получатель, на которого было отправлено письмо. |

**Возвращает:** Ссылка. Бросает исключение, если ссылки нет.

### GetOne

```csharp
public string GetOne(string email)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L91)

Последнее письмо ящика клиента (`/v1/mail/one`).

| Параметр | Описание |
|---|---|
| `email` | Исходный получатель; не используется. |

### GetOTP

```csharp
public string GetOTP(string email)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L129)

Берёт последнее письмо; если его первый получатель содержит `email`, возвращает первое 6-значное число из темы, текста или HTML.

| Параметр | Описание |
|---|---|
| `email` | Исходный получатель, на которого было отправлено письмо. |

**Возвращает:** Код. Бросает исключение, если последнее письмо адресовано другому адресу или в нём нет кода.

### Otp

```csharp
public string Otp(string email)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L193)

Просматривает последние 5 писем INBOX, ищет отправленное на `email` и возвращает первое 6-значное число из его темы, текста или HTML.

| Параметр | Описание |
|---|---|
| `email` | Исходный получатель, на которого было отправлено письмо. |

**Возвращает:** Код. Бросает исключение, если код не найден.

