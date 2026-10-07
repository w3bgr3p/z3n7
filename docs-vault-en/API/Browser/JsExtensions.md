---
title: "JsExtensions"
tags: [api, Browser]
generated: z3n7-docgen
---

# JsExtensions

`static class` · namespace `z3n7` · source [Browser/js.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L18)

```csharp
public static class JsExtensions
```

Extension methods on `Instance` that act on the page through JavaScript in the active tab.

## Methods

### CenterMouse

```csharp
public static void CenterMouse(this Instance instance)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L25)

Dispatches a `mousemove` event to the element at the centre of the window.

### JsClick

```csharp
public static string JsClick(this Instance instance, string selector, double delay = 1.0)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L40)

Finds the element by CSS selector, also inside shadow roots, scrolls it into view, focuses it and dispatches a `click` event.

| Parameter | Description |
|---|---|
| `selector` | CSS selector. |
| `delay` | Seconds to wait before acting. |

**Returns:** The script result, or `Error: {message}` (e.g. when the element is not found).

```csharp
public static void JsClick(this Instance instance, int x, int y)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L110)

Dispatches `mousedown`, `mouseup` and `click` at the given client coordinates to the first `canvas` of the page.

| Parameter | Description |
|---|---|
| `x` | Client X. |
| `y` | Client Y. |

```csharp
public static void JsClick(this Instance instance, int[] pos)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L144)

Same as `JsClick(x, y)` with `[x, y]`.

### JsGet

```csharp
public static string JsGet(this Instance instance, string jsSelector, string property)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L226)

Evaluates `jsSelector` to an element and returns one of its properties: any attribute, or `innerText`, `innerHTML`, `textContent`, `value`, `checked`, `tagName`.

| Parameter | Description |
|---|---|
| `jsSelector` | JavaScript expression returning the element, e.g. `document.querySelector('#id')`. |
| `property` | Property name. |

**Returns:** The value. Throws when the element or the property is missing; the message lists the available ones.

### JsPost

```csharp
public static string JsPost(this Instance instance, string script, int delay = 0)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L271)

Runs a script in the active tab. Double quotes in the script are replaced with single quotes first.

| Parameter | Description |
|---|---|
| `script` | JavaScript. |
| `delay` | Seconds to wait before running. |

**Returns:** The script result, or the exception message.

### JsSet

```csharp
public static string JsSet(this Instance instance, string selector, string value, double delay = 1.0)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/js.cs#L158)

Finds the element by CSS selector, clicks and focuses it, clears it and types `value` with `insertText`, then dispatches `input` and `change`.

| Parameter | Description |
|---|---|
| `selector` | CSS selector. |
| `value` | Text to enter. |
| `delay` | Seconds to wait before acting. |

**Returns:** The script result, or `Error: {message}`.

