---
title: "GmailClient"
tags: [api, Mail]
generated: z3n7-docgen
---

# GmailClient

`class` · пространство имён `z3n7.Api` · исходник [Mail/GMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L18)

```csharp
public class GmailClient
```

Доступ к Gmail через Gmail API по OAuth refresh token. Учётные данные берутся из таблицы `_api`, строка `id = 'gmail'`: `client_id`, `client_secret`, `refresh_token`. Перед каждой операцией запрашивается новый access token.

## Конструкторы

### GmailClient

```csharp
public GmailClient(IZennoPosterProjectModel project, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L34)

Создаёт клиент и читает учётные данные из базы.

| Параметр | Описание |
|---|---|
| `log` | Писать запросы и ответы в лог. |

## Методы

### GetLink

```csharp
public string GetLink(string targetEmail)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L190)

Возвращает первую ссылку http(s) из текстового тела самого нового из последних 5 писем (за 5 минут), отправленных на `targetEmail`.

| Параметр | Описание |
|---|---|
| `targetEmail` | Адрес, на который должно быть отправлено письмо (сверяется с заголовком `To`). |

**Возвращает:** Ссылка. Бросает исключение, если ссылка не найдена.

### Otp

```csharp
public string Otp(string targetEmail, int maxResults = 10)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L159)

Просматривает письма за последние 5 минут, отправленные на `targetEmail`, и возвращает первое 6-значное число из темы, иначе из текстового тела.

| Параметр | Описание |
|---|---|
| `targetEmail` | Адрес, на который должно быть отправлено письмо (сверяется с заголовком `To`). |
| `maxResults` | Сколько последних писем проверять. |

**Возвращает:** Код. Бросает исключение, если код не найден.

### SendMail

```csharp
public void SendMail(string to, string subject, string body)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/GMail.cs#L224)

Отправляет текстовое письмо из этого ящика.

| Параметр | Описание |
|---|---|
| `to` | Получатель. |
| `subject` | Тема. |
| `body` | Текст. |

