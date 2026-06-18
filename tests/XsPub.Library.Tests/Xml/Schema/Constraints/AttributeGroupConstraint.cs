using System.Linq;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class AttributeGroupConstraint : AnnotatedConstraint<XsAttributeGroup, XmlSchemaAttributeGroup>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("Name").EqualTo(Expected!.Name));
        Add(ActualHas.Property("QualifiedName").ComparableTo(Expected.QualifiedName));
        Add(ActualHas.Property("AnyAttribute").ComparableTo(Expected.AnyAttribute));
        Add(ActualHas.Property("Attributes").ComparableTo(Expected.Attributes.Cast<XmlSchemaAttribute>()));
    }

    public override string? GetActualName(XsAttributeGroup actual) => actual.Name;
    public override string? GetExpectedName(XmlSchemaAttributeGroup expected) => expected.Name;
}
