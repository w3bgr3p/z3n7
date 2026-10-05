---
title: "Extension"
tags: [api, Browser]
generated: z3n7-docgen
---

# Extension

`class` · пространство имён `z3n7` · исходник [Browser/ChromeExt.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L16)

```csharp
public class Extension
```

*Описания пока нет.*

## Конструкторы

### Extension

```csharp
public Extension(IZennoPosterProjectModel project, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L28)

```csharp
public Extension(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L35)

## Методы

### GetVer

```csharp
public string GetVer(string extId)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L42)

### InstallFromCrx

```csharp
public bool InstallFromCrx(string extId, string fileName, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L120)

### InstallFromStore

```csharp
public bool InstallFromStore(string url, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L78)

### Rm

```csharp
public void Rm(string[] ExtToRemove)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L216)

### Switch

```csharp
public bool Switch(string toUse = "", bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L146)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
