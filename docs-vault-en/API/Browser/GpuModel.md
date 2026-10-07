---
title: "GpuModel"
tags: [api, Browser]
generated: z3n7-docgen
---

# GpuModel

`class` · namespace `z3n7` · source [Browser/GpuSpoof.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L17)

```csharp
public class GpuModel
```

One GPU model from the PCI ID list.

## Properties

### Chip

```csharp
public string Chip { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L24)

Chip code, e.g. `GA104`; empty when the list does not give one.

### DeviceId

```csharp
public string DeviceId { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L20)

PCI device id, 4 hex digits, e.g. `2486`.

### Name

```csharp
public string Name { get; set; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L22)

Model name, e.g. `GeForce RTX 3060 Ti`.

