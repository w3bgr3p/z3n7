---
title: "Справочник API"
tags: [api]
generated: z3n7-docgen
---

# Справочник API

Все публичные типы библиотеки, сгруппированные по папке исходников. Страницы собраны из исходного кода и его XML-комментариев.

[[Методы расширения]]

## Accounts

| Тип | Вид | Описание |
|---|---|---|
| [[AccountRunner]] | static class | Выбор следующего аккаунта для работы из базы по условию, приоритетам диапазона и фильтрам по соцсетям. |
| [[Disposer]] | class | Завершение сессии аккаунта: отчёт, сохранение профиля браузера, уборка. |
| [[InstanceManager]] | class | Запускает браузер для текущего аккаунта с его профилем, прокси и куками, а в конце сохраняет и убирает за собой. |
| [[ProfileSync]] | class | Сохраняет профиль ZennoPoster, настройки инстанса, куки и настройки WebGL текущего аккаунта в таблицы базы и восстанавливает их. |
| [[ProjectExtensions (Accounts)]] | static class | Методы расширения для `IZennoPosterProjectModel`: запуск и завершение браузера для аккаунта. |
| [[PropertyManager]] | static class | Копирует простые свойства объектов в строки базы и обратно (через рефлексию). |

## Api

| Тип | Вид | Описание |
|---|---|---|
| [[Aiio]] | class | Клиент чат-API io.net intelligence (`api.intelligence.io.solutions`). |
| [[OmniRoute]] | class | Клиент локального OpenAI-совместимого роутера на `http://localhost:20128` (без API-ключа). |
| [[Telegram]] | class | Отправляет сообщения в тему чата Telegram через Bot API (`sendMessage` через `NetHttp`). |
| [[Webshare]] | class | Клиент API прокси Webshare. |
| [[ZbDbManager]] | static class | Чтение базы профилей ZennoBrowser и разбор её списков профилей. |
| [[ZennoBrowser]] | static class | Профили ZennoBrowser (ZP8): их id и запуск вспомогательного проекта `ZB.zp`. |

## Browser

