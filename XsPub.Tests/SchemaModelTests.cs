using Xunit;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using XsPub.Library.Xml;
using XsPub.Library.Xml.Schema;

namespace XsPub.Tests;

// ── XsElement property round-trips ────────────────────────────────────────

public class XsElementTests
{
    [Fact]
    public void Create_ValidElement()
    {
        var e = new XsElement(new XElement(Xs.Element));
        Assert.NotNull(e);
    }

    [Fact]
    public void Create_RejectsWrongElement()
    {
        Assert.Throws<ArgumentException>(() => new XsElement(new XElement(Xs.ComplexType)));
    }

    [Fact]
    public void IsAbstract_RoundTrip()
    {
        var e = new XsElement(new XElement(Xs.Element));
        Assert.False(e.IsAbstract);
        Assert.Null(e.Element.Attribute(XsA.Abstract));

        e.IsAbstract = true;
        Assert.True(e.IsAbstract);
        Assert.Equal("true", e.Element.Attribute(XsA.Abstract)!.Value);

        e.IsAbstract = false;
        Assert.False(e.IsAbstract);
        Assert.Null(e.Element.Attribute(XsA.Abstract));
    }

    [Fact]
    public void DefaultValue_RoundTrip()
    {
        var e = new XsElement(new XElement(Xs.Element));
        Assert.Null(e.DefaultValue);
        Assert.Null(e.Element.Attribute(XsA.Default));

        e.DefaultValue = "abc";
        Assert.Equal("abc", e.DefaultValue);
        Assert.Equal("abc", e.Element.Attribute(XsA.Default)!.Value);

        e.DefaultValue = null;
        Assert.Null(e.DefaultValue);
        Assert.Null(e.Element.Attribute(XsA.Default));
    }

    [Fact]
    public void Block_NoneDoesNotWriteAttribute()
    {
        var e = new XsElement(new XElement(Xs.Element));
        e.Block = XmlSchemaDerivationMethod.None;
        Assert.Null(e.Element.Attribute(XsA.Block));
    }

    [Fact]
    public void Block_SingleValueRoundTrip()
    {
        var e = new XsElement(new XElement(Xs.Element));

        e.Block = XmlSchemaDerivationMethod.Extension;
        Assert.Equal(XmlSchemaDerivationMethod.Extension, e.Block);
        Assert.Equal("extension", e.Element.Attribute(XsA.Block)!.Value);

        e.Block = XmlSchemaDerivationMethod.Restriction;
        Assert.Equal(XmlSchemaDerivationMethod.Restriction, e.Block);
        Assert.Equal("restriction", e.Element.Attribute(XsA.Block)!.Value);

        e.Block = XmlSchemaDerivationMethod.Substitution;
        Assert.Equal(XmlSchemaDerivationMethod.Substitution, e.Block);
        Assert.Equal("substitution", e.Element.Attribute(XsA.Block)!.Value);
    }

    [Fact]
    public void Block_CombinedFlagsRoundTrip()
    {
        var e = new XsElement(new XElement(Xs.Element));

        e.Block = XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Restriction;
        Assert.Equal(XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Restriction, e.Block);
        Assert.Equal("extension restriction", e.Element.Attribute(XsA.Block)!.Value);

        e.Block = XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Substitution;
        Assert.Equal(XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Substitution, e.Block);
        Assert.Equal("restriction substitution", e.Element.Attribute(XsA.Block)!.Value);

        e.Block = XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Substitution;
        Assert.Equal(XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Substitution, e.Block);
        Assert.Equal("extension restriction substitution", e.Element.Attribute(XsA.Block)!.Value);
    }

    [Fact]
    public void Block_AllMapsToHashAll()
    {
        var e = new XsElement(new XElement(Xs.Element));
        e.Block = XmlSchemaDerivationMethod.All;
        Assert.Equal(XmlSchemaDerivationMethod.All, e.Block);
        Assert.Equal("#all", e.Element.Attribute(XsA.Block)!.Value);
    }

