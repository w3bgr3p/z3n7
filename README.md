<p align="center"><img src=".github/logo.png" alt="z3n7" width="160"></p>

# z3n7

`z3n7` is a helper .NET library for ZennoPoster projects.

## Requirements

- .NET Framework 4.8
- ZennoPoster 7.7.21 or later

## Installation

1. Copy `z3n7.dll` into the `ExternalAssemblies` folder of your ZennoPoster installation.
2. Add `z3n7` to the using directives in the project settings.

Video guide: [adding external assemblies in ZennoPoster](https://www.youtube.com/watch?v=Wp6gP_qig_c)

## Dependencies

Embedded into `z3n7.dll` with Costura.Fody (see `z3n7/FodyWeavers.xml`):
Npgsql, AForge.Imaging, System.Net.Http, Svg, ZXing.Net, Otp.NET.

Referenced but not embedded: Newtonsoft.Json, HtmlAgilityPack, Microsoft.CSharp,
System.Management, System.Data.Odbc.

## Source layout

- `z3n7/Accounts`
- `z3n7/Api`
- `z3n7/Browser`
- `z3n7/Db`
- `z3n7/DbUtils`
- `z3n7/Diagnostic`
- `z3n7/Essentials`
- `z3n7/Mail`
- `z3n7/MethodExtensions`
- `z3n7/Reports`
- `z3n7/Requests`
- `z3n7/Server`
- `z3n7/Tools`
- `z3n7/Traffic`

## Building

The project references ZennoLab assemblies from the `Progs` folder of an installed ZennoPoster.
The default path is set in `ZennoLab.props`; override it with the `ZennoProgs` environment
variable or `/p:ZennoProgs=<path to Progs>`.

```bash
dotnet build z3n7.sln -c Release
```

## Documentation

- English: [`docs-vault-en/`](docs-vault-en/)
- Russian: [`docs-vault-ru/`](docs-vault-ru/)
