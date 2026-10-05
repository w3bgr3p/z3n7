namespace DocGen;

/// <summary>Resolves &lt;see cref="..."/&gt; to a wikilink target ("Label" or "Label#Member").</summary>
public sealed class CrefLinks
{
    readonly Dictionary<string, List<string>> _types = new();
    readonly Dictionary<(string Type, string Member), string> _members = new();

    public void AddType(string name, string label)
    {
        if (!_types.TryGetValue(name, out var labels)) _types[name] = labels = new List<string>();
        if (!labels.Contains(label)) labels.Add(label);
    }

    public void AddMember(string type, string member, string label) => _members.TryAdd((type, member), label);

    public string Resolve(string cref)
    {
        var text = DocXml.CrefText(cref);
        var dot = text.LastIndexOf('.');

        // A type: link only when the name maps to exactly one page.
        if (_types.TryGetValue(text, out var labels))
            return labels.Count == 1 ? labels[0] : null;

        if (dot > 0)
        {
            var type = text[..dot];
            var member = text[(dot + 1)..];
            if (_members.TryGetValue((type, member), out var label)) return $"{label}#{member}";
        }
        return null; // unknown or external (BCL, ZennoPoster): rendered as code
    }
}
