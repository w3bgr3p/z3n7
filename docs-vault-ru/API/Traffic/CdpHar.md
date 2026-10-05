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

HAR recorder that talks to the instance browser over its own DevTools endpoint (the browser writes the port to &lt;user-data-dir&gt;\DevToolsActivePort). Does not depend on Tab.GetTraffic. Records only what happens after Start().

## Свойства

### Count

```csharp
public int Count { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L458)

### DevToolsPort

```csharp
public int DevToolsPort { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L43)

### IsAlive

```csharp
public bool IsAlive { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L45)

### LastError

```csharp
public string LastError { get; }
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L44)

## Методы

### Clear

```csharp
public void Clear()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L460)

### Connect

```csharp
public static CdpHar Connect(int devToolsPort)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L73)

Recorder bound to an explicit DevTools port (not registered per instance).

### Dispose

```csharp
public void Dispose()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L626)

### For

```csharp
public static CdpHar For(Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L82)

Running recorder for this instance or null.

### ResolveDevToolsPort

```csharp
public static int ResolveDevToolsPort(Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L110)

Candidates: instance.ProfilePath, ProjectMaker browser dir, ZP Trash\Profiles\*. Only one live candidate → it. Several → the one whose page url equals ActiveTab.URL.

### Save

```csharp
public int Save(string path, string urlRegex = null, int waitBodiesMs = 3000)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L470)

Writes recorded traffic as HAR 1.2. urlRegex = null → everything. Returns entry count.

### Start

```csharp
public static CdpHar Start(Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L52)

Starts (or returns the running) recorder for this instance.

### Stop

```csharp
public static void Stop(Instance instance)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L92)

## Поля

### CommandTimeoutMs

```csharp
public int CommandTimeoutMs;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L27)

### MaxBodyBytes

```csharp
public long MaxBodyBytes;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L26)

### MaxEntries

```csharp
public int MaxEntries;
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L25)

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
