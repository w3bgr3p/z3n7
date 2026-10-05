---
title: "Telegram"
tags: [api, Api]
generated: z3n7-docgen
---

# Telegram

`class` · пространство имён `z3n7` · исходник [Api/Telegram.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L16)

```csharp
public class Telegram
```

Sends messages to a Telegram chat topic through the Bot API (`sendMessage` over `NetHttp`). Missing token, chat and topic are read from the `_api` table, row `id = 'tg_logger'`: `apikey` and `extra` = `{chat}/{topic}`.

## Конструкторы

### Telegram

```csharp
public Telegram(IZennoPosterProjectModel project, Logger log = null, string token = null, string group = null, string topic = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L30)

Creates a client.

| Параметр | Описание |
|---|---|
| `log` | Logger for progress; `null` logs nothing. |
| `token` | Bot token. |
| `group` | Chat id. |
| `topic` | Message id of the topic to reply to. |

## Методы

### Report

```csharp
public void Report()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L74)

Sends the `failReport` variable as a MarkdownV2 message when it is set; otherwise a success line with the project name and `acc0` as hashtags.

### SendCommitsSummary

```csharp
public string SendCommitsSummary(string summary, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L154)

Sends a text summary as plain text: drops a leading `---` block and replaces `## ` and `# ` headings with emoji markers.

| Параметр | Описание |
|---|---|
| `summary` | Text. |
| `log` | Not used. |

### SendLongMessage

```csharp
public string SendLongMessage(string message, bool useMarkdown = false, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L188)

Sends a message, split into parts of up to 4000 characters at paragraph or line boundaries; stops at the first failed part.

| Параметр | Описание |
|---|---|
| `message` | Text. |
| `useMarkdown` | Send with Markdown parsing. |
| `log` | Not used. |

**Возвращает:** Comma-separated links of the parts, or the error of the failed part.

### SendMarkdown

```csharp
public string SendMarkdown(string message, bool useMarkdownV2 = false, bool disableWebPagePreview = true, bool replyToTopic = true, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L106)

Sends a message with Markdown parsing.

| Параметр | Описание |
|---|---|
| `message` | Text. |
| `useMarkdownV2` | Use MarkdownV2 instead of Markdown. |
| `disableWebPagePreview` | Disable link previews. |
| `replyToTopic` | Post into the topic. |
| `log` | Not used. |

**Возвращает:** The link `https://t.me/c/{chat}/{messageId}` on success; otherwise the Telegram answer or `❌ Exception: …`.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
