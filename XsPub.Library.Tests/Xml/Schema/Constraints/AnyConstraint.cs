using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class AnyConstraint : ParticleConstraint<XsAny, XmlSchemaAny>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("Namespace").EqualTo(Expected!.Namespace));
        Add(ActualHas.Property("ProcessContents").EqualTo(Expected.ProcessContents));
    }

    public override string? GetActualName(XsAny actual) => actual.Namespace;
    public override string? GetExpectedName(XmlSchemaAny expected) => expected.Namespace;
}
