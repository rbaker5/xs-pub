using System.Linq;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class SchemaConstraint : SchemaObjectConstraint<XsSchema, XmlSchema>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("AttributeFormDefault").EqualTo(Expected!.AttributeFormDefault));
        Add(ActualHas.Property("BlockDefault").EqualTo(Expected.BlockDefault));
        Add(ActualHas.Property("ElementFormDefault").EqualTo(Expected.ElementFormDefault));
        Add(ActualHas.Property("FinalDefault").EqualTo(Expected.FinalDefault));
        Add(ActualHas.Property("Id").EqualTo(Expected.Id));
        Add(ActualHas.Property("TargetNamespace").EqualTo(Expected.TargetNamespace));
        Add(ActualHas.Property("Version").EqualTo(Expected.Version));

        Add(ActualHas.Property("Elements").ComparableTo(Expected.Elements.Values.Cast<XmlSchemaElement>()));
        Add(ActualHas.Property("AttributeGroups").ComparableTo(Expected.AttributeGroups.Values.Cast<XmlSchemaAttributeGroup>()));
        Add(ActualHas.Property("Attributes").ComparableTo(Expected.Attributes.Values.Cast<XmlSchemaAttribute>()));
        Add(ActualHas.Property("Groups").ComparableTo(Expected.Groups.Values.Cast<XmlSchemaGroup>()));
        Add(ActualHas.Property("Includes").ComparableTo(Expected.Includes.Cast<XmlSchemaExternal>()));
        Add(ActualHas.Property("Items").ComparableTo(Expected.Items.Cast<XmlSchemaObject>()));
        Add(ActualHas.Property("Notations").ComparableTo(Expected.Notations.Values.Cast<XmlSchemaNotation>()));
        Add(ActualHas.Property("SchemaTypes").ComparableTo(Expected.SchemaTypes.Values.Cast<XmlSchemaType>()));
        Add(ActualHas.Property("UnhandledAttributes").ComparableTo(Expected.UnhandledAttributes.EmptyIfNull()));
    }

    public override string? GetActualName(XsSchema actual) =>
        $"{actual.TargetNamespace} {{{actual.SourceUri}}}";

    public override string? GetExpectedName(XmlSchema expected) =>
        $"{expected.TargetNamespace} {{{expected.SourceUri}}}";
}
