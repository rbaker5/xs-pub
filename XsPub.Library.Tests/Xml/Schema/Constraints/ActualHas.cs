using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using NUnit.Comparisons;
using NUnit.Comparisons.Xml;
using NUnit.Framework.Constraints;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

internal static class ActualHas
{
    public static RegisteredCompareConstraint ComparableTo(XmlSchemaObject? expected) =>
        new RegisteredCompareConstraint(expected);

    public static XAttributeConstraint ComparableTo(XmlAttribute? expected)
    {
        var constraint = new XAttributeConstraint();
        if (expected != null) constraint.Initialize(expected);
        return constraint;
    }

    public static NameConstraint ComparableTo(XmlQualifiedName? expected)
    {
        var constraint = new NameConstraint();
        if (expected != null) constraint.Initialize(expected);
        return constraint;
    }

    public static ResolvableConstraintExpression Property(string name) =>
        new ConstraintExpression().PropertyExt(name);
}
