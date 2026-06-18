using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public abstract class TypeConstraint<TActual, TExpected> : AnnotatedConstraint<TActual, TExpected>
    where TActual : XsType
    where TExpected : XmlSchemaType
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("Final").EqualTo(Expected!.Final));
        Add(ActualHas.Property("FinalResolved").EqualTo(Expected.FinalResolved));
        Add(ActualHas.Property("IsMixed").EqualTo(Expected.IsMixed));
        Add(ActualHas.Property("Name").EqualTo(Expected.Name));
        Add(ActualHas.Property("QualifiedName").ComparableTo(Expected.QualifiedName));
    }

    public override string? GetActualName(TActual actual) => actual.Name;
    public override string? GetExpectedName(TExpected expected) => expected.Name;
}
