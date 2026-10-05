---
title: "StringExtensions"
tags: [api, MethodExtensions]
generated: z3n7-docgen
---

# StringExtensions

`static class` · namespace `z3n7` · source [MethodExtensions/StringExtentions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L16)

```csharp
public static class StringExtensions
```

*No description yet.*

## Methods

### CleanFilePath

```csharp
public static string CleanFilePath(this string text)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L265)

### ConvertUrl

```csharp
public static string ConvertUrl(this string url, bool oneline = false)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L163)

### EscapeMarkdown

```csharp
public static string EscapeMarkdown(this string text)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L308)

### FromBase64

```csharp
public static string FromBase64(this string base64Cookies)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L93)

### GetFileNameFromUrl

```csharp
public static string GetFileNameFromUrl(string input, bool withExtension = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L280)

### HexToString

```csharp
public static string HexToString(this string hexValue, string convert = "")
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L55)

### JsonToDic

```csharp
public static Dictionary<string, string> JsonToDic(this string json, bool ignoreEmpty = true)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L114)

### NewPassword

```csharp
public static string NewPassword(int length = 16, bool includeDigits = true, bool randomizeCase = true, bool includeSymbols = true)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L406)

### ParseJwt

```csharp
public static Dictionary<string, object> ParseJwt(this string jwt)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L321)

### Range

```csharp
public static string[] Range(this string accRange)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L242)

### StringToHex

```csharp
public static string StringToHex(this string value, string convert = "")
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L22)

### ToBase64

```csharp
public static string ToBase64(this string cookiesJson)
```

Extension method for `string`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/MethodExtensions/StringExtentions.cs#L84)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
