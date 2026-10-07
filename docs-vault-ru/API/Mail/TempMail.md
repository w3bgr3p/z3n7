---
title: "TempMail"
tags: [api, Mail]
generated: z3n7-docgen
---

# TempMail

`class` · пространство имён `z3n7.Api` · исходник [Mail/TempMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L19)

```csharp
public class TempMail
```

Клиент сервиса Temp Mail (Privatix) на RapidAPI. Id ящика — MD5 адреса; хранится в `tempMailId`.

## Конструкторы

### TempMail

```csharp
public TempMail(IZennoPosterProjectModel project, string apikey, bool log = false, bool useNetHttp = false, string proxy = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L35)

Создаёт клиент.

| Параметр | Описание |
|---|---|
| `apikey` | Ключ RapidAPI; обязателен. |
| `log` | Писать запросы и ответы в лог. |
| `useNetHttp` | Отправлять запросы через `NetHttp`, а не через HTTP-клиент ZennoPoster. |
| `proxy` | Прокси в формате `Rqst`; пусто — без прокси. |

## Методы

### CreateAddress

```csharp
public static string CreateAddress(string login, string domain)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L216)

Склеивает логин и домен в адрес.

### GetDomains

```csharp
public string[] GetDomains()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L68)

Домены, которые предлагает сервис.

**Возвращает:** Бросает исключение, если список пуст.

### GetHrefs

```csharp
public HashSet<string> GetHrefs(int deadline = 120)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L177)

Ждёт письмо и собирает ссылки из его HTML, пропуская якоря, `mailto:`, `tel:`, `javascript:`, `data:` и ссылки на картинки, стили, скрипты и шрифты.

| Параметр | Описание |
|---|---|
| `deadline` | Сколько секунд ждать; потом `TimeoutException`. |

### GetMail

```csharp
public string GetMail(int deadline = 120)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L129)

Ждёт письмо, проверяя каждые 5 секунд.

| Параметр | Описание |
|---|---|
| `deadline` | Сколько секунд ждать; потом `TimeoutException`. |

**Возвращает:** JSON первого письма.

### GetMessages

```csharp
public string GetMessages()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L114)

Текущие письма ящика из `tempMailId` (иначе из `mailId`), без ожидания.

**Возвращает:** Сырой ответ в JSON. Бросает исключение, если ящик не создан.

### HashEmail

```csharp
public static string HashEmail(string email)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L227)

MD5 адреса в нижнем регистре, в hex: id ящика, который использует сервис.

### Link

```csharp
public string Link(string urlPattern, int deadline = 120)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L206)

Ждёт письмо и возвращает первое совпадение `urlPattern` в его JSON с декодированием HTML.

| Параметр | Описание |
|---|---|
| `urlPattern` | Регулярное выражение. |
| `deadline` | Сколько секунд ждать; потом `TimeoutException`. |

**Возвращает:** Совпадение. Бросает исключение, если совпадения нет.

### NewMail

```csharp
public string[] NewMail(string login = null, string domain = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L87)

Строит адрес на домене сервиса; запроса на создание нет, сервис принимает почту на любой логин. Сохраняет id в `tempMailId` и `mailId`, адрес — в `email` и `project.Profile.Email`.

| Параметр | Описание |
|---|---|
| `login` | Локальная часть; если пусто — 10 случайных hex-символов. |
| `domain` | Домен; если пусто — случайный домен сервиса. |

**Возвращает:** `[md5, email]`.

### Otp

```csharp
public string Otp(int deadline = 120)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/TempMail.cs#L154)

Ждёт письмо и возвращает первое 6-значное число из темы, иначе из текста.

| Параметр | Описание |
|---|---|
| `deadline` | Сколько секунд ждать; потом `TimeoutException`. |

**Возвращает:** Код. Бросает исключение, если кода нет.

