---
title: "z3nmail"
tags: [api, Mail]
generated: z3n7-docgen
---

# z3nmail

`class` · пространство имён `z3n7.Api` · исходник [Mail/z3nmail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L16)

```csharp
public class z3nmail
```

Клиент сервиса временных ящиков (по умолчанию `https://mail.autoz3n.xyz`); API то же, что у `BestMailBox`. Ответ, у которого `success` не true, приводит к исключению с ошибкой сервиса.

## Конструкторы

### z3nmail

```csharp
public z3nmail(IZennoPosterProjectModel project, string apikey = null, string baseUrl = null, bool useNetHttp = false, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L30)

Создаёт клиент.

| Параметр | Описание |
|---|---|
| `apikey` | API-ключ; по умолчанию `Z3NMAIL_API_KEY` из `.env` проекта. |
| `baseUrl` | URL сервиса; по умолчанию `https://mail.autoz3n.xyz`. |
| `useNetHttp` | Отправлять запросы через `NetHttp`, а не через HTTP-клиент ZennoPoster. |
| `log` | Писать запросы и ответы в лог. |

## Методы

### DeleteMail

```csharp
public bool DeleteMail(string id = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L238)

Удаляет ящик и его письма.

| Параметр | Описание |
|---|---|
| `id` | Id или адрес ящика; по умолчанию переменная `mailId`, затем `bestMailId`, затем `email`. |

**Возвращает:** `true`, если сервис подтвердил; `false` при любой ошибке или если id нет.

### GetDomains

```csharp
public List<string> GetDomains()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L259)

Домены, которые предлагает сервис.

### GetHrefs

```csharp
public HashSet<string> GetHrefs(int deadline = 60, string id = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L170)

Каждые 2,5 секунды запрашивает последнее письмо и возвращает его ссылки: ссылки подтверждения от сервиса, иначе все ссылки из HTML-тела. Якоря, `mailto:`, `tel:`, `javascript:`, `data:` и ссылки на картинки, стили, скрипты и шрифты пропускаются.

| Параметр | Описание |
|---|---|
| `deadline` | Сколько секунд ждать; потом `TimeoutException`. |
| `id` | Id или адрес ящика; по умолчанию переменная `mailId`, затем `bestMailId`, затем `email`. |

### GetMail

```csharp
public string GetMail(int deadline = 60, string id = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L133)

Каждые 2,5 секунды запрашивает последнее письмо.

| Параметр | Описание |
|---|---|
| `deadline` | Сколько секунд ждать; потом `TimeoutException`. |
| `id` | Id или адрес ящика; по умолчанию переменная `mailId`, затем `bestMailId`, затем `email`. |

**Возвращает:** HTML-тело или текстовое тело, если HTML нет.

### NewMail

```csharp
public string[] NewMail(string domain = null, string prefix = null, int ttl = 1200)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L70)

Создаёт ящик. Сохраняет id в `mailId`, адрес — в `email` и `project.Profile.Email`.

| Параметр | Описание |
|---|---|
| `domain` | Домен ящика; случайный, если `null`. |
| `prefix` | Локальная часть адреса; генерируется, если `null`. |
| `ttl` | Время жизни ящика в секундах. |

**Возвращает:** `[id, email]`.

### Otp

```csharp
public string Otp(int deadline = 60, string id = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/z3nmail.cs#L99)

Каждые 2,5 секунды спрашивает у сервиса одноразовый код, найденный сервисом в ящике.

| Параметр | Описание |
|---|---|
| `deadline` | Сколько секунд ждать; потом `TimeoutException`. |
| `id` | Id или адрес ящика; по умолчанию переменная `mailId`, затем `bestMailId`, затем `email`. |

**Возвращает:** Код.

