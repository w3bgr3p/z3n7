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

Методы расширения для `IZennoPosterProjectModel`: выгрузка HAR.

## Методы

### SaveSuccessHar

```csharp
public static void SaveSuccessHar(this IZennoPosterProjectModel project, Instance instance, string filter = null, string result = "success")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.cs#L489)

Сохраняет трафик браузера в `{project.Path}/har/{yyyy-MM-dd}/{result}/{project.Name}/{unix ms}.har`. Использует запись через CDP, если был вызван `instance.StartHar()`; иначе `HarTraffic.Save` с основным доменом в качестве фильтра и предупреждение в логе.

| Параметр | Описание |
|---|---|
| `instance` | Инстанс браузера. |
| `filter` | Фильтр URL; по умолчанию — всё (CDP) или основной домен (GetTraffic). |
| `result` | Имя подпапки, например `success` или `fail`. |

