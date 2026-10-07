using System.Text;
using System.Text.RegularExpressions;

namespace DocGen;

/// <summary>Renders pages to Markdown. Returns path (relative to API/) → content; touches no files.</summary>
public sealed class Writer
{
    public const string Marker = "generated: z3n7-docgen";

    readonly List<TypePage> _pages;
    readonly Strings _s;
    readonly string _sourceUrl;

    public Writer(List<TypePage> pages, Strings strings, string sourceUrl)
    {
        _pages = pages;
        _s = strings;
        _sourceUrl = sourceUrl.TrimEnd('/') + "/";
    }

    public SortedDictionary<string, string> Render()
    {
        var files = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var p in _pages)
            files[$"{p.Folder}/{p.Label}.md"] = TypePage(p);
        files[$"{_s.HubTitle}.md"] = Hub();
        files[$"{_s.ExtTitle}.md"] = Extensions();
        return files;
    }

    string Header(string title, params string[] tags)
    {
        var sb = new StringBuilder();
        sb.Append("---\n");
        sb.Append($"title: \"{title}\"\n");
        sb.Append($"tags: [{string.Join(", ", tags)}]\n");
        sb.Append(Marker + "\n");
        sb.Append("---\n\n");
        sb.Append($"# {title}\n\n");
        return sb.ToString();
    }

    string TypePage(TypePage p)
    {
        var sb = new StringBuilder(Header(p.Label, "api", p.Folder));
        var sources = string.Join(", ", p.Sources.Select(s => $"[{s.File}]({_sourceUrl}{s.File}#L{s.Line})"));
        sb.Append($"`{p.Kind}` · {_s.Namespace} `{p.Namespace}` · {_s.Source} {sources}\n\n");
        sb.Append($"```csharp\n{p.Signature}\n```\n\n");

        if (p.SplitByFolder)
        {
            var others = _pages.Where(o => o != p && o.SplitByFolder && o.Name == p.Name && o.Namespace == p.Namespace)
                .Select(o => $"[[{o.Label}]]");
            sb.Append($"{_s.OtherParts}: {string.Join(", ", others)}\n\n");
        }

        AppendDoc(sb, p.Doc, emptyNote: true);

        Section(sb, _s.Values, p.Members.Where(m => m.Kind == MemberKind.EnumValue), enumTable: true);
        Section(sb, _s.Constructors, p.Members.Where(m => m.Kind == MemberKind.Constructor));
        Section(sb, _s.Properties, p.Members.Where(m => m.Kind == MemberKind.Property));
        Section(sb, _s.Methods, p.Members.Where(m => m.Kind == MemberKind.Method));
        Section(sb, _s.Fields, p.Members.Where(m => m.Kind == MemberKind.Field));
        Section(sb, _s.Events, p.Members.Where(m => m.Kind == MemberKind.Event));

        return sb.ToString();
    }

    void Section(StringBuilder sb, string title, IEnumerable<MemberInfo> members, bool enumTable = false)
    {
        var list = members.ToList();
        if (list.Count == 0) return;
        sb.Append($"## {title}\n\n");

        if (enumTable)
        {
            sb.Append($"| | {_s.Description} |\n|---|---|\n");
            foreach (var m in list)
                sb.Append($"| `{m.Name}` | {OneLine(m.Doc.Summary)} |\n");
            sb.Append('\n');
            return;
        }

        // Overloads share one heading so the page outline lists each name once.
        foreach (var group in list.GroupBy(m => m.Name).OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase))
        {
            sb.Append($"### {group.Key}\n\n");
            foreach (var m in group)
            {
                sb.Append($"```csharp\n{m.Signature}\n```\n\n");
                if (m.ExtendedType != null)
                    sb.Append($"{_s.Extends} `{m.ExtendedType}`. ");
                sb.Append($"[{_s.Source}]({_sourceUrl}{m.File}#L{m.Line})\n\n");
                AppendDoc(sb, m.Doc, emptyNote: false);
            }
        }
    }

    void AppendDoc(StringBuilder sb, DocComment d, bool emptyNote)
    {
        if (d.IsEmpty)
        {
            if (emptyNote) sb.Append($"*{_s.NoDescription}*\n\n");
            return;
        }
        if (d.Summary != "") sb.Append(d.Summary + "\n\n");

        var documented = d.Params.Where(p => p.Text != "").ToList();
        if (documented.Count > 0)
        {
            sb.Append($"| {_s.Parameter} | {_s.Description} |\n|---|---|\n");
            foreach (var (name, text) in documented)
                sb.Append($"| `{name}` | {OneLine(text)} |\n");
            sb.Append('\n');
        }
        if (d.Returns != "") sb.Append($"**{_s.Returns}:** {d.Returns}\n\n");
        if (d.Exceptions.Count > 0)
        {
            sb.Append($"**{_s.Exceptions}:**\n\n");
            foreach (var (type, text) in d.Exceptions)
                sb.Append($"- `{type}` — {text}\n");
            sb.Append('\n');
        }
        if (d.Remarks != "") sb.Append($"**{_s.Remarks}:** {d.Remarks}\n\n");
        if (d.Example != "") sb.Append($"**{_s.Example}:**\n\n{d.Example}\n\n");
    }

    string Hub()
    {
        var sb = new StringBuilder(Header(_s.HubTitle, "api"));
        sb.Append(_s.HubIntro + "\n\n");
        sb.Append($"[[{_s.ExtTitle}]]\n\n");
        foreach (var folder in _pages.GroupBy(p => p.Folder).OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase))
        {
            sb.Append($"## {folder.Key}\n\n| {_s.Type} | {_s.Kind} | {_s.Summary} |\n|---|---|---|\n");
            foreach (var p in folder.OrderBy(p => p.Label, StringComparer.OrdinalIgnoreCase))
                sb.Append($"| [[{p.Label}]] | {p.Kind} | {FirstSentence(p.Doc.Summary)} |\n");
            sb.Append('\n');
        }
        return sb.ToString();
    }

    string Extensions()
    {
        var sb = new StringBuilder(Header(_s.ExtTitle, "api"));
        sb.Append(_s.ExtIntro + "\n\n");
        var ext = _pages.SelectMany(p => p.Members.Where(m => m.ExtendedType != null).Select(m => (Page: p, Member: m)));
        foreach (var target in ext.GroupBy(e => e.Member.ExtendedType)
                     .OrderByDescending(g => g.Count()).ThenBy(g => g.Key, StringComparer.Ordinal))
        {
            sb.Append($"## {_s.ExtOn} {target.Key}\n\n| | {_s.Summary} |\n|---|---|\n");
            foreach (var g in target.GroupBy(e => (e.Page.Label, e.Member.Name))
                         .OrderBy(g => g.Key.Name, StringComparer.OrdinalIgnoreCase))
            {
                var summary = g.Select(e => e.Member.Doc.Summary).FirstOrDefault(s => s != "") ?? "";
                sb.Append($"| [[{g.Key.Label}#{g.Key.Name}\\|{g.Key.Name}]] | {FirstSentence(summary)} |\n");
            }
            sb.Append('\n');
        }
        return sb.ToString();
    }

    /// <summary>Text safe inside a table cell: one line, wikilink aliases with an escaped pipe.</summary>
    static string OneLine(string s)
    {
        s = Regex.Replace(s, @"\s*\n+\s*", " ");
        return Regex.Replace(s, @"\[\[([^\]|]+)\|([^\]]+)\]\]", @"[[$1\|$2]]");
    }

    static string FirstSentence(string s)
    {
        s = OneLine(s);
        var m = Regex.Match(s, @"[.!?](\s|$)");
        return m.Success ? s[..(m.Index + 1)] : s;
    }
}
