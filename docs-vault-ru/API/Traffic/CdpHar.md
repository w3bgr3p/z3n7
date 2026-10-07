---
title: "CdpHar"
tags: [api, Traffic]
generated: z3n7-docgen
---

# CdpHar

`class` · пространство имён `z3n7` · исходник [Traffic/CdpHar.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L23)

```csharp
public sealed class CdpHar : IDisposable
```

Запись HAR, которая общается с браузером инстанса через его собственный DevTools endpoint (браузер пишет порт в &lt;user-data-dir&gt;\DevToolsActivePort). Не зависит от Tab.GetTraffic. Записывает только то, что происходит после Start().

## Свойства

### Count

```csharp
public int Count { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L472)

Число записанных записей.

### DevToolsPort

```csharp
public int DevToolsPort { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L47)

Порт DevTools браузера.

### IsAlive

```csharp
public bool IsAlive { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L51)

Открыто ли соединение с DevTools.

### LastError

```csharp
public string LastError { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L49)

Последняя ошибка фонового цикла приёма или `null`.

## Методы

### Clear

```csharp
public void Clear()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L475)

Забывает все записанные записи.

### Connect

```csharp
public static CdpHar Connect(int devToolsPort)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L79)

Запись, привязанная к явно заданному порту DevTools (не регистрируется для инстанса).

### Dispose

```csharp
public void Dispose()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L649)

Закрывает соединение с DevTools.

### For

```csharp
public static CdpHar For(Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L88)

Работающая запись для этого инстанса или null.

### ResolveDevToolsPort

```csharp
public static int ResolveDevToolsPort(Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L123)

Находит порт DevTools браузера инстанса по файлам `DevToolsActivePort`: в папке профиля инстанса, в папке браузера ProjectMaker и в `Trash\Profiles\*` ZennoPoster. Выигрывает собственный порт профиля, если он живой; иначе единственный живой кандидат; иначе тот, у которого URL страницы равен `ActiveTab.URL`.

**Возвращает:** Порт. Бросает `InvalidOperationException`, если живого браузера нет или подходящих несколько.

### Save

```csharp
public int Save(string path, string urlRegex = null, int waitBodiesMs = 3000)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L492)

Записывает записанный трафик в файл HAR 1.2 (недостающие папки создаются, существующий файл заменяется).

| Параметр | Описание |
|---|---|
| `path` | Целевой файл. |
| `urlRegex` | Регулярное выражение без учёта регистра, которому должен соответствовать URL; `null` оставляет всё. |
| `waitBodiesMs` | Сколько ждать тела ответов, которые ещё загружаются. |

**Возвращает:** Число записанных записей.

### Start

```csharp
public static CdpHar Start(Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L58)

Запускает запись для этого инстанса (или возвращает уже работающую).

### Stop

```csharp
public static void Stop(Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L99)

Останавливает и забывает запись этого инстанса, если она есть.

## Поля

### CommandTimeoutMs

```csharp
public int CommandTimeoutMs;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L30)

Таймаут на подключение и на каждую команду DevTools, мс.

### MaxBodyBytes

```csharp
public long MaxBodyBytes;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L28)

Тела ответов больше этого размера (в закодированном виде) не загружаются.

### MaxEntries

```csharp
public int MaxEntries;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L26)

Сколько записей хранить; сверх этого самые старые отбрасываются.

