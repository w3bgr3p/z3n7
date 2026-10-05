---
title: "Init"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Init

`class` · пространство имён `z3n7` · исходник [Essentials/Init.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Init.cs#L16)

```csharp
public class Init
```

Project start-up: session, account range, encrypted storage and the start banner in the log. Usually called through `project.InitVariables(instance)`.

## Конструкторы

### Init

```csharp
public Init(IZennoPosterProjectModel project, Instance instance, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Init.cs#L27)

Creates the initializer for a project and its browser instance.

| Параметр | Описание |
|---|---|
| `log` | Write the initializer's own messages to the log. |

## Методы

### InitVariables

```csharp
public void InitVariables(string author = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Init.cs#L50)

Runs the start-up sequence: disables ZennoPoster file logs (`LogDisabler.DisableLogs`), replaces the `jVars` variable (a file path) with that file's content, starts the session, fills `rangeStart`, `rangeEnd` and `range` from `cfgAccRange`, initialises SAFU with the key at `.internal/safu.key` under the project folder and writes the start banner with ZennoPoster, .NET and library versions.

| Параметр | Описание |
|---|---|
| `author` | Script author shown in the banner; empty to omit. |

**Примечания:** Reads `jVars` with `File.ReadAllText`: the variable must hold a path to an existing file.

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
