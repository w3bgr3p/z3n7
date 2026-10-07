---
title: "Helper"
tags: [api, Tools]
generated: z3n7-docgen
---

# Helper

`static class` · namespace `z3n7` · source [Tools/Helper.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Helper.cs#L13)

```csharp
public static class Helper
```

Developer aids shown as Windows forms inside ZennoPoster.

## Methods

### Help

```csharp
public static void Help(this IZennoPosterProjectModel project, string toSearch = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Helper.cs#L368)

Opens a searchable API browser window. The index is built once per process from the XML documentation files next to the ZennoPoster executable and, by reflection, from the public members of the loaded `ZennoLab*` assemblies. Every search term must match the member's name, type or summary.

| Parameter | Description |
|---|---|
| `toSearch` | Initial search text. |

