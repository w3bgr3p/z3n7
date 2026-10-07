---
title: "Logger"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Logger

`class` · пространство имён `z3n7` · исходник [Essentials/Logger.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L24)

```csharp
public class Logger
```

Пишет сообщения в лог ZennoPoster. Поля заголовка включаются подстроками переменной проекта `cfgLog`: `acc` (аккаунт), `time` (возраст проекта), `port` (порт инстанса), `caller` (вызывающий член), `wrap` (вообще печатать заголовок), `force` (не применять фильтр по уровню).

## Конструкторы

### Logger

```csharp
public Logger(IZennoPosterProjectModel project, Instance instance = null, LogLevel logLevel = LogLevel.Info, string classEmoji = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L60)

Создаёт логгер, привязанный к проекту ZennoPoster. Минимальный уровень берётся из переменной проекта `logLevel`, если она разбирается как `LogLevel`; иначе `Debug`, если `debug` равно `True`; иначе аргумент `logLevel`.

| Параметр | Описание |
|---|---|
| `instance` | Не используется; оставлен для совместимости. |
| `logLevel` | Минимальный уровень, если переменные проекта его не задают. |
| `classEmoji` | Значение `Emoji`. |

```csharp
public Logger(LogLevel logLevel = LogLevel.Info, string classEmoji = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L89)

Создаёт логгер без проекта ZennoPoster. Писать ему некуда, поэтому все сообщения отбрасываются; `thrw` тоже не бросает исключение.

| Параметр | Описание |
|---|---|
| `logLevel` | Минимальный уровень. |
| `classEmoji` | Значение `Emoji`. |

## Свойства

### Emoji

```csharp
public string Emoji { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L45)

Префикс в скобках перед каждым сообщением, например маркер класса.

## Методы

### ClearCache

```csharp
public static void ClearCache(IZennoPosterProjectModel project)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L31)

Ничего не делает. Оставлен для совместимости: логгер больше не кеширует экземпляры.

### Debug

```csharp
public void Debug(object msg, [CallerMemberName] string caller = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L154)

Пишет сообщение уровня `Debug`.

### Error

```csharp
public void Error(object msg, [CallerMemberName] string caller = "", bool thrw = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L169)

Пишет ошибку. Ошибки пишутся всегда, независимо от минимального уровня.

| Параметр | Описание |
|---|---|
| `thrw` | После записи бросить `Exception` с сообщением. |

### Get

```csharp
public static Logger Get(IZennoPosterProjectModel project, Instance instance = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L27)

Создаёт логгер для проекта с настройками по умолчанию.

### Info

```csharp
public void Info(object msg, [CallerMemberName] string caller = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L158)

Пишет сообщение уровня `Info`.

### Send

```csharp
public void Send(object toLog, [CallerMemberName] string caller = "", bool show = false, bool thrw = false, bool toZp = true, int cut = 0, LogLevel level = LogLevel.Info, LogType type = LogType.Info, LogColor color = LogColor.Default)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L123)

Пишет сообщение. Сообщения ниже минимального уровня отбрасываются, если только `show` не равно true или `cfgLog` не содержит `force`. Тип записи в логе ZennoPoster следует уровню; текст, содержащий `!W` или `!E`, пишется как предупреждение или ошибка.

| Параметр | Описание |
|---|---|
| `toLog` | Сообщение; используется `ToString()`, `null` пишется как «null». |
| `caller` | Подставляется компилятором: имя вызывающего члена. |
| `show` | Писать, даже если ниже минимального уровня. |
| `thrw` | После записи в лог ZennoPoster бросить `Exception` с сообщением. Только если у логгера есть проект и `toZp` равно true. |
| `toZp` | Писать в лог ZennoPoster. |
| `cut` | Если в сообщении больше переводов строк, чем это число, оно склеивается в одну строку. 0 — оставить как есть. |
| `level` | Важность, по которой фильтруются сообщения. |
| `type` | Тип записи в логе ZennoPoster; переопределяется `level` Warning/Error и маркерами `!W`/`!E`. |
| `color` | Цвет записи в логе ZennoPoster. |

### Warn

```csharp
public void Warn(object msg, [CallerMemberName] string caller = "", bool show = false, bool thrw = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L164)

Пишет предупреждение.

| Параметр | Описание |
|---|---|
| `show` | Писать, даже если ниже минимального уровня. |
| `thrw` | После записи бросить `Exception` с сообщением. |

### WithInstance

```csharp
public Logger WithInstance(Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L37)

Возвращает копию этого логгера. Оставлен для совместимости: экземпляр не используется.

