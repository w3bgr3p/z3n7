---
title: "ZennoBrowser"
tags: [api, Api]
generated: z3n7-docgen
---

# ZennoBrowser

`static class` · пространство имён `z3n7` · исходник [Api/ZB.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L12)

```csharp
public static class ZennoBrowser
```

ZennoBrowser (ZP8) profiles: their ids and running the helper project `ZB.zp`.

## Методы

### ZB

```csharp
public static bool ZB(this IZennoPosterProjectModel project, string toDo)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L67)

Stores `toDo` in the `toDo` variable and runs `{project.Path}/.internal/ZB.zp`, passing `acc0`, `cfgLog`, `cfgPin`, `DBmode`, `DBpstgrPass`, `DBpstgrUser`, `DBsqltPath`, `instancePort`, `lastQuery`, `cookies`, `varSessionId` and `toDo` by name.

| Параметр | Описание |
|---|---|
| `toDo` | Command for the helper project. |

**Возвращает:** The result of `ExecuteProject`.

### ZBids

```csharp
public static Dictionary<string, string> ZBids(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L21)

Reads `id` and `name` of every ZennoBrowser profile except `template` from the `ProfileInfos` table of `%LOCALAPPDATA%\ZennoLab\ZP8\.zp8\ProfileManagement.db`. The file is read directly; project variables are not touched.

**Возвращает:** Profile id → profile name. Throws when the ZennoBrowser database file is missing.

