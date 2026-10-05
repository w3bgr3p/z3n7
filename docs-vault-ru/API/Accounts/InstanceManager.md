---
title: "InstanceManager"
tags: [api, Accounts]
generated: z3n7-docgen
---

# InstanceManager

`class` · пространство имён `z3n7` · исходник [Accounts/InstanceManager.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L15)

```csharp
public class InstanceManager
```

*Описания пока нет.*

## Конструкторы

### InstanceManager

```csharp
public InstanceManager(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L24)

## Методы

### Cleanup

```csharp
public void Cleanup()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L469)

### Initialize

```csharp
public void Initialize(string browserToLaunch = null, bool fixTimezone = false, bool useLegacy = true, bool useZpprofile = false, bool useFolder = true)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L37)

### ProxySet

```csharp
public bool ProxySet(string proxyString = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L365)

### SaveProfile

```csharp
public void SaveProfile(bool saveCookies = true, bool saveProfile = true, string saveTo = "folder", bool saveZpProfile = true)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L428)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
