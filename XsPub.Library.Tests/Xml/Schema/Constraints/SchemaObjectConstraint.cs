using System.Xml.Schema;
using NUnit.Comparisons;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

// TODO: XmlSchemaKey, XmlSchemaKeyref, XmlSchemaUnique, XmlSchemaXPath not yet covered.

public abstract class SchemaObjectConstraint<TActual, TExpected> : CompareConstraint<TActual, TExpected>
    where TActual : XsObject
    where TExpected : XmlSchemaObject
{
    protected override void AddCustomConstraints()
    {
        Add(ActualHas.Property("SourceUri").EqualTo(Expected!.SourceUri));
        Add(ActualHas.Property("LineNumber").EqualTo(Expected.LineNumber));
        Add(ActualHas.Property("LinePosition").EqualTo(Expected.LinePosition));
        Add(ActualHas.Property("Namespaces").EquivalentTo(Expected.Namespaces.ToArray()));
    }
}
