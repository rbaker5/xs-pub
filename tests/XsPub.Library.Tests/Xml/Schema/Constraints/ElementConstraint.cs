using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class ElementConstraint : ParticleConstraint<XsElement, XmlSchemaElement>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("Block").EqualTo(Expected!.Block));
        Add(ActualHas.Property("BlockResolved").EqualTo(Expected.BlockResolved));
        Add(ActualHas.Property("DefaultValue").EqualTo(Expected.DefaultValue));
        Add(ActualHas.Property("Final").EqualTo(Expected.Final));
        Add(ActualHas.Property("FinalResolved").EqualTo(Expected.FinalResolved));
        Add(ActualHas.Property("FixedValue").EqualTo(Expected.FixedValue));
        Add(ActualHas.Property("Form").EqualTo(Expected.Form));
        Add(ActualHas.Property("IsAbstract").EqualTo(Expected.IsAbstract));
        Add(ActualHas.Property("IsNillable").EqualTo(Expected.IsNillable));
        Add(ActualHas.Property("Name").EqualTo(Expected.Name));
        Add(ActualHas.Property("QualifiedName").ComparableTo(Expected.QualifiedName));
        Add(ActualHas.Property("RefName").ComparableTo(Expected.RefName));
        Add(ActualHas.Property("SchemaTypeName").ComparableTo(Expected.SchemaTypeName));
        Add(ActualHas.Property("SubstitutionGroup").ComparableTo(Expected.SubstitutionGroup));
        Add(ActualHas.Property("SchemaType").ComparableTo(Expected.SchemaType));
    }

    public override string? GetActualName(XsElement actual) => actual.Name;
    public override string? GetExpectedName(XmlSchemaElement expected) => expected.Name;
}
