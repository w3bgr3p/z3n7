---
title: "Telegram"
tags: [api, Api]
generated: z3n7-docgen
---

# Telegram

`class` · namespace `z3n7` · source [Api/Telegram.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L10)

```csharp
public class Telegram
```

*No description yet.*

## Constructors

### Telegram

```csharp
public Telegram(IZennoPosterProjectModel project, Logger log = null, string token = null, string group = null, string topic = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L19)

## Methods

### Report

```csharp
public void Report()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L59)

### SendCommitsSummary

```csharp
public string SendCommitsSummary(string summary, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L123)

### SendLongMessage

```csharp
public string SendLongMessage(string message, bool useMarkdown = false, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L149)

### SendMarkdown

```csharp
public string SendMarkdown(string message, bool useMarkdownV2 = false, bool disableWebPagePreview = true, bool replyToTopic = true, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L81)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
