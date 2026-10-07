---
title: "NetHttp"
tags: [api, Requests]
generated: z3n7-docgen
---

# NetHttp

`class` · пространство имён `z3n7` · исходник [Requests/NetHttp.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L657)

```csharp
public class NetHttp
```

Блокирующая обёртка над `NetHttpAsync` для C#-кубиков, в которых нельзя использовать await.

## Конструкторы

### NetHttp

```csharp
public NetHttp(IZennoPosterProjectModel project, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L663)

Создаёт клиент.

| Параметр | Описание |
|---|---|
| `log` | Логгер для запросов, ответов и ошибок; `null` — ничего не писать. |

## Методы

### DELETE

```csharp
public string DELETE(string url, string proxyString = "", Dictionary<string, string> headers = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L772)

Отправляет DELETE-запрос и ждёт его. См. `NetHttpAsync.DeleteAsync`.

**Возвращает:** Тело без пробелов по краям или сообщение об ошибке.

### GET

```csharp
public string GET(string url, string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L685)

Отправляет GET-запрос и ждёт его. См. `NetHttpAsync.GetAsync`.

| Параметр | Описание |
|---|---|
| `url` | URL запроса. |
| `proxyString` | Пусто — прямой запрос. `+` — колонка `proxy` строки аккаунта в `_instance`; иначе `[scheme://][user:pass@]host:port`. Прокси всегда используется как HTTP-прокси. |
| `headers` | Дополнительные заголовки, отправляются вместе с user agent профиля; транспортные заголовки вроде Host и Content-Length пропускаются. |
| `parse` | Загрузить тело ответа в `project.Json`. |
| `deadline` | Таймаут в секундах; сам клиент никогда не ждёт дольше 30 с. |
| `throwOnFail` | Бросать исключение при статусе не 2xx или ошибке, а не возвращать сообщение. |

**Возвращает:** Тело без пробелов по краям. При статусе не 2xx: `{code} !!! {reason}`; при таймауте `Timeout: …`; при других ошибках `Error: …`.

### POST

```csharp
public string POST(string url, string body, string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L720)

Отправляет POST-запрос с телом в JSON и ждёт его. См. `NetHttpAsync.PostAsync`.

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

### PUT

```csharp
public string PUT(string url, string body = "", string proxyString = "", Dictionary<string, string> headers = null, bool parse = false, int deadline = 15, bool throwOnFail = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L754)

Отправляет PUT-запрос и ждёт его. См. `NetHttpAsync.PutAsync`.

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

