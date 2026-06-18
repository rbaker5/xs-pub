using System.Linq;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class RedefineConstraint : ExternalConstraint<XsRedefine, XmlSchemaRedefine>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("AttributeGroups").ComparableTo(Expected!.AttributeGroups.Values.Cast<XmlSchemaAttributeGroup>()));
        Add(ActualHas.Property("Groups").ComparableTo(Expected.Groups.Values.Cast<XmlSchemaGroup>()));
        Add(ActualHas.Property("Items").ComparableTo(Expected.Items.Cast<XmlSchemaObject>()));
        Add(ActualHas.Property("SchemaTypes").ComparableTo(Expected.SchemaTypes.Values.Cast<XmlSchemaType>()));
    }

    public override string? GetActualName(XsRedefine actual) => null;
    public override string? GetExpectedName(XmlSchemaRedefine expected) => null;
}
