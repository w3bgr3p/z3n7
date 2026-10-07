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

Отрисовка SVG.

## Методы

### DrawSvgAsBase64

```csharp
public static string DrawSvgAsBase64(string svgContent)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Img.cs#L26)

Отрисовывает SVG-разметку в PNG.

| Параметр | Описание |
|---|---|
| `svgContent` | SVG-разметка. |

**Возвращает:** PNG в Base64.

### ImgFromSvg

```csharp
public static void ImgFromSvg(string svgContent, string pathToScreen)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Img.cs#L14)

Отрисовывает SVG-разметку в файл картинки; формат определяется расширением файла.

| Параметр | Описание |
|---|---|
| `svgContent` | SVG-разметка. |
| `pathToScreen` | Целевой файл. |

