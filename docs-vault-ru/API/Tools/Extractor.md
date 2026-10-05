---
title: "Extractor"
tags: [api, Tools]
generated: z3n7-docgen
---

# Extractor

`static class` · пространство имён `z3n7.Tools` · исходник [Tools/Extractor.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L11)

```csharp
public static class Extractor
```

*Описания пока нет.*

## Методы

### BuildZpFromXml

```csharp
public static void BuildZpFromXml(this IZennoPosterProjectModel project, string xml = null, string zpPath = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L121)

### ExtractInputSettingsHtml

```csharp
public static string ExtractInputSettingsHtml(string zpPath)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L46)

### ExtractXml

```csharp
public static string ExtractXml(string zpPath)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L16)

### SaveAsXml

```csharp
public static void SaveAsXml(this IZennoPosterProjectModel project, string xmlPath = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L38)

### SaveInputSettingsHtml

```csharp
public static void SaveInputSettingsHtml(string zpPath, string html)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L69)

```csharp
public static void SaveInputSettingsHtml(string zpPath, string html, string outputZpPath)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L74)

### SearchInZp

```csharp
public static List<SearchHit> SearchInZp(this IZennoPosterProjectModel project, string text, string folder = null, bool recursive = true, bool cache = true, int padding = 60)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L197)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
