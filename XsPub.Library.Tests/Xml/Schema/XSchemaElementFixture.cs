using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using NUnit.Framework;
using XsPub.Library.Xml;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema
{
    [TestFixture]
    public class XSchemaElementFixture
    {
        [Test]
        public void Create()
        {
            new XsElement(new XElement(Xs.Element));
        }

        [Test]
        public void RejectInvalid()
        {
            Assert.Throws<ArgumentException>(() => new XsElement(new XElement(Xs.ComplexType)));
        }

        [Test]
        public void ReadElementWithoutParent()
        {
            var doc = XDocument.Load("TestData\\Minimal.xsd");
            var elements = doc.Descendants(Xs.Element);
            foreach (var element in elements)
            {
                var schemaElement = new XsElement(element);
                Assert.AreEqual(0, schemaElement.Namespaces.Count());
            }
        }

        [Test]
        public void TestAbstractWrites()
        {
            var element = new XsElement(new XElement(Xs.Element));
            Assert.IsNull(element.Element.Attribute(XsA.Block));

            element.IsAbstract = true;
            Assert.AreEqual(true, element.IsAbstract);
            Assert.AreEqual("true", element.Element.Attribute(XsA.Abstract).Value);

            element.IsAbstract = false;
            Assert.AreEqual(false, element.IsAbstract);
            Assert.IsNull(element.Element.Attribute(XsA.Block));
        }

        [Test]
        public void TestBlockWrites()
        {
            var element = new XsElement(new XElement(Xs.Element));
            Assert.That(element.Element.Attribute(XsA.Block), Is.Null);

            element.Block = XmlSchemaDerivationMethod.None;
            Assert.That(element.Element.Attribute(XsA.Block), Is.Null);

            element.Block = XmlSchemaDerivationMethod.Extension;
            Assert.That(element.Block, Is.EqualTo(XmlSchemaDerivationMethod.Extension));
            Assert.That(element.Element.Attribute(XsA.Block).Value, Is.EqualTo("extension"));

            element.Block = XmlSchemaDerivationMethod.Restriction;
            Assert.AreEqual(XmlSchemaDerivationMethod.Restriction, element.Block);
            Assert.AreEqual("restriction", element.Element.Attribute(XsA.Block).Value);

            element.Block = XmlSchemaDerivationMethod.Substitution;
            Assert.AreEqual(XmlSchemaDerivationMethod.Substitution, element.Block);
            Assert.AreEqual("substitution", element.Element.Attribute(XsA.Block).Value);

            element.Block = XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Restriction;
            Assert.AreEqual(XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Restriction, element.Block);
            Assert.AreEqual("extension restriction", element.Element.Attribute(XsA.Block).Value);

            element.Block = XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Substitution;
            Assert.AreEqual(XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Substitution, element.Block);
            Assert.AreEqual("restriction substitution", element.Element.Attribute(XsA.Block).Value);

            element.Block = XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Substitution;
            Assert.AreEqual(XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Substitution, element.Block);
            Assert.AreEqual("extension substitution", element.Element.Attribute(XsA.Block).Value);

            element.Block = XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Substitution;
            Assert.AreEqual(XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Substitution, element.Block);
            Assert.AreEqual("extension restriction substitution", element.Element.Attribute(XsA.Block).Value);

            element.Block = XmlSchemaDerivationMethod.All;
            Assert.AreEqual(XmlSchemaDerivationMethod.All, element.Block);
            Assert.AreEqual("#all", element.Element.Attribute(XsA.Block).Value);

            element.Block = XmlSchemaDerivationMethod.None;
            Assert.IsNull(element.Element.Attribute(XsA.Block));
        }

        [Test]
        public void TestDefaultWrites()
        {
            var element = new XsElement(new XElement(Xs.Element));
            Assert.AreEqual(null, element.DefaultValue);
            Assert.IsNull(element.Element.Attribute(XsA.Default));

            element.DefaultValue = "abc";
            Assert.AreEqual("abc", element.DefaultValue);
            Assert.AreEqual("abc", element.Element.Attribute(XsA.Default).Value);

            element.DefaultValue = null;
            Assert.AreEqual(null, element.DefaultValue);
            Assert.IsNull(element.Element.Attribute(XsA.Default));
        }

    }
}
