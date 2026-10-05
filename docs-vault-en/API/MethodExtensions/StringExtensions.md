---
title: "StringExtensions"
tags: [api, MethodExtensions]
generated: z3n7-docgen
---

# StringExtensions

`static class` · namespace `z3n7` · source [MethodExtensions/StringExtentions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L17)

```csharp
public static class StringExtensions
```

Extension methods on strings: hex, Base64, JSON, ranges, Markdown escaping, JWT, passwords.

## Methods

### CleanFilePath

```csharp
public static string CleanFilePath(this string text)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L297)

Removes characters that are not allowed in file names.

### ConvertUrl

```csharp
public static string ConvertUrl(this string url, bool oneline = false)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L189)

Shows the query parameters of a URL, one per line. When there is an `addEthereumChainParameter` parameter with JSON, returns that JSON instead.

| Parameter | Description |
|---|---|
| `url` | URL. |
| `oneline` | Put everything on one line. |

**Returns:** The text, or a message starting with `Error:`.

### EscapeMarkdown

```csharp
public static string EscapeMarkdown(this string text)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L345)

Escapes Telegram MarkdownV2 special characters with a backslash.

### FromBase64

```csharp
public static string FromBase64(this string base64Cookies)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L106)

Decodes UTF-8 Base64; empty for empty input; the input unchanged when it is not Base64.

### GetFileNameFromUrl

```csharp
public static string GetFileNameFromUrl(string input, bool withExtension = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L316)

File name from a URL, or from the `src`/`href` attribute in an HTML fragment.

| Parameter | Description |
|---|---|
| `input` | URL or HTML fragment. |
| `withExtension` | Keep the extension. |

**Returns:** The file name, or the input when none is found.

### HexToString

```csharp
public static string HexToString(this string hexValue, string convert = "")
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L66)

Converts a hex number (with or without `0x`) to decimal text, optionally scaling it down.

| Parameter | Description |
|---|---|
| `hexValue` | Hex number. |
| `convert` | `gwei` (×10⁹), `eth` (×10¹⁸), or empty for the plain number. |

**Returns:** The number; `0` for empty or invalid input.

### JsonToDic

```csharp
public static Dictionary<string, string> JsonToDic(this string json, bool ignoreEmpty = true)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L133)

Flattens a JSON object: nested keys are joined with `_`, array items get their index (`a_b_0`).

| Parameter | Description |
|---|---|
| `json` | JSON object. |
| `ignoreEmpty` | Leave out empty values. |

### NewPassword

```csharp
public static string NewPassword(int length = 16, bool includeDigits = true, bool randomizeCase = true, bool includeSymbols = true)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L458)

Random password from lowercase letters plus the selected groups; at least one character of each selected group is included.

| Parameter | Description |
|---|---|
| `length` | Length. |
| `includeDigits` | Include digits. |
| `randomizeCase` | Include uppercase letters. |
| `includeSymbols` | Include `!@#$%^&*()`. |

**Returns:** The password. Throws when the length is below 1 or too small for the selected groups.

### ParseJwt

```csharp
public static Dictionary<string, object> ParseJwt(this string jwt)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L364)

Decodes a JWT without checking its signature.

**Returns:** `alg`, `typ`, `kid`, `iss`, `sub`, `aud`, `iat`/`exp` with dates, `ttl_seconds`, `is_expired`, raw header and payload JSON and the signature; or `error`.

### Range

```csharp
public static string[] Range(this string accRange)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L273)

Expands an account range: `1,4,7` as is, `1-10` to every number, a single number `n` to `1…n`.

**Returns:** The numbers as strings. Throws for empty input.

### StringToHex

```csharp
public static string StringToHex(this string value, string convert = "")
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L27)

Converts a decimal number to a `0x` hex string, optionally scaling it first.

| Parameter | Description |
|---|---|
| `value` | Number in invariant culture. |
| `convert` | `gwei` (×10⁹), `eth` (×10¹⁸), or empty for the plain number. |

**Returns:** The hex value; `0x0` for empty or invalid input.

### ToBase64

```csharp
public static string ToBase64(this string cookiesJson)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L96)

UTF-8 Base64 of the text; empty for empty input.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
