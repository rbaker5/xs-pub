using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class GroupRefConstraint : ParticleConstraint<XsGroupRef, XmlSchemaGroupRef>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("RefName").ComparableTo(Expected!.RefName));
    }

    public override string? GetActualName(XsGroupRef actual) =>
        $"{{{actual.RefName.NamespaceName}}}{actual.RefName.LocalName}";

    public override string? GetExpectedName(XmlSchemaGroupRef expected) =>
        $"{{{expected.RefName.Namespace}}}{expected.RefName.Name}";
}
