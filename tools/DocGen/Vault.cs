namespace DocGen;

/// <summary>
/// The API/ folder of one vault. Only files carrying the generator marker are ever
/// overwritten or deleted; a hand-written file found there stops the run.
/// </summary>
public sealed class Vault
{
    readonly string _root;
    readonly string _api;

    public Vault(string root)
    {
        _root = root;
        _api = Path.Combine(root, "API");
    }

    /// <summary>Writes changed pages, deletes generated pages that no longer exist. Returns what changed.</summary>
    public List<string> Write(IDictionary<string, string> rendered)
    {
        var changes = Diff(rendered);
        foreach (var rel in Existing().Where(rel => !rendered.ContainsKey(rel)))
            File.Delete(Full(rel));
        foreach (var (rel, content) in rendered)
        {
            var full = Full(rel);
            if (File.Exists(full) && Read(full) == content) continue;
            Directory.CreateDirectory(Path.GetDirectoryName(full));
            File.WriteAllText(full, content);
        }
        RemoveEmptyDirs(_api);
        return changes;
    }

    /// <summary>Pages that would be added, changed or removed. Nothing is written.</summary>
    public List<string> Diff(IDictionary<string, string> rendered)
    {
        var existing = Existing();
        var changes = new List<string>();
        foreach (var (rel, content) in rendered)
        {
            if (!existing.Contains(rel)) changes.Add($"+ {rel}");
            else if (Read(Full(rel)) != content) changes.Add($"~ {rel}");
        }
        changes.AddRange(existing.Where(rel => !rendered.ContainsKey(rel)).Select(rel => $"- {rel}"));
        return changes;
    }

    /// <summary>
    /// Wikilinks resolve by file name, and the site silently picks the last of two equal
    /// names. Two notes with one name anywhere in the vault is an error.
    /// </summary>
    public void CheckUniqueLabels(IEnumerable<string> pendingApiFiles)
    {
        var files = Directory.Exists(_root)
            ? Directory.EnumerateFiles(_root, "*.md", SearchOption.AllDirectories)
                .Where(f => !f.Replace('\\', '/').Contains("/.obsidian/"))
                .Select(f => Path.GetRelativePath(_root, f).Replace('\\', '/'))
                .Where(rel => pendingApiFiles == null || !rel.StartsWith("API/"))
                .ToList()
            : new List<string>();
        if (pendingApiFiles != null) files.AddRange(pendingApiFiles.Select(p => "API/" + p));

        var dupes = files.GroupBy(f => Path.GetFileNameWithoutExtension(f), StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => string.Join(" & ", g))
            .ToList();
        if (dupes.Count > 0)
            throw new DocGenException("labels", "same note name in several places: " + string.Join("; ", dupes));
    }

    HashSet<string> Existing()
    {
        var set = new HashSet<string>(StringComparer.Ordinal);
        if (!Directory.Exists(_api)) return set;
        foreach (var full in Directory.EnumerateFiles(_api, "*", SearchOption.AllDirectories))
        {
            var rel = Path.GetRelativePath(_api, full).Replace('\\', '/');
            if (!full.EndsWith(".md") || !Read(full).Contains("\n" + Writer.Marker + "\n"))
                throw new DocGenException("write", $"API/{rel} is not a generated page; move it out of API/");
            set.Add(rel);
        }
        return set;
    }

    string Full(string rel) => Path.Combine(_api, rel.Replace('/', Path.DirectorySeparatorChar));

    static string Read(string path) => File.ReadAllText(path).Replace("\r\n", "\n");

    static void RemoveEmptyDirs(string dir)
    {
        if (!Directory.Exists(dir)) return;
        foreach (var sub in Directory.GetDirectories(dir)) RemoveEmptyDirs(sub);
        if (!Directory.EnumerateFileSystemEntries(dir).Any()) Directory.Delete(dir);
    }
}
