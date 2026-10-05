---
title: "ProjectExtensions (Traffic)"
tags: [api, Traffic]
generated: z3n7-docgen
---

# ProjectExtensions (Traffic)

`class` · namespace `z3n7` · source [Traffic/Har.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.cs#L478)

```csharp
public class ProjectExtensions
```

Other parts of this type: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (Mail)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]]

Extension methods on `IZennoPosterProjectModel`: HAR export.

## Methods

### SaveSuccessHar

```csharp
public static void SaveSuccessHar(this IZennoPosterProjectModel project, Instance instance, string filter = null, string result = "success")
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Traffic/Har.cs#L489)

Saves the browser traffic to `{project.Path}/har/{yyyy-MM-dd}/{result}/{project.Name}/{unix ms}.har`. Uses the CDP recorder when `instance.StartHar()` was called; otherwise `HarTraffic.Save` with the main domain as the filter, and a warning in the log.

| Parameter | Description |
|---|---|
| `instance` | Browser instance. |
| `filter` | URL filter; default: everything (CDP) or the main domain (GetTraffic). |
| `result` | Sub-folder name, e.g. `success` or `fail`. |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
