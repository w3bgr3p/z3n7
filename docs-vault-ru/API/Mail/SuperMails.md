---
title: "SuperMails"
tags: [api, Mail]
generated: z3n7-docgen
---

# SuperMails

`class` · пространство имён `z3n7.Api` · исходник [Mail/SuperMails.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/SuperMails.cs#L12)

```csharp
public class SuperMails
```

*Описания пока нет.*

## Конструкторы

### SuperMails

```csharp
public SuperMails(IZennoPosterProjectModel project, bool useNetHttp = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/SuperMails.cs#L19)

## Методы

### GetHrefs

```csharp
public HashSet<string> GetHrefs(int deadline = 60)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/SuperMails.cs#L56)

### GetMail

```csharp
public string GetMail(int deadline = 60)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/SuperMails.cs#L38)

### NewMail

```csharp
public string[] NewMail(string pool = null, string domain = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/SuperMails.cs#L26)

### Otp

```csharp
public string Otp()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/SuperMails.cs#L88)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
