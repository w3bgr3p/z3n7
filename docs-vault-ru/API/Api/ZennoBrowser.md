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

Профили ZennoBrowser (ZP8): их id и запуск вспомогательного проекта `ZB.zp`.

## Методы

### ZB

```csharp
public static bool ZB(this IZennoPosterProjectModel project, string toDo)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L67)

Записывает `toDo` в переменную `toDo` и запускает `{project.Path}/.internal/ZB.zp`, передавая по имени `acc0`, `cfgLog`, `cfgPin`, `DBmode`, `DBpstgrPass`, `DBpstgrUser`, `DBsqltPath`, `instancePort`, `lastQuery`, `cookies`, `varSessionId` и `toDo`.

| Параметр | Описание |
|---|---|
| `toDo` | Команда для вспомогательного проекта. |

**Возвращает:** Результат `ExecuteProject`.

### ZBids

```csharp
public static Dictionary<string, string> ZBids(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/ZB.cs#L21)

Читает `id` и `name` всех профилей ZennoBrowser, кроме `template`, из таблицы `ProfileInfos` файла `%LOCALAPPDATA%\ZennoLab\ZP8\.zp8\ProfileManagement.db`. Файл читается напрямую; переменные проекта не трогаются.

**Возвращает:** Id профиля → имя профиля. Бросает исключение, если файла базы ZennoBrowser нет.

