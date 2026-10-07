---
title: "API reference"
tags: [api]
generated: z3n7-docgen
---

# API reference

Every public type of the library, grouped by the source folder it lives in. Generated from the source code and its XML comments.

[[Extension methods]]

## Accounts

| Type | Kind | Summary |
|---|---|---|
| [[AccountRunner]] | static class | Picking the next account to work on from the database by condition, range priorities and social-account filters. |
| [[Disposer]] | class | End of an account session: report, save the browser profile, clean up. |
| [[InstanceManager]] | class | Starts the browser for the current account with its profile, proxy and cookies, and saves and cleans up at the end. |
| [[ProfileSync]] | class | Saves the ZennoPoster profile, instance settings, cookies and WebGL settings of the current account to database tables and restores them. |
| [[ProjectExtensions (Accounts)]] | static class | Extension methods on `IZennoPosterProjectModel`: browser start and finish for an account. |
| [[PropertyManager]] | static class | Copies simple properties of objects to and from database rows (by reflection). |

## Api

| Type | Kind | Summary |
|---|---|---|
| [[Aiio]] | class | Client of the io.net intelligence chat API (`api.intelligence.io.solutions`). |
| [[OmniRoute]] | class | Client of a local OpenAI-compatible router at `http://localhost:20128` (no API key). |
| [[Telegram]] | class | Sends messages to a Telegram chat topic through the Bot API (`sendMessage` over `NetHttp`). |
| [[Webshare]] | class | Client of the Webshare proxy API. |
| [[ZbDbManager]] | static class | Reading the ZennoBrowser profile database and parsing its profile lists. |
| [[ZennoBrowser]] | static class | ZennoBrowser (ZP8) profiles: their ids and running the helper project `ZB.zp`. |

## Browser

| Type | Kind | Summary |
|---|---|---|
| [[BetterBrowser]] | static class | Preparing a browser instance for a session: cookies, profile data and a browser profile matching the proxy's exit point. |
| [[BrowserScan]] | class | Reads the fingerprint report of `browserscan.net` in a browser instance. |
| [[ChromeExt]] | class | Older variant of `Extension`: Chromium instances only, manager installed from CRX. |
| [[CookieCollector]] | class | Collects cookies by visiting sites over plain HTTP (not the browser), starting from an existing cookie set, and returns them as browser-extension style JSON. |
| [[Cookies]] | static class | Browser cookies: read and write in an instance, store as Base64 in the account's database row, convert between JSON and Netscape formats. |
| [[Cookies.CookieInfo]] | class | Summary of a stored cookie set (see `AnalyzeCookies`). |
| [[Extension]] | class | Chrome extension management in a ZennoPoster instance: version, install, enable/disable, remove. |
| [[GpuArch]] | class | GPU models of one architecture. |
| [[GpuModel]] | class | One GPU model from the PCI ID list. |
| [[GpuSpoof]] | static class | Picks a plausible WebGL vendor/renderer pair of the same GPU architecture as the machine's card, from the public PCI ID list. |
| [[GpuVendor]] | class | GPU architectures of one vendor. |
| [[HtmlExtensions]] | static class | Helpers for ZennoPoster `HtmlElement`: centre point, QR decoding, XPath. |
| [[InstanceExtensions (Browser)]] | static class | Extension methods on `Instance`: image search on page screenshots, clicks, taps and swipes by coordinates, viewport helpers. |
| [[JsExtensions]] | static class | Extension methods on `Instance` that act on the page through JavaScript in the active tab. |
| [[ProjectExtensions (Browser)]] | static class | Extension methods on `IZennoPosterProjectModel`: WebGL spoofing. |

## Db

