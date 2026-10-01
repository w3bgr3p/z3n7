# z3n7

A .NET Framework 4.8 helper library for [ZennoPoster](https://zennolab.com) projects.
It wraps the routine parts of browser automation — project setup, logging, HTTP,
databases, mailboxes, cookies, traffic capture and reporting — behind extension
methods on `IZennoPosterProjectModel` and `Instance`, so C# actions stay short.

## Requirements

- ZennoPoster 7.7.21 or later
- .NET Framework 4.8

## Installation

1. Download `z3n7.dll` from the [latest release](https://github.com/w3bgr3p/z3n7/releases/latest).
2. Copy it into the `ExternalAssemblies` folder of your ZennoPoster installation.
3. Add `using z3n7;` to your C# actions.

Video guide: [adding external assemblies in ZennoPoster](https://www.youtube.com/watch?v=Wp6gP_qig_c)

## Quick start

```csharp
using z3n7;

// Initialise project variables and the browser instance
project.InitVariables(instance);

// HTTP with project-aware logging
var http = new Rqst(project, log: true);

// Database: PostgreSQL by default, SQLite on request
var db = new Db("SQLite", sqLitePath: @"C:\data\accounts.db");
```

## What's inside

| Area | Folder | Highlights |
|---|---|---|
| **Accounts** | `Accounts/` | Account runner, instance and profile management, PID ↔ account binding, profile sync |
| **Browser** | `Browser/` | Instance extensions, JS helpers, cookie storage and collection, Chrome extension management, canvas / image recognition, GPU spoofing, browser fingerprint scan |
| **Database** | `Db/` | Unified `Db` / `Sql` layer over **PostgreSQL** and **SQLite**, table schemas, JSON helpers, file-locked fast storage |
| **Mail** | `Mail/` | Gmail, Outlook/Microsoft, FirstMail, TempMail and custom-domain mailboxes; OTP extraction from incoming mail |
| **HTTP** | `Requests/` | `Rqst` and `NetHttp` (sync and async) clients with proxy support and logging |
| **Traffic** | `Traffic/` | HAR recording over Chrome DevTools, HAR export, GraphQL helpers, traffic counters |
| **Reports** | `Reports/` | HTML and JSON reports for balances and account status; error reports to log, database or Telegram |
| **Server** | `Server/` | Embedded HTTP server inside ZennoPoster (`project.StartZpServer()`): `/state`, `/traffic`, `/command`, `/version`, bearer-token auth |
| **APIs** | `Api/` | Telegram, Webshare proxies, ZennoBrowser, OmniRoute, Aiio |
| **Essentials** | `Essentials/` | Init, levelled logger, variables / global variables, timing and deadlines, encrypted storage (SAFU) |
| **Tools** | `Tools/` | TOTP codes, randomisation, image helpers, ZennoPoster project search and `.zp` → `.csx` conversion |
| **Extensions** | `MethodExtensions/` | String, list and dictionary extensions |

## Dependencies

Npgsql, AForge.Imaging, System.Net.Http, Svg, ZXing.Net and Otp.NET are embedded into
`z3n7.dll` via Costura.Fody — no extra files are needed for them.
Newtonsoft.Json and HtmlAgilityPack are **not** embedded and must be resolvable by ZennoPoster.

## Building from source

```bash
dotnet build z3n7/z3n7.csproj -c Release
```

The ZennoPoster assemblies are referenced from the installed ZennoPoster.
The default path is set in [`ZennoLab.props`](ZennoLab.props); override it without editing files:

```bash
dotnet build z3n7/z3n7.csproj -c Release /p:ZennoProgs="D:\ZennoPoster\Progs"
```

or set the `ZennoProgs` environment variable.
