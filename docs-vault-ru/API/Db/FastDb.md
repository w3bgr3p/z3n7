---
title: "FastDb"
tags: [api, Db]
generated: z3n7-docgen
---

# FastDb

`class` · пространство имён `z3n7` · исходник [Db/FastDb.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L18)

```csharp
public class FastDb
```

*Описания пока нет.*

## Конструкторы

### FastDb

```csharp
public FastDb(IZennoPosterProjectModel project, string dbName = null, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L24)

## Методы

### dbList

```csharp
public List<string> dbList(string query)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L53)

### dbString

```csharp
public string dbString(string query)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L43)

### ExportToCsv

```csharp
public void ExportToCsv(string tableName, string fileName)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L60)

```csharp
public void ExportToCsv(string tableName, string fileName, string columns = "*")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Db/FastDb.cs#L79)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
