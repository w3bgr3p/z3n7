---
title: "ProjectExtensions (Mail)"
tags: [api, Mail]
generated: z3n7-docgen
---

# ProjectExtensions (Mail)

`static class` · пространство имён `z3n7` · исходник [Mail/FirstMail.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L224)

```csharp
public static class ProjectExtensions
```

Другие части этого типа: [[ProjectExtensions (Accounts)]], [[ProjectExtensions (Browser)]], [[ProjectExtensions (Diagnostic)]], [[ProjectExtensions (Essentials)]], [[ProjectExtensions (MethodExtensions)]], [[ProjectExtensions (Reports)]], [[ProjectExtensions (Requests)]], [[ProjectExtensions (Traffic)]]

Extension methods on `IZennoPosterProjectModel`: one-time codes from mail.

## Методы

### OtpCode

```csharp
public static string OtpCode(this IZennoPosterProjectModel project, string source)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L231)

One-time code from a source: for an address (contains `@`), the code from its latest FirstMail message; otherwise `source` is a TOTP secret and the current code is computed locally.

| Параметр | Описание |
|---|---|
| `source` | Mailbox address or TOTP secret. |

