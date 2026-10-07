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

Отправляет сообщения в тему чата Telegram через Bot API (`sendMessage` через `NetHttp`). Недостающие токен, чат и тема читаются из таблицы `_api`, строка `id = 'tg_logger'`: `apikey` и `extra` = `{chat}/{topic}`.

## Конструкторы

### Telegram

```csharp
public Telegram(IZennoPosterProjectModel project, Logger log = null, string token = null, string group = null, string topic = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L30)

Создаёт клиент.

| Параметр | Описание |
|---|---|
| `log` | Логгер для хода работы; `null` — ничего не писать. |
| `token` | Токен бота. |
| `group` | Id чата. |
| `topic` | Id сообщения темы, в которую нужно ответить. |

## Методы

### Report

```csharp
public void Report()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L74)

Отправляет переменную `failReport` сообщением MarkdownV2, если она задана; иначе строку об успехе с именем проекта и `acc0` в виде хештегов.

### SendCommitsSummary

```csharp
public string SendCommitsSummary(string summary, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L154)

Отправляет текстовую сводку обычным текстом: отбрасывает блок `---` в начале и заменяет заголовки `## ` и `# ` маркерами-эмодзи.

| Параметр | Описание |
|---|---|
| `summary` | Текст. |
| `log` | Не используется. |

### SendLongMessage

```csharp
public string SendLongMessage(string message, bool useMarkdown = false, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L188)

Отправляет сообщение, разбивая его на части до 4000 символов по границам абзацев или строк; останавливается на первой части, которая не ушла.

| Параметр | Описание |
|---|---|
| `message` | Текст. |
| `useMarkdown` | Отправлять с разбором Markdown. |
| `log` | Не используется. |

**Возвращает:** Ссылки на части через запятую или ошибка той части, которая не загрузилась.

### SendMarkdown

```csharp
public string SendMarkdown(string message, bool useMarkdownV2 = false, bool disableWebPagePreview = true, bool replyToTopic = true, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Telegram.cs#L106)

Отправляет сообщение с разбором Markdown.

| Параметр | Описание |
|---|---|
| `message` | Текст. |
| `useMarkdownV2` | Использовать MarkdownV2 вместо Markdown. |
| `disableWebPagePreview` | Отключить превью ссылок. |
| `replyToTopic` | Отправить в тему. |
| `log` | Не используется. |

**Возвращает:** Ссылка `https://t.me/c/{chat}/{messageId}` при успехе; иначе ответ Telegram или `❌ Exception: …`.

