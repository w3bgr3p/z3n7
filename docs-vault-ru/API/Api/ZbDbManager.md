---
title: "ZbDbManager"
tags: [api, Api]
generated: z3n7-docgen
---

# ZbDbManager

`static class` · пространство имён `z3n7` · исходник [Api/ZB.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L84)

```csharp
public static class ZbDbManager
```

Чтение базы профилей ZennoBrowser и разбор её списков профилей.

## Методы

### ZBDbGet

```csharp
public static string ZBDbGet(this IZennoPosterProjectModel project, string query, string tableName = "ProfileInfos", bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L94)

Читает колонки `query` профиля, id которого лежит в переменной `zb_id`, напрямую из `%LOCALAPPDATA%\ZennoLab\ZP8\.zp8\ProfileManagement.db`.

| Параметр | Описание |
|---|---|
| `query` | Имена колонок через запятую. |
| `tableName` | Таблица. |
| `log` | Записать запрос и его результат в лог. |

**Возвращает:** Колонки через `¦`; пусто, если такого профиля нет.

### ZBIdDic

```csharp
public static Dictionary<string, string> ZBIdDic(this IZennoPosterProjectModel project, string json, string folder = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L110)

Сопоставляет имена профилей с id по JSON-массиву профилей ZennoBrowser (`Name`, `Id`, `FolderName`); при повторе имени побеждает первый.

| Параметр | Описание |
|---|---|
| `json` | JSON-массив профилей. |
| `folder` | Только профили этой папки; пусто — все. |

### ZBIdList

```csharp
public static List<string> ZBIdList(this IZennoPosterProjectModel project, string json, string folder = "Farm")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L128)

Id профилей папки из JSON-массива профилей (см. `ZBIdDic`).

| Параметр | Описание |
|---|---|
| `json` | JSON-массив профилей. |
| `folder` | Имя папки. |

