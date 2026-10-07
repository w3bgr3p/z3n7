---
title: "Rqst"
tags: [api, Requests]
generated: z3n7-docgen
---

# Rqst

`class` · пространство имён `z3n7` · исходник [Requests/Rqst.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L20)

```csharp
public class Rqst
```

HTTP-клиент для проектов ZennoPoster: прокси, заголовки и куки берутся из проекта, если не заданы. По умолчанию запросы идут через HTTP-клиент ZennoPoster и выполняются по очереди под общим для всех экземпляров `Rqst` замком. Каждый запрос дописывается строкой JSON в файл трафика (`ZpTraffic`).

## Конструкторы

### Rqst

```csharp
public Rqst(IZennoPosterProjectModel project, bool log = false, bool mask = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L31)

Создаёт клиент для проекта.

| Параметр | Описание |
|---|---|
| `log` | Писать в лог ответы и ошибки запросов. Без этого в лог ничего не пишется. |
| `mask` | Не используется. |

## Методы

### DELETE

```csharp
public string DELETE(string url, string proxy = "", string[] headers = null, string cookies = null, bool log = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L226)

Отправляет DELETE-запрос.

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
| `bodyOnly` | Запрашивать у ZennoPoster только тело; заголовки ответа тогда не записываются. |

**Возвращает:** Тело ответа без пробелов по краям. При статусе не 2xx — тело (или исключение при `thrw`). При транспортной ошибке — `Error: {message}` (или исключение при `thrw`).

### GET

```csharp
public string GET(string url, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L100)

Отправляет GET-запрос.

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
public string POST(string url, string body, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L142)

Отправляет POST-запрос.

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
public string PUT(string url, string body, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L185)

Отправляет PUT-запрос.

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

