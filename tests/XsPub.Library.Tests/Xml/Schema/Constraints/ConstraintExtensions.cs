using System.Xml;
using System.Xml.Linq;
using NUnit.Comparisons.Xml;
using NUnit.Framework.Constraints;

namespace XsPub.Library.Tests.Xml.Schema.Constraints;

// Extends NUnit.Comparisons.ConstraintExtensions with an XmlQualifiedName-specific
// overload.  C# prefers this over the library's `object?` catch-all for XmlQualifiedName
// arguments, so Property("RefName").ComparableTo(Expected.RefName) routes here.
internal static class LocalConstraintExtensions
{
    public static Constraint ComparableTo(this ConstraintExpression expr, XmlQualifiedName? expected)
    {
        // When there is no declared ref/name the BCL returns XmlQualifiedName.Empty;
        // our model returns null.  Both mean "not present" — accept either.
        if (expected == null || expected.IsEmpty)
            return expr.Append(new NullOrEmptyXNameConstraint());

        var constraint = new NameConstraint();
        constraint.Initialize(expected);
        return expr.Append(constraint);
    }
}

// Standalone NUnit Constraint (not ICompareConstraint) so MEF does not register it
// and does not collide with the NameConstraint already registered for (XName, XmlQualifiedName).
internal sealed class NullOrEmptyXNameConstraint : Constraint
{
    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        bool success = actual is null ||
                       actual is XName { LocalName: "", NamespaceName: "" };
        return new ConstraintResult(this, actual, success);
    }

    public override string Description => "null or empty XName";
}
