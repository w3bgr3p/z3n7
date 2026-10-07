---
title: "Reporter"
tags: [api, Reports]
generated: z3n7-docgen
---

# Reporter

`class` · пространство имён `z3n7` · исходник [Reports/Reporter.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L18)

```csharp
public class Reporter
```

Собирает отчёты о прогоне (ошибка или успех) и отправляет их в лог, в Telegram и в строку аккаунта в базе. Данные Telegram берутся из таблицы `_api`, строка `id = 'tg_logger'` (`apikey`, `extra` = `{chat}/{topic}`).

## Конструкторы

### Reporter

```csharp
public Reporter(IZennoPosterProjectModel project, Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L30)

Создаёт отчётчик; запоминает текущее время и число секунд, прошедших с начала сессии.

## Методы

### ReportError

```csharp
public string ReportError(bool toLog = true, bool toTelegram = false, bool toDb = true, bool screenshot = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L56)

Сообщает о последней ошибке проекта: аккаунт, id и комментарий действия, тип исключения, сообщение, внутреннее сообщение, первая строка стека и текущий URL.

| Параметр | Описание |
|---|---|
| `toLog` | Записать в лог как предупреждение. |
| `toTelegram` | Отправить в Telegram (также сохраняется в `failReport`). |
| `toDb` | Поставить `status = 'dropped'` и записать отчёт в `last` строки текущего аккаунта. |
| `screenshot` | Сохранить скриншот с отчётом в виде водяного знака в `{project.Path}/.failed/{projectName}/`, в масштабе 50%. |

**Возвращает:** Текст для лога; пусто, если последней ошибки нет.

### ReportSuccess

```csharp
public string ReportSuccess(bool toLog = true, bool toTelegram = false, bool toDb = true, string customMessage = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Reporter.cs#L101)

Сообщает об успешном прогоне: аккаунт, переменная `lastQuery`, необязательное сообщение и затраченное время.

| Параметр | Описание |
|---|---|
| `toLog` | Записать в лог. |
| `toTelegram` | Отправить в Telegram. |
| `toDb` | Поставить `status = 'idle'` и записать отчёт в `last` строки текущего аккаунта. |
| `customMessage` | Дополнительная строка. |

**Возвращает:** Текст для лога.

