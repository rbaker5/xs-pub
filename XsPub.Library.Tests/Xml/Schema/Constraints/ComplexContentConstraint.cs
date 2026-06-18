using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class ComplexContentConstraint : ContentModelConstraint<XsComplexContent, XmlSchemaComplexContent>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("IsMixed").EqualTo(Expected!.IsMixed));
        Add(ActualHas.Property("Content").ComparableTo(Expected.Content));
    }

    public override string? GetActualName(XsComplexContent actual) => null;
    public override string? GetExpectedName(XmlSchemaComplexContent expected) => null;
}
