using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class IncludeConstraint : ExternalConstraint<XsInclude, XmlSchemaInclude>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("Annotation").ComparableTo(Expected!.Annotation));
    }

    public override string? GetActualName(XsInclude actual) => null;
    public override string? GetExpectedName(XmlSchemaInclude expected) => null;
}
