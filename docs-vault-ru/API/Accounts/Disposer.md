---
title: "Disposer"
tags: [api, Accounts]
generated: z3n7-docgen
---

# Disposer

`class` · пространство имён `z3n7` · исходник [Accounts/Disposer.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L11)

```csharp
public class Disposer
```

*Описания пока нет.*

## Конструкторы

### Disposer

```csharp
public Disposer(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L21)

## Методы

### ErrorReport

```csharp
public string ErrorReport(bool toLog = true, bool toTelegram = false, bool toDb = false, bool screenshot = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L58)

### FinishSession

```csharp
public void FinishSession()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L35)

### SuccessReport

```csharp
public string SuccessReport(bool toLog = true, bool toTelegram = false, bool toDb = false, string customMessage = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L63)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
