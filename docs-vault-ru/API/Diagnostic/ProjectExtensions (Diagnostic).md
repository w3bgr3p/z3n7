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

Extension methods on `IZennoPosterProjectModel`: debugging aids.

## Методы

### CatchErrorFromTraffic

```csharp
public static void CatchErrorFromTraffic(this IZennoPosterProjectModel project, Instance instance, string url, string errField, int sleepBefore = 5000)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L132)

After a pause, finds the request to `url` in the traffic of the main domain and checks its JSON response. When `errField` is set, stores the body in `err` and throws through `warn`; otherwise writes the body to the log. Does nothing when the request is not found.

| Параметр | Описание |
|---|---|
| `instance` | Browser instance. |
| `url` | Exact request URL. |
| `errField` | Top-level JSON field that signals an error. |
| `sleepBefore` | Pause before reading, ms. |

### SaveDebugScreenshot

```csharp
public static void SaveDebugScreenshot(this IZennoPosterProjectModel project, Instance instance, string watermark = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Diagnostic/Diagnostic.cs#L28)

Saves a screenshot of the instance to `{project.Path}/debug_screens/{yyyy-MM-dd}/{project.Name}/{actionId} - {unix ms}.png` with a text box in the top-left corner (Iosevka 15 pt, white on dark).

| Параметр | Описание |
|---|---|
| `instance` | Browser instance. |
| `watermark` | Text of the box; default is the last error, the current URL and the last action id. |

