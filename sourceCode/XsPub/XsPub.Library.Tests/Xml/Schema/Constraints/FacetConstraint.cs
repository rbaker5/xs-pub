using System;
using System.Xml.Schema;
using NUnit.Framework.Constraints;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public class FacetConstraint : AnnotatedConstraint<XsFacet, XmlSchemaFacet>
    {
        protected override void AddCustomConstraints()
        {
            //?base.AddCustomConstraints();
            Add(ActualHas.Property("IsFixed").EqualTo(Expected.IsFixed));
            Add(ActualHas.Property("Value").EqualTo(Expected.Value));
            Add(
                new PredicateConstraint<XsFacet>(
                    actual => string.Format("XmlSchema{0}Facet", actual.FacetType).Equals(Expected.GetType().Name)));
            
        }

        public override string GetActualName(XsFacet actual)
        {
            return null;
        }

        public override string GetExpectedName(XmlSchemaFacet expected)
        {
            return null;
        }
    }
}