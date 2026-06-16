using System;
using System.Xml.Schema;
using NUnit.Comparisons;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public class AppInfoConstraint : SchemaObjectConstraint<XsAppInfo, XmlSchemaAppInfo>
    {
        protected override void AddCustomConstraints()
        {
            base.AddCustomConstraints();

            Add(ActualHas.Property("Source").EqualTo(Expected.Source));
            Add(ActualHas.Property("Element").Method("Nodes").ComparableTo(Expected.Markup.EmptyIfNull()));
            
        }

        public override string GetActualName(XsAppInfo actual)
        {
            return null;
        }

        public override string GetExpectedName(XmlSchemaAppInfo expected)
        {
            return null;
        }
    }
}