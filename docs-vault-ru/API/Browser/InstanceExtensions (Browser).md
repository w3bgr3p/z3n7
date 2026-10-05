---
title: "InstanceExtensions (Browser)"
tags: [api, Browser]
generated: z3n7-docgen
---

# InstanceExtensions (Browser)

`static class` · пространство имён `z3n7` · исходник [Browser/Canvas.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L18), [Browser/Cookies.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L676), [Browser/InstanceExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L14)

```csharp
public static class InstanceExtensions
```

Другие части этого типа: [[InstanceExtensions (Traffic)]]

Extension methods on `Instance`: image search on page screenshots, clicks, taps and swipes by coordinates, viewport helpers.

## Методы

### CenterArea

```csharp
public static int[] CenterArea(this Instance instance, int width = 0, int height = 0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L652)

Area `[x, y, width, height]` of the given size centred in the viewport.

| Параметр | Описание |
|---|---|
| `width` | Width; 0 together with `height` = 0 returns the whole viewport. |
| `height` | Height; 0 means equal to `width`. |

### ClearShit

```csharp
public static void ClearShit(this Instance instance, string domain)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L733)

Closes all tabs, clears cache and cookies of `domain` and opens `about:blank`.

### ClickCenter

```csharp
public static int[] ClickCenter(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L641)

Clicks the viewport centre; returns the point.

### ClickImg

```csharp
public static int[] ClickImg(this Instance instance, string imgFile, int[] searchArea, float threshold = 0.99f, bool nativeSearch = true, int delay = 0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L592)

Finds the image and clicks its centre.

| Параметр | Описание |
|---|---|
| `imgFile` | Template image: a file path (.png, .jpg, .jpeg, .gif, .bmp, .webp) or Base64. |
| `searchArea` | Search area `[x, y, width, height]` in page pixels. |
| `threshold` | Required similarity, 0–1. |
| `nativeSearch` | Use `FindImg` (default) instead of `FindImgFast`. |
| `delay` | Seconds to wait before clicking. |

**Возвращает:** The clicked point.

### CloseExtraTabs

```csharp
public static void CloseExtraTabs(this Instance instance, bool blank = false, int tabToKeep = 1)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L745)

Closes every tab after the first `tabToKeep`.

| Параметр | Описание |
|---|---|
| `blank` | Then open `about:blank` in the active tab. |
| `tabToKeep` | How many tabs to keep. |

### CloseNewTab

```csharp
public static void CloseNewTab(this Instance instance, int deadline = 10, int tabIndex = 2, bool thrw = true)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L768)

Waits until the number of tabs equals `tabIndex` and closes all but the first.

| Параметр | Описание |
|---|---|
| `deadline` | Seconds to wait. |
| `tabIndex` | Tab count to wait for. |
| `thrw` | Throw when it does not happen in time. |

### ConvertToSupportedFormat

```csharp
public static Bitmap ConvertToSupportedFormat(Bitmap source)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L28)

Returns the bitmap in 24bpp RGB (on a white background), the format AForge template matching needs; a 24bpp bitmap is returned as is.

### CtrlV

```csharp
public static void CtrlV(this Instance instance, string ToPaste)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L829)

Pastes text through the Windows clipboard (Ctrl+V); the previous clipboard text is restored. Errors are ignored.

### Down

```csharp
public static void Down(this Instance instance, int pauseAfterMs = 5000)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L871)

Closes the browser (launches "without browser") and waits.

| Параметр | Описание |
|---|---|
| `pauseAfterMs` | Pause afterwards, ms. |

### F5

```csharp
public static void F5(this Instance instance, bool WaitTillLoad = true)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L809)

Reloads the page.

| Параметр | Описание |
|---|---|
| `WaitTillLoad` | Wait for loading to finish. |

### FindAllInScreenshot

```csharp
public static Dictionary<string, List<int[]>> FindAllInScreenshot(this Instance instance, Dictionary<string, string> templates, int[] searchArea, float threshold = 0.9f, int minDistance = 30)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L224)

Takes one page preview (`GetPagePreview`) and finds every occurrence of each template in the area. Matches closer than `minDistance` to a better one are dropped.

| Параметр | Описание |
|---|---|
| `templates` | Name → Base64 image. |
| `searchArea` | Search area `[x, y, width, height]` in page pixels. |
| `threshold` | Required similarity, 0–1. |
| `minDistance` | Minimum distance between reported centres, px. |

**Возвращает:** Name → centres `[x, y]`; templates without matches or with unreadable images are left out.

### FindImg

```csharp
public static int[] FindImg(this Instance instance, string imgFile, int[] searchArea, double threshold = 0.99)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L86)

