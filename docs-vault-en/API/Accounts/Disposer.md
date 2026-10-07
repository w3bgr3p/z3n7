---
title: "Disposer"
tags: [api, Accounts]
generated: z3n7-docgen
---

# Disposer

`class` · namespace `z3n7` · source [Accounts/Disposer.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L12)

```csharp
public class Disposer
```

End of an account session: report, save the browser profile, clean up.

## Constructors

### Disposer

```csharp
public Disposer(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L24)

Creates the helper.

| Parameter | Description |
|---|---|
| `log` | Logger for progress; `null` logs nothing. |

## Methods

### ErrorReport

```csharp
public string ErrorReport(bool toLog = true, bool toTelegram = false, bool toDb = false, bool screenshot = false)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L72)

Same as `Reporter.ReportError`.

| Parameter | Description |
|---|---|
| `toLog` | Write it to the log. |
| `toTelegram` | Send it to Telegram. |
| `toDb` | Write it to the account's row. |
| `screenshot` | Save a screenshot. |

### FinishSession

```csharp
public void FinishSession()
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L44)

Finishes the session. When `acc0` is set, writes a success report (or, when `lastQuery` contains `dropped`, an error report with a screenshot) to the log and the account's row; then saves the profile (`InstanceManager.SaveProfile`), writes the final line to the log and cleans up (`InstanceManager.Cleanup`).

### SuccessReport

```csharp
public string SuccessReport(bool toLog = true, bool toTelegram = false, bool toDb = false, string customMessage = null)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/Disposer.cs#L82)

Same as `Reporter.ReportSuccess`.

| Parameter | Description |
|---|---|
| `toLog` | Write it to the log. |
| `toTelegram` | Send it to Telegram. |
| `toDb` | Write it to the account's row. |
| `customMessage` | Extra line. |

