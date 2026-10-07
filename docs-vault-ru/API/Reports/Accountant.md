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

HTML reports of account balances from the `_native` table, colour-coded by amount.

## Конструкторы

### Accountant

```csharp
public Accountant(IZennoPosterProjectModel project, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L46)

Creates the report builder.

| Параметр | Описание |
|---|---|
| `log` | Not used. |

## Методы

### ShowBalanceTable

```csharp
public void ShowBalanceTable(string chains = null, bool single = false, bool call = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L65)

Writes a balance table of accounts up to `rangeEnd` to `{project.Path}/.data/balanceReport.html`. Up to 3 columns and 100+ rows are laid out as several 50-row blocks side by side. The `{project.Path}/.data` folder must exist.

| Параметр | Описание |
|---|---|
| `chains` | Comma-separated columns of `_native`; default all. |
| `single` | Always use one table. |
| `call` | Open the file with the default program afterwards. |

### ShowBalanceTableFromList

```csharp
public void ShowBalanceTableFromList(List<string> data, bool call = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L206)

Writes a balance table from `account:balance` lines to `{project.Path}/.data/balanceListReport.html`; other lines are skipped. The `{project.Path}/.data` folder must exist.

| Параметр | Описание |
|---|---|
| `data` | Lines `account:balance`. |
| `call` | Open the file with the default program afterwards. |

### ShowBalanceTableHeatmap

```csharp
public void ShowBalanceTableHeatmap(string chains = null, bool call = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Reports/Accountant.cs#L115)

Writes a heatmap of balances per account and chain to `{project.Path}/.data/balanceHeatmap.html`. The `{project.Path}/.data` folder must exist.

| Параметр | Описание |
|---|---|
| `chains` | Comma-separated columns of `_native`; default all except `id`. |
| `call` | Open the file with the default program afterwards. |

