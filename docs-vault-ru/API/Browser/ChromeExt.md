---
title: "ChromeExt"
tags: [api, Browser]
generated: z3n7-docgen
---

# ChromeExt

`class` · пространство имён `z3n7` · исходник [Browser/ChromeExt.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L279)

```csharp
public class ChromeExt
```

Старый вариант `Extension`: только инстансы Chromium, менеджер ставится из CRX.

## Конструкторы

### ChromeExt

```csharp
public ChromeExt(IZennoPosterProjectModel project, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L292)

Создаёт помощника без инстанса; работает только `GetVer`.

| Параметр | Описание |
|---|---|
| `log` | Не используется. |

```csharp
public ChromeExt(IZennoPosterProjectModel project, Instance instance, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L301)

Создаёт помощника для инстанса.

| Параметр | Описание |
|---|---|
| `log` | Не используется. |

## Методы

### GetVer

```csharp
public string GetVer(string extId)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L315)

Читает версию установленного расширения из `{pathProfileFolder}\Default\Secure Preferences`.

| Параметр | Описание |
|---|---|
| `extId` | Id расширения. |

**Возвращает:** Версия. Бросает исключение, если в файле нет такого расширения или версии.

### Install

```csharp
public bool Install(string extId, string fileName, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L346)

Устанавливает CRX-файл, если расширение с таким id ещё не установлено.

| Параметр | Описание |
|---|---|
| `extId` | Id расширения. |
| `fileName` | Имя CRX-файла в `{project.Path}.crx\`. |
| `log` | Не используется. |

**Возвращает:** `true`, если установлено сейчас. Если файла нет, бросает исключение.

### Rm

```csharp
public void Rm(string[] ExtToRemove)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L424)

Удаляет расширения; сбои игнорируются.

| Параметр | Описание |
|---|---|
| `ExtToRemove` | Id расширений. |

### Switch

```csharp
public bool Switch(string toUse = "", bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L373)

Включает перечисленные расширения и выключает все остальные через страницу One-Click Extensions Manager (если его нет, сначала устанавливает). Эмуляция мыши потом восстанавливается. Только для инстансов Chromium.

| Параметр | Описание |
|---|---|
| `toUse` | Имена или id расширений, которые должны остаться включёнными; сверяются как подстроки этого текста. |
| `log` | Не используется. |

**Возвращает:** `true`, если какое-то из перечисленных расширений было включено.

