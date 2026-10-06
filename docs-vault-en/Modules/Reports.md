# Reports

Run reports and balance tables.

| Type | What it does |
|---|---|
| [[Reporter]] | Error and success reports to the log, Telegram (`_api` row `tg_logger`) and the account's row (`status`, `last`); optional screenshot. |
| [[Accountant]] | HTML tables and heatmaps of balances from the `_native` table. |
| [[ProjectExtensions (Reports)]] | `GenerateNative`. |

Reports are usually written by [[ProjectExtensions (Accounts)#Finish|project.Finish(instance)]] at the end of a session.

API: [[API reference#Reports|Reports]]. Source: [`Reports/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/Reports)
