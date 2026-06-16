using System;
using System.Linq;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public abstract class GroupBaseConstraint<TActual, TExpected> : ParticleConstraint<TActual, TExpected>
        where TActual : XsGroupBase
        where TExpected : XmlSchemaGroupBase
    {
        protected override void AddCustomConstraints()
        {
            base.AddCustomConstraints();
            Add(ActualHas.Property("Items").ComparableTo(Expected.Items.Cast<XmlSchemaObject>()));
        }
    }
}