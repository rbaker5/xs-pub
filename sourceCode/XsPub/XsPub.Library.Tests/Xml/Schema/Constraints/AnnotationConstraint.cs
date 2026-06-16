using System;
using System.Linq;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public class AnnotationConstraint : SchemaObjectConstraint<XsAnnotation, XmlSchemaAnnotation>
    {
        protected override void AddCustomConstraints()
        {
            base.AddCustomConstraints();
            Add(ActualHas.Property("Id").EqualTo(Expected.Id));
            Add(ActualHas.Property("Items").ComparableTo(Expected.Items.Cast<XmlSchemaObject>()));
        }

        public override string GetActualName(XsAnnotation actual)
        {
            return null;
        }

        public override string GetExpectedName(XmlSchemaAnnotation expected)
        {
            return null;
        }
    }
}