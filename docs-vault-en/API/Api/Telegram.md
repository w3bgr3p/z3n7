---
title: "Telegram"
tags: [api, Api]
generated: z3n7-docgen
---

# Telegram

`class` · namespace `z3n7` · source [Api/Telegram.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L16)

```csharp
public class Telegram
```

Sends messages to a Telegram chat topic through the Bot API (`sendMessage` over `NetHttp`). Missing token, chat and topic are read from the `_api` table, row `id = 'tg_logger'`: `apikey` and `extra` = `{chat}/{topic}`.

## Constructors

### Telegram

```csharp
public Telegram(IZennoPosterProjectModel project, Logger log = null, string token = null, string group = null, string topic = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L33)

Creates a client.

| Parameter | Description |
|---|---|
| `log` | Logger for progress. `SendMarkdown`, `SendCommitsSummary` and splitting in `SendLongMessage` call it without a null check. |
| `token` | Bot token. |
| `group` | Chat id. |
| `topic` | Message id of the topic to reply to. |

## Methods

### Report

```csharp
public void Report()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L77)

Sends the `failReport` variable as a MarkdownV2 message when it is set; otherwise a success line with the project name and `acc0` as hashtags.

### SendCommitsSummary

```csharp
public string SendCommitsSummary(string summary, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L157)

Sends a text summary as plain text: drops a leading `---` block and replaces `## ` and `# ` headings with emoji markers.

| Parameter | Description |
|---|---|
| `summary` | Text. |
| `log` | Not used. |

### SendLongMessage

```csharp
public string SendLongMessage(string message, bool useMarkdown = false, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L191)

Sends a message, split into parts of up to 4000 characters at paragraph or line boundaries; stops at the first failed part.

| Parameter | Description |
|---|---|
| `message` | Text. |
| `useMarkdown` | Send with Markdown parsing. |
| `log` | Not used. |

**Returns:** Comma-separated links of the parts, or the error of the failed part.

### SendMarkdown

```csharp
public string SendMarkdown(string message, bool useMarkdownV2 = false, bool disableWebPagePreview = true, bool replyToTopic = true, bool log = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L109)

Sends a message with Markdown parsing.

| Parameter | Description |
|---|---|
| `message` | Text. |
| `useMarkdownV2` | Use MarkdownV2 instead of Markdown. |
| `disableWebPagePreview` | Disable link previews. |
| `replyToTopic` | Post into the topic. |
| `log` | Not used. |

**Returns:** The link `https://t.me/c/{chat}/{messageId}` on success; otherwise the Telegram answer or `❌ Exception: …`.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
