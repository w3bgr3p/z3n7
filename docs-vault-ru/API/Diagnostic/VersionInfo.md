---
title: "VersionInfo"
tags: [api, Diagnostic]
generated: z3n7-docgen
---

# VersionInfo

`class` · пространство имён `z3n7` · исходник [Diagnostic/Diagnostic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L165)

```csharp
public sealed class VersionInfo
```

Версии окружения узла. Пустая строка значит, что значение не удалось прочитать.

## Свойства

### framework

```csharp
public string framework { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L176)

Описание среды выполнения .NET.

### machine

```csharp
public string machine { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L178)

Имя машины.

### process

```csharp
public string process { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L174)

Имя файла процесса-хоста.

### product

```csharp
public string product { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L172)

Название продукта процесса-хоста.

### z3n7

```csharp
public string z3n7 { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L168)

Версия `z3n7.dll`.

### zennoposter

```csharp
public string zennoposter { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L170)

Версия продукта процесса-хоста (ZennoPoster).

