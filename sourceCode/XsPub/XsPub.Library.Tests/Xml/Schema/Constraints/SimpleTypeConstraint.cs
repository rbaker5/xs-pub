using System;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema.Constraints
{
    public class SimpleTypeConstraint : TypeConstraint<XsSimpleType, XmlSchemaSimpleType>
    {
        protected override void AddCustomConstraints()
        {
            base.AddCustomConstraints();
            Add(ActualHas.Property("Content").ComparableTo(Expected.Content));
        }
    }
}