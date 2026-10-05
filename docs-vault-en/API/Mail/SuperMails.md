---
title: "SuperMails"
tags: [api, Mail]
generated: z3n7-docgen
---

# SuperMails

`class` · namespace `z3n7.Api` · source [Mail/SuperMails.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/SuperMails.cs#L12)

```csharp
public class SuperMails
```

*No description yet.*

## Constructors

### SuperMails

```csharp
public SuperMails(IZennoPosterProjectModel project, bool useNetHttp = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/SuperMails.cs#L19)

## Methods

### GetHrefs

```csharp
public HashSet<string> GetHrefs(int deadline = 60)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/SuperMails.cs#L56)

### GetMail

```csharp
public string GetMail(int deadline = 60)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/SuperMails.cs#L38)

### NewMail

```csharp
public string[] NewMail(string pool = null, string domain = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/SuperMails.cs#L26)

### Otp

```csharp
public string Otp()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/SuperMails.cs#L88)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
