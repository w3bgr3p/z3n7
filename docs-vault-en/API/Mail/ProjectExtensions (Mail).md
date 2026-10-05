---
title: "ProjectExtensions (Mail)"
tags: [api, Mail]
generated: z3n7-docgen
---

# ProjectExtensions (Mail)

`static class` · namespace `z3n7` · source [Mail/FirstMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L224)

```csharp
public static class ProjectExtensions
```

Other parts of this type: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

Extension methods on `IZennoPosterProjectModel`: one-time codes from mail.

## Methods

### OtpCode

```csharp
public static string OtpCode(this IZennoPosterProjectModel project, string source)
```

Extension method for `IZennoPosterProjectModel`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L231)

One-time code from a source: for an address (contains `@`), the code from its latest FirstMail message; otherwise `source` is a TOTP secret and the current code is computed locally.

| Parameter | Description |
|---|---|
| `source` | Mailbox address or TOTP secret. |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
