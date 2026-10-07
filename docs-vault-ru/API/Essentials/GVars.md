---
title: "GVars"
tags: [api, Essentials]
generated: z3n7-docgen
---

# GVars

`static class` · пространство имён `z3n7` · исходник [Essentials/Vars.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L307)

```csharp
public static class GVars
```

Глобальные переменные ZennoPoster, хранятся в пространстве имён по имени текущего пользователя Windows.

## Методы

### GClean

```csharp
public static List<int> GClean(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L450)

Очищает глобальные переменные `acc1` … `acc{rangeEnd}`.

**Возвращает:** Номера очищенных переменных.

### GGetBusyList

```csharp
public static List<string> GGetBusyList(this IZennoPosterProjectModel project, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L354)

Перечисляет аккаунты, занятые работающими потоками: все непустые глобальные переменные `acc1` … `acc{rangeEnd}`.

| Параметр | Описание |
|---|---|
| `log` | Записать список в лог. |

**Возвращает:** Записи вида `number:value`.

### GSetAcc

```csharp
public static bool GSetAcc(this IZennoPosterProjectModel project, string input = null, bool force = false, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L400)

Помечает текущий аккаунт (`acc0`) занятым: записывает `input` в глобальную переменную `acc{acc0}`.

| Параметр | Описание |
|---|---|
| `input` | Что сохранить; по умолчанию переменная `projectName`. |
| `force` | Перезаписать, даже если аккаунт уже занят. |
| `log` | Записать итог в лог. |

**Возвращает:** `false`, если аккаунт уже занят, а `force` равно false.

### GVar

```csharp
public static string GVar(this IZennoPosterProjectModel project, string var)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L311)

Возвращает глобальную переменную или пустую строку, если её нет.

```csharp
public static string GVar(this IZennoPosterProjectModel project, string var, object value)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L327)

Задаёт глобальную переменную, создавая её, если её нет. Ошибки проглатываются.

**Возвращает:** Всегда пустая строка.

