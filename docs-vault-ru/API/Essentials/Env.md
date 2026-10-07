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

Читает настройки из файла `.env`.

## Методы

### ReadEnv

```csharp
public static string ReadEnv(this IZennoPosterProjectModel project, string key, bool global = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Env.cs#L23)

Возвращает значение `key` из файла `.env` или `null`, если нет файла или ключа. Строки вида `KEY=value`; пустые строки и строки, начинающиеся с `#`, пропускаются. Ключ сравнивается без учёта регистра; одинарные или двойные кавычки вокруг значения убираются.

| Параметр | Описание |
|---|---|
| `key` | Имя настройки. |
| `global` | false — файл рядом с проектом (`project.Path`). true — файл рядом с `z3n7.dll`. |

**Возвращает:** Значение или `null`.

