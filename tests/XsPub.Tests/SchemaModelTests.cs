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

// ── XsComplexType property round-trips ───────────────────────────────────

public class XsComplexTypeTests
{
    [Fact]
    public void IsMixed_DefaultFalse_NoAttribute()
    {
        var t = new XsComplexType(new XElement(Xs.ComplexType));
        Assert.False(t.IsMixed);
        Assert.Null(t.Element.Attribute(XsA.Mixed));
    }

    [Fact]
    public void IsMixed_SetTrue_WritesAttribute()
    {
        var t = new XsComplexType(new XElement(Xs.ComplexType));
        t.IsMixed = true;
        Assert.True(t.IsMixed);
        Assert.Equal("true", t.Element.Attribute(XsA.Mixed)!.Value);
    }

    [Fact]
    public void IsMixed_ResetToFalse_RemovesAttribute()
    {
        var t = new XsComplexType(new XElement(Xs.ComplexType));
        t.IsMixed = true;
        t.IsMixed = false;
        Assert.False(t.IsMixed);
        Assert.Null(t.Element.Attribute(XsA.Mixed));
    }

    [Fact]
    public void IsAbstract_DefaultFalse_NoAttribute()
    {
        var t = new XsComplexType(new XElement(Xs.ComplexType));
        Assert.False(t.IsAbstract);
        Assert.Null(t.Element.Attribute(XsA.Abstract));
    }

    [Fact]
    public void IsAbstract_SetTrue_WritesAttribute()
    {
        var t = new XsComplexType(new XElement(Xs.ComplexType));
        t.IsAbstract = true;
        Assert.True(t.IsAbstract);
        Assert.Equal("true", t.Element.Attribute(XsA.Abstract)!.Value);
    }

    [Fact]
    public void IsAbstract_ResetToFalse_RemovesAttribute()
    {
        var t = new XsComplexType(new XElement(Xs.ComplexType));
        t.IsAbstract = true;
        t.IsAbstract = false;
        Assert.Null(t.Element.Attribute(XsA.Abstract));
    }

    [Fact]
    public void Block_NoneDoesNotWriteAttribute()
    {
        var t = new XsComplexType(new XElement(Xs.ComplexType));
        t.Block = XmlSchemaDerivationMethod.None;
        Assert.Null(t.Element.Attribute(XsA.Block));
    }

    [Fact]
    public void Block_ExtensionRoundTrip()
    {
        var t = new XsComplexType(new XElement(Xs.ComplexType));
        t.Block = XmlSchemaDerivationMethod.Extension;
        Assert.Equal(XmlSchemaDerivationMethod.Extension, t.Block);
        Assert.Equal("extension", t.Element.Attribute(XsA.Block)!.Value);
    }

    [Fact]
    public void Block_AllMapsToHashAll()
    {
        var t = new XsComplexType(new XElement(Xs.ComplexType));
        t.Block = XmlSchemaDerivationMethod.All;
        Assert.Equal(XmlSchemaDerivationMethod.All, t.Block);
        Assert.Equal("#all", t.Element.Attribute(XsA.Block)!.Value);
    }

    [Fact]
    public void Block_ResetToNoneRemovesAttribute()
    {
        var t = new XsComplexType(new XElement(Xs.ComplexType));
        t.Block = XmlSchemaDerivationMethod.Restriction;
        t.Block = XmlSchemaDerivationMethod.None;
        Assert.Null(t.Element.Attribute(XsA.Block));
    }
}

// ── XsAttribute property round-trips ─────────────────────────────────────

public class XsAttributeTests
{
    private static XsAttribute Make() => new XsAttribute(new XElement(Xs.Attribute), null);

    [Fact]
    public void Use_DefaultNone_NoAttribute()
    {
        var a = Make();
        Assert.Equal(XmlSchemaUse.None, a.Use);
        Assert.Null(a.Element.Attribute(XsA.Use));
    }

    [Fact]
    public void Use_Required_WritesAttribute()
    {
        var a = Make();
        a.Use = XmlSchemaUse.Required;
        Assert.Equal(XmlSchemaUse.Required, a.Use);
        Assert.Equal("required", a.Element.Attribute(XsA.Use)!.Value);
    }

    [Fact]
    public void Use_Prohibited_WritesAttribute()
    {
        var a = Make();
        a.Use = XmlSchemaUse.Prohibited;
        Assert.Equal(XmlSchemaUse.Prohibited, a.Use);
        Assert.Equal("prohibited", a.Element.Attribute(XsA.Use)!.Value);
    }

    [Fact]
    public void Use_ResetToNone_RemovesAttribute()
    {
        var a = Make();
        a.Use = XmlSchemaUse.Required;
        a.Use = XmlSchemaUse.None;
        Assert.Null(a.Element.Attribute(XsA.Use));
    }

    [Fact]
    public void Form_DefaultNone_NoAttribute()
    {
        var a = Make();
        Assert.Equal(XmlSchemaForm.None, a.Form);
        Assert.Null(a.Element.Attribute(XsA.Form));
    }

    [Fact]
    public void Form_Qualified_WritesAttribute()
    {
        var a = Make();
        a.Form = XmlSchemaForm.Qualified;
        Assert.Equal(XmlSchemaForm.Qualified, a.Form);
        Assert.Equal("qualified", a.Element.Attribute(XsA.Form)!.Value);
    }

    [Fact]
    public void DefaultValue_RoundTrip()
    {
        var a = Make();
        Assert.Null(a.DefaultValue);
        a.DefaultValue = "hello";
        Assert.Equal("hello", a.DefaultValue);
        a.DefaultValue = null;
        Assert.Null(a.DefaultValue);
        Assert.Null(a.Element.Attribute(XsA.Default));
    }

