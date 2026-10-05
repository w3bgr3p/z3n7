---
title: "Logger"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Logger

`class` · пространство имён `z3n7` · исходник [Essentials/Logger.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L17)

```csharp
public class Logger
```

*Описания пока нет.*

## Конструкторы

### Logger

```csharp
public Logger(IZennoPosterProjectModel project, Instance instance = null, LogLevel logLevel = LogLevel.Info, string logHost = null, bool http = true, int timezoneOffset = -5, string classEmoji = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L47)

```csharp
public Logger(LogLevel logLevel = LogLevel.Info, string logHost = null, bool http = true, int timezoneOffset = -5, string classEmoji = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L87)

Standalone — без ZennoPoster контекста.

## Свойства

### Emoji

```csharp
public string Emoji { get; set; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L39)

## Методы

### ClearCache

```csharp
public static void ClearCache(IZennoPosterProjectModel project)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L22)

### Debug

```csharp
public void Debug(object msg, [CallerMemberName] string caller = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L136)

### Error

```csharp
public void Error(object msg, [CallerMemberName] string caller = "", bool thrw = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L145)

### Get

```csharp
public static Logger Get(IZennoPosterProjectModel project, Instance instance = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L19)

### Info

```csharp
public void Info(object msg, [CallerMemberName] string caller = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L139)

### Send

```csharp
public void Send(object toLog, [CallerMemberName] string caller = "", bool show = false, bool thrw = false, bool toZp = true, int cut = 0, LogLevel level = LogLevel.Info, LogType type = LogType.Info, LogColor color = LogColor.Default)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L104)

### Warn

```csharp
public void Warn(object msg, [CallerMemberName] string caller = "", bool show = false, bool thrw = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L142)

### WithInstance

```csharp
public Logger WithInstance(Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Logger.cs#L27)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
