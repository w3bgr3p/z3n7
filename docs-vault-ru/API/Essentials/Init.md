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

Старт проекта: сессия, диапазон аккаунтов, зашифрованное хранилище и стартовый баннер в логе. Обычно вызывается через `project.InitVariables(instance)`.

## Конструкторы

### Init

```csharp
public Init(IZennoPosterProjectModel project, Instance instance, bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Init.cs#L27)

Создаёт инициализатор для проекта и его инстанса браузера.

| Параметр | Описание |
|---|---|
| `log` | Писать в лог собственные сообщения инициализатора. |

## Методы

### InitVariables

```csharp
public void InitVariables(string author = "")
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Init.cs#L50)

Выполняет последовательность старта: выключает файловые логи ZennoPoster (`LogDisabler.DisableLogs`), заменяет переменную `jVars` (путь к файлу) содержимым этого файла, начинает сессию, заполняет `rangeStart`, `rangeEnd` и `range` из `cfgAccRange`, инициализирует SAFU ключом `.internal/safu.key` в папке проекта и пишет стартовый баннер с версиями ZennoPoster, .NET и библиотеки.

| Параметр | Описание |
|---|---|
| `author` | Автор скрипта для баннера; пусто — не показывать. |

**Примечания:** Читает `jVars` через `File.ReadAllText`: в переменной должен лежать путь к существующему файлу.

