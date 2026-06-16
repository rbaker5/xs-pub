using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema
{
    [TestFixture]
    public class SchemaObjectFixture
    {
        [Test]
        public void ReadWriteEnum()
        {
            var testObject = new TestSchemaObject();
            Assert.AreEqual(XmlSchemaDerivationMethod.None, testObject.GetAttributeValueInternal("newAttribute", XmlSchemaDerivationMethod.None));
            Assert.IsNull(testObject.Element.Attribute("newAttribute"));

            testObject.SetAttributeValueInternal("newAttribute", XmlSchemaDerivationMethod.None, XmlSchemaDerivationMethod.None);
            Assert.AreEqual(XmlSchemaDerivationMethod.None, testObject.GetAttributeValueInternal("newAttribute", XmlSchemaDerivationMethod.None));
            Assert.IsNull(testObject.Element.Attribute("newAttribute"));

            testObject.SetAttributeValueInternal("newAttribute", XmlSchemaDerivationMethod.All, XmlSchemaDerivationMethod.None);
            Assert.AreEqual(XmlSchemaDerivationMethod.All, testObject.GetAttributeValueInternal("newAttribute", XmlSchemaDerivationMethod.None));

            testObject.SetAttributeValueInternal("newAttribute", XmlSchemaDerivationMethod.None, XmlSchemaDerivationMethod.None);
            Assert.AreEqual(XmlSchemaDerivationMethod.None, testObject.GetAttributeValueInternal("newAttribute", XmlSchemaDerivationMethod.None));
            Assert.IsNull(testObject.Element.Attribute("newAttribute"));
        }
    }

    public class TestSchemaObject : XsObject
    {
        public TestSchemaObject() : base(new XElement("test"))
        {
        }

        //public TestSchemaObject(XsObject parent)
        //    : base(new XElement("test"), parent)
        //{
        //}

        
    }
}
