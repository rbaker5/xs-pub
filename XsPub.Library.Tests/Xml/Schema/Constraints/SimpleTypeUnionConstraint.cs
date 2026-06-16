using System;
using System.Linq;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public class SimpleTypeUnionConstraint : SimpleTypeContentConstraint<XsSimpleTypeUnion, XmlSchemaSimpleTypeUnion>
    {
        protected override void AddCustomConstraints()
        {
            base.AddCustomConstraints();
            //Add(ActualHas.Property("MemberTypes").ComparableTo(Expected.MemberTypes));
            //Add(ActualHas.Property("BaseMemberTypes").ComparableTo(Expected.BaseMemberTypes.EmptyIfNull()));
            Add(ActualHas.Property("BaseTypes").ComparableTo(Expected.BaseTypes.Cast<XmlSchemaSimpleType>()));
        }

        public override string GetActualName(XsSimpleTypeUnion actual)
        {
            return null;
        }

        public override string GetExpectedName(XmlSchemaSimpleTypeUnion expected)
        {
            return null;
        }
    }
}