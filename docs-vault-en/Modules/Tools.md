# Tools

Random data, one-time codes, images, and tools for ZennoPoster project files.

| Type | What it does |
|---|---|
| [[Rnd]] | Random strings, nicknames, e-mail addresses, passwords, numbers from variables, pauses. |
| [[Otp]] | TOTP codes from a Base32 secret; codes from FirstMail. |
| [[Img]] | SVG to PNG. |
| [[Extractor]] | Unpacking and repacking .zp files, searching text in their actions (ProjectMaker only). |
| [[ZpToCsx]] | A C# script outline from a .zp project (ProjectMaker only). |
| [[Helper]] | `project.Help()`: an API browser window. |
| [[Diagnostic]], [[ProjectExtensions (Diagnostic)]] | Environment versions; debug screenshots with a watermark; errors from traffic. |

```csharp
string code = z3n7.Tools.Otp.Offline(totpSecret);
project.Profile.Password = Rnd.RndPass();
var hits = project.SearchInZp("ChooseAccountByCondition");   // in ProjectMaker
```

API: [[API reference#Tools|Tools]], [[API reference#Diagnostic|Diagnostic]]. Source: [`Tools/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/Tools), [`Diagnostic/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/Diagnostic)
