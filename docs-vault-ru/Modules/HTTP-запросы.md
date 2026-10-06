# HTTP-запросы

HTTP-клиенты, которые берут прокси, заголовки и cookies из проекта, если их не передали.

| Тип | Что делает |
|---|---|
| [[Rqst]] | GET/POST/PUT/DELETE через HTTP-клиент ZennoPoster (или `NetHttp` при `useNetHttp`). Каждый запрос дописывается в файл трафика, который отдаёт [[Встроенный сервер]]. |
| [[RqstExtensions]] | `project.GET(...)`, `project.POST(...)`: один запрос через новый `Rqst`. |
| [[NetHttpAsync]], [[NetHttp]] | .NET `HttpClient` с общими клиентами на каждый прокси; асинхронный и блокирующий. |
| [[ProjectExtensions (Requests)]] | `NetGet`, `NetPost`. |

Сокращения прокси в [[Rqst]]: `"+"` — переменная `proxy`, иначе строка аккаунта в `_instance`; `"z"` — `z_proxy` той же строки; иначе `[scheme://][user:pass@]host:port`.

```csharp
var http = new Rqst(project, log: true);
string body = http.GET("https://api.example.com/me", proxy: "+",
    headers: new[] { "Authorization: Bearer " + token }, parse: true);   // parse: в project.Json
```

API: [[Справочник API#Requests|Requests]]. Исходники: [`Requests/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/Requests)
