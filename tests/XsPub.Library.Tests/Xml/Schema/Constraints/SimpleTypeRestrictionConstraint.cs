using System.Linq;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class SimpleTypeRestrictionConstraint : SimpleTypeContentConstraint<XsSimpleTypeRestriction, XmlSchemaSimpleTypeRestriction>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("BaseTypeName").ComparableTo(Expected!.BaseTypeName));
        Add(ActualHas.Property("BaseType").ComparableTo(Expected.BaseType));
        Add(ActualHas.Property("Facets").ComparableTo(Expected.Facets.Cast<XmlSchemaFacet>()));
    }

    public override string? GetActualName(XsSimpleTypeRestriction actual) => null;
    public override string? GetExpectedName(XmlSchemaSimpleTypeRestriction expected) => null;
}
