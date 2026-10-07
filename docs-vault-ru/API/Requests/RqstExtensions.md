---
title: "RqstExtensions"
tags: [api, Requests]
generated: z3n7-docgen
---

# RqstExtensions

`static class` · пространство имён `z3n7` · исходник [Requests/Rqst.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L799)

```csharp
public static class RqstExtensions
```

Сокращения, которые создают `Rqst` для одного запроса.

## Методы

### DELETE

```csharp
public static string DELETE(this IZennoPosterProjectModel project, string url, string proxy = "", string[] headers = null, string cookies = null, bool log = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L962)

Отправляет DELETE-запрос новым `Rqst`; `log` заодно включает его логирование.

| Параметр | Описание |
|---|---|
| `url` | URL запроса. |
| `proxy` | Пусто — без прокси. `+` — переменная `proxy`, иначе колонка `proxy` строки аккаунта в `_instance`; `z` — колонка `z_proxy` этой строки; иначе `[scheme://][user:pass@]host:port` (схема по умолчанию http). |
| `headers` | Строки вида `Name: value`. Если пусто, берутся строки переменной `headers`. Строки User-Agent и Content-Type задают эти значения (по умолчанию user agent профиля и `application/json`); Host, Connection, Content-Length и подобные транспортные заголовки отбрасываются. |
| `cookies` | `-` — без кук. Любой другой текст отправляется как заголовок Cookie. Пусто — куки для домена URL из переменной `cookies` (JSON-массив); если она пуста, а `acc0` и `dbSource` заданы, — из колонки `cookies` в Base64 строки `_instance` (заодно сохраняются в переменную); если ничего не найдено — контейнер кук профиля. |
| `log` | Писать тело ответа в лог. Работает, только если `Rqst` создан с `log: true`. |
| `deadline` | Таймаут в секундах. |
| `thrw` | Бросать исключение при статусе не 2xx или транспортной ошибке. |
| `useNetHttp` | Отправлять через `NetHttp` (.NET HttpClient), а не через HTTP-клиент ZennoPoster. |
| `returnSuccessWithStatus` | Возвращать `{status}\r\n\r\n{body}` для любого статуса, без обработки статусов не 2xx. |

**Возвращает:** Тело ответа без пробелов по краям. При статусе не 2xx — тело (или исключение при `thrw`). При транспортной ошибке — `Error: {message}` (или исключение при `thrw`).

### GET

```csharp
public static string GET(this IZennoPosterProjectModel project, string url, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L834)

Отправляет GET-запрос новым `Rqst`; `log` заодно включает его логирование.

| Параметр | Описание |
|---|---|
| `url` | URL запроса. |
| `proxy` | Пусто — без прокси. `+` — переменная `proxy`, иначе колонка `proxy` строки аккаунта в `_instance`; `z` — колонка `z_proxy` этой строки; иначе `[scheme://][user:pass@]host:port` (схема по умолчанию http). |
| `headers` | Строки вида `Name: value`. Если пусто, берутся строки переменной `headers`. Строки User-Agent и Content-Type задают эти значения (по умолчанию user agent профиля и `application/json`); Host, Connection, Content-Length и подобные транспортные заголовки отбрасываются. |
| `cookies` | `-` — без кук. Любой другой текст отправляется как заголовок Cookie. Пусто — куки для домена URL из переменной `cookies` (JSON-массив); если она пуста, а `acc0` и `dbSource` заданы, — из колонки `cookies` в Base64 строки `_instance` (заодно сохраняются в переменную); если ничего не найдено — контейнер кук профиля. |
| `log` | Писать тело ответа в лог. Работает, только если `Rqst` создан с `log: true`. |
| `parse` | Загрузить тело ответа в `project.Json`. |
| `deadline` | Таймаут в секундах. |
| `thrw` | Бросать исключение при статусе не 2xx или транспортной ошибке. |
| `useNetHttp` | Отправлять через `NetHttp` (.NET HttpClient), а не через HTTP-клиент ZennoPoster. |
| `returnSuccessWithStatus` | Возвращать `{status}\r\n\r\n{body}` для любого статуса, без обработки статусов не 2xx. |
| `bodyOnly` | Запрашивать у ZennoPoster только тело; заголовки ответа тогда не записываются. |

**Возвращает:** Тело ответа без пробелов по краям. При статусе не 2xx — тело (или исключение при `thrw`). При транспортной ошибке — `Error: {message}` (или исключение при `thrw`).

### POST

```csharp
public static string POST(this IZennoPosterProjectModel project, string url, string body, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L878)

Отправляет POST-запрос новым `Rqst`; `log` заодно включает его логирование.

| Параметр | Описание |
|---|---|
| `url` | URL запроса. |
| `body` | Тело запроса. |
| `proxy` | Пусто — без прокси. `+` — переменная `proxy`, иначе колонка `proxy` строки аккаунта в `_instance`; `z` — колонка `z_proxy` этой строки; иначе `[scheme://][user:pass@]host:port` (схема по умолчанию http). |
| `headers` | Строки вида `Name: value`. Если пусто, берутся строки переменной `headers`. Строки User-Agent и Content-Type задают эти значения (по умолчанию user agent профиля и `application/json`); Host, Connection, Content-Length и подобные транспортные заголовки отбрасываются. |
| `cookies` | `-` — без кук. Любой другой текст отправляется как заголовок Cookie. Пусто — куки для домена URL из переменной `cookies` (JSON-массив); если она пуста, а `acc0` и `dbSource` заданы, — из колонки `cookies` в Base64 строки `_instance` (заодно сохраняются в переменную); если ничего не найдено — контейнер кук профиля. |
| `log` | Писать тело ответа в лог. Работает, только если `Rqst` создан с `log: true`. |
| `parse` | Загрузить тело ответа в `project.Json`. |
| `deadline` | Таймаут в секундах. |
| `thrw` | Бросать исключение при статусе не 2xx или транспортной ошибке. |
| `useNetHttp` | Отправлять через `NetHttp` (.NET HttpClient), а не через HTTP-клиент ZennoPoster. |
| `returnSuccessWithStatus` | Возвращать `{status}\r\n\r\n{body}` для любого статуса, без обработки статусов не 2xx. |
| `bodyOnly` | Запрашивать у ZennoPoster только тело; заголовки ответа тогда не записываются. |

**Возвращает:** Тело ответа без пробелов по краям. При статусе не 2xx — тело (или исключение при `thrw`). При транспортной ошибке — `Error: {message}` (или исключение при `thrw`).

### PUT

```csharp
public static string PUT(this IZennoPosterProjectModel project, string url, string body, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L921)

Отправляет PUT-запрос новым `Rqst`; `log` заодно включает его логирование.

| Параметр | Описание |
|---|---|
| `url` | URL запроса. |
| `body` | Тело запроса. |
| `proxy` | Пусто — без прокси. `+` — переменная `proxy`, иначе колонка `proxy` строки аккаунта в `_instance`; `z` — колонка `z_proxy` этой строки; иначе `[scheme://][user:pass@]host:port` (схема по умолчанию http). |
| `headers` | Строки вида `Name: value`. Если пусто, берутся строки переменной `headers`. Строки User-Agent и Content-Type задают эти значения (по умолчанию user agent профиля и `application/json`); Host, Connection, Content-Length и подобные транспортные заголовки отбрасываются. |
| `cookies` | `-` — без кук. Любой другой текст отправляется как заголовок Cookie. Пусто — куки для домена URL из переменной `cookies` (JSON-массив); если она пуста, а `acc0` и `dbSource` заданы, — из колонки `cookies` в Base64 строки `_instance` (заодно сохраняются в переменную); если ничего не найдено — контейнер кук профиля. |
| `log` | Писать тело ответа в лог. Работает, только если `Rqst` создан с `log: true`. |
| `parse` | Загрузить тело ответа в `project.Json`. |
| `deadline` | Таймаут в секундах. |
| `thrw` | Бросать исключение при статусе не 2xx или транспортной ошибке. |
| `useNetHttp` | Отправлять через `NetHttp` (.NET HttpClient), а не через HTTP-клиент ZennoPoster. |
| `returnSuccessWithStatus` | Возвращать `{status}\r\n\r\n{body}` для любого статуса, без обработки статусов не 2xx. |

**Возвращает:** Тело ответа без пробелов по краям. При статусе не 2xx — тело (или исключение при `thrw`). При транспортной ошибке — `Error: {message}` (или исключение при `thrw`).

