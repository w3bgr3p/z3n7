using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DocGen;

/// <summary>
/// Translation memory for one language: English text from the XML comments → translated text.
/// Keyed by the English text itself, so a changed comment no longer matches its old translation
/// and the page falls back to English until the entry is updated.
/// </summary>
public sealed class Translations
{
    readonly Dictionary<string, string> _map;
    readonly HashSet<string> _used = new(StringComparer.Ordinal);
    readonly SortedSet<string> _missing = new(StringComparer.Ordinal);
    readonly List<string> _rejected = new();

    public static readonly Translations None = new(new Dictionary<string, string>());

    Translations(Dictionary<string, string> map) => _map = map;

    public static Translations Load(string path)
    {
        if (!File.Exists(path)) return new Translations(new Dictionary<string, string>(StringComparer.Ordinal));
        try
        {
            var map = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path))
                      ?? new Dictionary<string, string>();
            return new Translations(new Dictionary<string, string>(map, StringComparer.Ordinal));
        }
        catch (JsonException e)
        {
            throw new DocGenException("translations", $"{path}: {e.Message}");
        }
    }

    public IReadOnlyCollection<string> Missing => _missing;
    public IReadOnlyList<string> Rejected => _rejected;
    public IEnumerable<string> Unused => _map.Keys.Where(k => !_used.Contains(k));
    public bool Active => !ReferenceEquals(this, None);

    /// <summary>Translated text, or the English text when there is no usable translation.</summary>
    public string T(string en)
    {
        if (!Active || string.IsNullOrWhiteSpace(en)) return en;
        if (!_map.TryGetValue(en, out var ru) || string.IsNullOrWhiteSpace(ru))
        {
            _missing.Add(en);
            return en;
        }
        _used.Add(en);
        // Code spans and wikilinks must survive translation unchanged, otherwise links break.
        if (!SameTokens(en, ru))
        {
            _rejected.Add(en);
            return en;
        }
        return ru;
    }

    static readonly Regex Tokens = new(@"`[^`]+`|\[\[[^\]]+\]\]|\]\([^)]+\)", RegexOptions.Compiled);

    static bool SameTokens(string a, string b) =>
        Tokens.Matches(a).Select(m => m.Value).OrderBy(s => s, StringComparer.Ordinal)
            .SequenceEqual(Tokens.Matches(b).Select(m => m.Value).OrderBy(s => s, StringComparer.Ordinal));

    public static void WriteMap(string path, IDictionary<string, string> map)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        var sorted = new SortedDictionary<string, string>(map, StringComparer.Ordinal);
        File.WriteAllText(path, JsonSerializer.Serialize(sorted, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        }) + "\n");
    }
}
