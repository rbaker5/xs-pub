using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class SimpleContentConstraint : ContentModelConstraint<XsSimpleContent, XmlSchemaSimpleContent>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("Content").ComparableTo(Expected!.Content));
    }

    public override string? GetActualName(XsSimpleContent actual) => null;
    public override string? GetExpectedName(XmlSchemaSimpleContent expected) => null;
}
