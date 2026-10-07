---
title: "Traffic.TrafficElement"
tags: [api, Traffic]
generated: z3n7-docgen
---

# Traffic.TrafficElement

`class` · пространство имён `z3n7` · исходник [Traffic/Traffic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L147)

```csharp
public class TrafficElement
```

One recorded request with its response. All fields are text; missing values are empty strings.

## Свойства

### Method

```csharp
public string Method { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L150)

HTTP method.

### RequestBody

```csharp
public string RequestBody { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L160)

Request body.

### RequestCookies

```csharp
public string RequestCookies { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L158)

Request cookies.

### RequestHeaders

```csharp
public string RequestHeaders { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L156)

Request headers as recorded by ZennoPoster.

### ResponseBody

```csharp
public string ResponseBody { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L166)

Response body as UTF-8 text, decompressed when it is gzip.

### ResponseCookies

```csharp
public string ResponseCookies { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L164)

Response cookies.

### ResponseHeaders

```csharp
public string ResponseHeaders { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L162)

Response headers as recorded by ZennoPoster.

### StatusCode

```csharp
public string StatusCode { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L154)

Response status code.

### Url

```csharp
public string Url { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L152)

Request URL.

