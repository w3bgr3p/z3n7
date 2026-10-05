---
title: "CdpHar"
tags: [api, Traffic]
generated: z3n7-docgen
---

# CdpHar

`class` · namespace `z3n7` · source [Traffic/CdpHar.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L23)

```csharp
public sealed class CdpHar : IDisposable
```

HAR recorder that talks to the instance browser over its own DevTools endpoint (the browser writes the port to &lt;user-data-dir&gt;\DevToolsActivePort). Does not depend on Tab.GetTraffic. Records only what happens after Start().

## Properties

### Count

```csharp
public int Count { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L472)

Number of recorded entries.

### DevToolsPort

```csharp
public int DevToolsPort { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L47)

DevTools port of the browser.

### IsAlive

```csharp
public bool IsAlive { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L51)

Whether the DevTools connection is open.

### LastError

```csharp
public string LastError { get; }
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L49)

Last error of the background receive loop, or `null`.

## Methods

### Clear

```csharp
public void Clear()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L475)

Forgets all recorded entries.

### Connect

```csharp
public static CdpHar Connect(int devToolsPort)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L79)

Recorder bound to an explicit DevTools port (not registered per instance).

### Dispose

```csharp
public void Dispose()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L649)

Closes the DevTools connection.

### For

```csharp
public static CdpHar For(Instance instance)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L88)

Running recorder for this instance or null.

### ResolveDevToolsPort

```csharp
public static int ResolveDevToolsPort(Instance instance)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L123)

Finds the DevTools port of the instance's browser from `DevToolsActivePort` files: the instance profile folder, the ProjectMaker browser folder and ZennoPoster's `Trash\Profiles\*`. The profile's own port wins when it is live; otherwise the only live candidate; otherwise the one whose page URL equals `ActiveTab.URL`.

**Returns:** The port. Throws `InvalidOperationException` when no live browser or several matching ones are found.

### Save

```csharp
public int Save(string path, string urlRegex = null, int waitBodiesMs = 3000)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L492)

Writes the recorded traffic to a HAR 1.2 file (missing folders are created, an existing file is replaced).

| Parameter | Description |
|---|---|
| `path` | Target file. |
| `urlRegex` | Case-insensitive regex the URL must match; `null` keeps everything. |
| `waitBodiesMs` | How long to wait for response bodies still being fetched. |

**Returns:** Number of entries written.

### Start

```csharp
public static CdpHar Start(Instance instance)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L58)

Starts (or returns the running) recorder for this instance.

### Stop

```csharp
public static void Stop(Instance instance)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L99)

Stops and forgets the recorder of this instance, if any.

## Fields

### CommandTimeoutMs

```csharp
public int CommandTimeoutMs;
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L30)

Timeout for connecting and for each DevTools command, ms.

### MaxBodyBytes

```csharp
public long MaxBodyBytes;
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L28)

Response bodies larger than this (encoded size) are not fetched.

### MaxEntries

```csharp
public int MaxEntries;
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/CdpHar.cs#L26)

Most entries kept; the oldest are dropped beyond it.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
