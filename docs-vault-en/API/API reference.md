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
| [[AccountRunner]] | static class |  |
| [[Disposer]] | class |  |
| [[InstanceManager]] | class |  |
| [[ProfileSync]] | class |  |
| [[ProjectExtensions (Accounts)]] | static class |  |
| [[PropertyManager]] | static class |  |

## Api

| Type | Kind | Summary |
|---|---|---|
| [[Aiio]] | class |  |
| [[OmniRoute]] | class |  |
| [[Telegram]] | class |  |
| [[Webshare]] | class |  |
| [[ZbDbManager]] | static class |  |
| [[ZennoBrowser]] | static class |  |

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
| [[ProcessManager]] | static class |  |
| [[TaskManager]] | static class |  |

## Diagnostic

| Type | Kind | Summary |
|---|---|---|
| [[Diagnostic]] | static class |  |
| [[ProjectExtensions (Diagnostic)]] | static class |  |
| [[VersionInfo]] | class | Версии окружения узла. |

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
| [[ListExtensions]] | static class |  |
| [[ProjectExtensions (MethodExtensions)]] | static class |  |
| [[StringExtensions]] | static class |  |

## Reports

| Type | Kind | Summary |
|---|---|---|
| [[Accountant]] | class |  |
| [[Accountant.HtmlEncoder]] | static class |  |
| [[ProjectExtensions (Reports)]] | static class |  |
| [[Reporter]] | class | Отвечает за создание, форматирование и отправку отчетов |

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
| [[ZpAuth]] | static class | Токен доступа к ZpServer: хранение, выдача, проверка запроса. |
| [[ZpServer]] | static class | HTTP-сервер внутри ZennoPoster. |

## Tools

| Type | Kind | Summary |
|---|---|---|
| [[Extractor]] | static class |  |
| [[Extractor.SearchHit]] | class |  |
| [[Helper]] | static class |  |
| [[Img]] | class |  |
| [[Otp]] | static class |  |
| [[Rnd]] | static class |  |
| [[ZpToCsx]] | static class |  |

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

> This page is generated from the source code. Do not edit it: changes will be overwritten.
