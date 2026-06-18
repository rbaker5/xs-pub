using System.Linq;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class ComplexContentExtensionConstraint : ContentConstraint<XsComplexContentExtension, XmlSchemaComplexContentExtension>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("BaseTypeName").ComparableTo(Expected!.BaseTypeName));
        Add(ActualHas.Property("AnyAttribute").ComparableTo(Expected.AnyAttribute));
        Add(ActualHas.Property("Particle").ComparableTo(Expected.Particle));
        Add(ActualHas.Property("Attributes").ComparableTo(Expected.Attributes.Cast<XmlSchemaObject>()));
    }

    public override string? GetActualName(XsComplexContentExtension actual) => null;
    public override string? GetExpectedName(XmlSchemaComplexContentExtension expected) => null;
}
