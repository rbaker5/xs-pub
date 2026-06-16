using System;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public abstract class ExternalConstraint<TActual, TExpected> : SchemaObjectConstraint<TActual, TExpected>
        where TActual : XsExternal
        where TExpected : XmlSchemaExternal
    {
        protected override void AddCustomConstraints()
        {
            base.AddCustomConstraints();
            Add(ActualHas.Property("Id").EqualTo(Expected.Id));
            Add(ActualHas.Property("SchemaLocation").EqualTo(Expected.SchemaLocation));

            Add(ActualHas.Property("Schema").ComparableTo(Expected.Schema));
            Add(ActualHas.Property("UnhandledAttributes").ComparableTo(Expected.UnhandledAttributes.EmptyIfNull()));
        }
    }
}