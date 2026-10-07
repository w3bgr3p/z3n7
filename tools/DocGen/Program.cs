using System.Text;

namespace DocGen;

/// <summary>
/// Generates the API/ section of the documentation vaults from the library sources.
///
///   dotnet run --project tools/DocGen                 write docs-vault-en/API and docs-vault-ru/API
///   dotnet run --project tools/DocGen -- --check      exit 1 if the committed pages are out of date
///   dotnet run --project tools/DocGen -- --todo       write tools/DocGen/i18n/ru.todo.json: texts without a translation
///
/// Russian descriptions come from tools/DocGen/i18n/ru.json (English text → Russian text).
/// A text without a usable translation stays English on the Russian page.
/// </summary>
public static class Program
{
    const int ExitOk = 0, ExitStale = 1, ExitConfig = 2, ExitFailed = 3;
    const string SourceUrl = "https://github.com/w3bgr3p/z3n7/blob/master/z3n7/";

    public static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        var stage = "args";
        try
        {
            var check = args.Contains("--check");
            var todo = args.Contains("--todo");
            var repo = FindRepoRoot(Directory.GetCurrentDirectory());
            var projectDir = Path.Combine(repo, "z3n7");
            var vaults = new[] { ("docs-vault-en", "en"), ("docs-vault-ru", "ru") };

            stage = "sources";
            var files = Sources.Collect(projectDir);

            stage = "parse";
            var pages = Parser.Parse(projectDir, files);
            Console.WriteLine($"{files.Count} source files, {pages.Count} pages, " +
                              $"{pages.Sum(p => p.Members.Count)} members");

            var stale = new List<string>();
            foreach (var (dir, lang) in vaults)
            {
                stage = $"render {dir}";
                var i18n = Path.Combine(repo, "tools", "DocGen", "i18n");
                var tr = lang == "en" ? Translations.None : Translations.Load(Path.Combine(i18n, lang + ".json"));
                var rendered = new Writer(pages, Strings.For(lang), SourceUrl, tr).Render();
                if (tr.Active)
                {
                    Console.WriteLine($"  {lang}: untranslated {tr.Missing.Count}, rejected {tr.Rejected.Count}, unused {tr.Unused.Count()}");
                    foreach (var r in tr.Rejected) Console.WriteLine($"  [rejected: code spans or links differ] {r[..Math.Min(r.Length, 120)]}");
                    if (todo) Translations.WriteMap(Path.Combine(i18n, lang + ".todo.json"), tr.Missing.ToDictionary(k => k, _ => ""));
                }
                var vault = new Vault(Path.Combine(repo, dir));

                stage = $"write {dir}";
                var changes = check ? vault.Diff(rendered) : vault.Write(rendered);
                foreach (var c in changes) Console.WriteLine($"  {c[0]} {dir}/API/{c[2..]}");
                stale.AddRange(changes);

                stage = $"labels {dir}";
                vault.CheckUniqueLabels(check ? rendered.Keys : null);
            }

            if (check && stale.Count > 0)
            {
                Console.WriteLine($"[stale] {stale.Count} generated pages differ from the sources. Run: dotnet run --project tools/DocGen");
                return ExitStale;
            }
            Console.WriteLine(check ? "[ok] generated pages are up to date" : $"[ok] {stale.Count} pages written or removed");
            return ExitOk;
        }
        catch (DocGenException e)
        {
            Console.WriteLine($"[FAIL] step={e.Step} | {e.Message}");
            return e.Step is "args" or "sources" ? ExitConfig : ExitFailed;
        }
        catch (Exception e)
        {
            Console.WriteLine($"[FAIL] step={stage} | {e.GetType().Name}: {e.Message}");
            return ExitFailed;
        }
    }

    static string FindRepoRoot(string start)
    {
        for (var dir = new DirectoryInfo(start); dir != null; dir = dir.Parent)
            if (File.Exists(Path.Combine(dir.FullName, "z3n7.sln"))) return dir.FullName;
        throw new DocGenException("args", $"z3n7.sln not found above {start}");
    }
}