| Type | Kind | Summary |
|---|---|---|
| [[DatabaseType]] | enum | Kind of database behind an `Sql` connection. |
| [[Db]] | class | SQL helper over PostgreSQL or SQLite with one API for both. |
| [[DbColumn]] | static class | Adding, dropping and reordering columns. |
| [[DbCore]] | static class | Single entry point that runs SQL against the project database. |
| [[DbJson]] | static class | Storing a JSON object as table columns and rebuilding it. |
| [[DbLine]] | static class | Operations on whole rows. |
| [[DbLock]] | static class | Shared lock object for code that must not access the database concurrently. |
| [[DbMigration]] | static class | Copying tables inside the project database and between PostgreSQL and SQLite. |
| [[DbRange]] | static class | Filling a table with account rows. |
| [[DbSchema]] | static class | Names and layouts of the library's own tables. |
| [[DbSql]] | static class | Lower-level SELECT and UPDATE helpers behind the `Db*` methods. |
| [[DbTable]] | static class | Creating and inspecting tables of the project database. |
| [[DbUpdate]] | static class | Writing to the project database. |
| [[FastDb]] | class | SQLite access through ZennoPoster's built-in ODBC query runner, without opening own connections. |
| [[Get]] | static class | Reading from the project database. |
| [[Sql]] | class | One open connection to SQLite (through the SQLite3 ODBC driver) or PostgreSQL (Npgsql). |
| [[TableSchema]] | class | Name and column definitions of a table. |

## DbUtils

| Type | Kind | Summary |
|---|---|---|
| [[ProcessManager]] | static class | Keeps the `_processes` table up to date with this machine's ZennoPoster and `zbe1` processes. |
| [[TaskManager]] | static class | Conversion helpers for ZennoPoster task input settings (internal). |

## Diagnostic

| Type | Kind | Summary |
|---|---|---|
| [[Diagnostic]] | static class | Environment information for logs and diagnostics. |
| [[ProjectExtensions (Diagnostic)]] | static class | Extension methods on `IZennoPosterProjectModel`: debugging aids. |
| [[VersionInfo]] | class | Versions of the node's environment. |

## Essentials

| Type | Kind | Summary |
|---|---|---|
| [[Constantes]] | static class | Project name, its database table and the standard folders of the profile storage. |
| [[Env]] | static class | Reads settings from a `.env` file. |
| [[FunctionStorage]] | static class | Process-wide registry of delegates by name. |
| [[GVars]] | static class | Global ZennoPoster variables, kept in a namespace named after the current Windows user. |
| [[Init]] | class | Project start-up: session, account range, encrypted storage and the start banner in the log. |
| [[ISAFU]] | interface | Encryption used by SAFU (secure storage of account secrets). |
| [[LogDisabler]] | class | Stops ZennoPoster from writing its own log files to the `Logs` folder next to the running executable. |
| [[Logger]] | class | Writes messages to the ZennoPoster log. |
| [[LogLevel]] | enum | Message severity. |
| [[ProjectExtensions (Essentials)]] | static class | Extension methods on `IZennoPosterProjectModel`: start-up, logging, timing and running other projects. |
| [[SAFU]] | static class | Entry point to secure storage. |
| [[Time]] | class | Time helpers: timestamps, deadlines, random pauses. |
| [[Time.Deadline]] | class | Stopwatch that throws once a time limit is exceeded. |
| [[Time.Sleeper]] | class | Random pause within a fixed range. |
| [[Vars]] | static class | Short accessors for project variables: read, write, parse, count. |
| [[Z3n8SAFU]] | class | SAFU implementation: AES-256-CBC with an HMAC-SHA256 tag, keys derived with PBKDF2-SHA256 (100 000 iterations). |

## Mail

| Type | Kind | Summary |
|---|---|---|
| [[AnyMessage]] | class | Client of the AnyMessage mailbox service (`api.anymessage.shop`): short-term and long-term mailboxes. |
| [[BestMailBox]] | class | Client of the BestMailBox temporary mailbox service (default `https://mail.autoz3n.xyz`). |
| [[FirstMail]] | class | Client of the FirstMail mailbox API (`firstmail.ltd`). |
| [[GmailClient]] | class | Gmail access over the Gmail API with an OAuth refresh token. |
| [[MSMail]] | class | Microsoft mailbox access over Microsoft Graph with an OAuth refresh token. |
| [[ProjectExtensions (Mail)]] | static class | Extension methods on `IZennoPosterProjectModel`: one-time codes from mail. |
| [[TempMail]] | class | Client of the Temp Mail service (Privatix) on RapidAPI. |
| [[z3nmail]] | class | Client of the temporary mailbox service (default `https://mail.autoz3n.xyz`); the same API as `BestMailBox`. |

