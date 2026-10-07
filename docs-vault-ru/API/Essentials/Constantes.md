---
title: "Constantes"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Constantes

`static class` · пространство имён `z3n7` · исходник [Essentials/Vars.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L492)

```csharp
public static class Constantes
```

Project name, its database table and the standard folders of the profile storage.

## Методы

### FullPath

```csharp
public static string FullPath(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L536)

Returns the full path of the project file.

### PathCookies

```csharp
public static string PathCookies(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L573)

Returns `{profiles}/accounts/cookies/{acc0}.json`, or an empty string with a warning when `acc0` is empty.

### PathProfileFolder

```csharp
public static string PathProfileFolder(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L587)

Returns `{profiles}/accounts/profilesFolder/{acc0}`, or an empty string with a warning when `acc0` is empty.

### PathProfiles

```csharp
public static string PathProfiles(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L548)

Returns the profile storage root: the `profiles_folder` variable, else the global variable of the same name. Whichever is found is copied to the other one.

**Примечания:** Throws when neither is set.

### ProjectName

```csharp
public static string ProjectName(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L497)

Returns the project file name up to the first dot and stores it in `projectName`.

### ProjectTable

```csharp
public static string ProjectTable(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L528)

Returns `__` + project name and stores it in `projectTable`.

### SecureVar

```csharp
public static string SecureVar(this IZennoPosterProjectModel project, string key)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L605)

Reads a value from the encrypted `jVars` variable: decrypts it with `SAFU.DecryptHWID`, decodes Base64 and looks the key up in the resulting JSON object.

**Возвращает:** The value, or an empty string when `jVars` is empty, cannot be decrypted, or has no such key.

