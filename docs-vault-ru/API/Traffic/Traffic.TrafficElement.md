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

Один записанный запрос с ответом. Все поля текстовые; отсутствующие значения — пустые строки.

## Свойства

### Method

```csharp
public string Method { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L150)

HTTP-метод.

### RequestBody

```csharp
public string RequestBody { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L160)

Тело запроса.

### RequestCookies

```csharp
public string RequestCookies { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L158)

Куки запроса.

### RequestHeaders

```csharp
public string RequestHeaders { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L156)

Заголовки запроса в том виде, в каком их записал ZennoPoster.

### ResponseBody

```csharp
public string ResponseBody { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L166)

Тело ответа текстом в UTF-8, распакованное, если оно в gzip.

### ResponseCookies

```csharp
public string ResponseCookies { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L164)

Куки ответа.

### ResponseHeaders

```csharp
public string ResponseHeaders { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L162)

Заголовки ответа в том виде, в каком их записал ZennoPoster.

### StatusCode

```csharp
public string StatusCode { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L154)

Код статуса ответа.

### Url

```csharp
public string Url { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Traffic.cs#L152)

URL запроса.

