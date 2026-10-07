---
title: "Cookies"
tags: [api, Browser]
generated: z3n7-docgen
---

# Cookies

`static class` · пространство имён `z3n7` · исходник [Browser/Cookies.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L19)

```csharp
public static class Cookies
```

Куки браузера: чтение и запись в инстансе, хранение в Base64 в строке аккаунта в базе, перевод между форматами JSON и Netscape.

## Методы

### AnalyzeCookies

```csharp
public static CookieInfo AnalyzeCookies(this IZennoPosterProjectModel project, string table = "_instance", string column = "cookies")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L44)

Читает сохранённые куки текущего аккаунта и сводит их.

| Параметр | Описание |
|---|---|
| `table` | Таблица с куками аккаунта. |
| `column` | Колонка с куками в Base64 (JSON или Netscape). |

**Возвращает:** Сводка; если ничего не сохранено — нулевые счётчики.

### CleanDomainInDb

```csharp
public static void CleanDomainInDb(this IZennoPosterProjectModel project, string domain, string table = "_instance", string column = "cookies")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L489)

Удаляет из сохранённого набора текущего аккаунта куки домена (а для кук, сохранённых с точкой в начале, — и его поддоменов).

| Параметр | Описание |
|---|---|
| `domain` | Домен, например `x.com`. |
| `table` | Таблица с куками аккаунта. |
| `column` | Колонка с куками в Base64 (JSON или Netscape). |

### ConvertCookieFormat

```csharp
public static string ConvertCookieFormat(string input, string output = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L167)

Переводит куки между JSON (формат браузерных расширений) и Netscape (через табуляцию). Текст, начинающийся с `[` или `{`, считается JSON; текст с табуляциями — Netscape.

| Параметр | Описание |
|---|---|
| `input` | Куки. |
| `output` | `json`, `netscape` или пусто — перевести в другой формат. |

**Возвращает:** Преобразованный текст или вход без изменений, если он уже в нужном формате. При неизвестном формате бросает исключение.

### GetCookies

```csharp
public static string GetCookies(this Instance instance, string domainFilter = null, string format = "json")
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L311)

Читает куки инстанса.

| Параметр | Описание |
|---|---|
| `domainFilter` | Только куки этого домена; `.` — основной домен активной вкладки; пусто — все. |
| `format` | `json`, `netscape`, `base64Json` или `base64Netscape`. |

### GetCookiesByJs

```csharp
public static string GetCookiesByJs(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L393)

Читает `document.cookie` активной страницы в виде JSON (путь `/`, без срока действия; куки HttpOnly скриптам не видны).

### ParseJwt

```csharp
public static Dictionary<string, object> ParseJwt(string jwt)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L593)

Декодирует JWT без проверки подписи.

**Возвращает:** `alg`, `typ`, `kid`, `iss`, `sub`, `aud`, `iat`/`exp` с датами, `ttl_seconds`, `is_expired`, сырой JSON заголовка и payload и подпись; или `error`.

### PrintCookieReport

```csharp
public static void PrintCookieReport(this IZennoPosterProjectModel project, string table = "_instance", string column = "cookies")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L132)

Пишет в лог сводку `AnalyzeCookies` (10 главных доменов, 5 самых больших кук).

| Параметр | Описание |
|---|---|
| `table` | Таблица с куками аккаунта. |
| `column` | Колонка с куками в Base64 (JSON или Netscape). |

### PruneAllCookies

```csharp
public static void PruneAllCookies(this IZennoPosterProjectModel project, bool removeExpired = true, bool removeOld = true, bool removeNonGoogle = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L114)

Выполняет `PruneCookies` для таблиц `_instance` и `folder_profile` по каждому аккаунту от `rangeStart` до `rangeEnd`, затем очищает `acc0`.

| Параметр | Описание |
|---|---|
| `removeExpired` | Удалить истёкшие куки. |
| `removeOld` | Удалить куки, истёкшие больше 6 месяцев назад. |
| `removeNonGoogle` | Оставить только куки, у которых домен содержит `google`. |

### PruneCookies

```csharp
public static void PruneCookies(this IZennoPosterProjectModel project, bool removeExpired = true, bool removeOld = true, bool removeNonGoogle = false, string table = "_instance", string column = "cookies")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L75)

Удаляет куки из сохранённого набора текущего аккаунта и записывает его обратно в виде JSON в Base64.

| Параметр | Описание |
|---|---|
| `removeExpired` | Удалить истёкшие куки. |
| `removeOld` | Удалить куки, истёкшие больше 6 месяцев назад. |
| `removeNonGoogle` | Оставить только куки, у которых домен содержит `google`. |
| `table` | Таблица с куками аккаунта. |
| `column` | Колонка с куками в Base64 (JSON или Netscape). |

### SaveAllCookies

```csharp
public static void SaveAllCookies(this IZennoPosterProjectModel project, Instance instance, string jsonPath = null, string table = "_instance", bool saveJsonToDb = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L337)

Записывает все куки инстанса в Base64 в колонку `cookies` строки текущего аккаунта.

| Параметр | Описание |
|---|---|
| `instance` | Инстанс браузера. |
| `jsonPath` | Заодно записать куки в этот файл в виде JSON. |
| `table` | Целевая таблица. |
| `saveJsonToDb` | Сохранять в JSON, а не в Netscape. |

### SaveDomainCookies

```csharp
public static void SaveDomainCookies(this IZennoPosterProjectModel project, Instance instance, string domain = null, string jsonPath = null, string tableName = "_instance", bool saveJsonToDb = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L357)

Записывает куки инстанса одного домена в Base64 в колонку `cookies` строки текущего аккаунта.

| Параметр | Описание |
|---|---|
| `instance` | Инстанс браузера. |
| `domain` | Домен; по умолчанию основной домен активной вкладки. |
| `jsonPath` | Заодно записать куки в этот файл в виде JSON. |
| `tableName` | Целевая таблица. |
| `saveJsonToDb` | Сохранять в JSON, а не в Netscape. |

### SetCookiesByJs

```csharp
public static void SetCookiesByJs(this Instance instance, string cookiesJson)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L428)

Ставит куки домена активной вкладки через `document.cookie` — на родительский домен, с Secure и сроком в год, если сохранённая дата уже прошла.

| Параметр | Описание |
|---|---|
| `cookiesJson` | JSON-массив кук; при повторе домена и имени побеждает последняя, другие домены пропускаются. |

