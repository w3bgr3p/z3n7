# External APIs

Clients of third-party services.

| Type | Service |
|---|---|
| [[Telegram]] | Messages to a chat topic through the Bot API; long texts are split. Credentials: `_api` row `tg_logger`. |
| [[Webshare]] | Proxy list of a Webshare account. |
| [[Aiio]] | io.net intelligence chat API; keys from the `__aiio` table. |
| [[OmniRoute]] | Local OpenAI-compatible router at `localhost:20128`, text and vision. |
| [[ZennoBrowser]], [[ZbDbManager]] | ZennoBrowser (ZP8) profile ids from its `ProfileManagement.db`, and the helper project `ZB.zp`. |

```csharp
var ai = new OmniRoute();
if (ai.Check())
    project.Var("answer", ai.Complete(model, "You are terse.", "Summarise: ..."));
```

API: [[API reference#Api|Api]]. Source: [`Api/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/Api)
