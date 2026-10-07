---
title: "Vars"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Vars

`static class` · пространство имён `z3n7` · исходник [Essentials/Vars.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L13)

```csharp
public static class Vars
```

Короткие методы для переменных проекта: чтение, запись, разбор, счётчики.

## Методы

### Bool

```csharp
public static bool Bool(this IZennoPosterProjectModel project, string var)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L96)

Возвращает `true`, если переменная проекта в точности равна `True`.

### Decimal

```csharp
public static decimal Decimal(this IZennoPosterProjectModel project, string var)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L83)

Возвращает переменную проекта, разобранную как `decimal` (текущая культура), или 0, если разобрать не удалось.

### Int

```csharp
public static int Int(this IZennoPosterProjectModel project, string var)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L59)

Возвращает переменную проекта, разобранную как `int`, или 0, если она пустая или не число.

```csharp
public static int Int(this IZennoPosterProjectModel project, string varName, int input)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L73)

Прибавляет `input` к целочисленной переменной проекта и сохраняет результат.

**Возвращает:** Новое значение.

### MaxErr

```csharp
public static void MaxErr(this IZennoPosterProjectModel project, int maxAttempts, Exception ex = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L108)

Счётчик ошибок для циклов с повтором. Сохраняет текст ошибки в `err` и увеличивает `maxErr`; когда `maxErr` превышает `maxAttempts`, пишет предупреждение и бросает исключение.

| Параметр | Описание |
|---|---|
| `maxAttempts` | Сколько ошибок допускается. |
| `ex` | Ошибка; если null, используется `project.LastErrorComment`. |

### Range

```csharp
public static List<string> Range(this IZennoPosterProjectModel project, string accRange = null, string output = null, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L258)

Разбирает диапазон аккаунтов и сохраняет его в `rangeStart`, `rangeEnd` и `range` (список через запятую). Допустимые формы: `5`, `1-10`, `1,4,7`. Всё после `:` игнорируется.

| Параметр | Описание |
|---|---|
| `accRange` | Текст диапазона; если пусто, берётся переменная `cfgAccRange`. |
| `output` | Не используется. |
| `log` | Не используется. |

**Возвращает:** Номера аккаунтов строками или `null` (с предупреждением), если диапазон не задан.

### Var

```csharp
public static string Var(this IZennoPosterProjectModel project, string var)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L22)

Возвращает значение переменной проекта. Если переменной нет, это пишется в лог и возвращается пустая строка.

| Параметр | Описание |
|---|---|
| `var` | Имя переменной. |

```csharp
public static string Var(this IZennoPosterProjectModel project, string var, object value)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L45)

Записывает в переменную проекта `value.ToString()`. `null` игнорируется. Если переменной нет, это пишется в лог, исключение не бросается.

| Параметр | Описание |
|---|---|
| `var` | Имя переменной. |
| `value` | Новое значение. |

**Возвращает:** Всегда пустая строка.

### VarAdd

```csharp
public static bool VarAdd(this IZennoPosterProjectModel project, string name, string defaultValue = "", string comment = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L140)

Добавляет переменную в проект, открытый в ProjectMaker, через локальный API ZennoPoster (`http://localhost:5299`). Только для разработки: меняет копию проекта в памяти ProjectMaker, на задачу в раннере это не влияет. Чтобы изменение осталось, сохрани проект. API-ключ читается из `ZENNO_API_KEY` в `.env` рядом с `z3n7.dll`; уровень ключа — T1 или выше.

| Параметр | Описание |
|---|---|
| `name` | Имя переменной. |
| `defaultValue` | Начальное значение. |
| `comment` | Комментарий к переменной. |

**Возвращает:** `true`, если API ответил `RESULT_OK`; иначе ответ пишется в лог как предупреждение.

### VarCounter

```csharp
public static int VarCounter(this IZennoPosterProjectModel project, string varName, int input)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L188)

Прибавляет `input` к целочисленной переменной проекта и сохраняет результат. То же, что `Int(varName, input)`.

**Возвращает:** Новое значение.

### VarRnd

```csharp
public static string VarRnd(this IZennoPosterProjectModel project, string var)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L162)

Читает переменную проекта. Значение вида `10-20` возвращает случайное целое от 10 (включительно) до 20 (не включая); любое другое значение возвращается без пробелов по краям.

| Параметр | Описание |
|---|---|
| `var` | Имя переменной. |

### VarsFromDict

```csharp
public static void VarsFromDict(this IZennoPosterProjectModel project, Dictionary<string, string> dict)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L232)

Задаёт переменную проекта для каждого ключа словаря.

### VarsFromJson

```csharp
public static void VarsFromJson(this IZennoPosterProjectModel project, string json = "jVars")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L242)

Задаёт переменные проекта из плоского JSON-объекта со строковыми значениями.

| Параметр | Описание |
|---|---|
| `json` | Текст JSON или значение по умолчанию `jVars`, чтобы прочитать JSON из переменной `jVars`. |

### VarsMath

```csharp
public static decimal VarsMath(this IZennoPosterProjectModel project, string varA, string operation, string varB, string resultVar = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Vars.cs#L203)

Применяет `+`, `-`, `*` или `/` к двум переменным проекта, разобранным как `decimal` (инвариантная культура). Другие операции бросают исключение.

| Параметр | Описание |
|---|---|
| `varA` | Переменная левого операнда. |
| `operation` | Одна из `+ - * /`. |
| `varB` | Переменная правого операнда. |
| `resultVar` | Переменная, в которую записывается результат; пусто — не записывать. |

**Возвращает:** Результат.

