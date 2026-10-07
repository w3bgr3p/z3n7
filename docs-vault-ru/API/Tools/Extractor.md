---
title: "Extractor"
tags: [api, Tools]
generated: z3n7-docgen
---

# Extractor

`static class` · пространство имён `z3n7.Tools` · исходник [Tools/Extractor.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L16)

```csharp
public static class Extractor
```

Чтение и запись файлов проектов ZennoPoster (.zp) собственным загрузчиком ProjectMaker и поиск по их действиям. Работает только внутри ProjectMaker: использует сборку ProjectMaker, загруженную в процесс, а в других местах ничего не делает.

## Методы

### BuildZpFromXml

```csharp
public static void BuildZpFromXml(this IZennoPosterProjectModel project, string xml = null, string zpPath = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L146)

Собирает файл .zp из XML проекта; контейнером служит файл текущего проекта.

| Параметр | Описание |
|---|---|
| `xml` | XML проекта; по умолчанию `.xml` текущего проекта рядом с ним. |
| `zpPath` | Целевой файл; по умолчанию имя проекта с суффиксом в Unix-мс, например `name.1730000000000.zp`. |

### ExtractInputSettingsHtml

```csharp
public static string ExtractInputSettingsHtml(string zpPath)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L58)

Читает HTML входных настроек (`InputSettings/inputSettings.html`) файла .zp.

**Возвращает:** HTML без BOM; `null` вне ProjectMaker.

### ExtractXml

```csharp
public static string ExtractXml(string zpPath)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L24)

Распаковывает XML проекта из файла .zp.

| Параметр | Описание |
|---|---|
| `zpPath` | Файл проекта. |

**Возвращает:** XML; текст исключения загрузчика при сбое; `null` вне ProjectMaker.

### SaveAsXml

```csharp
public static void SaveAsXml(this IZennoPosterProjectModel project, string xmlPath = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L48)

Распаковывает текущий проект в XML (UTF-16).

| Параметр | Описание |
|---|---|
| `xmlPath` | Целевой файл; по умолчанию имя файла проекта с `.xml`. |

### SaveInputSettingsHtml

```csharp
public static void SaveInputSettingsHtml(string zpPath, string html)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L82)

Заменяет HTML входных настроек внутри самого файла .zp.

```csharp
public static void SaveInputSettingsHtml(string zpPath, string html, string outputZpPath)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L94)

Записывает копию файла .zp с заменённым HTML входных настроек. Если выходной файл — это исходный, запись идёт через временный файл.

| Параметр | Описание |
|---|---|
| `zpPath` | Проект-источник. |
| `html` | Новый HTML входных настроек. |
| `outputZpPath` | Целевой файл проекта. |

### SearchInZp

```csharp
public static List<SearchHit> SearchInZp(this IZennoPosterProjectModel project, string text, string folder = null, bool recursive = true, bool cache = true, int padding = 60)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L243)

Ищет текст в действиях всех файлов .zp в папке (без учёта регистра, в атрибутах и значениях; XML-сущности при сравнении декодируются). Одно совпадение на действие; каждое совпадение также пишется в лог. Распаковка занимает около секунды на файл, поэтому распакованный XML можно кешировать в скрытой папке `.xml`; в имени файла кеша — время изменения и размер проекта, так что изменённый проект перечитывается сам.

| Параметр | Описание |
|---|---|
| `text` | Текст для поиска. |
| `folder` | Папка; по умолчанию папка проекта. |
| `recursive` | Включать подпапки. |
| `cache` | Использовать кеш XML. |
| `padding` | Сколько символов контекста брать с каждой стороны. |

**Примечания:** Работает только внутри ProjectMaker: использует сборку ProjectMaker, загруженную в процесс, а в других местах ничего не делает.

