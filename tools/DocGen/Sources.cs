using System.Xml.Linq;

namespace DocGen;

/// <summary>
/// The .cs files that actually compile into the library: everything under the project
/// folder minus bin/obj and the csproj's &lt;Compile Remove="..."/&gt; entries.
/// </summary>
public static class Sources
{
    public static List<string> Collect(string projectDir)
    {
        var csproj = Directory.GetFiles(projectDir, "*.csproj").FirstOrDefault()
            ?? throw new DocGenException("sources", $"no .csproj in {projectDir}");

        var removed = XDocument.Load(csproj)
            .Descendants()
            .Where(e => e.Name.LocalName == "Compile" && e.Attribute("Remove") != null)
            .Select(e => Normalize(e.Attribute("Remove").Value))
            .ToList();

        return Directory.EnumerateFiles(projectDir, "*.cs", SearchOption.AllDirectories)
            .Select(f => Normalize(Path.GetRelativePath(projectDir, f)))
            .Where(rel => !rel.StartsWith("bin/") && !rel.StartsWith("obj/"))
            .Where(rel => !removed.Any(pattern => Matches(pattern, rel)))
            .OrderBy(rel => rel, StringComparer.Ordinal)
            .ToList();
    }

    static string Normalize(string path) => path.Replace('\\', '/').Trim();

    // Only the two shapes the csproj uses: "Dir/**" and an exact file path.
    static bool Matches(string pattern, string rel)
    {
        if (pattern.EndsWith("/**"))
            return rel.StartsWith(pattern[..^2], StringComparison.OrdinalIgnoreCase);
        if (pattern.Contains('*'))
            throw new DocGenException("sources", $"unsupported Compile Remove pattern: {pattern}");
        return string.Equals(pattern, rel, StringComparison.OrdinalIgnoreCase);
    }
}

public sealed class DocGenException : Exception
{
    public string Step { get; }
    public DocGenException(string step, string message) : base(message) => Step = step;
}
