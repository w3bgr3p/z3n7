---
title: "CookieCollector"
tags: [api, Browser]
generated: z3n7-docgen
---

# CookieCollector

`class` · пространство имён `z3n7` · исходник [Browser/CookieCollector.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L15)

```csharp
public class CookieCollector
```

Собирает куки, обходя сайты по обычному HTTP (не браузером), начиная с имеющегося набора кук, и возвращает их в JSON в формате браузерных расширений.

## Свойства

### Accept

```csharp
public string Accept { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L39)

Заголовок Accept запросов.

### AcceptLanguage

```csharp
public string AcceptLanguage { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L43)

Заголовок Accept-Language запросов.

### AllowRedirects

```csharp
public bool AllowRedirects { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L24)

Следовать редиректам (до 10).

### Log

```csharp
public Action<string> Log { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L47)

Получает `COOKIES={count}` в конце `Run`; `null` — ничего.

### MaxCookieAgeDays

```csharp
public int MaxCookieAgeDays { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L28)

Верхняя граница выдуманного возраста новых кук, в днях.

### MaxLastAccessAgeDays

```csharp
public int MaxLastAccessAgeDays { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L30)

Выдуманные даты последнего обращения у новых кук попадают в столько дней до текущего момента.

### MinCookieAgeDays

```csharp
public int MinCookieAgeDays { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L26)

Нижняя граница выдуманного возраста новых кук, в днях.

### TimeoutSeconds

```csharp
public int TimeoutSeconds { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L22)

Таймаут каждого запроса, секунды.

### UserAgent

```csharp
public string UserAgent { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L33)

Заголовок User-Agent запросов.

## Методы

### Run

```csharp
public string Run(IEnumerable<string> services, string cookiesJson, string proxy = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/CookieCollector.cs#L63)

Загружает `cookiesJson`, отправляет GET на каждый сервис и сохраняет все куки затронутых доменов. Сервис, который не ответил вовремя или упал, пропускается. Куки без `creationDate` получают случайные даты создания и последнего обращения в пределах заданных возрастов.

| Параметр | Описание |
|---|---|
| `services` | URL или имена хостов; если нет `https://`, он добавляется. |
| `cookiesJson` | Начальные куки: JSON-массив с `name`, `value`, `domain`, `path`, `secure`, `httpOnly`, `expirationDate` и необязательными `creationDate`/`lastAccessDate`. |
| `proxy` | `[scheme://][user:pass@]host:port`; пусто — без прокси. |

**Возвращает:** JSON-массив кук в том же формате.

