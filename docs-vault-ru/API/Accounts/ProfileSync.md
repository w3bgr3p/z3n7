---
title: "ProfileSync"
tags: [api, Accounts]
generated: z3n7-docgen
---

# ProfileSync

`class` · пространство имён `z3n7.Utilities` · исходник [Accounts/ProfileSync.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L10)

```csharp
public class ProfileSync
```

*Описания пока нет.*

## Конструкторы

### ProfileSync

```csharp
public ProfileSync(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L16)

## Методы

### AddStructureToDb

```csharp
public void AddStructureToDb(bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L131)

### RestoreProfile

```csharp
public void RestoreProfile(string restoreFrom, bool restoreProfile = true, bool restoreCookies = true, bool restoreInstance = true, bool restoreWebgl = true, bool rebuildWebgl = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L23)

### SaveProfile

```csharp
public void SaveProfile(string saveTo, bool saveProfile = true, bool saveInstance = true, bool saveCookies = true, bool saveWebgl = true)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L79)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
