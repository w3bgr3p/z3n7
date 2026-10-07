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

Собирает GraphQL-операции из трафика инстанса браузера.

## Конструкторы

### GraphQL

```csharp
public GraphQL(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/GraphQL.cs#L28)

Включает мониторинг трафика для инстанса.

| Параметр | Описание |
|---|---|
| `log` | Логгер для пошагового хода работы; `null` — ничего не писать. |

## Методы

### GetGraphQLStructure

```csharp
public string GetGraphQLStructure(string urlFilter)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/GraphQL.cs#L48)

Строит JSON с отступами `{ totalOperations, operations: [...] }` из запросов, URL которых содержит `urlFilter`. Операция определяется по нормализованному тексту `query`, иначе по `operationName` + хешу persisted query, иначе по `operationName`; каждая попадает один раз. Запросы без JSON-тела или без всего перечисленного пропускаются. У каждого элемента есть `operationType`, `operationName`, `url`, `statusCode`, `isPersistedQuery` (с `queryHash`), `requestBody` и `responseBody`.

| Параметр | Описание |
|---|---|
| `urlFilter` | Текст, который должен содержаться в URL запроса. |

