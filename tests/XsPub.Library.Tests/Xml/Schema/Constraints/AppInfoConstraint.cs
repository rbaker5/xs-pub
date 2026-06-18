using NUnit.Comparisons;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

public class AppInfoConstraint : SchemaObjectConstraint<XsAppInfo, XmlSchemaAppInfo>
{
    protected override void AddCustomConstraints()
    {
        base.AddCustomConstraints();
        Add(ActualHas.Property("Source").EqualTo(Expected!.Source));
        Add(ActualHas.Property("Element").Method("Nodes").ComparableTo(Expected.Markup.EmptyIfNull()));
    }

    public override string? GetActualName(XsAppInfo actual) => null;
    public override string? GetExpectedName(XmlSchemaAppInfo expected) => null;
}
