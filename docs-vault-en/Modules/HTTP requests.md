# HTTP requests

HTTP clients that take proxy, headers and cookies from the project when they are not given.

| Type | What it does |
|---|---|
| [[Rqst]] | GET/POST/PUT/DELETE through ZennoPoster's HTTP client (or `NetHttp` with `useNetHttp`). Every request is appended to the traffic file read by the [[Embedded server]]. |
| [[RqstExtensions]] | `project.GET(...)`, `project.POST(...)`: one request with a new `Rqst`. |
| [[NetHttpAsync]], [[NetHttp]] | .NET `HttpClient` with shared clients per proxy; async and blocking. |
| [[ProjectExtensions (Requests)]] | `NetGet`, `NetPost`. |

Proxy shortcuts of [[Rqst]]: `"+"` — the `proxy` variable, else the account's `_instance` row; `"z"` — `z_proxy` of that row; otherwise `[scheme://][user:pass@]host:port`.

```csharp
var http = new Rqst(project, log: true);
string body = http.GET("https://api.example.com/me", proxy: "+",
    headers: new[] { "Authorization: Bearer " + token }, parse: true);   // parse: into project.Json
```

API: [[API reference#Requests|Requests]]. Source: [`Requests/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/Requests)
