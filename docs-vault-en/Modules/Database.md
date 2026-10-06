# Database

One API over PostgreSQL and SQLite: the [[Db]] class with explicit settings, and `project.Db*` helpers that work on the current account's row. See [[05. Database]].

| Type | What it does |
|---|---|
| [[Db]] | Queries, reads, updates, table and column management, JSON storage, copying tables between databases. |
| [[Get]], [[DbUpdate]], [[DbSql]] | `DbGet`, `DbGetColumns`, `DbGetLines`, `DbUpd`, `DbInsert`, `DbDone`: the current account's row. |
| [[DbTable]], [[DbColumn]], [[DbRange]] | Creating tables, adding/dropping/reordering columns, filling account rows. |
| [[DbJson]] | Storing a JSON object as columns and rebuilding it. |
| [[DbCore]] | `DbQ`: one SQL statement against the `dbSource` database. |
| [[Sql]] | A single open connection; also table copy and full migration between PostgreSQL and SQLite. |
| [[FastDb]] | SQLite through ZennoPoster's own query runner. |
| [[ProcessManager]], [[TaskManager]] | The `_processes` table of running ZennoPoster processes. |

API: [[API reference#Db|Db]], [[API reference#DbUtils|DbUtils]]. Source: [`Db/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/Db), [`DbUtils/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/DbUtils)
