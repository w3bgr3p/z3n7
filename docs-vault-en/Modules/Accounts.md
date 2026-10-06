# Accounts

Choosing the next account to work on, starting the browser with its profile, and saving and cleaning up at the end. See [[06. Account workflow]] for the whole cycle.

| Type | What it does |
|---|---|
| [[AccountRunner]] | `ChooseAccountByCondition`, `ChooseAndRunByCondition`, `QuantityByCondition`: picks accounts by an SQL condition, range priorities and social filters. |
| [[InstanceManager]] | Starts the browser for `acc0` with its profile folder, proxy and cookies; saves the profile; frees the account. |
| [[ProjectExtensions (Accounts)]] | `RunBrowser`, `Finish`, `ReportError`, `ReportSuccess`, `ProxySet`. |
| [[Disposer]] | End of session: report, save, clean up. |
| [[ProfileSync]] | Saves and restores profile, instance settings, cookies and WebGL to `folder_*`, `zb_*`, `zpprofile_*` tables. |
| [[PropertyManager]] | Copies simple object properties to and from table rows. |

```csharp
if (project.QuantityByCondition("daily < NOW OR daily = ''") == 0) return;
project.ChooseAndRunByCondition(instance, "daily < NOW OR daily = ''", browser: true);
// ...
project.Finish(instance);
```

API: [[API reference#Accounts|Accounts]]. Source: [`Accounts/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/Accounts)