    [Fact]
    public void Block_ResetToNoneRemovesAttribute()
    {
        var e = new XsElement(new XElement(Xs.Element));
        e.Block = XmlSchemaDerivationMethod.Extension;
        e.Block = XmlSchemaDerivationMethod.None;
        Assert.Null(e.Element.Attribute(XsA.Block));
    }
}

// ── XsObject enum attribute infrastructure ────────────────────────────────

public class XsObjectTests
{
    private sealed class TestObject : XsObject
    {
        public TestObject() : base(new XElement("test")) { }

        public XmlSchemaDerivationMethod GetEnum(string name) =>
            GetAttributeValueInternal(name, XmlSchemaDerivationMethod.None);

        public void SetEnum(string name, XmlSchemaDerivationMethod value) =>
            SetAttributeValueInternal(name, value, XmlSchemaDerivationMethod.None);
    }

    [Fact]
    public void EnumAttribute_DefaultNotWritten()
    {
        var obj = new TestObject();
        Assert.Equal(XmlSchemaDerivationMethod.None, obj.GetEnum("attr"));
        Assert.Null(obj.Element.Attribute("attr"));
    }

    [Fact]
    public void EnumAttribute_NonDefaultWritten()
    {
        var obj = new TestObject();
        obj.SetEnum("attr", XmlSchemaDerivationMethod.All);
        Assert.Equal(XmlSchemaDerivationMethod.All, obj.GetEnum("attr"));
        Assert.NotNull(obj.Element.Attribute("attr"));
    }

    [Fact]
    public void EnumAttribute_ResetToDefaultRemovesAttribute()
    {
        var obj = new TestObject();
        obj.SetEnum("attr", XmlSchemaDerivationMethod.All);
        obj.SetEnum("attr", XmlSchemaDerivationMethod.None);
        Assert.Equal(XmlSchemaDerivationMethod.None, obj.GetEnum("attr"));
        Assert.Null(obj.Element.Attribute("attr"));
    }
}

// ── Schema loading and descendant traversal ───────────────────────────────

public class XsSchemaLoadTests
{
    private static string TestFile(string name) =>
        Path.Combine(AppContext.BaseDirectory, "TestData", name);

    [Theory]
    [InlineData("Minimal.xsd")]
    [InlineData("BitOfEverything.xsd")]
    [InlineData("TurkSchema.xsd")]
    public void Load_SchemaFromFile_SucceedsAndEnumeratesDescendants(string fileName)
    {
        var schema = XsSchema.Load(TestFile(fileName));
        Assert.NotNull(schema);
        foreach (var obj in schema.Descendents)
            Assert.Equal(schema, obj.GetSchema());
    }

    [Fact]
    public void Load_ImporterXsd_TraversesImportedSchema()
    {
        var schema = XsSchema.Load(TestFile("Importer.xsd"));
        var imported = schema.Includes.First();
        Assert.NotNull(imported);
        var child = imported.Schema;
        Assert.NotNull(child);
    }

    [Fact]
    public void Load_IncluderXsd_TraversesIncludedSchema()
    {
        var schema = XsSchema.Load(TestFile("Includer.xsd"));
        var included = schema.Includes.First();
        Assert.NotNull(included);
        var child = included.Schema;
        Assert.NotNull(child);
    }

    [Fact]
    public void Load_String_DefaultsToLocalOnlyResolver()
    {
        // XsSchema.Load(string) must install LocalOnlyXmlResolver by default.
        // Verify: an http:// schemaLocation blocks when the include is accessed.
        var tmpDir = Path.Combine(Path.GetTempPath(), "xspub-schemaload-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tmpDir);
        var schemaPath = Path.Combine(tmpDir, "test.xsd");
        File.WriteAllText(schemaPath, """
            <?xml version="1.0" encoding="utf-8"?>
            <xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
              <xs:include schemaLocation="https://example.com/external.xsd" />
            </xs:schema>
            """);
        try
        {
            var schema = XsSchema.Load(schemaPath);
            var ex = Assert.Throws<InvalidOperationException>(
                () => { var _ = schema.Includes.First().Schema; });
            Assert.Contains("--allow-external", ex.Message);
        }
        finally
        {
            Directory.Delete(tmpDir, recursive: true);
        }
    }
}
