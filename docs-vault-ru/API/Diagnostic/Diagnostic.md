---
title: "Diagnostic"
tags: [api, Diagnostic]
generated: z3n7-docgen
---

# Diagnostic

`static class` · пространство имён `z3n7` · исходник [Diagnostic/Diagnostic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L158)

```csharp
public static class Diagnostic
```

*Описания пока нет.*

## Методы

### Info

```csharp
public static VersionInfo Info()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L166)

Версии сборки, ZennoPoster, рантайма и имя машины. Каждое поле добывается отдельно: сорвавшееся чтение одного не должно уносить остальные. Наружу исключений не выпускает.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
