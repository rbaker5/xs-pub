using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using NUnit.Comparisons;
using NUnit.Framework;
using XsPub.Library.Tests.Xml.Schema.Constraints;
using XsPub.Library.Xml.Schema;

namespace XsPub.Library.Tests.Xml.Schema
{
    [TestFixture]
    class XSchemaFixture
    {
        [TestFixtureSetUp]
        public void SetupAssembly()
        {
            CompareConstraintFactory.AddAssembly(Assembly.GetExecutingAssembly());
        }

        [Test]
        public void ReadMinimalSchema()
        {
            readAndCompare(@"TestData\Minimal.xsd");
        }

        [Test]
        public void EnumerateMinimalSchema()
        {
            enumerateDescendants(@"TestData\Minimal.xsd");
        }

        [Test]
        public void ReadBitOfEverythingSchema()
        {
            readAndCompare(@"TestData\BitOfEverything.xsd");
        }

        [Test]
        public void EnumerateBitOfEverythingSchema()
        {
            enumerateDescendants(@"TestData\BitOfEverything.xsd");
        }

        [Test]
        public void ReadTurkSchema()
        {
            readAndCompare(@"TestData\TurkSchema.xsd");
        }

        [Test]
        public void EnumerateTurkSchema()
        {
            enumerateDescendants(@"TestData\TurkSchema.xsd");
        }

        [Test]
        public void ReadImporter()
        {
            readAndCompare(@"TestData\Importer.xsd");
        }

        [Test]
        public void EnumerateImporter()
        {
            enumerateDescendants(@"TestData\Importer.xsd");
        }

        [Test]
        public void ReadIncluder()
        {
            readAndCompare(@"TestData\Includer.xsd");
        }

        [Test]
        public void EnumerateIncluder()
        {
            enumerateDescendants(@"TestData\Includer.xsd");
        }

        private void readAndCompare(string schemaPath)
        {
            var schema = XsSchema.Load(schemaPath);
            XmlSchema xmlSchema;
            using (var reader = XmlReader.Create(schemaPath))
            {
                xmlSchema = XmlSchema.Read(reader, null);
            }
            var schemaSet = new XmlSchemaSet {XmlResolver = new XmlUrlResolver()};
            schemaSet.Add(xmlSchema);
            schemaSet.Compile();

            Assert.That(schema, ActualHas.ComparableTo(xmlSchema));
        }

        private void enumerateDescendants(string schemaPath)
        {
            var schema = XsSchema.Load(schemaPath);
            foreach (var xsObject in schema.Descendents)
            {
                Assert.AreEqual(schema, xsObject.GetSchema());
            }
        }
    }
}
