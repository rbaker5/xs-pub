using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class AttributeConstraint : AnnotatedConstraint<XsAttribute, XmlSchemaAttribute>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("DefaultValue").EqualTo(Expected!.DefaultValue));
        Add(ActualHas.Property("FixedValue").EqualTo(Expected.FixedValue));
        Add(ActualHas.Property("Form").EqualTo(Expected.Form));
        Add(ActualHas.Property("Name").EqualTo(Expected.Name));
        Add(ActualHas.Property("QualifiedName").ComparableTo(Expected.QualifiedName));
        Add(ActualHas.Property("RefName").ComparableTo(Expected.RefName));
        Add(ActualHas.Property("SchemaTypeName").ComparableTo(Expected.SchemaTypeName));
        Add(ActualHas.Property("SchemaType").ComparableTo(Expected.SchemaType));
    }

    public override string? GetActualName(XsAttribute actual) => actual.Name;
    public override string? GetExpectedName(XmlSchemaAttribute expected) => expected.Name;
}
