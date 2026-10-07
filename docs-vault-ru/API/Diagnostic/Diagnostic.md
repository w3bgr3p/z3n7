---
title: "Diagnostic"
tags: [api, Diagnostic]
generated: z3n7-docgen
---

# Diagnostic

`static class` · пространство имён `z3n7` · исходник [Diagnostic/Diagnostic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L182)

```csharp
public static class Diagnostic
```

Сведения об окружении для логов и диагностики.

## Методы

### Info

```csharp
public static VersionInfo Info()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L188)

Читает версии библиотеки, ZennoPoster и среды выполнения и имя машины. Каждое поле читается отдельно, при сбое пустым остаётся только оно; исключений не бросает.

