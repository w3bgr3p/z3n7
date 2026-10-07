---
title: "Traffic.TrafficElement"
tags: [api, Traffic]
generated: z3n7-docgen
---

# Traffic.TrafficElement

`class` · namespace `z3n7` · source [Traffic/Traffic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L147)

```csharp
public class TrafficElement
```

One recorded request with its response. All fields are text; missing values are empty strings.

## Properties

### Method

```csharp
public string Method { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L150)

HTTP method.

### RequestBody

```csharp
public string RequestBody { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L160)

Request body.

### RequestCookies

```csharp
public string RequestCookies { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L158)

Request cookies.

### RequestHeaders

```csharp
public string RequestHeaders { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L156)

Request headers as recorded by ZennoPoster.

### ResponseBody

```csharp
public string ResponseBody { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L166)

Response body as UTF-8 text, decompressed when it is gzip.

### ResponseCookies

```csharp
public string ResponseCookies { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L164)

Response cookies.

### ResponseHeaders

```csharp
public string ResponseHeaders { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L162)

Response headers as recorded by ZennoPoster.

### StatusCode

```csharp
public string StatusCode { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L154)

Response status code.

### Url

```csharp
public string Url { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L152)

Request URL.

