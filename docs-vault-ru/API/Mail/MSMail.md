---
title: "MSMail"
tags: [api, Mail]
generated: z3n7-docgen
---

# MSMail

`class` · пространство имён `z3n7.Api` · исходник [Mail/MSMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L17)

```csharp
public class MSMail
```

Доступ к ящикам Microsoft через Microsoft Graph по OAuth refresh token. Учётные данные хранятся в таблице `mail` базы `FastDb` (создаётся, если её нет): строка адреса из переменной проекта `mail` даёт `thunderbird_client_id` и `graph_refresh_token`.

## Конструкторы

### MSMail

```csharp
public MSMail(IZennoPosterProjectModel project, FastDb db, string proxy = "", bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L40)

Создаёт клиент. Если задана переменная `mail`, загружает её учётные данные и сразу получает токен доступа; бросает исключение, если в таблице нет учётных данных для неё или запрос токена не удался.

| Параметр | Описание |
|---|---|
| `db` | База с таблицей `mail`. |
| `proxy` | Прокси для запросов к Graph (формат `Rqst`). Сам запрос токена идёт напрямую. |
| `log` | Писать запросы и ответы в лог. |

## Методы

### CleanAll

```csharp
public void CleanAll(int batchSize = 50)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L242)

Удаляет все письма порциями по `batchSize`.

| Параметр | Описание |
|---|---|
| `batchSize` | Сколько писем запрашивать за один проход. |

### Delete

```csharp
public string Delete(string endpoint)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L122)

Отправляет DELETE-запрос в Microsoft Graph.

| Параметр | Описание |
|---|---|
| `endpoint` | Путь после `https://graph.microsoft.com/v1.0/`, например `me/messages`. |

**Возвращает:** Тело ответа. При статусе не 2xx бросает исключение.

### DelLast

```csharp
public void DelLast()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L221)

Удаляет самое новое письмо, если оно есть.

### Get

```csharp
public string Get(string endpoint)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L98)

Отправляет GET-запрос в Microsoft Graph.

| Параметр | Описание |
|---|---|
| `endpoint` | Путь после `https://graph.microsoft.com/v1.0/`, например `me/messages`. |

**Возвращает:** Тело ответа. При статусе не 2xx бросает исключение.

### GetMessages

```csharp
public JArray GetMessages(int top = 10)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L132)

Письма ящика, сначала новые.

| Параметр | Описание |
|---|---|
| `top` | Сколько. |

### ImportFromJson

```csharp
public void ImportFromJson(string json)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L291)

Добавляет ящики в таблицу `mail` из JSON-массива; уже существующие адреса не трогает.

| Параметр | Описание |
|---|---|
| `json` | Массив объектов с `email`, `password`, `access_token`, `refresh_token`, `thunderbird_client_id`, `graph_access_token`, `graph_refresh_token`. |

### Post

```csharp
public string Post(string endpoint, string jsonBody)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L110)

Отправляет POST-запрос с телом в JSON в Microsoft Graph.

| Параметр | Описание |
|---|---|
| `endpoint` | Путь после `https://graph.microsoft.com/v1.0/`, например `me/messages`. |
| `jsonBody` | Тело в JSON. |

**Возвращает:** Тело ответа. При статусе не 2xx бросает исключение.

### SelfCheck

```csharp
public bool SelfCheck(int timeoutSeconds = 30, int checkIntervalSeconds = 3)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L170)

Отправляет письмо с уникальной темой самому ящику и ждёт, пока оно появится среди последних 20 писем.

| Параметр | Описание |
|---|---|
| `timeoutSeconds` | Сколько ждать. |
| `checkIntervalSeconds` | Пауза между проверками. |

**Возвращает:** `true`, если письмо пришло вовремя.

### SendMail

```csharp
public void SendMail(string toEmail, string subject, string body)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L143)

Отправляет текстовое сообщение.

| Параметр | Описание |
|---|---|
| `toEmail` | Получатель. |
| `subject` | Тема. |
| `body` | Текст. |

