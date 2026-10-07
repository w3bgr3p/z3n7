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

Имя проекта, его таблица в базе и стандартные папки хранилища профилей.

## Методы

### FullPath

```csharp
public static string FullPath(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L536)

Возвращает полный путь к файлу проекта.

### PathCookies

```csharp
public static string PathCookies(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L573)

Возвращает `{profiles}/accounts/cookies/{acc0}.json` или пустую строку с предупреждением, если `acc0` пуст.

### PathProfileFolder

```csharp
public static string PathProfileFolder(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L587)

Возвращает `{profiles}/accounts/profilesFolder/{acc0}` или пустую строку с предупреждением, если `acc0` пуст.

### PathProfiles

```csharp
public static string PathProfiles(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L548)

Возвращает корень хранилища профилей: переменная `profiles_folder`, иначе глобальная переменная с тем же именем. Найденное значение копируется во вторую.

**Примечания:** Бросает исключение, если не задано ни то, ни другое.

### ProjectName

```csharp
public static string ProjectName(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L497)

Возвращает имя файла проекта до первой точки и сохраняет его в `projectName`.

### ProjectTable

```csharp
public static string ProjectTable(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L528)

Возвращает `__` + имя проекта и сохраняет его в `projectTable`.

### SecureVar

```csharp
public static string SecureVar(this IZennoPosterProjectModel project, string key)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L605)

Читает значение из зашифрованной переменной `jVars`: расшифровывает через `SAFU.DecryptHWID`, декодирует Base64 и ищет ключ в получившемся JSON-объекте.

**Возвращает:** Значение или пустая строка, если `jVars` пуст, не расшифровывается или в нём нет такого ключа.

