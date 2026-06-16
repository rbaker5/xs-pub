using System;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public class NotationConstraint : AnnotatedConstraint<XsNotation, XmlSchemaNotation>
    {
        protected override void AddCustomConstraints()
        {
            base.AddCustomConstraints();
            Add(ActualHas.Property("Name").EqualTo(Expected.Name));
            Add(ActualHas.Property("Public").EqualTo(Expected.Public));
            Add(ActualHas.Property("System").EqualTo(Expected.System));
        }

        public override string GetActualName(XsNotation actual)
        {
            return null;
        }

        public override string GetExpectedName(XmlSchemaNotation expected)
        {
            return null;
        }
    }
}