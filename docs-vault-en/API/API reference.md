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
| [[BetterBrowser]] | static class |  |
| [[BrowserScan]] | class |  |
| [[ChromeExt]] | class |  |
| [[CookieCollector]] | class |  |
| [[Cookies]] | static class |  |
| [[Cookies.CookieInfo]] | class |  |
| [[Extension]] | class |  |
| [[GpuArch]] | class |  |
| [[GpuModel]] | class |  |
| [[GpuSpoof]] | static class |  |
| [[GpuVendor]] | class |  |
| [[HtmlExtensions]] | static class |  |
| [[InstanceExtensions (Browser)]] | static class |  |
| [[JsExtensions]] | static class |  |
| [[ProjectExtensions (Browser)]] | static class |  |

## Db

| Type | Kind | Summary |
|---|---|---|
| [[DatabaseType]] | enum |  |
| [[Db]] | class |  |
| [[DbColumn]] | static class |  |
| [[DbCore]] | static class |  |
| [[DbJson]] | static class |  |
| [[DbLine]] | static class |  |
| [[DbLock]] | static class |  |
| [[DbMigration]] | static class |  |
| [[DbRange]] | static class |  |
| [[DbSchema]] | static class |  |
| [[DbSql]] | static class |  |
| [[DbTable]] | static class |  |
| [[DbUpdate]] | static class |  |
| [[FastDb]] | class |  |
| [[Get]] | static class |  |
| [[Sql]] | class |  |
| [[TableSchema]] | class | Централизованное хранилище имён таблиц с дефолтными значениями. |

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
| [[Constantes]] | static class |  |
| [[Env]] | static class |  |
| [[FunctionStorage]] | static class |  |
| [[GVars]] | static class |  |
| [[Init]] | class |  |
| [[ISAFU]] | interface |  |
| [[LogDisabler]] | class |  |
| [[Logger]] | class |  |
| [[LogLevel]] | enum |  |
| [[ProjectExtensions (Essentials)]] | static class |  |
| [[SAFU]] | static class |  |
| [[Time]] | class |  |
| [[Time.Deadline]] | class |  |
| [[Time.Sleeper]] | class |  |
| [[Vars]] | static class |  |
| [[Z3n8SAFU]] | class |  |

## Mail

| Type | Kind | Summary |
|---|---|---|
| [[AnyMessage]] | class |  |
| [[BestMailBox]] | class |  |
| [[FirstMail]] | class |  |
| [[GmailClient]] | class |  |
| [[MSMail]] | class |  |
| [[ProjectExtensions (Mail)]] | static class |  |
| [[SuperMails]] | class |  |
| [[TempMail]] | class |  |
| [[z3nmail]] | class |  |

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
| [[NetHttp]] | class | СИНХРОННЫЕ ОБЕРТКИ для ZennoPoster Project (не поддерживает async) ⚠️ ВНИМАНИЕ: Используй NetHttpAsync если можешь работать с async/await Этот класс - только адаптер для legacy кода |
| [[NetHttpAsync]] | class | ИСПРАВЛЕНО: Основной класс для HTTP запросов с ASYNC методами ✅ Использует singleton HttpClient для предотвращения socket exhaustion ✅ Кеширует клиенты с proxy для переиспользования |
| [[ProjectExtensions (Requests)]] | static class | Extension методы для удобного вызова из Project Остаются синхронными для совместимости с ZennoPoster |
| [[Rqst]] | class |  |
| [[RqstExtensions]] | static class |  |

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
| [[GraphQL]] | class |  |
| [[HarTraffic]] | static class | Standalone HAR exporter for a ZennoPoster C# action. |
| [[InstanceExtensions (Traffic)]] | static class |  |
| [[ProjectExtensions (Traffic)]] | class |  |
| [[Traffic]] | class |  |
| [[Traffic.TrafficElement]] | class |  |
| [[TrafficCounter]] | static class |  |
| [[TrafficCounter.TrafficStep]] | class |  |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
