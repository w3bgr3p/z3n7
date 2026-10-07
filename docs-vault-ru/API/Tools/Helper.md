---
title: "Helper"
tags: [api, Tools]
generated: z3n7-docgen
---

# Helper

`static class` · пространство имён `z3n7` · исходник [Tools/Helper.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Helper.cs#L13)

```csharp
public static class Helper
```

Инструменты разработчика в виде окон Windows внутри ZennoPoster.

## Методы

### Help

```csharp
public static void Help(this IZennoPosterProjectModel project, string toSearch = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Helper.cs#L368)

Открывает окно просмотра API с поиском. Индекс строится один раз за процесс из файлов XML-документации рядом с исполняемым файлом ZennoPoster и, через рефлексию, из публичных членов загруженных сборок `ZennoLab*`. Каждое слово поиска должно совпасть с именем, типом или описанием члена.

| Параметр | Описание |
|---|---|
| `toSearch` | Начальный текст поиска. |

