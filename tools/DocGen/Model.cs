namespace DocGen;

/// <summary>Parsed XML documentation of one declaration. Empty strings when absent.</summary>
public sealed class DocComment
{
    public string Summary = "";
    public string Remarks = "";
    public string Returns = "";
    public string Example = "";
    public readonly List<(string Name, string Text)> Params = new();
    public readonly List<(string Name, string Text)> TypeParams = new();
    public readonly List<(string Type, string Text)> Exceptions = new();

    public bool IsEmpty =>
        Summary == "" && Remarks == "" && Returns == "" && Example == "" &&
        Params.Count == 0 && TypeParams.Count == 0 && Exceptions.Count == 0;
}

public enum MemberKind { Constructor, Property, Method, Field, Event, EnumValue }

public sealed class MemberInfo
{
    public MemberKind Kind;
    public string Name;
    public string Signature;
    public DocComment Doc;
    public string File;
    public int Line;

    /// <summary>For extension methods: the type of the <c>this</c> parameter, as written in source.</summary>
    public string ExtendedType;
}

/// <summary>
/// One documentation page. A type normally maps to one page; a partial type whose
/// declarations live in several source folders gets one page per folder.
/// </summary>
public sealed class TypePage
{
    public string Kind;       // class, static class, struct, interface, enum, record
    public string Name;       // simple name, Outer.Inner for nested types
    public string Namespace;
    public string Folder;     // source folder under the library root, e.g. "Browser"
    public bool SplitByFolder;
    /// <summary>File name without extension; also the [[wikilink]] target. Must be unique in the vault.</summary>
    public string Label;
    public string Signature;
    public DocComment Doc = new();
    public readonly List<MemberInfo> Members = new();
    public readonly List<(string File, int Line)> Sources = new();
}
