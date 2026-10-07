---
title: "ZpToCsx"
tags: [api, Tools]
generated: z3n7-docgen
---

# ZpToCsx

`static class` · namespace `z3n7.Tools` · source [Tools/ZpToCsx.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/ZpToCsx.cs#L16)

```csharp
public static class ZpToCsx
```

Converts a ZennoPoster project (.zp) into a C# script (.csx) outline and builds .zp files from XML. Works only inside ProjectMaker: it uses the ProjectMaker assembly loaded in the process.

## Methods

### ExtractXml

```csharp
public static string ExtractXml(string zpPath)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/ZpToCsx.cs#L26)

Unpacks the project XML of a .zp file.

**Returns:** The XML; the loader's exception text when it fails; `null` outside ProjectMaker.

### GenerateCsx

```csharp
public static string GenerateCsx(string zpPath)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/ZpToCsx.cs#L92)

Generates a C# script from a project: `#r` references, usings, `InitVariables` with the variable defaults, and `Execute` with one labelled block per action in reachability order, `goto` jumps for the branches, and each action's type and parameters as comments.

| Parameter | Description |
|---|---|
| `zpPath` | Project file. |

### Template

```csharp
public static string Template()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/ZpToCsx.cs#L111)

The embedded empty project (.zp) as Base64.

### XmlToZp

```csharp
public static void XmlToZp(string xml, string zpPath)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/ZpToCsx.cs#L53)

Builds a .zp file from project XML, using an empty project embedded in the library as the container.

| Parameter | Description |
|---|---|
| `xml` | Project XML. |
| `zpPath` | Target file. |

