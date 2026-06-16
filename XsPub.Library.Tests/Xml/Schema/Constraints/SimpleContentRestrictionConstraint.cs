using System;
using System.Linq;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public class SimpleContentRestrictionConstraint : ContentConstraint<XsSimpleContentRestriction, XmlSchemaSimpleContentRestriction>
    {
        protected override void AddCustomConstraints()
        {
            base.AddCustomConstraints();
            Add(ActualHas.Property("BaseTypeName").ComparableTo(Expected.BaseTypeName));
            Add(ActualHas.Property("AnyAttribute").ComparableTo(Expected.AnyAttribute));
            Add(ActualHas.Property("BaseType").ComparableTo(Expected.BaseType));
            Add(ActualHas.Property("Attributes").ComparableTo(Expected.Attributes.Cast<XmlSchemaObject>()));
            Add(ActualHas.Property("Facets").ComparableTo(Expected.Facets.Cast<XmlSchemaFacet>()));
        }

        public override string GetActualName(XsSimpleContentRestriction actual)
        {
            return null;
        }

        public override string GetExpectedName(XmlSchemaSimpleContentRestriction expected)
        {
            return null;
        }
    }
}