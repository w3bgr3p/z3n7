---
title: "ProjectExtensions (Diagnostic)"
tags: [api, Diagnostic]
generated: z3n7-docgen
---

# ProjectExtensions (Diagnostic)

`static class` · пространство имён `z3n7` · исходник [Diagnostic/Diagnostic.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L19)

```csharp
public static class ProjectExtensions
```

Другие части этого типа: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

Методы расширения для `IZennoPosterProjectModel`: помощь в отладке.

## Методы

### CatchErrorFromTraffic

```csharp
public static void CatchErrorFromTraffic(this IZennoPosterProjectModel project, Instance instance, string url, string errField, int sleepBefore = 5000)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L132)

После паузы находит в трафике основного домена запрос к `url` и проверяет его JSON-ответ. Если задан `errField`, сохраняет тело в `err` и бросает исключение через `warn`; иначе пишет тело в лог. Если запрос не найден, ничего не делает.

| Параметр | Описание |
|---|---|
| `instance` | Инстанс браузера. |
| `url` | Точный URL запроса. |
| `errField` | Поле JSON верхнего уровня, которое означает ошибку. |
| `sleepBefore` | Пауза перед чтением, мс. |

### SaveDebugScreenshot

```csharp
public static void SaveDebugScreenshot(this IZennoPosterProjectModel project, Instance instance, string watermark = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L28)

Сохраняет скриншот инстанса в `{project.Path}/debug_screens/{yyyy-MM-dd}/{project.Name}/{actionId} - {unix ms}.png` с текстовой плашкой в левом верхнем углу (Iosevka 15 pt, белым по тёмному).

| Параметр | Описание |
|---|---|
| `instance` | Инстанс браузера. |
| `watermark` | Текст плашки; по умолчанию последняя ошибка, текущий URL и id последнего действия. |

