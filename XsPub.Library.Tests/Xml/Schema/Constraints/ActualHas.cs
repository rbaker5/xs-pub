using System;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework.Constraints;
using NUnit.Comparisons;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    internal static class ActualHas
    {
        public static RegisteredCompareConstraint ComparableTo(XmlSchemaObject expected)
        {
            return new RegisteredCompareConstraint(expected);
        }

        public static XAttributeConstraint ComparableTo(XmlAttribute expected)
        {
            var constraint = new XAttributeConstraint();
            constraint.Initialize(expected);
            return constraint;
        }

        public static NameConstraint ComparableTo(XmlQualifiedName expected)
        {
            var constraint = new NameConstraint();
            constraint.Initialize(expected);
            return constraint;
        }

        public static ResolvableConstraintExpression Property(string name)
        {
            return new ConstraintExpression().PropertyExt(name);
        }
    }
}