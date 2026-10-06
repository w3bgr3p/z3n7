# z3n7

z3n7 is a .NET Framework 4.8 helper library for [ZennoPoster](https://zennolab.com) projects.
It wraps the routine parts of browser automation — project setup, logging, HTTP, databases,
mailboxes, cookies, traffic capture and reporting — behind extension methods on
`IZennoPosterProjectModel` and `Instance`, so C# actions stay short.

## Requirements

- ZennoPoster 7.7.21 or later
- .NET Framework 4.8

## Where to start

- [[01. Installation]] — put `z3n7.dll` where ZennoPoster loads it.
- [[02. Quick start]] — the first lines of a C# action.
- [[03. Building from source]] — build the library yourself.
- [[04. Project variables]] — the project variables the library reads and writes.
- [[05. Database]] — the `Db` class and the `project.Db*` helpers.
- [[06. Account workflow]] — pick an account, run the browser, record the result.

## Modules

| Module | Folder |
|---|---|
| [[Accounts]] | `Accounts/` |
| [[Browser]] | `Browser/` |
| [[Database]] | `Db/`, `DbUtils/` |
| [[Mail]] | `Mail/` |
| [[HTTP requests]] | `Requests/` |
| [[Traffic capture]] | `Traffic/` |
| [[Reports]] | `Reports/` |
| [[Embedded server]] | `Server/` |
| [[External APIs]] | `Api/` |
| [[Essentials]] | `Essentials/` |
| [[Tools]] | `Tools/`, `Diagnostic/` |
| [[Helper extensions]] | `MethodExtensions/` |

## Reference

- [[API reference]] — every public type, generated from the source code.
- [[Extension methods]] — what is available on `project` and `instance`.