Finds an image in the area with ZennoPoster's own image search.

| Параметр | Описание |
|---|---|
| `imgFile` | Template image: a file path (.png, .jpg, .jpeg, .gif, .bmp, .webp) or Base64. |
| `searchArea` | Search area `[x, y, width, height]` in page pixels. |
| `threshold` | Required similarity, 0–1. |

**Возвращает:** Centre `[x, y]` of the match, or `null` when not found.

### FindImgFast

```csharp
public static int[] FindImgFast(this Instance instance, string imgFile, int[] searchArea, float threshold = 0.99f, bool thrw = true)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L132)

Takes one page preview (`GetPagePreview`) and finds the image in the area with AForge template matching.

| Параметр | Описание |
|---|---|
| `imgFile` | Template image: a file path (.png, .jpg, .jpeg, .gif, .bmp, .webp) or Base64. |
| `searchArea` | Search area `[x, y, width, height]` in page pixels. |
| `threshold` | Required similarity, 0–1. |
| `thrw` | Throw when not found; otherwise return `null`. |

**Возвращает:** Centre `[x, y]` of the first match.

### FindMultipleInCachedScreenshot

```csharp
public static Dictionary<string, int[]> FindMultipleInCachedScreenshot(string base64Screenshot, Dictionary<string, string> templates, int[] searchArea, float threshold = 0.95f)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L475)

Like `FindMultipleInScreenshot`, on a screenshot taken earlier.

| Параметр | Описание |
|---|---|
| `base64Screenshot` | Screenshot as Base64. |
| `templates` | Name → Base64 image. |
| `searchArea` | Search area `[x, y, width, height]` in page pixels. |
| `threshold` | Required similarity, 0–1. |

### FindMultipleInMultipleAreas

```csharp
public static Dictionary<string, int[]> FindMultipleInMultipleAreas(this Instance instance, Dictionary<string, (string template, int[] area)> templatesWithAreas, float threshold = 0.95f)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L397)

Takes one page preview (`GetPagePreview`) and finds each template in its own area.

| Параметр | Описание |
|---|---|
| `templatesWithAreas` | Name → (Base64 image, area `[x, y, width, height]`). |
| `threshold` | Required similarity, 0–1. |

**Возвращает:** Name → centre `[x, y]`; templates without a match are left out.

### FindMultipleInScreenshot

```csharp
public static Dictionary<string, int[]> FindMultipleInScreenshot(this Instance instance, Dictionary<string, string> templates, int[] searchArea, float threshold = 0.95f)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L326)

Takes one page preview (`GetPagePreview`) and finds the first match of each template in the area.

| Параметр | Описание |
|---|---|
| `templates` | Name → Base64 image. |
| `searchArea` | Search area `[x, y, width, height]` in page pixels. |
| `threshold` | Required similarity, 0–1. |

**Возвращает:** Name → centre `[x, y]`; templates without a match are left out.

### FixTimezone

```csharp
public static void FixTimezone(this Instance instance, IZennoPosterProjectModel project)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L932)

Opens `browserscan.net`, takes the IP timezone from its visitor-IP request in the traffic and sets it as the instance's IANA timezone.

**Примечания:** Throws when the response does not arrive within about 60 seconds or has no timezone.

### GetCenter

```csharp
public static int[] GetCenter(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L609)

Centre `[x, y]` of the page viewport (`window.innerWidth/innerHeight`).

### GetCookies

```csharp
public static string GetCookies(this Instance instance, IZennoPosterProjectModel project)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L684)

Collects fresh cookies from 5–15 random popular sites with `CookieCollector` (the profile's user agent and languages, requests sent directly without the instance proxy) and loads them into the instance.

**Возвращает:** The cookies in Netscape format.

### GetHe

```csharp
public static HtmlElement GetHe(this Instance instance, object obj, string method = "")
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L41)

Finds an element in the active tab.

