using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public abstract class ContentModelConstraint<TActual, TExpected> : AnnotatedConstraint<TActual, TExpected>
    where TActual : XsContentModel
    where TExpected : XmlSchemaContentModel
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("Content").ComparableTo(Expected!.Content));
    }
}
