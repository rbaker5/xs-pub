using System.Xml.Schema;
using NUnit.Framework.Constraints;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class FacetConstraint : AnnotatedConstraint<XsFacet, XmlSchemaFacet>
{
    protected override void AddCustomConstraints()
    {
        Add(ActualHas.Property("IsFixed").EqualTo(Expected!.IsFixed));
        Add(ActualHas.Property("Value").EqualTo(Expected.Value));
        Add(new PredicateConstraint<XsFacet>(
            actual => $"XmlSchema{actual.FacetType}Facet".Equals(Expected.GetType().Name)));
    }

    public override string? GetActualName(XsFacet actual) => null;
    public override string? GetExpectedName(XmlSchemaFacet expected) => null;
}