| Тип | Вид | Описание |
|---|---|---|
| [[BetterBrowser]] | static class | Подготовка инстанса браузера к сессии: куки, данные профиля и профиль браузера под точку выхода прокси. |
| [[BrowserScan]] | class | Читает отчёт об отпечатке `browserscan.net` в инстансе браузера. |
| [[ChromeExt]] | class | Старый вариант `Extension`: только инстансы Chromium, менеджер ставится из CRX. |
| [[CookieCollector]] | class | Собирает куки, обходя сайты по обычному HTTP (не браузером), начиная с имеющегося набора кук, и возвращает их в JSON в формате браузерных расширений. |
| [[Cookies]] | static class | Куки браузера: чтение и запись в инстансе, хранение в Base64 в строке аккаунта в базе, перевод между форматами JSON и Netscape. |
| [[Cookies.CookieInfo]] | class | Сводка по сохранённому набору кук (см. |
| [[Extension]] | class | Управление расширениями Chrome в инстансе ZennoPoster: версия, установка, включение и выключение, удаление. |
| [[GpuArch]] | class | Модели GPU одной архитектуры. |
| [[GpuModel]] | class | Одна модель GPU из списка PCI ID. |
| [[GpuSpoof]] | static class | Подбирает правдоподобную пару WebGL vendor/renderer той же архитектуры GPU, что и карта машины, по публичному списку PCI ID. |
| [[GpuVendor]] | class | Архитектуры GPU одного производителя. |
| [[HtmlExtensions]] | static class | Помощники для `HtmlElement` ZennoPoster: точка центра, распознавание QR, XPath. |
| [[InstanceExtensions (Browser)]] | static class | Методы расширения для `Instance`: поиск картинки на скриншотах страницы, клики, тапы и свайпы по координатам, помощь с областью просмотра. |
| [[JsExtensions]] | static class | Методы расширения для `Instance`, которые работают со страницей через JavaScript в активной вкладке. |
| [[ProjectExtensions (Browser)]] | static class | Методы расширения для `IZennoPosterProjectModel`: подмена WebGL. |

## Db

| Тип | Вид | Описание |
|---|---|---|
| [[DatabaseType]] | enum | Вид базы данных за соединением `Sql`. |
| [[Db]] | class | Помощник SQL поверх PostgreSQL или SQLite с одним API для обоих. |
| [[DbColumn]] | static class | Добавление, удаление и перестановка колонок. |
| [[DbCore]] | static class | Единая точка входа, через которую выполняется SQL к базе проекта. |
| [[DbJson]] | static class | Хранение JSON-объекта в виде колонок таблицы и его восстановление. |
| [[DbLine]] | static class | Операции над строками целиком. |
| [[DbLock]] | static class | Общий объект блокировки для кода, который не должен обращаться к базе одновременно. |
| [[DbMigration]] | static class | Копирование таблиц внутри базы проекта и между PostgreSQL и SQLite. |
| [[DbRange]] | static class | Заполнение таблицы строками аккаунтов. |
| [[DbSchema]] | static class | Имена и раскладки собственных таблиц библиотеки. |
| [[DbSql]] | static class | Низкоуровневые помощники SELECT и UPDATE, на которых построены методы `Db*`. |
| [[DbTable]] | static class | Создание и просмотр таблиц базы проекта. |
| [[DbUpdate]] | static class | Запись в базу проекта. |
| [[FastDb]] | class | Доступ к SQLite через встроенный в ZennoPoster ODBC-исполнитель запросов, без открытия собственных соединений. |
| [[Get]] | static class | Чтение из базы проекта. |
| [[Sql]] | class | Одно открытое соединение с SQLite (через ODBC-драйвер SQLite3) или PostgreSQL (Npgsql). |
| [[TableSchema]] | class | Имя таблицы и определения её колонок. |

## DbUtils

| Тип | Вид | Описание |
|---|---|---|
| [[ProcessManager]] | static class | Поддерживает актуальность таблицы `_processes` по процессам ZennoPoster и `zbe1` этой машины. |
| [[TaskManager]] | static class | Вспомогательные преобразования для входных настроек задачи ZennoPoster (внутреннее). |

## Diagnostic

| Тип | Вид | Описание |
|---|---|---|
| [[Diagnostic]] | static class | Сведения об окружении для логов и диагностики. |
| [[ProjectExtensions (Diagnostic)]] | static class | Методы расширения для `IZennoPosterProjectModel`: помощь в отладке. |
| [[VersionInfo]] | class | Версии окружения узла. |

## Essentials

| Тип | Вид | Описание |
|---|---|---|
| [[Constantes]] | static class | Имя проекта, его таблица в базе и стандартные папки хранилища профилей. |
| [[Env]] | static class | Читает настройки из файла `.env`. |
| [[FunctionStorage]] | static class | Реестр делегатов по имени на весь процесс. |
| [[GVars]] | static class | Глобальные переменные ZennoPoster, хранятся в пространстве имён по имени текущего пользователя Windows. |
| [[Init]] | class | Старт проекта: сессия, диапазон аккаунтов, зашифрованное хранилище и стартовый баннер в логе. |
| [[ISAFU]] | interface | Шифрование, которое использует SAFU (защищённое хранение секретов аккаунтов). |
| [[LogDisabler]] | class | Запрещает ZennoPoster писать собственные файлы логов в папку `Logs` рядом с запущенным исполняемым файлом. |
| [[Logger]] | class | Пишет сообщения в лог ZennoPoster. |
| [[LogLevel]] | enum | Важность сообщения. |
| [[ProjectExtensions (Essentials)]] | static class | Методы расширения для `IZennoPosterProjectModel`: старт, логирование, тайминги и запуск других проектов. |
| [[SAFU]] | static class | Точка входа в защищённое хранилище. |
| [[Time]] | class | Помощники для времени: метки времени, дедлайны, случайные паузы. |
| [[Time.Deadline]] | class | Секундомер, который бросает исключение при превышении предела времени. |
| [[Time.Sleeper]] | class | Случайная пауза в фиксированном диапазоне. |
| [[Vars]] | static class | Короткие методы для переменных проекта: чтение, запись, разбор, счётчики. |
| [[Z3n8SAFU]] | class | Реализация SAFU: AES-256-CBC с тегом HMAC-SHA256, ключи выводятся через PBKDF2-SHA256 (100 000 итераций). |

## Mail

| Тип | Вид | Описание |
|---|---|---|
| [[AnyMessage]] | class | Клиент почтового сервиса AnyMessage (`api.anymessage.shop`): краткосрочные и долгосрочные ящики. |
| [[BestMailBox]] | class | Клиент сервиса временных ящиков BestMailBox (по умолчанию `https://mail.autoz3n.xyz`). |
| [[FirstMail]] | class | Клиент API почты FirstMail (`firstmail.ltd`). |
| [[GmailClient]] | class | Доступ к Gmail через Gmail API по OAuth refresh token. |
| [[MSMail]] | class | Доступ к ящикам Microsoft через Microsoft Graph по OAuth refresh token. |
| [[ProjectExtensions (Mail)]] | static class | Методы расширения для `IZennoPosterProjectModel`: одноразовые коды из почты. |
| [[TempMail]] | class | Клиент сервиса Temp Mail (Privatix) на RapidAPI. |
| [[z3nmail]] | class | Клиент сервиса временных ящиков (по умолчанию `https://mail.autoz3n.xyz`); API то же, что у `BestMailBox`. |

## MethodExtensions

| Тип | Вид | Описание |
|---|---|---|
| [[ListExtensions]] | static class | Методы расширения для списков. |
| [[ProjectExtensions (MethodExtensions)]] | static class | Методы расширения: словари и списки ZennoPoster. |
| [[StringExtensions]] | static class | Методы расширения для строк: hex, Base64, JSON, диапазоны, экранирование Markdown, JWT, пароли. |

## Reports

| Тип | Вид | Описание |
|---|---|---|
| [[Accountant]] | class | HTML-отчёты о балансах аккаунтов из таблицы `_native` с цветовой разметкой по сумме. |
| [[Accountant.HtmlEncoder]] | static class | Помощники для экранирования HTML. |
| [[ProjectExtensions (Reports)]] | static class | Методы расширения для `IZennoPosterProjectModel`: отчёты о балансах. |
| [[Reporter]] | class | Собирает отчёты о прогоне (ошибка или успех) и отправляет их в лог, в Telegram и в строку аккаунта в базе. |

## Requests

| Тип | Вид | Описание |
|---|---|---|
| [[NetHttp]] | class | Блокирующая обёртка над `NetHttpAsync` для C#-кубиков, в которых нельзя использовать await. |
| [[NetHttpAsync]] | class | HTTP-клиент на .NET `HttpClient` с асинхронными методами. |
| [[ProjectExtensions (Requests)]] | static class | Сокращения для запросов `NetHttp` из проекта. |
| [[Rqst]] | class | HTTP-клиент для проектов ZennoPoster: прокси, заголовки и куки берутся из проекта, если не заданы. |
| [[RqstExtensions]] | static class | Сокращения, которые создают `Rqst` для одного запроса. |

## Server

| Тип | Вид | Описание |
|---|---|---|
| [[ZpAuth]] | static class | Токен доступа `ZpServer`: хранение, выдача, проверка запросов. |
| [[ZpServer]] | static class | HTTP-сервер внутри ZennoPoster, который принимает команды от оркестратора напрямую, без базы. |

## Tools

| Тип | Вид | Описание |
|---|---|---|
| [[Extractor]] | static class | Чтение и запись файлов проектов ZennoPoster (.zp) собственным загрузчиком ProjectMaker и поиск по их действиям. |
| [[Extractor.SearchHit]] | class | Одно совпадение `SearchInZp`. |
| [[Helper]] | static class | Инструменты разработчика в виде окон Windows внутри ZennoPoster. |
| [[Img]] | class | Отрисовка SVG. |
| [[Otp]] | static class | Одноразовые коды. |
| [[Rnd]] | static class | Случайные значения: строки, никнеймы, адреса почты, пароли, числа из переменных проекта, паузы. |
| [[ZpToCsx]] | static class | Превращает проект ZennoPoster (.zp) в каркас C#-скрипта (.csx) и собирает файлы .zp из XML. |

## Traffic

| Тип | Вид | Описание |
|---|---|---|
| [[CdpHar]] | class | Запись HAR, которая общается с браузером инстанса через его собственный DevTools endpoint (браузер пишет порт в &lt;user-data-dir&gt;\DevToolsActivePort). |
| [[GraphQL]] | class | Собирает GraphQL-операции из трафика инстанса браузера. |
| [[HarTraffic]] | static class | Выгрузка в HAR 1.2 трафика браузера (`GetTraffic`) и сохранённого трафика `Rqst`. |
| [[InstanceExtensions (Traffic)]] | static class | Методы расширения для `Instance`: запись HAR через DevTools. |
| [[ProjectExtensions (Traffic)]] | class | Методы расширения для `IZennoPosterProjectModel`: выгрузка HAR. |
| [[Traffic]] | class | Читает трафик, записанный активной вкладкой инстанса ZennoPoster (`ActiveTab.GetTraffic`). |
| [[Traffic.TrafficElement]] | class | Один записанный запрос с ответом. |
| [[TrafficCounter]] | static class | Считает трафик по помеченным шагам прогона проекта и выдаёт отчёт в JSON. |
| [[TrafficCounter.TrafficStep]] | class | Один учтённый шаг. |

