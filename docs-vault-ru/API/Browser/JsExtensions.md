---
title: "JsExtensions"
tags: [api, Browser]
generated: z3n7-docgen
---

# JsExtensions

`static class` · пространство имён `z3n7` · исходник [Browser/js.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L18)

```csharp
public static class JsExtensions
```

Методы расширения для `Instance`, которые работают со страницей через JavaScript в активной вкладке.

## Методы

### CenterMouse

```csharp
public static void CenterMouse(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L25)

Отправляет событие `mousemove` элементу в центре окна.

### JsClick

```csharp
public static string JsClick(this Instance instance, string selector, double delay = 1.0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L40)

Находит элемент по CSS-селектору, в том числе внутри shadow root, прокручивает к нему, ставит фокус и отправляет событие `click`.

| Параметр | Описание |
|---|---|
| `selector` | CSS-селектор. |
| `delay` | Сколько секунд ждать перед действием. |

**Возвращает:** Результат скрипта или `Error: {message}` (например, если элемент не найден).

```csharp
public static void JsClick(this Instance instance, int x, int y)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L110)

Отправляет `mousedown`, `mouseup` и `click` в заданных клиентских координатах первому `canvas` страницы.

| Параметр | Описание |
|---|---|
| `x` | Client X. |
| `y` | Client Y. |

```csharp
public static void JsClick(this Instance instance, int[] pos)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L144)

То же, что `JsClick(x, y)` с `[x, y]`.

### JsGet

```csharp
public static string JsGet(this Instance instance, string jsSelector, string property)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L226)

Вычисляет `jsSelector` в элемент и возвращает одно из его свойств: любой атрибут или `innerText`, `innerHTML`, `textContent`, `value`, `checked`, `tagName`.

| Параметр | Описание |
|---|---|
| `jsSelector` | Выражение JavaScript, возвращающее элемент, например `document.querySelector('#id')`. |
| `property` | Имя свойства. |

**Возвращает:** Значение. Бросает исключение, если нет элемента или свойства; в сообщении перечислены доступные.

### JsPost

```csharp
public static string JsPost(this Instance instance, string script, int delay = 0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L271)

Выполняет скрипт в активной вкладке. Двойные кавычки в скрипте сначала заменяются на одинарные.

| Параметр | Описание |
|---|---|
| `script` | JavaScript. |
| `delay` | Сколько секунд ждать перед запуском. |

**Возвращает:** Результат скрипта или сообщение исключения.

### JsSet

```csharp
public static string JsSet(this Instance instance, string selector, string value, double delay = 1.0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L158)

Находит элемент по CSS-селектору, кликает и ставит фокус, очищает и печатает `value` через `insertText`, затем отправляет `input` и `change`.

| Параметр | Описание |
|---|---|
| `selector` | CSS-селектор. |
| `value` | Текст для ввода. |
| `delay` | Сколько секунд ждать перед действием. |

**Возвращает:** Результат скрипта или `Error: {message}`.

