using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class SimpleTypeListConstraint : SimpleTypeContentConstraint<XsSimpleTypeList, XmlSchemaSimpleTypeList>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("ItemTypeName").ComparableTo(Expected!.ItemTypeName));
        Add(ActualHas.Property("ItemType").ComparableTo(Expected.ItemType));
    }

    public override string? GetActualName(XsSimpleTypeList actual) => null;
    public override string? GetExpectedName(XmlSchemaSimpleTypeList expected) => null;
}
