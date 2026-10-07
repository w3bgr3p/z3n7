---
title: "BestMailBox"
tags: [api, Mail]
generated: z3n7-docgen
---

# BestMailBox

`class` · пространство имён `z3n7.Api` · исходник [Mail/BestMailBox.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L16)

```csharp
public class BestMailBox
```

Клиент сервиса временных ящиков BestMailBox (по умолчанию `https://mail.autoz3n.xyz`). Ответ, у которого `success` не true, приводит к исключению с ошибкой сервиса.

## Конструкторы

### BestMailBox

```csharp
public BestMailBox(IZennoPosterProjectModel project, string apikey = null, string baseUrl = null, bool useNetHttp = false, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L36)

Создаёт клиент.

| Параметр | Описание |
|---|---|
| `apikey` | API-ключ; по умолчанию `BESTMAILBOX_API_KEY` из `.env` проекта. Бросает исключение, если не задан ни тот, ни другой. |
| `baseUrl` | URL сервиса; по умолчанию `BESTMAILBOX_BASE_URL` из `.env` проекта, иначе встроенный. |
| `useNetHttp` | Отправлять запросы через `NetHttp`, а не через HTTP-клиент ZennoPoster. |
| `log` | Писать запросы и ответы в лог. |

## Методы

### DeleteMail

```csharp
public bool DeleteMail(string id = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L257)

Удаляет ящик и его письма.

| Параметр | Описание |
|---|---|
| `id` | Id или адрес ящика; по умолчанию переменная `mailId`, затем `bestMailId`, затем `email`. |

**Возвращает:** `true`, если сервис подтвердил; `false` при любой ошибке или если id нет.

### GetDomains

```csharp
public List<string> GetDomains()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L278)

Домены, которые предлагает сервис.

### GetHrefs

```csharp
public HashSet<string> GetHrefs(int deadline = 60, string id = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L191)

Каждые 2,5 секунды запрашивает последнее письмо и возвращает его ссылки: ссылки подтверждения от сервиса, иначе все ссылки из HTML-тела. Якоря, `mailto:`, `tel:`, `javascript:`, `data:` и ссылки на картинки, стили, скрипты и шрифты пропускаются.

| Параметр | Описание |
|---|---|
| `deadline` | Сколько секунд ждать; потом `TimeoutException`. |
| `id` | Id или адрес ящика; по умолчанию переменная `mailId`, затем `bestMailId`, затем `email`. |

### GetMail

```csharp
public string GetMail(int deadline = 60, string id = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L154)

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

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L90)

Создаёт ящик. Сохраняет id в `mailId` и `bestMailId`, адрес — в `email` и `project.Profile.Email`.

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

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/BestMailBox.cs#L120)

Каждые 2,5 секунды спрашивает у сервиса одноразовый код, найденный сервисом в ящике.

| Параметр | Описание |
|---|---|
| `deadline` | Сколько секунд ждать; потом `TimeoutException`. |
| `id` | Id или адрес ящика; по умолчанию переменная `mailId`, затем `bestMailId`, затем `email`. |

**Возвращает:** Код.

