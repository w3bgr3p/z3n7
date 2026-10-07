---
title: "Accountant"
tags: [api, Reports]
generated: z3n7-docgen
---

# Accountant

`class` · пространство имён `z3n7.Utilities` · исходник [Reports/Accountant.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L14)

```csharp
public class Accountant
```

HTML-отчёты о балансах аккаунтов из таблицы `_native` с цветовой разметкой по сумме.

## Конструкторы

### Accountant

```csharp
public Accountant(IZennoPosterProjectModel project, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L46)

Создаёт построитель отчётов.

| Параметр | Описание |
|---|---|
| `log` | Не используется. |

## Методы

### ShowBalanceTable

```csharp
public void ShowBalanceTable(string chains = null, bool single = false, bool call = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L65)

Пишет таблицу балансов аккаунтов до `rangeEnd` в `{project.Path}/.data/balanceReport.html`. До 3 колонок и 100+ строк раскладываются в несколько блоков по 50 строк рядом. Папка `{project.Path}/.data` должна существовать.

| Параметр | Описание |
|---|---|
| `chains` | Колонки `_native` через запятую; по умолчанию все. |
| `single` | Всегда использовать одну таблицу. |
| `call` | Потом открыть файл программой по умолчанию. |

### ShowBalanceTableFromList

```csharp
public void ShowBalanceTableFromList(List<string> data, bool call = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L206)

Пишет таблицу балансов из строк `account:balance` в `{project.Path}/.data/balanceListReport.html`; остальные строки пропускаются. Папка `{project.Path}/.data` должна существовать.

| Параметр | Описание |
|---|---|
| `data` | Строки вида `account:balance`. |
| `call` | Потом открыть файл программой по умолчанию. |

### ShowBalanceTableHeatmap

```csharp
public void ShowBalanceTableHeatmap(string chains = null, bool call = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L115)

Пишет тепловую карту балансов по аккаунтам и сетям в `{project.Path}/.data/balanceHeatmap.html`. Папка `{project.Path}/.data` должна существовать.

| Параметр | Описание |
|---|---|
| `chains` | Колонки `_native` через запятую; по умолчанию все, кроме `id`. |
| `call` | Потом открыть файл программой по умолчанию. |