| Параметр | Описание |
|---|---|
| `obj` | Element: an `HtmlElement`; `(value, "id")` or `(value, "name")`; or `(tag, attribute, pattern, mode, index)` as in `FindElementByAttribute`. |
| `method` | For a 5-part selector: `random` picks a random match, `last` the last one; otherwise the index is used. |

**Возвращает:** The element. Throws when it is not found or the selector shape is unsupported.

### Go

```csharp
public static void Go(this Instance instance, string url, bool strict = false, bool waitTdle = false, bool newTab = false)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L791)

Navigates the active tab unless it is already on the URL.

| Параметр | Описание |
|---|---|
| `url` | Target URL. |
| `strict` | Compare the whole URL; otherwise skip when the current URL contains it. |
| `waitTdle` | Wait for loading to finish. |
| `newTab` | Open a new tab first. |

### HeCatch

```csharp
public static string HeCatch(this Instance instance, object obj, string method = "", int deadline = 10, string atr = "innertext", int delay = 1, string pathToScript = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L247)

Watches for an element that must not appear (e.g. an error message) for `deadline` seconds.

| Параметр | Описание |
|---|---|
| `obj` | Element: an `HtmlElement`; `(value, "id")` or `(value, "name")`; or `(tag, attribute, pattern, mode, index)` as in `FindElementByAttribute`. |
| `method` | For a 5-part selector: `random` picks a random match, `last` the last one; otherwise the index is used. |
| `deadline` | Seconds to keep looking (every 0.5 s). |
| `atr` | Attribute used as the exception message. |
| `delay` | Seconds to wait before starting. |
| `pathToScript` | Not used. |

**Возвращает:** `null` when the element never appeared. When it appears, throws an exception whose message is its `atr`.

```csharp
public static string HeCatch(this Instance instance, IZennoPosterProjectModel project, object obj, string method = "", int deadline = 10, string atr = "innertext", int delay = 1, string pathToScript = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L299)

Same as the overload without `project`; also stores the message in the `err` variable before throwing.

| Параметр | Описание |
|---|---|
| `project` | Project for the `err` variable. |
| `obj` | Element: an `HtmlElement`; `(value, "id")` or `(value, "name")`; or `(tag, attribute, pattern, mode, index)` as in `FindElementByAttribute`. |
| `method` | For a 5-part selector: `random` picks a random match, `last` the last one; otherwise the index is used. |
| `deadline` | Seconds to keep looking (every 0.5 s). |
| `atr` | Attribute used as the exception message. |
| `delay` | Seconds to wait before starting. |
| `pathToScript` | Not used. |

### HeClick

```csharp
public static void HeClick(this Instance instance, object obj, string method = "", int deadline = 10, double delay = 1, string comment = "", bool thrw = true, bool thr0w = true, int emu = 0, string pathToScript = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L361)

Waits for an element and clicks it after a random pause of about 1–1.3 s × `delay`.

| Параметр | Описание |
|---|---|
| `obj` | Element: an `HtmlElement`; `(value, "id")` or `(value, "name")`; or `(tag, attribute, pattern, mode, index)` as in `FindElementByAttribute`. |
| `method` | For a 5-part selector: `random` picks a random match, `last` the last one; otherwise the index is used. `clickOut` keeps clicking until the element disappears. |
| `deadline` | Seconds to keep looking (every 0.5 s). |
| `delay` | Multiplier of the pause before clicking. |
| `comment` | Text added to the timeout message. |
| `thrw` | Throw when the element is not found in time; otherwise return quietly. |
| `thr0w` | Legacy switch: `false` also turns `thrw` off. |
| `emu` | 1 — use full mouse emulation for this action, −1 — turn it off, 0 — leave the instance setting. |
| `pathToScript` | When set, appends the action and the element's XPath to this file. |

### HeDragAndDrop

```csharp
public static Point HeDragAndDrop(this Instance instance, HtmlElement element, int offsetX, int offsetY = 0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L656)

Drags from the element's centre by the given offset with a human-like path: easing, slight vertical wobble and, for longer moves, a small overshoot and correction.

| Параметр | Описание |
|---|---|
| `element` | Element to drag. |
| `offsetX` | Horizontal offset, px. |
| `offsetY` | Vertical offset, px. |

**Возвращает:** The drop point.

### HeDrop

```csharp
public static void HeDrop(this Instance instance, object obj, string method = "", int deadline = 10, bool thrw = true)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L619)

Waits for an element and removes it from the page.

