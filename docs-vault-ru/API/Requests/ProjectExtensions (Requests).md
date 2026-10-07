---
title: "ProjectExtensions (Requests)"
tags: [api, Requests]
generated: z3n7-docgen
---

# ProjectExtensions (Requests)

`static class` · пространство имён `z3n7` · исходник [Requests/NetHttp.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L786)

```csharp
public static class ProjectExtensions
```

Другие части этого типа: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Traffic)]]

Сокращения для запросов `NetHttp` из проекта.

## Методы

### NetGet

```csharp
public static string NetGet(this IZennoPosterProjectModel project, string url, string proxyString = "", string[] headers = null, bool parse = false, int deadline = 15, bool thrw = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L834)

Отправляет GET-запрос через `NetHttp` без логирования.

| Параметр | Описание |
|---|---|
| `url` | URL запроса. |
| `proxyString` | Пусто — прямой запрос. `+` — колонка `proxy` строки аккаунта в `_instance`; иначе `[scheme://][user:pass@]host:port`. Прокси всегда используется как HTTP-прокси. |
| `headers` | Строки вида `Name: value`. |
| `parse` | Загрузить тело ответа в `project.Json`. |
| `deadline` | Таймаут в секундах; сам клиент никогда не ждёт дольше 30 с. |
| `thrw` | Бросать исключение при статусе не 2xx или ошибке, а не возвращать сообщение. |

**Возвращает:** Тело без пробелов по краям. При статусе не 2xx: `{code} !!! {reason}`; при таймауте `Timeout: …`; при других ошибках `Error: …`.

### NetPost

```csharp
public static string NetPost(this IZennoPosterProjectModel project, string url, string body, string proxyString = "", string[] headers = null, bool parse = false, int deadline = 15, bool thrw = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/NetHttp.cs#L860)

Отправляет POST-запрос с телом в JSON через `NetHttp` без логирования.

| Параметр | Описание |
|---|---|
| `url` | URL запроса. |
| `body` | Тело в JSON. |
| `proxyString` | Пусто — прямой запрос. `+` — колонка `proxy` строки аккаунта в `_instance`; иначе `[scheme://][user:pass@]host:port`. Прокси всегда используется как HTTP-прокси. |
| `headers` | Строки вида `Name: value`. |
| `parse` | Загрузить тело ответа в `project.Json`. |
| `deadline` | Таймаут в секундах; сам клиент никогда не ждёт дольше 30 с. |
| `thrw` | Бросать исключение при статусе не 2xx или ошибке, а не возвращать сообщение. |

**Возвращает:** Тело без пробелов по краям. При статусе не 2xx: `{code} !!! {reason}`; при таймауте `Timeout: …`; при других ошибках `Error: …`.

