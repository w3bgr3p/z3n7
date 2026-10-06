# Essentials

The basics most scripts start with: start-up, logging, variables, timing, secrets.

| Type | What it does |
|---|---|
| [[Init]] / [[ProjectExtensions (Essentials)]] | `InitVariables`: session start, account range, SAFU, start banner, embedded server. Also `log`, `warn`, `Age`, `TimeOut`, `Deadline`, `RunZp`. |
| [[Logger]] | Levelled log with optional header fields switched by `cfgLog`. |
| [[Vars]], [[GVars]] | `Var`, `Int`, `Bool`, `VarRnd`, `Range`, `MaxErr`; global variables and the "account is busy" flags. |
| [[Constantes]] | Project name and table, profile paths, `SecureVar`. |
| [[Time]] | Timestamps, cooldowns (`Cd`), stopwatches, random pauses. |
| [[SAFU]], [[Z3n8SAFU]] | Encryption bound to the machine, PIN and account. |
| [[Env]] | Values from a `.env` file. |
| [[LogDisabler]] | Stops ZennoPoster from writing its own log files. |

```csharp
project.InitVariables(instance);
project.log("start");                       // to the project log
var d = new Time.Deadline();
// ...
d.Check(120);                               // throws after 120 s
string key = project.ReadEnv("API_KEY");    // .env next to the project
```

See [[04. Project variables]]. API: [[API reference#Essentials|Essentials]]. Source: [`Essentials/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/Essentials)
