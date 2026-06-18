using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using NUnit.Comparisons.Xml;
using NUnit.Framework;
using XsPub.Library.Tests.Xml.Schema.Constraints;
// Alias resolves the ambiguity between NUnit.Comparisons.Is and NUnit.Framework.Is;
// NUnit.Comparisons.Is inherits all NUnit.Framework.Is members so nothing is lost.
using Is = NUnit.Comparisons.Is;

namespace XsPub.Library.Tests;

[TestFixture]
class XSchemaFixture
{
    [OneTimeSetUp]
    public void SetupAssembly()
    {
        // Register constraint implementations in this assembly (all the XsXxx constraints)
        CompareConstraintFactory.AddAssembly(Assembly.GetExecutingAssembly());
        // Register XElement/XText/XComment/XAttribute constraints from NUnit.Comparisons.Xml
        // (needed for AppInfo/Documentation markup comparison)
        CompareConstraintFactory.AddAssembly(typeof(XElementConstraint).Assembly);
    }

    [Test] public void ReadMinimalSchema()            => ReadAndCompare(TestFile("Minimal.xsd"));
    [Test] public void EnumerateMinimalSchema()       => EnumerateDescendants(TestFile("Minimal.xsd"));
    [Test] public void ReadBitOfEverythingSchema()    => ReadAndCompare(TestFile("BitOfEverything.xsd"));
    [Test] public void EnumerateBitOfEverythingSchema() => EnumerateDescendants(TestFile("BitOfEverything.xsd"));
    [Test] public void ReadTurkSchema()               => ReadAndCompare(TestFile("TurkSchema.xsd"));
    [Test] public void EnumerateTurkSchema()          => EnumerateDescendants(TestFile("TurkSchema.xsd"));
    [Test] public void ReadImporter()                 => ReadAndCompare(TestFile("Importer.xsd"));
    [Test] public void EnumerateImporter()            => EnumerateDescendants(TestFile("Importer.xsd"));
    [Test] public void ReadIncluder()                 => ReadAndCompare(TestFile("Includer.xsd"));
    [Test] public void EnumerateIncluder()            => EnumerateDescendants(TestFile("Includer.xsd"));

    private static void ReadAndCompare(string schemaPath)
    {
        var schema = XsSchema.Load(schemaPath);

        XmlSchema xmlSchema;
        using (var reader = XmlReader.Create(schemaPath))
            xmlSchema = XmlSchema.Read(reader, null)!;

        var schemaSet = new XmlSchemaSet { XmlResolver = new XmlUrlResolver() };
        schemaSet.Add(xmlSchema);
        schemaSet.Compile();

        Assert.That(schema, Is.ComparableTo(xmlSchema));
    }

    private static void EnumerateDescendants(string schemaPath)
    {
        var schema = XsSchema.Load(schemaPath);
        foreach (var obj in schema.Descendents)
            Assert.That(obj.GetSchema(), Is.SameAs(schema));
    }

    private static string TestFile(string name) =>
        Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", name);
}
