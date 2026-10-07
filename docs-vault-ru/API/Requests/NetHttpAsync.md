---
title: "NetHttpAsync"
tags: [api, Requests]
generated: z3n7-docgen
---

# NetHttpAsync

`class` · пространство имён `z3n7` · исходник [Requests/NetHttp.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L22)

```csharp
public class NetHttpAsync
```

HTTP-клиент на .NET `HttpClient` с асинхронными методами. Клиенты общие: один для прямых запросов и по одному на каждую строку прокси (кешируется до 100). Значения Set-Cookie из ответа пишутся в переменную `debugCookies`.

## Конструкторы

### NetHttpAsync

```csharp
public NetHttpAsync(IZennoPosterProjectModel project, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L43)

Создаёт клиент. Ставит инвариантную культуру текущему потоку.

| Параметр | Описание |
|---|---|
| `log` | Логгер для запросов, ответов и ошибок; `null` — ничего не писать. |

## Методы

### ClearProxyCache

```csharp
public static void ClearProxyCache()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L639)

Освобождает и забывает все закешированные прокси-клиенты.

### DeleteAsync

```csharp
public async Task<string> DeleteAsync(string url, string proxyString = "", Dictionary<string, string> headers = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L535)

Отправляет DELETE-запрос с таймаутом 30 секунд.

| Параметр | Описание |
|---|---|
| `url` | URL запроса. |
| `proxyString` | Пусто — прямой запрос. `+` — колонка `proxy` строки аккаунта в `_instance`; иначе `[scheme://][user:pass@]host:port`. Прокси всегда используется как HTTP-прокси. |
| `headers` | Дополнительные заголовки; user agent профиля отправляется, если не задан `User-Agent`. |

**Возвращает:** Тело без пробелов по краям или сообщение об ошибке. Исключений не бросает.

### GetAsync

```csharp
public async Task<string> GetAsync(string url, string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L174)

Отправляет GET-запрос.

| Параметр | Описание |
|---|---|
| `url` | URL запроса. |
| `proxyString` | Пусто — прямой запрос. `+` — колонка `proxy` строки аккаунта в `_instance`; иначе `[scheme://][user:pass@]host:port`. Прокси всегда используется как HTTP-прокси. |
| `headers` | Дополнительные заголовки, отправляются вместе с user agent профиля; транспортные заголовки вроде Host и Content-Length пропускаются. |
| `parse` | Загрузить тело ответа в `project.Json`. |
| `deadline` | Таймаут в секундах; сам клиент никогда не ждёт дольше 30 с. |
| `throwOnFail` | Бросать исключение при статусе не 2xx или ошибке, а не возвращать сообщение. |

**Возвращает:** Тело без пробелов по краям. При статусе не 2xx: `{code} !!! {reason}`; при таймауте `Timeout: …`; при других ошибках `Error: …`.

### PostAsync

```csharp
public async Task<string> PostAsync(string url, string body, string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L310)

Отправляет POST-запрос с телом в JSON (`application/json; charset=UTF-8`).

| Параметр | Описание |
|---|---|
| `url` | URL запроса. |
| `body` | Тело в JSON. |
| `proxyString` | Пусто — прямой запрос. `+` — колонка `proxy` строки аккаунта в `_instance`; иначе `[scheme://][user:pass@]host:port`. Прокси всегда используется как HTTP-прокси. |
| `headers` | Дополнительные заголовки, отправляются вместе с user agent профиля; транспортные заголовки вроде Host и Content-Length пропускаются. |
| `parse` | Загрузить тело ответа в `project.Json`. |
| `deadline` | Таймаут в секундах; сам клиент никогда не ждёт дольше 30 с. |
| `throwOnFail` | Бросать исключение при статусе не 2xx или ошибке, а не возвращать сообщение. |

**Возвращает:** Тело без пробелов по краям. При статусе не 2xx: `{code} !!! {reason}`; при таймауте `Timeout: …`; при других ошибках `Error: …`.

### PutAsync

```csharp
public async Task<string> PutAsync(string url, string body = "", string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L423)

Отправляет PUT-запрос; непустое тело отправляется как JSON.

| Параметр | Описание |
|---|---|
| `url` | URL запроса. |
| `body` | Тело в JSON; может быть пустым. |
| `proxyString` | Пусто — прямой запрос. `+` — колонка `proxy` строки аккаунта в `_instance`; иначе `[scheme://][user:pass@]host:port`. Прокси всегда используется как HTTP-прокси. |
| `headers` | Дополнительные заголовки, отправляются вместе с user agent профиля; транспортные заголовки вроде Host и Content-Length пропускаются. |
| `parse` | Загрузить тело ответа в `project.Json`. |
| `deadline` | Таймаут в секундах; сам клиент никогда не ждёт дольше 30 с. |
| `throwOnFail` | Бросать исключение при статусе не 2xx или ошибке, а не возвращать сообщение. |

**Возвращает:** Тело без пробелов по краям. При статусе не 2xx: `{code} !!! {reason}`; при таймауте `Timeout: …`; при других ошибках `Error: …`.

