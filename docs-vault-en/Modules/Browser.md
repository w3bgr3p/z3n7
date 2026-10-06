# Browser

Working with the page and the browser instance: finding and clicking elements, typing, JavaScript, cookies, extensions, image search on screenshots, fingerprint settings.

| Type | What it does |
|---|---|
| [[InstanceExtensions (Browser)]] | `HeGet`, `HeClick`, `HeSet`, `HeCatch`, `HeDrop`, `HeDragAndDrop`; tab and launch helpers; image search (`FindImg`, `FindImgFast`, `ClickImg`, `TapImg`), swipes. |
| [[JsExtensions]] | `JsClick`, `JsSet`, `JsGet`, `JsPost`: the same through JavaScript, shadow DOM included. |
| [[Cookies]] | Read, save and load cookies; JSON ↔ Netscape; stored cookies in the account's row. |
| [[CookieCollector]] | Collects fresh cookies from popular sites over HTTP. |
| [[Extension]] | Installs, enables and disables Chrome extensions. |
| [[GpuSpoof]] | Picks a WebGL vendor/renderer pair of the machine's GPU architecture. |
| [[BetterBrowser]] | Applies a browser profile matching the proxy exit and checks the fingerprint. |
| [[BrowserScan]] | Reads the browserscan.net report and fixes the timezone. |
| [[HtmlExtensions]] | Element centre, QR decoding, XPath of an element. |

Elements are addressed by a tuple: `("id-value", "id")`, `("name-value", "name")` or `(tag, attribute, pattern, mode, index)` as in ZennoPoster's `FindElementByAttribute`.

```csharp
instance.HeSet(("email", "name"), project.Profile.Email);
instance.HeClick(("button", "innertext", "Continue", "regexp", 0));
string title = instance.HeGet(("h1", "fulltagname", "h1", "regexp", 0));
```

API: [[API reference#Browser|Browser]]. Source: [`Browser/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/Browser)
