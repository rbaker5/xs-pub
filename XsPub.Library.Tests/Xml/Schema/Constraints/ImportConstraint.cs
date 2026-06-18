using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class ImportConstraint : ExternalConstraint<XsImport, XmlSchemaImport>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("Namespace").EqualTo(Expected!.Namespace));
        Add(ActualHas.Property("Annotation").ComparableTo(Expected.Annotation));
    }

    public override string? GetActualName(XsImport actual) => null;
    public override string? GetExpectedName(XmlSchemaImport expected) => null;
}
