---
title: "InstanceExtensions (Browser)"
tags: [api, Browser]
generated: z3n7-docgen
---

# InstanceExtensions (Browser)

`static class` · namespace `z3n7` · source [Browser/Canvas.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L17), [Browser/Cookies.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L594), [Browser/InstanceExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L14)

```csharp
public static class InstanceExtensions
```

Other parts of this type: [[InstanceExtensions (Traffic)]]

*No description yet.*

## Methods

### CenterArea

```csharp
public static int[] CenterArea(this Instance instance, int width = 0, int height = 0)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L590)

Returns [x, y, width, height]. If width=0 and height=0, returns full viewport

### ClearShit

```csharp
public static void ClearShit(this Instance instance, string domain)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L573)

### ClickCenter

```csharp
public static int[] ClickCenter(this Instance instance)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L581)

### ClickImg

```csharp
public static int[] ClickImg(this Instance instance, string imgFile, int[] searchArea, float threshold = 0.99f, bool nativeSearch = true, int delay = 0)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L538)

### CloseExtraTabs

```csharp
public static void CloseExtraTabs(this Instance instance, bool blank = false, int tabToKeep = 1)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L582)

### CloseNewTab

```csharp
public static void CloseNewTab(this Instance instance, int deadline = 10, int tabIndex = 2, bool thrw = true)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L601)

### ConvertToSupportedFormat

```csharp
public static Bitmap ConvertToSupportedFormat(Bitmap source)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L24)

AForge requires 24bppRgb/32bppRgb/8bppIndexed, converts to 24bppRgb with white background

### CtrlV

```csharp
public static void CtrlV(this Instance instance, string ToPaste)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L649)

### Down

```csharp
public static void Down(this Instance instance, int pauseAfterMs = 5000)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L684)

### F5

```csharp
public static void F5(this Instance instance, bool WaitTillLoad = true)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L635)

### FindAllInScreenshot

```csharp
public static Dictionary<string, List<int[]>> FindAllInScreenshot(this Instance instance, Dictionary<string, string> templates, int[] searchArea, float threshold = 0.9f, int minDistance = 30)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L198)

Finds all matches, filters by minDistance to avoid duplicates

### FindImg

```csharp
public static int[] FindImg(this Instance instance, string imgFile, int[] searchArea, double threshold = 0.99)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L78)

searchArea: [x, y, width, height]

### FindImgFast

```csharp
public static int[] FindImgFast(this Instance instance, string imgFile, int[] searchArea, float threshold = 0.99f, bool thrw = true)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L116)

Single screenshot approach, memory-optimized

### FindMultipleInCachedScreenshot

```csharp
public static Dictionary<string, int[]> FindMultipleInCachedScreenshot(string base64Screenshot, Dictionary<string, string> templates, int[] searchArea, float threshold = 0.95f)
```

[source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L435)

Reuses cached screenshot base64, no Instance required

### FindMultipleInMultipleAreas

```csharp
public static Dictionary<string, int[]> FindMultipleInMultipleAreas(this Instance instance, Dictionary<string, (string template, int[] area)> templatesWithAreas, float threshold = 0.95f)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L361)

Single screenshot, each template has its own search area

### FindMultipleInScreenshot

```csharp
public static Dictionary<string, int[]> FindMultipleInScreenshot(this Instance instance, Dictionary<string, string> templates, int[] searchArea, float threshold = 0.95f)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L293)

Single screenshot, multiple templates search

### FixTimezone

```csharp
public static void FixTimezone(this Instance instance, IZennoPosterProjectModel project)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L735)

### GetCenter

```csharp
public static int[] GetCenter(this Instance instance)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L554)

### GetCookies

```csharp
public static string GetCookies(this Instance instance, IZennoPosterProjectModel project)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L596)

### GetHe

```csharp
public static HtmlElement GetHe(this Instance instance, object obj, string method = "")
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L31)

### Go

```csharp
public static void Go(this Instance instance, string url, bool strict = false, bool waitTdle = false, bool newTab = false)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L619)

### HeCatch

```csharp
public static string HeCatch(this Instance instance, object obj, string method = "", int deadline = 10, string atr = "innertext", int delay = 1, string pathToScript = null)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L198)

```csharp
public static string HeCatch(this Instance instance, IZennoPosterProjectModel project, object obj, string method = "", int deadline = 10, string atr = "innertext", int delay = 1, string pathToScript = null)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L233)

### HeClick

```csharp
public static void HeClick(this Instance instance, object obj, string method = "", int deadline = 10, double delay = 1, string comment = "", bool thrw = true, bool thr0w = true, int emu = 0, string pathToScript = null)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L276)

### HeDragAndDrop

```csharp
public static Point HeDragAndDrop(this Instance instance, HtmlElement element, int offsetX, int offsetY = 0)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L504)

### HeDrop

```csharp
public static void HeDrop(this Instance instance, object obj, string method = "", int deadline = 10, bool thrw = true)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L475)

### HeGet

```csharp
public static string HeGet(this Instance instance, object obj, string method = "", int deadline = 10, string atr = "innertext", int delay = 1, bool thrw = true, bool thr0w = true, bool waitTillVoid = false, string pathToScript = null)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L135)

### HeLongClick

```csharp
public static void HeLongClick(this Instance instance, object obj, int holdMs = 3, string method = "", int deadline = 10, double delay = 1, string comment = "", bool thrw = true, bool thr0w = true, int emu = 0)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L339)

```csharp
public static void HeLongClick(this Instance instance, int x, int y, int holdMs = 3, double delay = 1, int emu = 0)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L390)

### HeMultiClick

```csharp
public static void HeMultiClick(this Instance instance, List<object> selectors)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L270)

### HePeakRandom

```csharp
public static void HePeakRandom(this Instance instance, object obj, int min = 1, int max = 10)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L555)

### HeSet

```csharp
public static void HeSet(this Instance instance, object obj, string value, string method = "id", int deadline = 10, double delay = 1, string comment = "", bool thrw = true, bool thr0w = true, int emu = 0, string pathToScript = null)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L436)

### MousePOsCenter

```csharp
public static int[] MousePOsCenter(this Instance instance, bool moveMouse = false)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L560)

### SaveCookies

```csharp
public static string SaveCookies(this Instance instance)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L695)

### ScrollDown

```csharp
public static void ScrollDown(this Instance instance, int y = 420)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L641)

### SetTimeFromDb

```csharp
public static void SetTimeFromDb(this Instance instance, IZennoPosterProjectModel project)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L721)

### SwipeFromCenter

```csharp
public static int[] SwipeFromCenter(this Instance instance, int distance, string direction = null, int[] bounds = null)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L615)

direction: left, right, up, down. Random if null. Coordinates limited by bounds [x, y, width, height]

### SwipeImgToCenter

```csharp
public static int[] SwipeImgToCenter(this Instance instance, string imgFile, int[] searchArea, float threshold = 0.95f, bool nativeSearch = false)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L664)

### TapCenter

```csharp
public static int[] TapCenter(this Instance instance)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L574)

### TapImg

```csharp
public static int[] TapImg(this Instance instance, string imgFile, int[] searchArea, float threshold = 0.99f, bool nativeSearch = false, int delay = 0)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L527)

### UpEmpty

```csharp
public static void UpEmpty(this Instance instance)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L679)

### UpFromFolder

```csharp
public static void UpFromFolder(this Instance instance, string pathProfile, bool useProfile = false, BrowserType browserType = BrowserType.Chromium)
```

Extension method for `Instance`. [source](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L669)

> This page is generated from the source code. Do not edit it: changes will be overwritten.
