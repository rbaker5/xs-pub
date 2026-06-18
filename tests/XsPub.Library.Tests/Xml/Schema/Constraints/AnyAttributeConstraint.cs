using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class AnyAttributeConstraint : AnnotatedConstraint<XsAnyAttribute, XmlSchemaAnyAttribute>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("Namespace").EqualTo(Expected!.Namespace));
        Add(ActualHas.Property("ProcessContents").EqualTo(Expected.ProcessContents));
    }

    public override string? GetActualName(XsAnyAttribute actual) => actual.Namespace;
    public override string? GetExpectedName(XmlSchemaAnyAttribute expected) => expected.Namespace;
}
