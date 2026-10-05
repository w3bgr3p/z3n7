---
title: "HtmlExtensions"
tags: [api, Browser]
generated: z3n7-docgen
---

# HtmlExtensions

`static class` · namespace `z3n7` · source [Browser/HtmlExtensions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/HtmlExtensions.cs#L10)

```csharp
public static class HtmlExtensions
```

Helpers for ZennoPoster `HtmlElement`: centre point, QR decoding, XPath.

## Methods

### Center

```csharp
public static Point Center(this HtmlElement element, Point origin)
```

Extension method for `HtmlElement`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/HtmlExtensions.cs#L19)

Centre of the element relative to `origin` (bounding-client size when known, else the element size).

| Parameter | Description |
|---|---|
| `origin` | Top-left corner of the element. |

**Returns:** The point. Throws when the element is null or void.

### DecodeQr

```csharp
public static string DecodeQr(this HtmlElement element)
```

Extension method for `HtmlElement`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/HtmlExtensions.cs#L35)

Draws the element and decodes a QR code from the picture (ZXing).

**Returns:** The decoded text, or one of `elementZeroSize`, `bitmapIsNull`, `qrIsNull`, or an exception message. Never throws.

### GetXPath

```csharp
public static string GetXPath(this HtmlElement element)
```

Extension method for `HtmlElement`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/HtmlExtensions.cs#L62)

Builds an XPath for the element by walking up to `body`. Each step uses `@id`, else the first class, else `@name`, else the position among same-tag siblings.

**Returns:** The XPath, starting with `//*`; empty for a void element.

### VerifyXPath

```csharp
public static bool VerifyXPath(Tab tab, HtmlElement originalElement, string xpath)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/HtmlExtensions.cs#L141)

Checks that the first element found by `xpath` in `tab` has the same outer HTML as `originalElement`.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
