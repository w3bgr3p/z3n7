---
title: "Env"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Env

`static class` · пространство имён `z3n7` · исходник [Essentials/Env.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Env.cs#L8)

```csharp
public static class Env
```

Reads settings from a `.env` file.

## Методы

### ReadEnv

```csharp
public static string ReadEnv(this IZennoPosterProjectModel project, string key, bool global = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Env.cs#L23)

Returns the value of `key` from a `.env` file, or `null` when the file or the key is missing. Lines are `KEY=value`; empty lines and lines starting with `#` are skipped. The key is matched case-insensitively; surrounding single or double quotes are removed from the value.

| Параметр | Описание |
|---|---|
| `key` | Name of the setting. |
| `global` | false: the file next to the project (`project.Path`). true: the file next to `z3n7.dll`. |

**Возвращает:** The value, or `null`.

