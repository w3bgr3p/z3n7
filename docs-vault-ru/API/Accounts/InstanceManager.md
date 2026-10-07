---
title: "InstanceManager"
tags: [api, Accounts]
generated: z3n7-docgen
---

# InstanceManager

`class` · пространство имён `z3n7` · исходник [Accounts/InstanceManager.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L19)

```csharp
public class InstanceManager
```

Запускает браузер для текущего аккаунта с его профилем, прокси и куками, а в конце сохраняет и убирает за собой.

## Конструкторы

### InstanceManager

```csharp
public InstanceManager(IZennoPosterProjectModel project, Instance instance, Logger log = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L30)

Создаёт менеджер.

| Параметр | Описание |
|---|---|
| `log` | Логгер для хода работы; `null` — ничего не писать. |

## Методы

### Cleanup

```csharp
public void Cleanup()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L507)

Освобождает аккаунт (очищает его глобальную `acc{n}`, ставит `state = 'idle'` в `_instance`), очищает `acc0` и останавливает инстанс.

### Initialize

```csharp
public void Initialize(string browserToLaunch = null, bool fixTimezone = false, bool useLegacy = true, bool useZpprofile = false, bool useFolder = true)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L56)

Запускает браузер для `acc0` и готовит его. Сохраняет порт и PID браузера в `port`, `pid` и `instancePort`. Старый путь: для Chromium применяет WebGL из базы, прокси и куки (из базы, иначе из файла кук); для остальных — только прокси; до 4 попыток, потом глобальная `acc{n}` аккаунта очищается и ошибка пробрасывается. Новый путь: восстанавливает профиль из таблиц `folder_*` (`ProfileSync`) и ставит прокси.

| Параметр | Описание |
|---|---|
| `browserToLaunch` | `Chromium` или `WithoutBrowser`; по умолчанию переменная `cfgBrowser`. |
| `fixTimezone` | Исправлять часовой пояс через browserscan.net, если в его оценке упоминается время. |
| `useLegacy` | Использовать старый путь подготовки. |
| `useZpprofile` | Загружать файл профиля ZennoPoster `{profileFolder}.zpprofile`, если он есть. |
| `useFolder` | Запускать Chromium с папкой профиля аккаунта. |

### ProxySet

```csharp
public bool ProxySet(string proxyString = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L390)

Проверяет прокси и применяет его к инстансу: сравнивает IP, который видят публичные echo-сервисы напрямую и через прокси, и ставит прокси только если они различаются.

| Параметр | Описание |
|---|---|
| `proxyString` | Прокси; по умолчанию колонка `proxy` строки аккаунта в `_instance`. |

**Возвращает:** `true`. Бросает исключение, если прокси пустой, не отвечает или показывает локальный IP.

### SaveProfile

```csharp
public void SaveProfile(bool saveCookies = true, bool saveProfile = true, string saveTo = "folder", bool saveZpProfile = true)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Accounts/InstanceManager.cs#L462)

Сохраняет данные браузера аккаунта, если браузер Chromium, `acc0` задан, а `accRnd` пуст: профиль, инстанс, куки и WebGL — в таблицы `saveTo` (`ProfileSync`), профиль ZennoPoster — в папку профиля. Ошибки пишутся в лог, а не бросаются.

| Параметр | Описание |
|---|---|
| `saveCookies` | Сохранять куки. |
| `saveProfile` | Сохранять профиль, инстанс и WebGL. |
| `saveTo` | Префикс таблиц: `folder`, `zb` или `zpprofile`. |
| `saveZpProfile` | Заодно сохранить профиль ZennoPoster в папку профиля. |

