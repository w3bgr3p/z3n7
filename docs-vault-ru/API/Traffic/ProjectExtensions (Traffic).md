---
title: "ProjectExtensions (Traffic)"
tags: [api, Traffic]
generated: z3n7-docgen
---

# ProjectExtensions (Traffic)

`class` · пространство имён `z3n7` · исходник [Traffic/Har.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.cs#L478)

```csharp
public class ProjectExtensions
```

Другие части этого типа: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]]

Extension methods on `IZennoPosterProjectModel`: HAR export.

## Методы

### SaveSuccessHar

```csharp
public static void SaveSuccessHar(this IZennoPosterProjectModel project, Instance instance, string filter = null, string result = "success")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.cs#L489)

Saves the browser traffic to `{project.Path}/har/{yyyy-MM-dd}/{result}/{project.Name}/{unix ms}.har`. Uses the CDP recorder when `instance.StartHar()` was called; otherwise `HarTraffic.Save` with the main domain as the filter, and a warning in the log.

| Параметр | Описание |
|---|---|
| `instance` | Browser instance. |
| `filter` | URL filter; default: everything (CDP) or the main domain (GetTraffic). |
| `result` | Sub-folder name, e.g. `success` or `fail`. |

