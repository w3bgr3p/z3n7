---
title: "Img"
tags: [api, Tools]
generated: z3n7-docgen
---

# Img

`class` · namespace `z3n7` · source [Tools/Img.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Img.cs#L8)

```csharp
public class Img
```

SVG rendering.

## Methods

### DrawSvgAsBase64

```csharp
public static string DrawSvgAsBase64(string svgContent)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Img.cs#L26)

Renders SVG markup to PNG.

| Parameter | Description |
|---|---|
| `svgContent` | SVG markup. |

**Returns:** The PNG as Base64.

### ImgFromSvg

```csharp
public static void ImgFromSvg(string svgContent, string pathToScreen)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Img.cs#L14)

Renders SVG markup to an image file; the format follows the file extension.

| Parameter | Description |
|---|---|
| `svgContent` | SVG markup. |
| `pathToScreen` | Target file. |

