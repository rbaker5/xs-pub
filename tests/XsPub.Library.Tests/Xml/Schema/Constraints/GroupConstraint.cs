using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class GroupConstraint : AnnotatedConstraint<XsGroup, XmlSchemaGroup>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("Name").EqualTo(Expected!.Name));
        Add(ActualHas.Property("QualifiedName").ComparableTo(Expected.QualifiedName));
        Add(ActualHas.Property("Particle").ComparableTo(Expected.Particle));
    }

    public override string? GetActualName(XsGroup actual) => actual.Name;
    public override string? GetExpectedName(XmlSchemaGroup expected) => expected.Name;
}
