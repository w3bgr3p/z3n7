# Embedded server

An HTTP server inside ZennoPoster for an orchestrator (DevDeck): task state and commands, ZennoPoster logs, request traffic.

```csharp
project.StartZpServer(port: 22222, log: true);   // also started by InitVariables
```

With `log: true` the node line is printed: a JSON with machine, address, port and token to paste into DevDeck. Every route needs the token (`Authorization: Bearer …` or `?token=`), stored as `ZP_TOKEN` in the `.env` next to `z3n7.dll`.

| Route | Purpose |
|---|---|
| `GET /state` | Tasks and processes of this machine. |
| `POST /command` | `{ action, task_id, payload }`: start, stop, interrupt, tries, threads, settings… |
| `GET/POST /task/xml`, `GET /task/settings` | Task settings. |
| `GET /log` | ZennoPoster log files. |
| `GET /traffic`, `GET /traffic/har` | [[Rqst]] traffic as JSONL pages or HAR. |
| `GET /version` | Library, ZennoPoster and runtime versions. |

Types: [[ZpServer]], [[ZpAuth]]. API: [[API reference#Server|Server]]. Source: [`Server/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/Server)
