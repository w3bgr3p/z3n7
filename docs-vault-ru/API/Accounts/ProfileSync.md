---
title: "ProfileSync"
tags: [api, Accounts]
generated: z3n7-docgen
---

# ProfileSync

`class` · пространство имён `z3n7.Utilities` · исходник [Accounts/ProfileSync.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L14)

```csharp
public class ProfileSync
```

Сохраняет профиль ZennoPoster, настройки инстанса, куки и настройки WebGL текущего аккаунта в таблицы базы и восстанавливает их.

## Конструкторы

### ProfileSync

```csharp
public ProfileSync(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L22)

Создаёт помощника.

| Параметр | Описание |
|---|---|
| `log` | Логгер для хода работы; `null` — ничего не писать. |

## Методы

### AddStructureToDb

```csharp
public void AddStructureToDb(bool log = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L169)

Создаёт таблицы `folder_*`, `zpprofile_*` и `zb_*` со строками аккаунтов и колонками профиля и инстанса, если `folder_profile` и `zb_profile` ещё не существуют.

| Параметр | Описание |
|---|---|
| `log` | Не используется. |

### RestoreProfile

```csharp
public void RestoreProfile(string restoreFrom, bool restoreProfile = true, bool restoreCookies = true, bool restoreInstance = true, bool restoreWebgl = true, bool rebuildWebgl = false)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L45)

Восстанавливает данные текущего аккаунта из таблиц `restoreFrom`. Для любого другого источника бросает исключение.

| Параметр | Описание |
|---|---|
| `restoreFrom` | `folder`, `zb` или `zpprofile`: префикс таблиц `{prefix}_profile`, `{prefix}_instance` и `{prefix}_webgl`. |
| `restoreProfile` | Свойства профиля. |
| `restoreCookies` | Куки (Base64 в колонке `cookies`). |
| `restoreInstance` | Свойства инстанса. |
| `restoreWebgl` | Настройки WebGL (колонка `_preferences`). |
| `rebuildWebgl` | Восстанавливать настройки WebGL из развёрнутых JSON-колонок (`DbToJson`), а не из `_preferences`. |

### SaveProfile

```csharp
public void SaveProfile(string saveTo, bool saveProfile = true, bool saveInstance = true, bool saveCookies = true, bool saveWebgl = true)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/ProfileSync.cs#L112)

Сохраняет данные текущего аккаунта в таблицы `saveTo`. Для любой другой цели бросает исключение.

| Параметр | Описание |
|---|---|
| `saveTo` | `folder`, `zb` или `zpprofile`: префикс таблиц `{prefix}_profile`, `{prefix}_instance` и `{prefix}_webgl`. |
| `saveProfile` | Свойства профиля. |
| `saveInstance` | Свойства инстанса. |
| `saveCookies` | Все куки (`SaveAllCookies`). |
| `saveWebgl` | Настройки WebGL — и в `_preferences`, и развёрнутые по колонкам. |

