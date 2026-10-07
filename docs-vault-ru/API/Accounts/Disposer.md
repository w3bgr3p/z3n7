---
title: "Disposer"
tags: [api, Accounts]
generated: z3n7-docgen
---

# Disposer

`class` · пространство имён `z3n7` · исходник [Accounts/Disposer.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L12)

```csharp
public class Disposer
```

Завершение сессии аккаунта: отчёт, сохранение профиля браузера, уборка.

## Конструкторы

### Disposer

```csharp
public Disposer(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L24)

Создаёт помощника.

| Параметр | Описание |
|---|---|
| `log` | Логгер для хода работы; `null` — ничего не писать. |

## Методы

### ErrorReport

```csharp
public string ErrorReport(bool toLog = true, bool toTelegram = false, bool toDb = false, bool screenshot = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L72)

То же, что `Reporter.ReportError`.

| Параметр | Описание |
|---|---|
| `toLog` | Записать в лог. |
| `toTelegram` | Отправить в Telegram. |
| `toDb` | Записать в строку аккаунта. |
| `screenshot` | Сохранить скриншот. |

### FinishSession

```csharp
public void FinishSession()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L44)

Завершает сессию. Если задан `acc0`, пишет в лог и в строку аккаунта отчёт об успехе (а если `lastQuery` содержит `dropped` — отчёт об ошибке со скриншотом); затем сохраняет профиль (`InstanceManager.SaveProfile`), пишет итоговую строку в лог и убирает за собой (`InstanceManager.Cleanup`).

### SuccessReport

```csharp
public string SuccessReport(bool toLog = true, bool toTelegram = false, bool toDb = false, string customMessage = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L82)

То же, что `Reporter.ReportSuccess`.

| Параметр | Описание |
|---|---|
| `toLog` | Записать в лог. |
| `toTelegram` | Отправить в Telegram. |
| `toDb` | Записать в строку аккаунта. |
| `customMessage` | Дополнительная строка. |

