using System.Linq;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class AnnotationConstraint : SchemaObjectConstraint<XsAnnotation, XmlSchemaAnnotation>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("Id").EqualTo(Expected!.Id));
        Add(ActualHas.Property("Items").ComparableTo(Expected.Items.Cast<XmlSchemaObject>()));
    }

    public override string? GetActualName(XsAnnotation actual) => null;
    public override string? GetExpectedName(XmlSchemaAnnotation expected) => null;
}
