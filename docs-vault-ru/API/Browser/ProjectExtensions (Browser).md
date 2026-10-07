---
title: "ProjectExtensions (Browser)"
tags: [api, Browser]
generated: z3n7-docgen
---

# ProjectExtensions (Browser)

`static class` · пространство имён `z3n7` · исходник [Browser/BrowserScan.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L228), [Browser/GpuSpoof.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L425)

```csharp
public static class ProjectExtensions
```

Другие части этого типа: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

Методы расширения для `IZennoPosterProjectModel`: подмена WebGL.

## Методы

### FixTime

```csharp
public static void FixTime(this IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/BrowserScan.cs#L236)

Запускает `BrowserScan.FixTime`; ошибка пишется в лог как предупреждение и не останавливает проект.

| Параметр | Описание |
|---|---|
| `instance` | Инстанс браузера. |
| `log` | Логгер для хода работы. |

### SpoofGpu

```csharp
public static void SpoofGpu(this IZennoPosterProjectModel project, Instance instance)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/GpuSpoof.cs#L434)

Берёт случайный профиль WebGL (JSON в Base64, по одному на строку) из `{project.Path}/resourses/webgl.txt`, заменяет в нём unmasked vendor и renderer на `RandomAngleString` (список GPU кешируется в `resourses/gpu.json`), сохраняет в `webgl` и загружает в инстанс. Если `webgl.txt` нет, ничего не делает.

| Параметр | Описание |
|---|---|
| `instance` | Инстанс браузера. |

