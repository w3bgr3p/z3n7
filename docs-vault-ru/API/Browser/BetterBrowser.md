---
title: "BetterBrowser"
tags: [api, Browser]
generated: z3n7-docgen
---

# BetterBrowser

`static class` · пространство имён `z3n7` · исходник [Browser/BetterBrowser.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BetterBrowser.cs#L15)

```csharp
public static class BetterBrowser
```

Подготовка инстанса браузера к сессии: куки, данные профиля и профиль браузера под точку выхода прокси.

## Методы

### ImproveBrowser

```csharp
public static void ImproveBrowser(this IZennoPosterProjectModel project, Instance instance)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BetterBrowser.cs#L52)

Применяет профиль браузера под точку выхода прокси и проверяет результат. 1. Открывает в браузере `check.z3n.pro/api/ip`, чтобы узнать выходной IP и версию Chrome. 2. Запрашивает под них и страну `proxy_iso` профиль у `check.z3n.pro/api/profile` (напрямую, без прокси). 3. Применяет его к `project.Profile.BrowserProfile` и инстансу. 4. Задаёт эмуляцию часового пояса и canvas с canvas-seed и размером окна из профиля. 5. Дописывает строку диагностики в `{project.Path}/diag/z3n-diag.jsonl`. 6. Открывает проверку отпечатка `check.z3n.pro`, ждёт до 60 секунд и пишет каждую находку в лог как предупреждение.

### PrepareSession

```csharp
public static void PrepareSession(this IZennoPosterProjectModel project, Instance instance)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BetterBrowser.cs#L23)

Загружает куки аккаунта (`instance.GetCookies`), ставит в профиль email `{NickName}@outlook.com` и случайный пароль из 12 символов, включает мониторинг трафика, ставит окно 1280×720, сохраняет `Time.Now()` в `ts0` и запускает `ImproveBrowser`. Ошибка пишется в лог и пробрасывается дальше.

