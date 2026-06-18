using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class AttributeGroupRefConstraint : AnnotatedConstraint<XsAttributeGroupRef, XmlSchemaAttributeGroupRef>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("RefName").ComparableTo(Expected!.RefName));
    }

    public override string? GetActualName(XsAttributeGroupRef actual) =>
        $"{{{actual.RefName.NamespaceName}}}{actual.RefName.LocalName}";

    public override string? GetExpectedName(XmlSchemaAttributeGroupRef expected) =>
        $"{{{expected.RefName.Namespace}}}{expected.RefName.Name}";
}
