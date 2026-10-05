---
title: "Rqst"
tags: [api, Requests]
generated: z3n7-docgen
---

# Rqst

`class` · пространство имён `z3n7` · исходник [Requests/Rqst.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L13)

```csharp
public class Rqst
```

*Описания пока нет.*

## Конструкторы

### Rqst

```csharp
public Rqst(IZennoPosterProjectModel project, bool log = false, bool mask = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L21)

## Методы

### DELETE

```csharp
public string DELETE(string url, string proxy = "", string[] headers = null, string cookies = null, bool log = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L83)

### GET

```csharp
public string GET(string url, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L57)

### POST

```csharp
public string POST(string url, string body, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L65)

### PUT

```csharp
public string PUT(string url, string body, string proxy = "", string[] headers = null, string cookies = null, bool log = false, bool parse = false, int deadline = 30, bool thrw = false, bool useNetHttp = false, bool returnSuccessWithStatus = false, bool bodyOnly = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Requests/Rqst.cs#L74)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
