---
title: "Img"
tags: [api, Tools]
generated: z3n7-docgen
---

# Img

`class` · пространство имён `z3n7` · исходник [Tools/Img.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Img.cs#L8)

```csharp
public class Img
```

SVG rendering.

## Методы

### DrawSvgAsBase64

```csharp
public static string DrawSvgAsBase64(string svgContent)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Img.cs#L26)

Renders SVG markup to PNG.

| Параметр | Описание |
|---|---|
| `svgContent` | SVG markup. |

**Возвращает:** The PNG as Base64.

### ImgFromSvg

```csharp
public static void ImgFromSvg(string svgContent, string pathToScreen)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Img.cs#L14)

Renders SVG markup to an image file; the format follows the file extension.

| Параметр | Описание |
|---|---|
| `svgContent` | SVG markup. |
| `pathToScreen` | Target file. |

