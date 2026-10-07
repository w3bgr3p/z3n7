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

Помощники для `HtmlElement` ZennoPoster: точка центра, распознавание QR, XPath.

## Методы

### Center

```csharp
public static Point Center(this HtmlElement element, Point origin)
```

Метод расширения для `HtmlElement`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/HtmlExtensions.cs#L19)

Центр элемента относительно `origin` (по размеру bounding client rect, если он известен, иначе по размеру элемента).

| Параметр | Описание |
|---|---|
| `origin` | Левый верхний угол элемента. |

**Возвращает:** Точка. Бросает исключение, если элемент null или пустой.

### DecodeQr

```csharp
public static string DecodeQr(this HtmlElement element)
```

Метод расширения для `HtmlElement`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/HtmlExtensions.cs#L35)

Отрисовывает элемент и распознаёт QR-код с картинки (ZXing).

**Возвращает:** Распознанный текст или одно из `elementZeroSize`, `bitmapIsNull`, `qrIsNull`, или сообщение исключения. Исключений не бросает.

### GetXPath

```csharp
public static string GetXPath(this HtmlElement element)
```

Метод расширения для `HtmlElement`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/HtmlExtensions.cs#L62)

Строит XPath элемента, поднимаясь до `body`. На каждом шаге берётся `@id`, иначе первый класс, иначе `@name`, иначе позиция среди соседей с тем же тегом.

**Возвращает:** XPath, начинающийся с `//*`; пусто для пустого элемента.

### VerifyXPath

```csharp
public static bool VerifyXPath(Tab tab, HtmlElement originalElement, string xpath)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/HtmlExtensions.cs#L141)

Проверяет, что у первого элемента, найденного по `xpath` в `tab`, такой же outer HTML, как у `originalElement`.

