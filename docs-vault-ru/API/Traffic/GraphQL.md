---
title: "GraphQL"
tags: [api, Traffic]
generated: z3n7-docgen
---

# GraphQL

`class` · пространство имён `z3n7` · исходник [Traffic/GraphQL.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/GraphQL.cs#L16)

```csharp
public class GraphQL
```

Collects the GraphQL operations seen in a browser instance's traffic.

## Конструкторы

### GraphQL

```csharp
public GraphQL(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/GraphQL.cs#L28)

Turns on traffic monitoring for the instance.

| Параметр | Описание |
|---|---|
| `log` | Logger for step-by-step progress; `null` logs nothing. |

## Методы

### GetGraphQLStructure

```csharp
public string GetGraphQLStructure(string urlFilter)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/GraphQL.cs#L48)

Builds indented JSON `{ totalOperations, operations: [...] }` from the requests whose URL contains `urlFilter`. An operation is identified by its normalised `query` text, else by `operationName` + persisted-query hash, else by `operationName`; each appears once. Requests without a JSON body or without any of these are skipped. Each item has `operationType`, `operationName`, `url`, `statusCode`, `isPersistedQuery` (with `queryHash`), `requestBody` and `responseBody`.

| Параметр | Описание |
|---|---|
| `urlFilter` | Text the request URL must contain. |

