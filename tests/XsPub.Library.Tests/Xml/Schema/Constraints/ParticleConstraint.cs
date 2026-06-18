using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public abstract class ParticleConstraint<TActual, TExpected> : AnnotatedConstraint<TActual, TExpected>
    where TActual : XsParticle
    where TExpected : XmlSchemaParticle
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("MaxOccurs").EqualTo(Expected!.MaxOccurs == decimal.MaxValue ? int.MaxValue : (int)Expected.MaxOccurs));
        Add(ActualHas.Property("MinOccurs").EqualTo(Expected.MinOccurs == decimal.MaxValue ? int.MaxValue : (int)Expected.MinOccurs));
    }
}
