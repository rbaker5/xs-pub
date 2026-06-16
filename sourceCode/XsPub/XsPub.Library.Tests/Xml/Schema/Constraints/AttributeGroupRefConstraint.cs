using System;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public class AttributeGroupRefConstraint : AnnotatedConstraint<XsAttributeGroupRef, XmlSchemaAttributeGroupRef>
    {
        protected override void AddCustomConstraints()
        {
            base.AddCustomConstraints();
            Add(ActualHas.Property("RefName").ComparableTo(Expected.RefName));
        }

        public override string GetActualName(XsAttributeGroupRef actual)
        {
            return String.Format("{{{0}}}{1}", actual.RefName.NamespaceName, actual.RefName.LocalName);
        }

        public override string GetExpectedName(XmlSchemaAttributeGroupRef expected)
        {
            return String.Format("{{{0}}}{1}", expected.RefName.Namespace, expected.RefName.Name);
        }
    }
}