| Параметр | Описание |
|---|---|
| `obj` | Element: an `HtmlElement`; `(value, "id")` or `(value, "name")`; or `(tag, attribute, pattern, mode, index)` as in `FindElementByAttribute`. |
| `method` | For a 5-part selector: `random` picks a random match, `last` the last one; otherwise the index is used. |
| `deadline` | Seconds to keep looking (every 0.5 s). |
| `thrw` | Throw when the element is not found in time; otherwise return quietly. |

### HeGet

```csharp
public static string HeGet(this Instance instance, object obj, string method = "", int deadline = 10, string atr = "innertext", int delay = 1, bool thrw = true, bool thr0w = true, bool waitTillVoid = false, string pathToScript = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L165)

Waits for an element and returns one of its attributes.

| Параметр | Описание |
|---|---|
| `obj` | Element: an `HtmlElement`; `(value, "id")` or `(value, "name")`; or `(tag, attribute, pattern, mode, index)` as in `FindElementByAttribute`. |
| `method` | For a 5-part selector: `random` picks a random match, `last` the last one; otherwise the index is used. |
| `deadline` | Seconds to keep looking (every 0.5 s). |
| `atr` | Attribute to read. |
| `delay` | Seconds to wait after finding it. |
| `thrw` | Throw when the element is not found in time; otherwise return quietly. |
| `thr0w` | Legacy switch: `false` also turns `thrw` off. |
| `waitTillVoid` | Wait until the element is gone instead; returns `null` at the deadline and throws while it is present. |
| `pathToScript` | When set, appends the action and the element's XPath to this file. |

**Возвращает:** The attribute value, or `null` when not found and `thrw` is false.

### HeLongClick

```csharp
public static void HeLongClick(this Instance instance, object obj, int holdMs = 3, string method = "", int deadline = 10, double delay = 1, string comment = "", bool thrw = true, bool thr0w = true, int emu = 0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L442)

Waits for an element and holds the left button at a random point inside it.

| Параметр | Описание |
|---|---|
| `obj` | Element: an `HtmlElement`; `(value, "id")` or `(value, "name")`; or `(tag, attribute, pattern, mode, index)` as in `FindElementByAttribute`. |
| `holdMs` | Hold time in milliseconds. |
| `method` | For a 5-part selector: `random` picks a random match, `last` the last one; otherwise the index is used. |
| `deadline` | Seconds to keep looking (every 0.5 s). |
| `delay` | Multiplier of the pause before pressing. |
| `comment` | Text added to the timeout message. |
| `thrw` | Throw when the element is not found in time; otherwise return quietly. |
| `thr0w` | Legacy switch: `false` also turns `thrw` off. |
| `emu` | 1 — use full mouse emulation for this action, −1 — turn it off, 0 — leave the instance setting. |

```csharp
public static void HeLongClick(this Instance instance, int x, int y, int holdMs = 3, double delay = 1, int emu = 0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L501)

Holds the left button at a point.

| Параметр | Описание |
|---|---|
| `x` | X in the tab. |
| `y` | Y in the tab. |
| `holdMs` | Hold time in milliseconds. |
| `delay` | Multiplier of the pause before pressing. |
| `emu` | 1 — use full mouse emulation for this action, −1 — turn it off, 0 — leave the instance setting. |

### HeMultiClick

```csharp
public static void HeMultiClick(this Instance instance, List<object> selectors)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L337)

Clicks each element in turn with `HeClick` defaults.

### HePeakRandom

```csharp
public static void HePeakRandom(this Instance instance, object obj, int min = 1, int max = 10)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L714)

Opens a drop-down by two long clicks and presses Down a random number of times.

| Параметр | Описание |
|---|---|
| `obj` | Element: an `HtmlElement`; `(value, "id")` or `(value, "name")`; or `(tag, attribute, pattern, mode, index)` as in `FindElementByAttribute`. |
| `min` | Fewest presses. |
| `max` | Upper bound of presses (exclusive). |

### HeSet

```csharp
public static void HeSet(this Instance instance, object obj, string value, string method = "id", int deadline = 10, double delay = 1, string comment = "", bool thrw = true, bool thr0w = true, int emu = 0, string pathToScript = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L569)

Waits for an input and enters `value` after a random pause of about 1.3–2 s × `delay`.

