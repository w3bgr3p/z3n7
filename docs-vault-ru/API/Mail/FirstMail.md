---
title: "FirstMail"
tags: [api, Mail]
generated: z3n7-docgen
---

# FirstMail

`class` · пространство имён `z3n7` · исходник [Mail/FirstMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L11)

```csharp
public class FirstMail
```

*Описания пока нет.*

## Конструкторы

### FirstMail

```csharp
public FirstMail(IZennoPosterProjectModel project, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L34)

```csharp
public FirstMail(IZennoPosterProjectModel project, string mail, string password, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L40)

## Методы

### Delete

```csharp
public string Delete(string email, bool seen = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L62)

### Get

```csharp
public string Get(int limit = 5)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L84)

### GetAll

```csharp
public string GetAll(string email)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L77)

### GetLink

```csharp
public string GetLink(string email)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L126)

### GetOne

```csharp
public string GetOne(string email)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L70)

### GetOTP

```csharp
public string GetOTP(string email)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L97)

### Otp

```csharp
public string Otp(string email)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L149)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
