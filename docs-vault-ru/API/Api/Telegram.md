---
title: "Telegram"
tags: [api, Api]
generated: z3n7-docgen
---

# Telegram

`class` · пространство имён `z3n7` · исходник [Api/Telegram.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L10)

```csharp
public class Telegram
```

*Описания пока нет.*

## Конструкторы

### Telegram

```csharp
public Telegram(IZennoPosterProjectModel project, Logger log = null, string token = null, string group = null, string topic = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L19)

## Методы

### Report

```csharp
public void Report()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L59)

### SendCommitsSummary

```csharp
public string SendCommitsSummary(string summary, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L123)

### SendLongMessage

```csharp
public string SendLongMessage(string message, bool useMarkdown = false, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L149)

### SendMarkdown

```csharp
public string SendMarkdown(string message, bool useMarkdownV2 = false, bool disableWebPagePreview = true, bool replyToTopic = true, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L81)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