| Параметр | Описание |
|---|---|
| `obj` | Element: an `HtmlElement`; `(value, "id")` or `(value, "name")`; or `(tag, attribute, pattern, mode, index)` as in `FindElementByAttribute`. |
| `value` | Text to enter. |
| `method` | For a 5-part selector: `random` picks a random match, `last` the last one; otherwise the index is used. |
| `deadline` | Seconds to keep looking (every 0.5 s). |
| `delay` | Multiplier of the pause. |
| `comment` | Text added to the timeout message. |
| `thrw` | Throw when the element is not found in time; otherwise return quietly. |
| `thr0w` | Legacy switch: `false` also turns `thrw` off. |
| `emu` | 0 — set the value with ZennoPoster's full emulation; above 0 — click the field and type the text; below 0 — nothing is entered. |
| `pathToScript` | When set, appends the action and the element's XPath to this file. |

### MousePOsCenter

```csharp
public static int[] MousePOsCenter(this Instance instance, bool moveMouse = false)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L618)

Turns on full mouse emulation and puts the cursor at the viewport centre.

| Параметр | Описание |
|---|---|
| `moveMouse` | Move the cursor there instead of setting its position. |

**Возвращает:** The centre.

### SaveCookies

```csharp
public static string SaveCookies(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L883)

Returns the instance's cookies as saved by `SaveCookie` (through a temporary file).

### ScrollDown

```csharp
public static void ScrollDown(this Instance instance, int y = 420)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L817)

Scrolls with the emulated mouse wheel.

| Параметр | Описание |
|---|---|
| `y` | Wheel delta. |

### SetTimeFromDb

```csharp
public static void SetTimeFromDb(this Instance instance, IZennoPosterProjectModel project)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L913)

Sets timezone emulation from the `timezone` JSON (`timezoneOffset`, `timezoneName`) of the account's `_instance` row; warns when there is none.

### SwipeFromCenter

```csharp
public static int[] SwipeFromCenter(this Instance instance, int distance, string direction = null, int[] bounds = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L681)

Swipes from the viewport centre.

| Параметр | Описание |
|---|---|
| `distance` | Swipe length, px. |
| `direction` | `left`, `right`, `up` or `down`; random when empty. |
| `bounds` | Keep the end point inside `[x, y, width, height]`. |

**Возвращает:** The end point.

### SwipeImgToCenter

```csharp
public static int[] SwipeImgToCenter(this Instance instance, string imgFile, int[] searchArea, float threshold = 0.95f, bool nativeSearch = false)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L736)

Finds the image and swipes from it to the viewport centre.

| Параметр | Описание |
|---|---|
| `imgFile` | Template image: a file path (.png, .jpg, .jpeg, .gif, .bmp, .webp) or Base64. |
| `searchArea` | Search area `[x, y, width, height]` in page pixels. |
| `threshold` | Required similarity, 0–1. |
| `nativeSearch` | Use `FindImg` instead of `FindImgFast`. |

**Возвращает:** The viewport centre.

### TapCenter

```csharp
public static int[] TapCenter(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L633)

Taps the viewport centre; returns the point.

### TapImg

```csharp
public static int[] TapImg(this Instance instance, string imgFile, int[] searchArea, float threshold = 0.99f, bool nativeSearch = false, int delay = 0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L574)

Finds the image and taps its centre (touch event).

| Параметр | Описание |
|---|---|
| `imgFile` | Template image: a file path (.png, .jpg, .jpeg, .gif, .bmp, .webp) or Base64. |
| `searchArea` | Search area `[x, y, width, height]` in page pixels. |
| `threshold` | Required similarity, 0–1. |
| `nativeSearch` | Use `FindImg` instead of `FindImgFast`. |
| `delay` | Seconds to wait before tapping. |

**Возвращает:** The tapped point.

### UpEmpty

```csharp
public static void UpEmpty(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L864)

Launches Chromium without a profile folder.

### UpFromFolder

```csharp
public static void UpFromFolder(this Instance instance, string pathProfile, bool useProfile = false, BrowserType browserType = BrowserType.Chromium)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L853)

Launches the browser with a profile folder.

| Параметр | Описание |
|---|---|
| `pathProfile` | Profile folder. |
| `useProfile` | Apply the ZennoPoster profile too. |
| `browserType` | Browser to launch. |

> Страница собрана из исходного кода. Не правь её руками: изменения будут перезаписаны.
