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

Методы расширения для `IZennoPosterProjectModel`: одноразовые коды из почты.

## Методы

### OtpCode

```csharp
public static string OtpCode(this IZennoPosterProjectModel project, string source)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Mail/FirstMail.cs#L231)

Одноразовый код из источника: для адреса (содержит `@`) — код из последнего письма FirstMail; иначе `source` считается секретом TOTP, и текущий код вычисляется локально.

| Параметр | Описание |
|---|---|
| `source` | Адрес ящика или секрет TOTP. |

