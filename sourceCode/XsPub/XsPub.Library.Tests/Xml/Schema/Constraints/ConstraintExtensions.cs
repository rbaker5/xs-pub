using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using NUnit.Comparisons;
using NUnit.Framework.Constraints;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    static class ConstraintExtensions
    {
        public static RegisteredCompareConstraint ComparableTo(this ConstraintExpression baseExpression, XmlSchemaObject expected)
        {
            return (RegisteredCompareConstraint)baseExpression.Append(new RegisteredCompareConstraint(expected));
        }

        public static XAttributeConstraint ComparableTo(this ConstraintExpression baseExpression, XmlAttribute expected)
        {
            return (XAttributeConstraint)baseExpression.Append(ActualHas.ComparableTo(expected));
        }

        public static NameConstraint ComparableTo(this ConstraintExpression baseExpression, XmlQualifiedName expected)
        {
            return (NameConstraint)baseExpression.Append(ActualHas.ComparableTo(expected));
        }

        public static CollectionItemsEqualConstraint ComparableTo(this ConstraintExpression baseExpression, IEnumerable<XmlSchemaObject> expected)
        {
            return (CollectionEquivalentConstraint)baseExpression.Append(new CollectionComparableConstraint(expected));
        }

        public static CollectionItemsEqualConstraint ComparableTo(this ConstraintExpression baseExpression, IEnumerable<XmlNode> expected)
        {
            return (CollectionEquivalentConstraint)baseExpression.Append(new CollectionComparableConstraint(expected));
        }

        public static CollectionItemsEqualConstraint ComparableTo(this ConstraintExpression baseExpression, IEnumerable<XmlQualifiedName> expected)
        {
            return (CollectionEquivalentConstraint)baseExpression.Append(new CollectionComparableConstraint(expected));
        }
    }
}