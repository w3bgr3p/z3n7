---
title: "Diagnostic"
tags: [api, Diagnostic]
generated: z3n7-docgen
---

# Diagnostic

`static class` · namespace `z3n7` · source [Diagnostic/Diagnostic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L182)

```csharp
public static class Diagnostic
```

Environment information for logs and diagnostics.

## Methods

### Info

```csharp
public static VersionInfo Info()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L188)

Reads library, ZennoPoster and runtime versions and the machine name. Each field is read separately and a failure leaves only that field empty; never throws.

