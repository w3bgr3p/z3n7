---
title: "PropertyManager"
tags: [api, Accounts]
generated: z3n7-docgen
---

# PropertyManager

`static class` · namespace `z3n7.Utilities` · source [Accounts/PropertyManager.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L9)

```csharp
public static class PropertyManager
```

Copies simple properties of objects to and from database rows (by reflection).

## Methods

### GetTypeProperties

```csharp
public static List<string> GetTypeProperties(Type type, bool requireSetter = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L18)

Names of public readable properties of simple types: primitives, `string`, `decimal`, `DateTime`, enums.

| Parameter | Description |
|---|---|
| `type` | Type to inspect. |
| `requireSetter` | Only properties that also have a public setter. |

```csharp
public static List<string> GetTypeProperties(object obj)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L42)

Same as `GetTypeProperties(obj.GetType())`.

### GetValuesByProperty

```csharp
public static Dictionary<string, string> GetValuesByProperty(this IZennoPosterProjectModel project, object obj, List<string> propertyList = null, string tableToUpd = null)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L55)

Reads property values as text, with single quotes doubled. Properties that fail to read are skipped.

| Parameter | Description |
|---|---|
| `obj` | Source object. |
| `propertyList` | Properties to read; default `GetTypeProperties`. |
| `tableToUpd` | When set, also writes the values to the current account's row of this table (`DicToDb`). |

**Returns:** Property → value.

### SetValuesFromDb

```csharp
public static void SetValuesFromDb(this IZennoPosterProjectModel project, object obj, string table = "profile", List<string> propertyList = null, string key = "id", object id = null, string where = "")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/PropertyManager.cs#L97)

Sets the object's writable properties from a database row, converting text to the property type. Empty values and failed conversions are skipped; other errors are logged as warnings.

| Parameter | Description |
|---|---|
| `obj` | Target object. |
| `table` | Table. |
| `propertyList` | Properties to set; default `GetTypeProperties`. |
| `key` | Column matched against `id`. |
| `id` | Row; default is the current account (`acc0`). |
| `where` | Raw SQL condition instead of `key`/`id`. |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
