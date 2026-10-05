---
title: "Extractor"
tags: [api, Tools]
generated: z3n7-docgen
---

# Extractor

`static class` · namespace `z3n7.Tools` · source [Tools/Extractor.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L16)

```csharp
public static class Extractor
```

Reading and writing ZennoPoster project files (.zp) through ProjectMaker's own loader, and searching their actions. Works only inside ProjectMaker: it uses the ProjectMaker assembly loaded in the process and does nothing elsewhere.

## Methods

### BuildZpFromXml

```csharp
public static void BuildZpFromXml(this IZennoPosterProjectModel project, string xml = null, string zpPath = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L146)

Builds a .zp file from project XML, using the current project file as the container.

| Parameter | Description |
|---|---|
| `xml` | Project XML; default is the current project's `.xml` next to it. |
| `zpPath` | Target file; default is the project name with a Unix-ms suffix, e.g. `name.1730000000000.zp`. |

### ExtractInputSettingsHtml

```csharp
public static string ExtractInputSettingsHtml(string zpPath)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L58)

Reads the input settings HTML (`InputSettings/inputSettings.html`) of a .zp file.

**Returns:** The HTML without BOM; `null` outside ProjectMaker.

### ExtractXml

```csharp
public static string ExtractXml(string zpPath)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L24)

Unpacks the project XML of a .zp file.

| Parameter | Description |
|---|---|
| `zpPath` | Project file. |

**Returns:** The XML; the loader's exception text when it fails; `null` outside ProjectMaker.

### SaveAsXml

```csharp
public static void SaveAsXml(this IZennoPosterProjectModel project, string xmlPath = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L48)

Unpacks the current project to XML (UTF-16).

| Parameter | Description |
|---|---|
| `xmlPath` | Target file; default is the project file name with `.xml`. |

### SaveInputSettingsHtml

```csharp
public static void SaveInputSettingsHtml(string zpPath, string html)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L82)

Replaces the input settings HTML inside the .zp file itself.

```csharp
public static void SaveInputSettingsHtml(string zpPath, string html, string outputZpPath)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L94)

Writes a copy of the .zp file with the input settings HTML replaced. When the output is the source file, it is written through a temporary file.

| Parameter | Description |
|---|---|
| `zpPath` | Source project. |
| `html` | New input settings HTML. |
| `outputZpPath` | Target project file. |

### SearchInZp

```csharp
public static List<SearchHit> SearchInZp(this IZennoPosterProjectModel project, string text, string folder = null, bool recursive = true, bool cache = true, int padding = 60)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L243)

Finds text in the actions of every .zp file in a folder (case-insensitive, in attributes and values; XML entities are decoded for matching). One hit per action; each hit is also written to the log. Unpacking takes about a second per file, so unpacked XML can be cached in a hidden `.xml` folder; the cache file name holds the project's modification time and size, so a changed project is re-read automatically.

| Parameter | Description |
|---|---|
| `text` | Text to find. |
| `folder` | Folder; default is the project folder. |
| `recursive` | Include subfolders. |
| `cache` | Use the XML cache. |
| `padding` | Characters of context on each side. |

**Remarks:** Works only inside ProjectMaker: it uses the ProjectMaker assembly loaded in the process and does nothing elsewhere.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
