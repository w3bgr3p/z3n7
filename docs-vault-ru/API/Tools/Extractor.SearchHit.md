---
title: "Extractor.SearchHit"
tags: [api, Tools]
generated: z3n7-docgen
---

# Extractor.SearchHit

`class` · пространство имён `z3n7.Tools` · исходник [Tools/Extractor.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L215)

```csharp
public class SearchHit
```

Одно совпадение `SearchInZp`.

## Методы

### ToString

```csharp
public override string ToString()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L224)

`ZpPath`, `StepId` и `Context` через табуляцию.

## Поля

### Context

```csharp
public string Context;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L222)

Совпадение с окружающим текстом; переводы строк заменены пробелами.

### StepId

```csharp
public string StepId;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L220)

Id действия (шага), в котором найдено совпадение.

### ZpPath

```csharp
public string ZpPath;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Extractor.cs#L218)

Файл проекта.

