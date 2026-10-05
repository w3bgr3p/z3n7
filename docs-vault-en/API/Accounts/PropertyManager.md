---
title: "PropertyManager"
tags: [api, Accounts]
generated: z3n7-docgen
---

# PropertyManager

`static class` · namespace `z3n7.Utilities` · source [Accounts/PropertyManager.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L7)

```csharp
public static class PropertyManager
```

*No description yet.*

## Methods

### GetTypeProperties

```csharp
public static List<string> GetTypeProperties(Type type, bool requireSetter = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L10)

```csharp
public static List<string> GetTypeProperties(object obj)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L33)

### GetValuesByProperty

```csharp
public static Dictionary<string, string> GetValuesByProperty(this IZennoPosterProjectModel project, object obj, List<string> propertyList = null, string tableToUpd = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L37)

### SetValuesFromDb

```csharp
public static void SetValuesFromDb(this IZennoPosterProjectModel project, object obj, string table = "profile", List<string> propertyList = null, string key = "id", object id = null, string where = "")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L69)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
