---
title: "FirstMail"
tags: [api, Mail]
generated: z3n7-docgen
---

# FirstMail

`class` · namespace `z3n7` · source [Mail/FirstMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L11)

```csharp
public class FirstMail
```

*No description yet.*

## Constructors

### FirstMail

```csharp
public FirstMail(IZennoPosterProjectModel project, Logger log = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L34)

```csharp
public FirstMail(IZennoPosterProjectModel project, string mail, string password, Logger log = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L40)

## Methods

### Delete

```csharp
public string Delete(string email, bool seen = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L62)

### Get

```csharp
public string Get(int limit = 5)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L84)

### GetAll

```csharp
public string GetAll(string email)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L77)

### GetLink

```csharp
public string GetLink(string email)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L126)

### GetOne

```csharp
public string GetOne(string email)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L70)

### GetOTP

```csharp
public string GetOTP(string email)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L97)

### Otp

```csharp
public string Otp(string email)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L149)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