## MethodExtensions

| Type | Kind | Summary |
|---|---|---|
| [[ListExtensions]] | static class | Extension methods on lists. |
| [[ProjectExtensions (MethodExtensions)]] | static class | Extension methods: dictionaries and ZennoPoster lists. |
| [[StringExtensions]] | static class | Extension methods on strings: hex, Base64, JSON, ranges, Markdown escaping, JWT, passwords. |

## Reports

| Type | Kind | Summary |
|---|---|---|
| [[Accountant]] | class | HTML reports of account balances from the `_native` table, colour-coded by amount. |
| [[Accountant.HtmlEncoder]] | static class | HTML escaping helpers. |
| [[ProjectExtensions (Reports)]] | static class | Extension methods on `IZennoPosterProjectModel`: balance reports. |
| [[Reporter]] | class | Builds run reports (error or success) and sends them to the log, Telegram and the account's database row. |

## Requests

| Type | Kind | Summary |
|---|---|---|
| [[NetHttp]] | class | Blocking wrapper over `NetHttpAsync` for C# actions that cannot await. |
| [[NetHttpAsync]] | class | HTTP client on .NET `HttpClient` with async methods. |
| [[ProjectExtensions (Requests)]] | static class | Project shortcuts for `NetHttp` requests. |
| [[Rqst]] | class | HTTP client for ZennoPoster projects: proxy, headers and cookies are taken from the project when not given. |
| [[RqstExtensions]] | static class | Shortcuts that create an `Rqst` for one request. |

## Server

| Type | Kind | Summary |
|---|---|---|
| [[ZpAuth]] | static class | Access token of `ZpServer`: storing, issuing, checking requests. |
| [[ZpServer]] | static class | HTTP server inside ZennoPoster that takes commands from an orchestrator directly, without the database. |

## Tools

| Type | Kind | Summary |
|---|---|---|
| [[Extractor]] | static class | Reading and writing ZennoPoster project files (.zp) through ProjectMaker's own loader, and searching their actions. |
| [[Extractor.SearchHit]] | class | One match of `SearchInZp`. |
| [[Helper]] | static class | Developer aids shown as Windows forms inside ZennoPoster. |
| [[Img]] | class | SVG rendering. |
| [[Otp]] | static class | One-time codes. |
| [[Rnd]] | static class | Random values: strings, nicknames, e-mail addresses, passwords, numbers from project variables, pauses. |
| [[ZpToCsx]] | static class | Converts a ZennoPoster project (.zp) into a C# script (.csx) outline and builds .zp files from XML. |

## Traffic

| Type | Kind | Summary |
|---|---|---|
| [[CdpHar]] | class | HAR recorder that talks to the instance browser over its own DevTools endpoint (the browser writes the port to &lt;user-data-dir&gt;\DevToolsActivePort). |
| [[GraphQL]] | class | Collects the GraphQL operations seen in a browser instance's traffic. |
| [[HarTraffic]] | static class | HAR 1.2 export of browser traffic (`GetTraffic`) and of saved `Rqst` traffic. |
| [[InstanceExtensions (Traffic)]] | static class | Extension methods on `Instance`: HAR recording over DevTools. |
| [[ProjectExtensions (Traffic)]] | class | Extension methods on `IZennoPosterProjectModel`: HAR export. |
| [[Traffic]] | class | Reads the traffic recorded by the active tab of a ZennoPoster instance (`ActiveTab.GetTraffic`). |
| [[Traffic.TrafficElement]] | class | One recorded request with its response. |
| [[TrafficCounter]] | static class | Counts traffic per labelled step of a project run and reports it as JSON. |
| [[TrafficCounter.TrafficStep]] | class | One counted step. |

