---
title: "Extension"
tags: [api, Browser]
generated: z3n7-docgen
---

# Extension

`class` · пространство имён `z3n7` · исходник [Browser/ChromeExt.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L19)

```csharp
public class Extension
```

Управление расширениями Chrome в инстансе ZennoPoster: версия, установка, включение и выключение, удаление.

## Конструкторы

### Extension

```csharp
public Extension(IZennoPosterProjectModel project, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L35)

Создаёт помощника без инстанса; работает только `GetVer`.

| Параметр | Описание |
|---|---|
| `log` | Логгер для хода работы; `null` — ничего не писать. |

```csharp
public Extension(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L44)

Создаёт помощника для инстанса.

| Параметр | Описание |
|---|---|
| `log` | Логгер для хода работы; `null` — ничего не писать. |

## Методы

### GetVer

```csharp
public string GetVer(string extId)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L56)

Читает версию установленного расширения из `{pathProfileFolder}\Default\Secure Preferences`.

| Параметр | Описание |
|---|---|
| `extId` | Id расширения. |

**Возвращает:** Версия. Бросает исключение, если в файле нет такого расширения или версии.

### InstallFromCrx

```csharp
public bool InstallFromCrx(string extId, string fileName, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L146)

Устанавливает CRX-файл, если расширение с таким id ещё не установлено.

| Параметр | Описание |
|---|---|
| `extId` | Id расширения. |
| `fileName` | Имя CRX-файла в `{project.Path}.crx\`. |
| `log` | Не используется. |

**Возвращает:** `true`, если установлено сейчас. Если файла нет, бросает исключение.

### InstallFromStore

```csharp
public bool InstallFromStore(string url, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L99)

Открывает страницу Chrome Web Store и устанавливает расширение, подтверждая диалог нажатиями клавиш. Если оно уже установлено, нажимает «Enable now», если такая кнопка есть.

| Параметр | Описание |
|---|---|
| `url` | Страница расширения в Web Store. |
| `log` | Не используется. |

**Возвращает:** `true`, если установка запущена; `false`, если расширение уже было установлено.

### Rm

```csharp
public void Rm(string[] ExtToRemove)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L252)

Удаляет расширения; сбои пишутся в лог и пропускаются.

| Параметр | Описание |
|---|---|
| `ExtToRemove` | Id расширений. |

### Switch

```csharp
public bool Switch(string toUse = "", bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/ChromeExt.cs#L180)

Включает перечисленные расширения и выключает все остальные через страницу One-Click Extensions Manager (если его нет, сначала устанавливает). Эмуляция мыши потом восстанавливается. Работает для инстансов Chromium (менеджер из CRX) и ChromiumFromZB (менеджер из Web Store).

| Параметр | Описание |
|---|---|
| `toUse` | Имена или id расширений, которые должны остаться включёнными; сверяются как подстроки этого текста. |
| `log` | Не используется. |

**Возвращает:** `true`, если включено хотя бы одно из перечисленных расширений.

