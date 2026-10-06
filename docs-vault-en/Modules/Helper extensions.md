# Helper extensions

Extension methods on strings, lists and dictionaries.

| Type | What it does |
|---|---|
| [[StringExtensions]] | Hex ↔ number (with gwei/eth scaling), Base64, flattening JSON, account ranges, Telegram Markdown escaping, JWT decoding, passwords. |
| [[ListExtensions]] | `Rnd`: a random item, optionally removed. |
| [[ProjectExtensions (MethodExtensions)]] | ZennoPoster lists ↔ `List<string>` (`ListSync`, `RndFromList`, `ListFromFile`), `DicToVars`, `ToJson`. |

```csharp
string wei = "0.01".StringToHex("eth");          // 0x2386F26FC10000
string next = project.RndFromList("proxies", remove: true);
```

API: [[API reference#MethodExtensions|MethodExtensions]]. Source: [`MethodExtensions/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/MethodExtensions)
