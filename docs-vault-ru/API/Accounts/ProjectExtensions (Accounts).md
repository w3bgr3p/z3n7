---
title: "ProjectExtensions (Accounts)"
tags: [api, Accounts]
generated: z3n7-docgen
---

# ProjectExtensions (Accounts)

`static class` · пространство имён `z3n7` · исходник [Accounts/InstanceManager.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L559)

```csharp
public static class ProjectExtensions
```

Другие части этого типа: [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

Методы расширения для `IZennoPosterProjectModel`: запуск и завершение браузера для аккаунта.

## Методы

### Finish

```csharp
public static void Finish(this IZennoPosterProjectModel project, Instance instance)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L588)

Завершает сессию аккаунта: `Disposer.FinishSession`.

### ProxySet

```csharp
public static bool ProxySet(this IZennoPosterProjectModel project, Instance instance, string proxyString = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L624)

Проверяет прокси и применяет его к инстансу: сравнивает IP, который видят публичные echo-сервисы напрямую и через прокси, и ставит прокси только если они различаются.

| Параметр | Описание |
|---|---|
| `proxyString` | Прокси; по умолчанию колонка `proxy` строки аккаунта в `_instance`. |
| `instance` | Инстанс браузера. |

**Возвращает:** `true`. Бросает исключение, если прокси пустой, не отвечает или показывает локальный IP.

### ReportError

```csharp
public static string ReportError(this IZennoPosterProjectModel project, Instance instance, bool toLog = true, bool toTelegram = false, bool toDb = false, bool screenshot = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L599)

Пишет отчёт об ошибке (`Reporter.ReportError`).

| Параметр | Описание |
|---|---|
| `instance` | Инстанс браузера. |
| `toLog` | Записать в лог. |
| `toTelegram` | Отправить в Telegram. |
| `toDb` | Записать в строку аккаунта. |
| `screenshot` | Сохранить скриншот. |

### ReportSuccess

```csharp
public static string ReportSuccess(this IZennoPosterProjectModel project, Instance instance, bool toLog = true, bool toTelegram = false, bool toDb = false, string customMessage = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L611)

Пишет отчёт об успехе (`Reporter.ReportSuccess`).

| Параметр | Описание |
|---|---|
| `instance` | Инстанс браузера. |
| `toLog` | Записать в лог. |
| `toTelegram` | Отправить в Telegram. |
| `toDb` | Записать в строку аккаунта. |
| `customMessage` | Дополнительная строка. |

### RunBrowser

```csharp
public static void RunBrowser(this IZennoPosterProjectModel project, Instance instance, string browserToLaunch = "Chromium", bool debug = false, bool fixTimezone = false, bool useLegacy = true, bool useZpprofile = false, bool useFolder = true)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L573)

Запускает браузер для текущего аккаунта (`InstanceManager.Initialize`) и ставит `state = 'busy'` в `_instance`. Ничего не делает, если в инстансе уже работает браузер Chromium.

| Параметр | Описание |
|---|---|
| `instance` | Инстанс браузера. |
| `browserToLaunch` | `Chromium` или `WithoutBrowser`. |
| `debug` | Писать ход работы в лог. |
| `fixTimezone` | См. `Initialize`. |
| `useLegacy` | См. `Initialize`. |
| `useZpprofile` | См. `Initialize`. |
| `useFolder` | См. `Initialize`. |

### SaveProfile

```csharp
public static void SaveProfile(this IZennoPosterProjectModel project, Instance instance)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L684)

Выгружает свойства профиля и инстанса, настройки WebGL и куки (Base64) в `{project.Directory}/profiles/zenno_profile_{yyyyMMdd_HHmmss}_{id}.json`.

| Параметр | Описание |
|---|---|
| `instance` | Инстанс браузера. |

