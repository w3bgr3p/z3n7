---
title: "Rqst"
tags: [api, Requests]
generated: z3n7-docgen
---

# Rqst

`class` · namespace `z3n7` · source [Requests/Rqst.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L13)

```csharp
public class Rqst
```

*No description yet.*

## Constructors

### Rqst

```csharp
public Rqst(IZennoPosterProjectModel project, bool log = false, bool mask = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L21)

## Methods

### DELETE

```csharp
public string DELETE(string url, string proxy = "", string[] headers = null, string cookies = null, bool log = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L83)

### GET

```csharp
public string GET(string url, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L57)

### POST

```csharp
public string POST(string url, string body, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L65)

### PUT

```csharp
public string PUT(string url, string body, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L74)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
