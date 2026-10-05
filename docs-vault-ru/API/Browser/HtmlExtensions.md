---
title: "HtmlExtensions"
tags: [api, Browser]
generated: z3n7-docgen
---

# HtmlExtensions

`static class` · пространство имён `z3n7` · исходник [Browser/HtmlExtensions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/HtmlExtensions.cs#L10)

```csharp
public static class HtmlExtensions
```

Helpers for ZennoPoster `HtmlElement`: centre point, QR decoding, XPath.

## Методы

### Center

```csharp
public static Point Center(this HtmlElement element, Point origin)
```

Метод расширения для `HtmlElement`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/HtmlExtensions.cs#L19)

Centre of the element relative to `origin` (bounding-client size when known, else the element size).

| Параметр | Описание |
|---|---|
| `origin` | Top-left corner of the element. |

**Возвращает:** The point. Throws when the element is null or void.

### DecodeQr

```csharp
public static string DecodeQr(this HtmlElement element)
```

Метод расширения для `HtmlElement`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/HtmlExtensions.cs#L35)

Draws the element and decodes a QR code from the picture (ZXing).

**Возвращает:** The decoded text, or one of `elementZeroSize`, `bitmapIsNull`, `qrIsNull`, or an exception message. Never throws.

### GetXPath

```csharp
public static string GetXPath(this HtmlElement element)
```

Метод расширения для `HtmlElement`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/HtmlExtensions.cs#L62)

Builds an XPath for the element by walking up to `body`. Each step uses `@id`, else the first class, else `@name`, else the position among same-tag siblings.

**Возвращает:** The XPath, starting with `//*`; empty for a void element.

### VerifyXPath

```csharp
public static bool VerifyXPath(Tab tab, HtmlElement originalElement, string xpath)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/HtmlExtensions.cs#L141)

Checks that the first element found by `xpath` in `tab` has the same outer HTML as `originalElement`.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
