# Внешние API

Клиенты сторонних сервисов.

| Тип | Сервис |
|---|---|
| [[Telegram]] | Сообщения в топик чата через Bot API; длинные тексты делятся. Учётные данные: строка `tg_logger` в `_api`. |
| [[Webshare]] | Список прокси аккаунта Webshare. |
| [[Aiio]] | Чат-API io.net intelligence; ключи из таблицы `__aiio`. |
| [[OmniRoute]] | Локальный OpenAI-совместимый роутер на `localhost:20128`, текст и изображения. |
| [[ZennoBrowser]], [[ZbDbManager]] | Id профилей ZennoBrowser (ZP8) из его `ProfileManagement.db` и вспомогательный проект `ZB.zp`. |

```csharp
var ai = new OmniRoute();
if (ai.Check())
    project.Var("answer", ai.Complete(model, "You are terse.", "Summarise: ..."));
```

API: [[Справочник API#Api|Api]]. Исходники: [`Api/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/Api)
