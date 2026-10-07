---
title: "ProjectExtensions (Essentials)"
tags: [api, Essentials]
generated: z3n7-docgen
---

# ProjectExtensions (Essentials)

`static class` · пространство имён `z3n7` · исходник [Essentials/ExternalCode.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/ExternalCode.cs#L9), [Essentials/Init.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Init.cs#L134), [Essentials/Logger.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L200), [Essentials/Time.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L174)

```csharp
public static class ProjectExtensions
```

Другие части этого типа: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

Методы расширения для `IZennoPosterProjectModel`: старт, логирование, тайминги и запуск других проектов.

## Методы

### Age

```csharp
public static T Age<T>(this IZennoPosterProjectModel project, string var = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L194)

Возраст сессии: время, прошедшее с момента в Unix-миллисекундах, сохранённого в `var`. Если в переменной не число, сначала в неё записывается текущий момент. `string` возвращает `TimeSpan.ToString()`, `TimeSpan` — сам интервал, любой другой тип получает целые секунды, приведённые через `Convert.ChangeType`.

| Параметр | Описание |
|---|---|
| `var` | Переменная с временем старта; по умолчанию `varSessionId`. |

### Deadline

```csharp
public static int Deadline(this IZennoPosterProjectModel project, int sec = 0, bool log = false)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L252)

Дедлайн на два вызова по переменной `t0`. При `sec` = 0 сохраняет текущее время и возвращает 0; при `sec` &gt; 0 возвращает число секунд с того момента и бросает исключение, если оно больше `sec`.

| Параметр | Описание |
|---|---|
| `sec` | Предел в секундах или 0 для старта. |
| `log` | Писать в лог число прошедших секунд. |

### InitVariables

```csharp
public static void InitVariables(this IZennoPosterProjectModel project, Instance instance, string author = "w3bgr3p")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Init.cs#L144)

Выполняет `Init.InitVariables`, затем запускает встроенный сервер (`StartZpServer`). Ошибка запуска сервера пишется в лог как предупреждение и не останавливает проект.

| Параметр | Описание |
|---|---|
| `instance` | Инстанс браузера проекта. |
| `author` | Автор скрипта, который показывается в стартовом баннере. |

### log

```csharp
public static void log(this IZennoPosterProjectModel project, object toLog, [CallerMemberName] string caller = "", bool show = true, bool toZp = true)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L209)

Пишет сообщение в лог проекта через `Logger` по умолчанию. Если вызван прямо из C#-кубика, сгенерированное имя кубика заменяется именем проекта.

| Параметр | Описание |
|---|---|
| `toLog` | Сообщение. |
| `show` | Писать, даже если ниже минимального уровня. |
| `toZp` | Писать в лог ZennoPoster. |

### RunZp

```csharp
public static bool RunZp(this IZennoPosterProjectModel project, List<string> vars = null)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/ExternalCode.cs#L19)

Запускает проект, путь к которому лежит в переменной `projectScript`, через `ExecuteProject`. Каждое имя из `vars` сопоставляется с переменной того же имени в вызываемом проекте. Исключение пишется в лог как предупреждение и пробрасывается дальше.

| Параметр | Описание |
|---|---|
| `vars` | Имена переменных, которые передаются в вызываемый проект. |

**Возвращает:** Результат `ExecuteProject`.

```csharp
public static bool RunZp(this IZennoPosterProjectModel project, string path)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/ExternalCode.cs#L52)

Запускает проект по пути `path` через `ExecuteProject` и передаёт по имени фиксированный набор переменных: `acc0`, `cfgLog`, `cfgPin`, `DBmode`, `DBpstgrPass`, `DBpstgrUser`, `DBsqltPath`, `instancePort`, `lastQuery`, `varSessionId`, `wkMode`.

| Параметр | Описание |
|---|---|
| `path` | Путь к файлу .zp. |

**Возвращает:** Результат `ExecuteProject`.

### StartSession

```csharp
public static void StartSession(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L273)

Ждёт случайные 0–1 с и сохраняет текущие Unix-миллисекунды в `varSessionId`.

### TimeElapsed

```csharp
public static int TimeElapsed(this IZennoPosterProjectModel project, string varName = "varSessionId")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L178)

Секунды, прошедшие с момента, сохранённого (в Unix-миллисекундах) в переменной проекта.

| Параметр | Описание |
|---|---|
| `varName` | Переменная с временем старта; по умолчанию начало сессии, `varSessionId`. |

### TimeOut

```csharp
public static void TimeOut(this IZennoPosterProjectModel project, int min = 0)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Time.cs#L233)

Бросает исключение, как только сессия (`varSessionId`) становится старше `min` минут. В сообщении указано последнее выполненное действие.

| Параметр | Описание |
|---|---|
| `min` | Предел в минутах; 0 — прочитать из переменной `timeOut`. |

### warn

```csharp
public static void warn(this IZennoPosterProjectModel project, string msg, bool thrw = false, bool show = true, [CallerMemberName] string caller = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L225)

Пишет предупреждение в лог проекта.

| Параметр | Описание |
|---|---|
| `msg` | Сообщение. |
| `thrw` | Заодно сохранить сообщение в переменную `err` и бросить `Exception`. |
| `show` | Писать, даже если ниже минимального уровня. |

```csharp
public static void warn(this IZennoPosterProjectModel project, Exception ex, bool thrw = false, bool withStack = false, bool toZp = true, [CallerMemberName] string caller = "")
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L244)

Пишет сообщение исключения как предупреждение и сохраняет его в переменную `err`.

| Параметр | Описание |
|---|---|
| `ex` | Исключение для отчёта. |
| `thrw` | После записи бросить `Exception` с сообщением. |
| `withStack` | Добавить стек вызовов. |
| `toZp` | Не используется. |

