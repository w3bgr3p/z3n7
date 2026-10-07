---
title: "StringExtensions"
tags: [api, MethodExtensions]
generated: z3n7-docgen
---

# StringExtensions

`static class` · пространство имён `z3n7` · исходник [MethodExtensions/StringExtentions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L17)

```csharp
public static class StringExtensions
```

Методы расширения для строк: hex, Base64, JSON, диапазоны, экранирование Markdown, JWT, пароли.

## Методы

### CleanFilePath

```csharp
public static string CleanFilePath(this string text)
```

Метод расширения для `string`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L297)

Удаляет символы, недопустимые в именах файлов.

### ConvertUrl

```csharp
public static string ConvertUrl(this string url, bool oneline = false)
```

Метод расширения для `string`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L189)

Показывает параметры запроса из URL, по одному на строку. Если есть параметр `addEthereumChainParameter` с JSON, возвращает этот JSON.

| Параметр | Описание |
|---|---|
| `url` | URL. |
| `oneline` | Вывести всё одной строкой. |

**Возвращает:** Текст или сообщение, начинающееся с `Error:`.

### EscapeMarkdown

```csharp
public static string EscapeMarkdown(this string text)
```

Метод расширения для `string`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L345)

Экранирует обратной косой чертой спецсимволы Telegram MarkdownV2.

### FromBase64

```csharp
public static string FromBase64(this string base64Cookies)
```

Метод расширения для `string`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L106)

Декодирует Base64 в UTF-8; для пустого входа — пусто; если вход не Base64, возвращает его без изменений.

### GetFileNameFromUrl

```csharp
public static string GetFileNameFromUrl(string input, bool withExtension = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L316)

Имя файла из URL или из атрибута `src`/`href` во фрагменте HTML.

| Параметр | Описание |
|---|---|
| `input` | URL или фрагмент HTML. |
| `withExtension` | Оставить расширение. |

**Возвращает:** Имя файла или вход без изменений, если имя не найдено.

### HexToString

```csharp
public static string HexToString(this string hexValue, string convert = "")
```

Метод расширения для `string`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L66)

Переводит hex-число (с `0x` или без) в десятичный текст, при необходимости уменьшая масштаб.

| Параметр | Описание |
|---|---|
| `hexValue` | Hex-число. |
| `convert` | `gwei` (×10⁹), `eth` (×10¹⁸) или пусто для числа как есть. |

**Возвращает:** Число; `0` для пустого или неверного входа.

### JsonToDic

```csharp
public static Dictionary<string, string> JsonToDic(this string json, bool ignoreEmpty = true)
```

Метод расширения для `string`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L133)

Разворачивает JSON-объект: вложенные ключи соединяются через `_`, элементы массивов получают свой индекс (`a_b_0`).

| Параметр | Описание |
|---|---|
| `json` | JSON-объект. |
| `ignoreEmpty` | Пропускать пустые значения. |

### NewPassword

```csharp
public static string NewPassword(int length = 16, bool includeDigits = true, bool randomizeCase = true, bool includeSymbols = true)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L458)

Случайный пароль из строчных букв и выбранных групп; в нём есть хотя бы один символ каждой выбранной группы.

| Параметр | Описание |
|---|---|
| `length` | Длина. |
| `includeDigits` | Включать цифры. |
| `randomizeCase` | Включать заглавные буквы. |
| `includeSymbols` | Включать `!@#$%^&*()`. |

**Возвращает:** Пароль. Бросает исключение, если длина меньше 1 или слишком мала для выбранных групп.

### ParseJwt

```csharp
public static Dictionary<string, object> ParseJwt(this string jwt)
```

Метод расширения для `string`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L364)

Декодирует JWT без проверки подписи.

**Возвращает:** `alg`, `typ`, `kid`, `iss`, `sub`, `aud`, `iat`/`exp` с датами, `ttl_seconds`, `is_expired`, сырой JSON заголовка и payload и подпись; или `error`.

### Range

```csharp
public static string[] Range(this string accRange)
```

Метод расширения для `string`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L273)

Разворачивает диапазон аккаунтов: `1,4,7` как есть, `1-10` — в каждое число, одно число `n` — в `1…n`.

**Возвращает:** Числа строками. Для пустого входа бросает исключение.

### StringToHex

```csharp
public static string StringToHex(this string value, string convert = "")
```

Метод расширения для `string`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L27)

Переводит десятичное число в hex-строку с `0x`, при необходимости сначала масштабируя его.

| Параметр | Описание |
|---|---|
| `value` | Число в инвариантной культуре. |
| `convert` | `gwei` (×10⁹), `eth` (×10¹⁸) или пусто для числа как есть. |

**Возвращает:** Hex-значение; `0x0` для пустого или неверного входа.

### ToBase64

```csharp
public static string ToBase64(this string cookiesJson)
```

Метод расширения для `string`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L96)

Текст в Base64 (UTF-8); для пустого входа — пусто.

