---
title: "ZpToCsx"
tags: [api, Tools]
generated: z3n7-docgen
---

# ZpToCsx

`static class` · пространство имён `z3n7.Tools` · исходник [Tools/ZpToCsx.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/ZpToCsx.cs#L16)

```csharp
public static class ZpToCsx
```

Превращает проект ZennoPoster (.zp) в каркас C#-скрипта (.csx) и собирает файлы .zp из XML. Работает только внутри ProjectMaker: использует сборку ProjectMaker, загруженную в процесс.

## Методы

### ExtractXml

```csharp
public static string ExtractXml(string zpPath)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/ZpToCsx.cs#L26)

Распаковывает XML проекта из файла .zp.

**Возвращает:** XML; текст исключения загрузчика при сбое; `null` вне ProjectMaker.

### GenerateCsx

```csharp
public static string GenerateCsx(string zpPath)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/ZpToCsx.cs#L92)

Генерирует C#-скрипт из проекта: ссылки `#r`, using-и, `InitVariables` со значениями переменных по умолчанию и `Execute` с помеченным блоком на каждое действие в порядке достижимости, переходами `goto` для ветвлений, а тип и параметры каждого действия — комментариями.

| Параметр | Описание |
|---|---|
| `zpPath` | Файл проекта. |

### Template

```csharp
public static string Template()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/ZpToCsx.cs#L111)

Встроенный пустой проект (.zp) в Base64.

### XmlToZp

```csharp
public static void XmlToZp(string xml, string zpPath)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/ZpToCsx.cs#L53)

Собирает файл .zp из XML проекта; контейнером служит пустой проект, встроенный в библиотеку.

| Параметр | Описание |
|---|---|
| `xml` | XML проекта. |
| `zpPath` | Целевой файл. |

