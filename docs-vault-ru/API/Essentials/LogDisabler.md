---
title: "LogDisabler"
tags: [api, Essentials]
generated: z3n7-docgen
---

# LogDisabler

`class` · пространство имён `z3n7` · исходник [Essentials/LogDisabler.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/LogDisabler.cs#L10)

```csharp
public class LogDisabler
```

Запрещает ZennoPoster писать собственные файлы логов в папку `Logs` рядом с запущенным исполняемым файлом.

## Методы

### DisableLogs

```csharp
public static void DisableLogs(bool aggressive = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/LogDisabler.cs#L23)

Заменяет папку `Logs` ссылкой на каталог `NUL`. Ничего не делает, если папка уже ссылка, файл или рядом с ней лежит маркер `Logs.lock`. Если не получилось, папка заменяется скрытым файлом только для чтения с именем `Logs`, и записывается маркер `Logs.lock`.

| Параметр | Описание |
|---|---|
| `aggressive` | Удалять папку через `rd /s /q` и повторять до 3 раз. |

**Примечания:** Удаляет существующую папку `Logs` вместе с содержимым.

