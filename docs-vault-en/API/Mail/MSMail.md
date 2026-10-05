---
title: "MSMail"
tags: [api, Mail]
generated: z3n7-docgen
---

# MSMail

`class` · namespace `z3n7.Api` · source [Mail/MSMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L11)

```csharp
public class MSMail
```

*No description yet.*

## Constructors

### MSMail

```csharp
public MSMail(IZennoPosterProjectModel project, FastDb db, string proxy = "", bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L27)

## Methods

### CleanAll

```csharp
public void CleanAll(int batchSize = 50)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L215)

Удаляет все письма из inbox.

### Delete

```csharp
public string Delete(string endpoint)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L99)

### DelLast

```csharp
public void DelLast()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L193)

Удаляет последнее письмо из inbox.

### Get

```csharp
public string Get(string endpoint)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L82)

### GetMessages

```csharp
public JArray GetMessages(int top = 10)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L108)

Inbox messages, newest first.

### ImportFromJson

```csharp
public void ImportFromJson(string json)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L257)

### Post

```csharp
public string Post(string endpoint, string jsonBody)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L90)

### SelfCheck

```csharp
public bool SelfCheck(int timeoutSeconds = 30, int checkIntervalSeconds = 3)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L140)

Отправляет письмо самому себе и проверяет его получение. Возвращает true если письмо успешно отправлено и получено.

### SendMail

```csharp
public void SendMail(string toEmail, string subject, string body)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/MSMail.cs#L116)

Send message.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
