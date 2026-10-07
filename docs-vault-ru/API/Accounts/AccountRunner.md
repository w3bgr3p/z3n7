---
title: "AccountRunner"
tags: [api, Accounts]
generated: z3n7-docgen
---

# AccountRunner

`static class` · пространство имён `z3n7` · исходник [Accounts/AccountRunner.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L16)

```csharp
public static class AccountRunner
```

Выбор следующего аккаунта для работы из базы по условию, приоритетам диапазона и фильтрам по соцсетям.

## Методы

### ChooseAccountByCondition

```csharp
public static void ChooseAccountByCondition(this IZennoPosterProjectModel project, string condition, string sortByTaskAge = null, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, string tableName = null, bool debugLog = false, bool sqlNow = true)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L204)

Выбирает аккаунт и сохраняет его в `acc0`; кандидаты остаются в списке `accs`, а строке аккаунта ставится `status = 'working...'`. Если задан `acc0Forced`, он используется как есть. Если `acc0` уже задан, поиск кандидатов пропускается. Бросает исключение, если ни один аккаунт не подходит.

| Параметр | Описание |
|---|---|
| `condition` | SQL-условие по таблице аккаунтов. Слово `NOW` заменяется текущим временем в формате `yyyy-MM-ddTHH:mm:ss`, если `sqlNow` равно true. |
| `useRange` | Ограничиться `cfgAccRange`. Группы, разделённые `:`, — приоритеты: берётся первая группа, в которой есть подходящие аккаунты. |
| `filterTwitter` | Оставлять только аккаунты, у которых строка в таблице `_twitter` имеет `status = 'ok'`. |
| `filterDiscord` | Оставлять только аккаунты, у которых строка в таблице `_discord` имеет `status = 'ok'`. |
| `filterGithub` | Оставлять только аккаунты, у которых строка в таблице `_github` имеет `status = 'ok'`. |
| `tableName` | Таблица аккаунтов; по умолчанию `__` + имя проекта. |
| `debugLog` | Писать запросы в лог. |
| `sqlNow` | Подставлять вместо `NOW` в условии. |
| `sortByTaskAge` | Колонка с метками времени ISO: берётся аккаунт с самым старым значением (пустые — первыми). Если пусто, берётся случайный аккаунт. |

```csharp
public static void ChooseAccountByCondition(this IZennoPosterProjectModel project, Dictionary<string, string> conditionsByTable, string sortByTaskAge = null, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, bool debugLog = false, bool sqlNow = true)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L261)

То же, что перегрузка с одним условием, но берёт аккаунты, которые подходят под каждое условие в своей таблице (пересечение).

| Параметр | Описание |
|---|---|
| `conditionsByTable` | Условие → таблица. |
| `sortByTaskAge` | Если задано, берётся первый оставшийся аккаунт, а не случайный; сортировки нет. |
| `useRange` | Ограничиться `cfgAccRange`. Группы, разделённые `:`, — приоритеты: берётся первая группа, в которой есть подходящие аккаунты. |
| `filterTwitter` | Оставлять только аккаунты, у которых строка в таблице `_twitter` имеет `status = 'ok'`. |
| `filterDiscord` | Оставлять только аккаунты, у которых строка в таблице `_discord` имеет `status = 'ok'`. |
| `filterGithub` | Оставлять только аккаунты, у которых строка в таблице `_github` имеет `status = 'ok'`. |
| `debugLog` | Писать запросы в лог; заодно пустое пересечение приводит к исключению. |
| `sqlNow` | Подставлять вместо `NOW` в условиях. |

### ChooseAndRunByCondition

```csharp
public static void ChooseAndRunByCondition(this IZennoPosterProjectModel project, Instance instance, string condition, bool browser = false, string sortByTaskAge = null, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, string tableName = null, bool debugLog = false, bool sqlNow = true, bool useLegacy = true, bool useZpprofile = false, bool useFolder = true)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L429)

Выбирает аккаунт (`ChooseAccountByCondition`) и запускает для него браузер (`RunBrowser`). Если браузер не удалось запустить в папке профиля, пробуется следующий аккаунт; любая другая ошибка пробрасывается.

| Параметр | Описание |
|---|---|
| `condition` | SQL-условие по таблице аккаунтов. Слово `NOW` заменяется текущим временем в формате `yyyy-MM-ddTHH:mm:ss`, если `sqlNow` равно true. |
| `useRange` | Ограничиться `cfgAccRange`. Группы, разделённые `:`, — приоритеты: берётся первая группа, в которой есть подходящие аккаунты. |
| `filterTwitter` | Оставлять только аккаунты, у которых строка в таблице `_twitter` имеет `status = 'ok'`. |
| `filterDiscord` | Оставлять только аккаунты, у которых строка в таблице `_discord` имеет `status = 'ok'`. |
| `filterGithub` | Оставлять только аккаунты, у которых строка в таблице `_github` имеет `status = 'ok'`. |
| `tableName` | Таблица аккаунтов; по умолчанию `__` + имя проекта. |
| `debugLog` | Писать запросы в лог. |
| `sqlNow` | Подставлять вместо `NOW` в условии. |
| `instance` | Инстанс браузера. |
| `browser` | Запустить Chromium; иначе работать без браузера. |
| `sortByTaskAge` | Колонка с метками времени ISO: берётся аккаунт с самым старым значением (пустые — первыми). Если пусто, берётся случайный аккаунт. |
| `useLegacy` | Передаётся в `RunBrowser`. |
| `useZpprofile` | Передаётся в `RunBrowser`. |
| `useFolder` | Передаётся в `RunBrowser`. |

### QuantityByCondition

```csharp
public static int QuantityByCondition(this IZennoPosterProjectModel project, string condition, bool useRange = true, bool filterTwitter = false, bool filterDiscord = false, bool filterGithub = false, string tableName = null, bool debugLog = false, bool sqlNow = true)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/AccountRunner.cs#L490)

Считает аккаунты, подходящие под условие, по всем группам диапазона, после фильтров по соцсетям.

| Параметр | Описание |
|---|---|
| `condition` | SQL-условие по таблице аккаунтов. Слово `NOW` заменяется текущим временем в формате `yyyy-MM-ddTHH:mm:ss`, если `sqlNow` равно true. |
| `useRange` | Ограничиться `cfgAccRange`. Группы, разделённые `:`, — приоритеты: берётся первая группа, в которой есть подходящие аккаунты. |
| `filterTwitter` | Оставлять только аккаунты, у которых строка в таблице `_twitter` имеет `status = 'ok'`. |
| `filterDiscord` | Оставлять только аккаунты, у которых строка в таблице `_discord` имеет `status = 'ok'`. |
| `filterGithub` | Оставлять только аккаунты, у которых строка в таблице `_github` имеет `status = 'ok'`. |
| `tableName` | Таблица аккаунтов; по умолчанию `__` + имя проекта. |
| `debugLog` | Писать запросы в лог. |
| `sqlNow` | Подставлять вместо `NOW` в условии. |

