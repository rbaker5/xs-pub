using System;
using System.Linq;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public abstract class ParticleConstraint<TActual, TExpected> : AnnotatedConstraint<TActual, TExpected>
        where TActual : XsParticle
        where TExpected : XmlSchemaParticle
    {
        protected override void AddCustomConstraints()
        {
            base.AddCustomConstraints();
            Add(ActualHas.Property("MaxOccurs").EqualTo(Expected.MaxOccurs == Decimal.MaxValue ? Int32.MaxValue : (int)Expected.MaxOccurs));
            Add(ActualHas.Property("MinOccurs").EqualTo(Expected.MinOccurs == Decimal.MaxValue ? Int32.MaxValue : (int)Expected.MinOccurs));
        }
    }
}