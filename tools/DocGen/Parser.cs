using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocGen;

/// <summary>
/// Syntax-only scan of the library sources. No compilation: the library references
/// ZennoPoster assemblies that are not available everywhere the docs are built.
/// </summary>
public static class Parser
{
    sealed class Decl
    {
        public BaseTypeDeclarationSyntax Node;
        public string Name;
        public string Namespace;
        public string Folder;
        public string File;
    }

    public static List<TypePage> Parse(string projectDir, IEnumerable<string> relFiles)
    {
        var options = new CSharpParseOptions(LanguageVersion.CSharp9, DocumentationMode.Parse);
        var decls = new List<Decl>();

        foreach (var rel in relFiles)
        {
            var text = File.ReadAllText(Path.Combine(projectDir, rel));
            var root = CSharpSyntaxTree.ParseText(text, options, rel).GetCompilationUnitRoot();
            var folder = rel.Contains('/') ? rel[..rel.IndexOf('/')] : "(root)";
            foreach (var type in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
            {
                if (!IsVisible(type)) continue;
                decls.Add(new Decl
                {
                    Node = type,
                    Name = QualifiedName(type),
                    Namespace = NamespaceOf(type),
                    Folder = folder,
                    File = rel,
                });
            }
        }

        // A name declared in several namespaces: pages outside the main one carry the namespace.
        var ambiguous = decls.GroupBy(d => d.Name).Where(g => g.Select(d => d.Namespace).Distinct().Count() > 1)
            .Select(g => g.Key).ToHashSet();

        var groups = decls.GroupBy(d => (d.Namespace, d.Name))
            .SelectMany(byType =>
            {
                var split = byType.Select(d => d.Folder).Distinct().Count() > 1;
                return byType.GroupBy(d => d.Folder).Select(g =>
                {
                    var first = g.First();
                    var label = first.Name;
                    if (ambiguous.Contains(first.Name) && first.Namespace != "z3n7") label += $" ({DisplayNamespace(first.Namespace)})";
                    if (split) label += $" ({first.Folder})";
                    return (Parts: g.ToList(), Split: split, Label: label);
                });
            })
            .ToList();

        // Every page label is known before any doc comment is read, so <see cref> can link.
        var links = new CrefLinks();
        foreach (var (parts, split, label) in groups)
        {
            links.AddType(parts[0].Name, label);
            foreach (var p in parts)
                if (p.Node is TypeDeclarationSyntax t)
                    foreach (var name in t.Members.Select(MemberName).Where(n => n != null))
                        links.AddMember(parts[0].Name, name, label);
        }
        DocXml.LinkResolver = links.Resolve;

        return groups.Select(g => BuildPage(g.Parts, g.Split, g.Label)).ToList();
    }

    public static string DisplayNamespace(string ns) => ns == "" ? "global" : ns;

    static string MemberName(MemberDeclarationSyntax m) => m switch
    {
        MethodDeclarationSyntax x => x.Identifier.Text,
        PropertyDeclarationSyntax x => x.Identifier.Text,
        FieldDeclarationSyntax x => x.Declaration.Variables.FirstOrDefault()?.Identifier.Text,
        _ => null,
    };

    static TypePage BuildPage(List<Decl> parts, bool split, string label)
    {
        var first = parts[0];
        var page = new TypePage
        {
            Name = first.Name,
            Namespace = DisplayNamespace(first.Namespace),
            Label = label,
            Folder = first.Folder,
            SplitByFolder = split,
            Kind = KindOf(first.Node),
            Signature = TypeSignature(first.Node),
        };

        foreach (var part in parts)
        {
            var line = part.Node.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            page.Sources.Add((part.File, line));
            var doc = DocXml.Read(part.Node);
            if (page.Doc.IsEmpty && !doc.IsEmpty) page.Doc = doc;
            CollectMembers(part, page);
        }
        return page;
    }

    static void CollectMembers(Decl part, TypePage page)
    {
        if (part.Node is EnumDeclarationSyntax en)
        {
            foreach (var v in en.Members)
                page.Members.Add(Member(MemberKind.EnumValue, v.Identifier.Text, v.ToString().Trim(), v, part.File));
            return;
        }

        var type = (TypeDeclarationSyntax)part.Node;
        var inInterface = type is InterfaceDeclarationSyntax;
        foreach (var m in type.Members)
        {
            if (m is BaseTypeDeclarationSyntax) continue; // nested types get their own page
            if (!inInterface && !m.Modifiers.Any(SyntaxKind.PublicKeyword)) continue;

            switch (m)
            {
                case ConstructorDeclarationSyntax c:
                    page.Members.Add(Member(MemberKind.Constructor, c.Identifier.Text, Signature(c), c, part.File));
                    break;
                case MethodDeclarationSyntax meth:
                    var mi = Member(MemberKind.Method, meth.Identifier.Text, Signature(meth), meth, part.File);
                    var first = meth.ParameterList.Parameters.FirstOrDefault();
                    if (first != null && first.Modifiers.Any(SyntaxKind.ThisKeyword))
                        mi.ExtendedType = first.Type?.ToString();
                    page.Members.Add(mi);
                    break;
                case PropertyDeclarationSyntax p:
                    page.Members.Add(Member(MemberKind.Property, p.Identifier.Text, Signature(p), p, part.File));
                    break;
                case IndexerDeclarationSyntax ix:
                    page.Members.Add(Member(MemberKind.Property, "this[]", Signature(ix), ix, part.File));
                    break;
                case FieldDeclarationSyntax f:
                    foreach (var v in f.Declaration.Variables)
                        page.Members.Add(Member(MemberKind.Field, v.Identifier.Text, FieldSignature(f, v), f, part.File));
                    break;
                case EventFieldDeclarationSyntax ev:
                    foreach (var v in ev.Declaration.Variables)
                        page.Members.Add(Member(MemberKind.Event, v.Identifier.Text,
                            $"{ev.Modifiers} event {ev.Declaration.Type} {v.Identifier};".Trim(), ev, part.File));
                    break;
                case DelegateDeclarationSyntax:
                    break;
            }
        }
    }

    static MemberInfo Member(MemberKind kind, string name, string signature, SyntaxNode node, string file) => new()
    {
        Kind = kind,
        Name = name,
        Signature = signature,
        Doc = DocXml.Read(node),
        File = file,
        Line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
    };

    /// <summary>A type is documented when it and every enclosing type are public.</summary>
    static bool IsVisible(BaseTypeDeclarationSyntax type)
    {
        for (SyntaxNode n = type; n != null; n = n.Parent)
        {
            if (n is BaseTypeDeclarationSyntax t && !t.Modifiers.Any(SyntaxKind.PublicKeyword))
                return false;
        }
        return true;
    }

    static string QualifiedName(BaseTypeDeclarationSyntax type)
    {
        var names = type.AncestorsAndSelf().OfType<BaseTypeDeclarationSyntax>()
            .Select(t => t.Identifier.Text + TypeParams(t)).Reverse();
        return string.Join(".", names);
    }

    static string TypeParams(BaseTypeDeclarationSyntax t) =>
        t is TypeDeclarationSyntax td && td.TypeParameterList != null ? td.TypeParameterList.ToString() : "";

    static string NamespaceOf(SyntaxNode node) =>
        string.Join(".", node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().Select(n => n.Name.ToString()).Reverse());

    static string KindOf(BaseTypeDeclarationSyntax t) => t switch
    {
        EnumDeclarationSyntax => "enum",
        InterfaceDeclarationSyntax => "interface",
        StructDeclarationSyntax => "struct",
        RecordDeclarationSyntax => "record",
        ClassDeclarationSyntax c when c.Modifiers.Any(SyntaxKind.StaticKeyword) => "static class",
        _ => "class",
    };

    static string TypeSignature(BaseTypeDeclarationSyntax t)
    {
        var mods = string.Join(" ", t.Modifiers.Where(m => !m.IsKind(SyntaxKind.PartialKeyword)).Select(m => m.Text));
        var keyword = t switch
        {
            EnumDeclarationSyntax => "enum",
            InterfaceDeclarationSyntax => "interface",
            StructDeclarationSyntax => "struct",
            RecordDeclarationSyntax => "record",
            _ => "class",
        };
        var bases = t.BaseList != null ? " " + t.BaseList.ToString().Trim() : "";
        return $"{mods} {keyword} {t.Identifier.Text}{TypeParams(t)}{bases}".Trim();
    }

    /// <summary>The declaration without attributes, body, initializer or comments, on one line.</summary>
    static string Signature(MemberDeclarationSyntax m)
    {
        MemberDeclarationSyntax bare = m switch
        {
            MethodDeclarationSyntax x => x.WithBody(null).WithExpressionBody(null).WithSemicolonToken(default),
            ConstructorDeclarationSyntax x => x.WithBody(null).WithExpressionBody(null).WithInitializer(null).WithSemicolonToken(default),
            PropertyDeclarationSyntax x => x.WithInitializer(null).WithSemicolonToken(default)
                .WithExpressionBody(null)
                .WithAccessorList(AccessorsOnly(x.AccessorList, x.ExpressionBody != null)),
            IndexerDeclarationSyntax x => x.WithExpressionBody(null).WithSemicolonToken(default)
                .WithAccessorList(AccessorsOnly(x.AccessorList, x.ExpressionBody != null)),
            _ => m,
        };
        bare = bare.WithAttributeLists(default);
        return OneLine(bare.WithoutTrivia().NormalizeWhitespace().ToFullString());
    }

    /// <summary>{ get; set; } with bodies stripped; non-public accessors dropped.</summary>
    static AccessorListSyntax AccessorsOnly(AccessorListSyntax list, bool expressionBodied)
    {
        if (expressionBodied || list == null)
            return SyntaxFactory.AccessorList(SyntaxFactory.SingletonList(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))));
        var kept = list.Accessors
            .Where(a => !a.Modifiers.Any(SyntaxKind.PrivateKeyword) && !a.Modifiers.Any(SyntaxKind.InternalKeyword))
            .Select(a => a.WithBody(null).WithExpressionBody(null).WithAttributeLists(default)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
        return SyntaxFactory.AccessorList(SyntaxFactory.List(kept));
    }

    static string FieldSignature(FieldDeclarationSyntax f, VariableDeclaratorSyntax v)
    {
        var mods = string.Join(" ", f.Modifiers.Select(m => m.Text));
        // Constants show their value; other initializers are implementation detail.
        var value = f.Modifiers.Any(SyntaxKind.ConstKeyword) && v.Initializer != null ? " " + v.Initializer.ToString().Trim() : "";
        return OneLine($"{mods} {f.Declaration.Type} {v.Identifier}{value};".Trim());
    }

    static string OneLine(string s) => System.Text.RegularExpressions.Regex.Replace(s, @"\s+", " ").Trim();
}
