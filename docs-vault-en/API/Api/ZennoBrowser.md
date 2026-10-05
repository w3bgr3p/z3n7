---
title: "ZennoBrowser"
tags: [api, Api]
generated: z3n7-docgen
---

# ZennoBrowser

`static class` · namespace `z3n7` · source [Api/ZB.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L12)

```csharp
public static class ZennoBrowser
```

ZennoBrowser (ZP8) profiles: their ids and running the helper project `ZB.zp`.

## Methods

### ZB

```csharp
public static bool ZB(this IZennoPosterProjectModel project, string toDo)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L77)

Stores `toDo` in the `toDo` variable and runs `{project.Path}/.internal/ZB.zp`, passing `acc0`, `cfgLog`, `cfgPin`, `DBmode`, `DBpstgrPass`, `DBpstgrUser`, `DBsqltPath`, `instancePort`, `lastQuery`, `cookies`, `varSessionId` and `toDo` by name.

| Parameter | Description |
|---|---|
| `toDo` | Command for the helper project. |

**Returns:** The result of `ExecuteProject`.

### ZBids

```csharp
public static Dictionary<string, string> ZBids(this IZennoPosterProjectModel project)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L25)

Reads `id` and `name` of every ZennoBrowser profile except `template` from the `ProfileInfos` table. While reading, the `DBmode` and `DBsqltPath` variables point at `%LOCALAPPDATA%\ZennoLab\ZP8\.zp8\ProfileManagement.db`; they are restored afterwards.

**Returns:** Profile id → profile name. Throws when the ZennoBrowser database file is missing.

**Remarks:** The read goes through `DbGetLines`/`DbGet` → `DbQ`, which picks the database by `dbSource` and does not consult `DBmode` or `DBsqltPath`.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
