using System;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public class SimpleTypeListConstraint : SimpleTypeContentConstraint<XsSimpleTypeList, XmlSchemaSimpleTypeList>
    {
        protected override void AddCustomConstraints()
        {
            base.AddCustomConstraints();
            Add(ActualHas.Property("ItemTypeName").ComparableTo(Expected.ItemTypeName));
            //Add(ActualHas.Property("BaseItemType").ComparableTo(Expected.BaseItemType));
            Add(ActualHas.Property("ItemType").ComparableTo(Expected.ItemType));
        }

        public override string GetActualName(XsSimpleTypeList actual)
        {
            return null;
        }

        public override string GetExpectedName(XmlSchemaSimpleTypeList expected)
        {
            return null;
        }
    }
}