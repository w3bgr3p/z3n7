# Traffic capture

Reading what the browser sent and received.

| Type | What it does |
|---|---|
| [[Traffic]] | Requests recorded by the active tab: `Find` (waits), `FindAll`, `GetApiStructure`. |
| [[CdpHar]] | Records traffic over the browser's DevTools and saves HAR 1.2; start it before the traffic you need. |
| [[InstanceExtensions (Traffic)]] | `StartHar`, `SaveHar`, `StopHar`, `GrabTrafficList`. |
| [[HarTraffic]] | HAR from `GetTraffic`, and HAR from the [[Rqst]] traffic file. |
| [[ProjectExtensions (Traffic)]] | `SaveSuccessHar`: HAR into the project's `har` folder. |
| [[GraphQL]] | The GraphQL operations seen in the traffic. |
| [[TrafficCounter]] | Traffic volume per labelled step of a run. |

```csharp
instance.StartHar();
instance.Go("https://example.com");
// ...
instance.SaveHar(Path.Combine(project.Path, "run.har"), urlRegex: "api\\.");
```

API: [[API reference#Traffic|Traffic]]. Source: [`Traffic/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/Traffic)
