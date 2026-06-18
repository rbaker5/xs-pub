using NUnit.Comparisons;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class DocumentationConstraint : SchemaObjectConstraint<XsDocumentation, XmlSchemaDocumentation>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("Language").EqualTo(Expected!.Language));
        Add(ActualHas.Property("Source").EqualTo(Expected.Source));
        Add(ActualHas.Property("Element").Method("Nodes").ComparableTo((IEnumerable<object>)Expected.Markup!.EmptyIfNull()));
    }

    public override string? GetActualName(XsDocumentation actual) => null;
    public override string? GetExpectedName(XmlSchemaDocumentation expected) => null;
}