    [Fact]
    public void FixedValue_RoundTrip()
    {
        var a = Make();
        Assert.Null(a.FixedValue);
        a.FixedValue = "42";
        Assert.Equal("42", a.FixedValue);
        a.FixedValue = null;
        Assert.Null(a.FixedValue);
        Assert.Null(a.Element.Attribute(XsA.Fixed));
    }
}

// ── XsParticle property round-trips ──────────────────────────────────────

public class XsParticleTests
{
    private static XsSequence MakeSeq() => new XsSequence(new XElement(Xs.Sequence));

    [Fact]
    public void MinOccurs_DefaultOne_NoAttribute()
    {
        var p = MakeSeq();
        Assert.Equal(1, p.MinOccurs);
        Assert.Null(p.Element.Attribute(XsA.MinOccurs));
    }

    [Fact]
    public void MinOccurs_Zero_WritesAttribute()
    {
        var p = MakeSeq();
        p.MinOccurs = 0;
        Assert.Equal(0, p.MinOccurs);
        Assert.Equal("0", p.Element.Attribute(XsA.MinOccurs)!.Value);
    }

    [Fact]
    public void MinOccurs_ExplicitOne_WritesAttribute()
    {
        var p = MakeSeq();
        p.MinOccurs = 1;
        Assert.Equal(1, p.MinOccurs);
        Assert.Equal("1", p.Element.Attribute(XsA.MinOccurs)!.Value);
    }

    [Fact]
    public void MaxOccurs_DefaultOne_NoAttribute()
    {
        var p = MakeSeq();
        Assert.Equal(1, p.MaxOccurs);
        Assert.Null(p.Element.Attribute(XsA.MaxOccurs));
    }

    [Fact]
    public void MaxOccurs_Unbounded_WritesUnbounded()
    {
        var p = MakeSeq();
        p.MaxOccurs = int.MaxValue;
        Assert.Equal(int.MaxValue, p.MaxOccurs);
        Assert.Equal("unbounded", p.Element.Attribute(XsA.MaxOccurs)!.Value);
    }

    [Fact]
    public void MaxOccurs_Negative_WritesUnbounded()
    {
        var p = MakeSeq();
        p.MaxOccurs = -1;
        Assert.Equal(int.MaxValue, p.MaxOccurs);
        Assert.Equal("unbounded", p.Element.Attribute(XsA.MaxOccurs)!.Value);
    }

    [Fact]
    public void MaxOccurs_BoundedValue_WritesNumber()
    {
        var p = MakeSeq();
        p.MaxOccurs = 5;
        Assert.Equal(5, p.MaxOccurs);
        Assert.Equal("5", p.Element.Attribute(XsA.MaxOccurs)!.Value);
    }
}

// ── XsFacet property round-trips ─────────────────────────────────────────

public class XsFacetTests
{
    private static XsFacet Make(XName elementName) =>
        XsFacet.Create(new XElement(elementName), null);

    [Fact]
    public void Value_RoundTrip()
    {
        var f = Make(Xs.MinLength);
        Assert.Null(f.Value);
        f.Value = "3";
        Assert.Equal("3", f.Value);
    }

    [Fact]
    public void IsFixed_DefaultFalse_NoAttribute()
    {
        var f = Make(Xs.Length);
        Assert.False(f.IsFixed);
        Assert.Null(f.Element.Attribute(XsA.Fixed));
    }

    [Fact]
    public void IsFixed_SetTrue_WritesAttribute()
    {
        var f = Make(Xs.Length);
        f.IsFixed = true;
        Assert.True(f.IsFixed);
        Assert.Equal("true", f.Element.Attribute(XsA.Fixed)!.Value);
    }

    [Fact]
    public void IsFixed_ResetToFalse_RemovesAttribute()
    {
        var f = Make(Xs.Length);
        f.IsFixed = true;
        f.IsFixed = false;
        Assert.Null(f.Element.Attribute(XsA.Fixed));
    }

    [Theory]
    [InlineData(nameof(Xs.MinLength),      FacetType.MinLength)]
    [InlineData(nameof(Xs.MaxLength),      FacetType.MaxLength)]
    [InlineData(nameof(Xs.Length),         FacetType.Length)]
    [InlineData(nameof(Xs.Pattern),        FacetType.Pattern)]
    [InlineData(nameof(Xs.Enumeration),    FacetType.Enumeration)]
    [InlineData(nameof(Xs.WhiteSpace),     FacetType.WhiteSpace)]
    [InlineData(nameof(Xs.MinInclusive),   FacetType.MinInclusive)]
    [InlineData(nameof(Xs.MaxInclusive),   FacetType.MaxInclusive)]
    [InlineData(nameof(Xs.MinExclusive),   FacetType.MinExclusive)]
    [InlineData(nameof(Xs.MaxExclusive),   FacetType.MaxExclusive)]
    [InlineData(nameof(Xs.TotalDigits),    FacetType.TotalDigits)]
    [InlineData(nameof(Xs.FractionDigits), FacetType.FractionDigits)]
    public void FacetType_DetectedFromElementName(string xsProperty, FacetType expected)
    {
        var elementName = (XName)typeof(Xs).GetProperty(xsProperty)!.GetValue(null)!;
        var f = Make(elementName);
        Assert.Equal(expected, f.FacetType);
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
        Path.Join(AppContext.BaseDirectory, "TestData", name);

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
        var tmpDir = Path.Join(Path.GetTempPath(), "xspub-schemaload-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tmpDir);
        var schemaPath = Path.Join(tmpDir, "test.xsd");
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
                () => { _ = schema.Includes.First().Schema; });
            Assert.Contains("--allow-external", ex.Message);
        }
        finally
        {
            Directory.Delete(tmpDir, recursive: true);
        }
    }
}
