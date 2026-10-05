using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocGen;

/// <summary>Turns /// comments into Markdown. Cross-references go through <see cref="LinkResolver"/>.</summary>
public static class DocXml
{
    /// <summary>Maps a cref like "Db", "T:z3n7.Db" or "Db.Query" to a wikilink, or null.</summary>
    public static Func<string, string> LinkResolver = _ => null;

    public static DocComment Read(SyntaxNode node)
    {
        var trivia = node.GetLeadingTrivia()
            .Select(t => t.GetStructure())
            .OfType<DocumentationCommentTriviaSyntax>()
            .FirstOrDefault();
        if (trivia == null) return new DocComment();

        var raw = Regex.Replace(trivia.ToFullString(), @"^[ \t]*///[ ]?", "", RegexOptions.Multiline);
        XElement root;
        try
        {
            root = XElement.Parse("<doc>" + raw + "</doc>", LoadOptions.PreserveWhitespace);
        }
        catch (XmlException)
        {
            // Malformed XML (e.g. a bare "&" or "<" in prose): keep the text, drop the tags.
            return new DocComment { Summary = Collapse(Regex.Replace(raw, "<[^>]+>", " ")) };
        }

        var doc = new DocComment
        {
            Summary = Block(root.Element("summary")),
            Remarks = Block(root.Element("remarks")),
            Returns = Block(root.Element("returns")),
            Example = Block(root.Element("example")),
        };
        foreach (var p in root.Elements("param"))
            doc.Params.Add(((string)p.Attribute("name") ?? "", Inline(p)));
        foreach (var p in root.Elements("typeparam"))
            doc.TypeParams.Add(((string)p.Attribute("name") ?? "", Inline(p)));
        foreach (var e in root.Elements("exception"))
            doc.Exceptions.Add((CrefText((string)e.Attribute("cref") ?? ""), Inline(e)));
        return doc;
    }

    /// <summary>Multi-paragraph content: &lt;para&gt; and &lt;code&gt; become their own blocks.</summary>
    static string Block(XElement el)
    {
        if (el == null) return "";
        var blocks = new List<string>();
        var current = new StringBuilder();

        void Flush()
        {
            var text = Collapse(current.ToString());
            if (text != "") blocks.Add(text);
            current.Clear();
        }

        foreach (var n in el.Nodes())
        {
            if (n is XElement child && child.Name.LocalName == "para")
            {
                Flush();
                blocks.Add(Block(child));
            }
            else if (n is XElement code && code.Name.LocalName == "code")
            {
                Flush();
                blocks.Add("```csharp\n" + Dedent(code.Value) + "\n```");
            }
            else
            {
                current.Append(InlineNode(n));
            }
        }
        Flush();
        return string.Join("\n\n", blocks.Where(b => b != ""));
    }

    static string Inline(XElement el) => el == null ? "" : Collapse(string.Concat(el.Nodes().Select(InlineNode)));

    static string InlineNode(XNode n)
    {
        if (n is XText t) return EscapeMarkdown(t.Value);
        if (n is not XElement e) return "";
        switch (e.Name.LocalName)
        {
            case "see":
            case "seealso":
                var cref = (string)e.Attribute("cref");
                if (cref != null)
                {
                    var label = e.Value.Trim() != "" ? e.Value.Trim() : CrefText(cref);
                    var link = LinkResolver(cref);
                    return link != null ? $"[[{link}|{label}]]" : $"`{label}`";
                }
                var word = (string)e.Attribute("langword");
                if (word != null) return $"`{word}`";
                var href = (string)e.Attribute("href");
                if (href != null) return $"[{(e.Value.Trim() != "" ? e.Value.Trim() : href)}]({href})";
                return e.Value;
            case "paramref":
            case "typeparamref":
                return $"`{(string)e.Attribute("name")}`";
            case "c":
                return $"`{e.Value}`";
            case "code":
                return $"`{Collapse(e.Value)}`";
            case "br":
                return "<br>";
            default:
                return string.Concat(e.Nodes().Select(InlineNode));
        }
    }

    /// <summary>"T:z3n7.Db.Query(System.String)" → "Db.Query".</summary>
    public static string CrefText(string cref)
    {
        var s = Regex.Replace(cref, @"^[A-Z]:", "");
        s = Regex.Replace(s, @"\(.*\)$", "");
        s = Regex.Replace(s, @"^z3n7(\.[A-Za-z]+)*?\.(?=[A-Z])", "");
        return s;
    }

    static string Collapse(string s) => Regex.Replace(s, @"\s+", " ").Trim();

    // Doc text is prose, not Markdown: neutralise characters that would change the rendering.
    static string EscapeMarkdown(string s) => s.Replace("|", "\\|").Replace("<", "&lt;").Replace(">", "&gt;");

    static string Dedent(string code)
    {
        var lines = code.Replace("\r", "").Split('\n').ToList();
        while (lines.Count > 0 && lines[0].Trim() == "") lines.RemoveAt(0);
        while (lines.Count > 0 && lines[^1].Trim() == "") lines.RemoveAt(lines.Count - 1);
        var indent = lines.Where(l => l.Trim() != "").Select(l => l.Length - l.TrimStart().Length).DefaultIfEmpty(0).Min();
        return string.Join("\n", lines.Select(l => l.Length >= indent ? l[indent..] : l.TrimStart()));
    }
}
