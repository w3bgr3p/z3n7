namespace DocGen;

/// <summary>Fixed text of generated pages. Doc comments themselves are copied as written.</summary>
public sealed class Strings
{
    public string HubTitle, HubIntro, ExtTitle, ExtIntro, ExtOn;
    public string Constructors, Properties, Methods, Fields, Events, Values;
    public string Namespace, Source, OtherParts, Parameter, Description, Returns, Exceptions, Remarks, Example;
    public string Extends, NoDescription, Type, Kind, Summary;

    public static readonly Strings En = new()
    {
        HubTitle = "API reference",
        HubIntro = "Every public type of the library, grouped by the source folder it lives in. " +
                   "Generated from the source code and its XML comments.",
        ExtTitle = "Extension methods",
        ExtIntro = "Extension methods grouped by the type they extend. In a ZennoPoster C# action " +
                   "`project` is `IZennoPosterProjectModel` and `instance` is `Instance`.",
        ExtOn = "On",
        Constructors = "Constructors", Properties = "Properties", Methods = "Methods",
        Fields = "Fields", Events = "Events", Values = "Values",
        Namespace = "namespace", Source = "source", OtherParts = "Other parts of this type",
        Parameter = "Parameter", Description = "Description", Returns = "Returns",
        Exceptions = "Exceptions", Remarks = "Remarks", Example = "Example",
        Extends = "Extension method for", NoDescription = "No description yet.",
        Type = "Type", Kind = "Kind", Summary = "Summary"
    };

    public static readonly Strings Ru = new()
    {
        HubTitle = "Справочник API",
        HubIntro = "Все публичные типы библиотеки, сгруппированные по папке исходников. " +
                   "Страницы собраны из исходного кода и его XML-комментариев.",
        ExtTitle = "Методы расширения",
        ExtIntro = "Методы расширения по типу, который они расширяют. В C#-кубике ZennoPoster " +
                   "`project` — это `IZennoPosterProjectModel`, `instance` — `Instance`.",
        ExtOn = "Для",
        Constructors = "Конструкторы", Properties = "Свойства", Methods = "Методы",
        Fields = "Поля", Events = "События", Values = "Значения",
        Namespace = "пространство имён", Source = "исходник", OtherParts = "Другие части этого типа",
        Parameter = "Параметр", Description = "Описание", Returns = "Возвращает",
        Exceptions = "Исключения", Remarks = "Примечания", Example = "Пример",
        Extends = "Метод расширения для", NoDescription = "Описания пока нет.",
        Type = "Тип", Kind = "Вид", Summary = "Описание"
    };

    public static Strings For(string lang) => lang switch
    {
        "en" => En,
        "ru" => Ru,
        _ => throw new DocGenException("args", $"unknown language '{lang}', expected en or ru"),
    };
}
