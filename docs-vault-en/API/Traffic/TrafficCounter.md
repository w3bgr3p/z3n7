---
title: "TrafficCounter"
tags: [api, Traffic]
generated: z3n7-docgen
---

# TrafficCounter

`static class` · namespace `z3n7` · source [Traffic/TrafficCounter.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/TrafficCounter.cs#L15)

```csharp
public static class TrafficCounter
```

Counts traffic per labelled step of a project run and reports it as JSON. Steps are kept in `project.Context`.

## Methods

### Add

```csharp
public static void Add(IZennoPosterProjectModel project, string label, string responseText)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/TrafficCounter.cs#L88)

Adds a step for traffic outside the browser, counted as the UTF-8 size of `responseText`.

| Parameter | Description |
|---|---|
| `label` | Step name. |
| `responseText` | Response text. |

### Checkpoint

```csharp
public static long Checkpoint(Instance instance, IZennoPosterProjectModel project, string label)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/TrafficCounter.cs#L51)

Adds a step: the summed request and response body sizes of the traffic returned by `ActiveTab.GetTraffic()` (blocked requests are skipped). Errors are written to the log as warnings.

| Parameter | Description |
|---|---|
| `label` | Step name. |

**Returns:** Bytes counted for this step.

### Init

```csharp
public static void Init(Instance instance)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/TrafficCounter.cs#L20)

Turns on traffic monitoring and reads the traffic recorded so far once.

### MergeAndReport

```csharp
public static string MergeAndReport(IZennoPosterProjectModel project, string existingJson)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/TrafficCounter.cs#L107)

Merges the steps of an earlier report with the current ones, sorted by time, and builds a new report.

| Parameter | Description |
|---|---|
| `existingJson` | Report from `ReportJson` or this method; ignored when empty or unreadable. |

**Returns:** JSON `{ total_kb, steps: [{ t, label, kb }] }`; `t` is seconds since 2020-01-01 UTC.

### ReportJson

```csharp
public static string ReportJson(IZennoPosterProjectModel project)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/TrafficCounter.cs#L150)

Builds a report from the current steps.

**Returns:** JSON `{ total_kb, steps: [{ t, label, kb }] }`; `t` is seconds since 2020-01-01 UTC.

> This page is generated from the source code. Do not edit it: changes will be overwritten.
