---
title: "PropertyManager"
tags: [api, Accounts]
generated: z3n7-docgen
---

# PropertyManager

`static class` · пространство имён `z3n7.Utilities` · исходник [Accounts/PropertyManager.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L7)

```csharp
public static class PropertyManager
```

*Описания пока нет.*

## Методы

### GetTypeProperties

```csharp
public static List<string> GetTypeProperties(Type type, bool requireSetter = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L10)

```csharp
public static List<string> GetTypeProperties(object obj)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L33)

### GetValuesByProperty

```csharp
public static Dictionary<string, string> GetValuesByProperty(this IZennoPosterProjectModel project, object obj, List<string> propertyList = null, string tableToUpd = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L37)

### SetValuesFromDb

```csharp
public static void SetValuesFromDb(this IZennoPosterProjectModel project, object obj, string table = "profile", List<string> propertyList = null, string key = "id", object id = null, string where = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L69)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
