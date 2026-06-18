using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public abstract class AnnotatedConstraint<TActual, TExpected> : SchemaObjectConstraint<TActual, TExpected>
    where TActual : XsAnnotated
    where TExpected : XmlSchemaAnnotated
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("Id").EqualTo(Expected!.Id));
        Add(ActualHas.Property("Annotation").ComparableTo(Expected.Annotation));
        Add(ActualHas.Property("UnhandledAttributes").ComparableTo(Expected.UnhandledAttributes!.EmptyIfNull()));
    }
}
