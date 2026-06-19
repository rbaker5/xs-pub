using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xsp;
using XsPub.Library.Xml.Schema;
using XsPub.Runtime;
using XsPub.Runtime.Settings;
using XsPub.Runtime.Transformations;

namespace XsPub.Tests;

// ── Transformation factory metadata ──────────────────────────────────────

public class TransformationFactoryMetadataTests
{
    [Fact]
    public void AllToSequenceFactory_Properties()
    {
        var f = new AllToSequenceFactory();
        Assert.Equal("AllToSequence", f.Name);
        Assert.NotEmpty(f.DisplayName);
        Assert.NotEmpty(f.Description);
        Assert.True(f.IsSingleton);
    }

    [Fact]
    public void InlineGroupsFactory_Properties()
    {
        var f = new InlineGroupsFactory();
        Assert.Equal("InlineGroups", f.Name);
        Assert.NotEmpty(f.DisplayName);
        Assert.NotEmpty(f.Description);
        Assert.True(f.IsSingleton);
    }

    [Fact]
    public void ExplicitFormsFactory_Properties()
    {
        var f = new ExplicitFormsFactory();
        Assert.Equal("ExplicitForms", f.Name);
        Assert.NotEmpty(f.DisplayName);
        Assert.NotEmpty(f.Description);
        Assert.True(f.IsSingleton);
    }

    [Fact]
    public void GetDefaultSettings_ReturnsSettingSetWithMatchingName()
    {
        var f = new AllToSequenceFactory();
        var s = f.GetDefaultSettings();
        Assert.Equal(f.Name, s.TransformationName);
    }

    [Theory]
    [InlineData(typeof(AllToSequenceFactory))]
    [InlineData(typeof(InlineGroupsFactory))]
    [InlineData(typeof(ExplicitFormsFactory))]
    public void CreateTransformations_WithActiveSetting_YieldsAtLeastOne(Type factoryType)
    {
        var factory = (TransformationFactoryBase)Activator.CreateInstance(factoryType)!;
        var settings = new RuntimeSettingSet(
            new SettingSet("Global", "Global Settings", "Global."),
            new[] { factory.GetDefaultSettings() });

        var transformations = factory.CreateTransformations(settings).ToList();
        Assert.Single(transformations);
    }
}

// ── XsSchema named collection lookups ────────────────────────────────────

public class XsSchemaCollectionTests
{
    private static string TestFile(string name) =>
        Path.Combine(AppContext.BaseDirectory, "TestData", name);

    // BitOfEverything.xsd targetNamespace
    private static readonly System.Xml.Linq.XNamespace Ns =
        System.Xml.Linq.XNamespace.Get("http://tempuri.org/Minimal.xsd");

    [Fact]
    public void Elements_IndexerByName_ReturnsElement()
    {
        var schema = XsSchema.Load(TestFile("BitOfEverything.xsd"));
        var el = schema.Elements[Ns + "TestElement"];
        Assert.NotNull(el);
        Assert.Equal("TestElement", el.Name);
    }

    [Fact]
    public void Elements_UnknownName_ReturnsNull()
    {
        var schema = XsSchema.Load(TestFile("BitOfEverything.xsd"));
        Assert.Null(schema.Elements[Ns + "NoSuchElement"]);
    }

    [Fact]
    public void Types_IndexerByName_ReturnsComplexType()
    {
        var schema = XsSchema.Load(TestFile("BitOfEverything.xsd"));
        var t = schema.SchemaTypes[Ns + "complexType"];
        Assert.NotNull(t);
    }

    [Fact]
    public void Groups_IndexerByName_ReturnsGroup()
    {
        var schema = XsSchema.Load(TestFile("BitOfEverything.xsd"));
        var g = schema.Groups[Ns + "groupSequence"];
        Assert.NotNull(g);
    }

    [Fact]
    public void AttributeGroups_IndexerByName_ReturnsAttributeGroup()
    {
        var schema = XsSchema.Load(TestFile("BitOfEverything.xsd"));
        var ag = schema.AttributeGroups[Ns + "attributeGroup"];
        Assert.NotNull(ag);
    }

    [Fact]
    public void GlobalElements_IncludesElementFromImportedSchema()
    {
        // Importer.xsd imports BitOfEverything.xsd; BitOfEverything defines TestElement
        var schema = XsSchema.Load(TestFile("Importer.xsd"));
        var imported = schema.Includes.First().Schema;
        Assert.NotNull(imported);
        Assert.NotNull(imported.Elements[Ns + "TestElement"]);
    }

    [Fact]
    public void Attributes_IndexerByName_ReturnsGlobalAttribute()
    {
        var schema = XsSchema.Load(TestFile("BitOfEverything.xsd"));
        var attr = schema.Attributes[Ns + "fixedAttribute"];
        Assert.NotNull(attr);
    }
}

// ── ArgumentParser ────────────────────────────────────────────────────────

public class ArgumentParserTests
{
    [Fact]
    public void Parse_NoArgs_Throws()
    {
        var p = new ArgumentParser(Array.Empty<string>());
        Assert.Throws<ArgumentParseException>(() => p.Parse());
    }

    [Fact]
    public void Parse_OneArg_Throws()
    {
        var p = new ArgumentParser(new[] { "input.xsd" });
        Assert.Throws<ArgumentParseException>(() => p.Parse());
    }

    [Fact]
    public void Parse_TwoArgs_SetsInputAndOutput()
    {
        var p = new ArgumentParser(new[] { "input.xsd", "out/" });
        p.Parse();
        Assert.Equal("input.xsd", p.InputFile);
        Assert.Equal("out/", p.OutputPath);
        Assert.Empty(p.ArgumentSettings);
    }

    [Fact]
    public void Parse_WithQualifiedSetting_ParsesTransformationAndName()
    {
        var p = new ArgumentParser(new[] { "in.xsd", "out/", "AllToSequence.Enabled=true" });
        p.Parse();
        Assert.Single(p.ArgumentSettings);
        Assert.Equal("AllToSequence", p.ArgumentSettings[0].TransformationName);
        Assert.Equal("Enabled", p.ArgumentSettings[0].SettingName);
        Assert.Equal("true", p.ArgumentSettings[0].Value);
    }

    [Fact]
    public void Parse_WithUnqualifiedSetting_NullTransformationName()
    {
        var p = new ArgumentParser(new[] { "in.xsd", "out/", "Count=5" });
        p.Parse();
        Assert.Single(p.ArgumentSettings);
        Assert.Null(p.ArgumentSettings[0].TransformationName);
        Assert.Equal("Count", p.ArgumentSettings[0].SettingName);
        Assert.Equal("5", p.ArgumentSettings[0].Value);
    }

    [Fact]
    public void Parse_InvalidSettingFormat_ThrowsOnFlush()
    {
        var p = new ArgumentParser(new[] { "in.xsd", "out/", "badarg" });
        Assert.Throws<ArgumentParseException>(() => p.Parse());
    }

    [Fact]
    public void Parse_MultipleSettings_AllParsed()
    {
        var p = new ArgumentParser(new[] { "in.xsd", "out/", "A.X=1", "B.Y=2", "Z=3" });
        p.Parse();
        Assert.Equal(3, p.ArgumentSettings.Count);
    }
}

// ── Runtime edge cases ────────────────────────────────────────────────────

public class RuntimeEdgeCaseTests : IDisposable
{
    private readonly string _outputDir;
    private readonly PublishingRuntime _runtime;

    public RuntimeEdgeCaseTests()
    {
        _outputDir = Path.Combine(Path.GetTempPath(), "xspub-edge-" + Guid.NewGuid().ToString("N"));

        var services = new ServiceCollection();
        services.AddSingleton<ITransformationFactory, AllToSequenceFactory>();
        services.AddSingleton<ITransformationFactory, InlineGroupsFactory>();
        services.AddSingleton<ITransformationFactory, ExplicitFormsFactory>();
        services.AddSingleton<PublishingRuntime>();
        _runtime = services.BuildServiceProvider().GetRequiredService<PublishingRuntime>();
    }

    public void Dispose()
    {
        if (Directory.Exists(_outputDir))
            Directory.Delete(_outputDir, recursive: true);
    }

    private static string TestFile(string name) =>
        Path.Combine(AppContext.BaseDirectory, "TestData", name);

    [Fact]
    public void Publish_OutputDirCreatedIfMissing()
    {
        var absent = Path.Combine(_outputDir, "new_subdir");
        Assert.False(Directory.Exists(absent));
        _runtime.Publish(TestFile("Minimal.xsd"), absent);
        Assert.True(Directory.Exists(absent));
    }

    [Fact]
    public void Publish_NonXsdNonWsdlFile_Throws()
    {
        Directory.CreateDirectory(_outputDir);
        var xmlPath = Path.Combine(_outputDir, "random.xml");
        File.WriteAllText(xmlPath, "<root />");
        Assert.Throws<InvalidOperationException>(() => _runtime.Publish(xmlPath, _outputDir));
    }

    [Fact]
    public void Publish_NullOutputPath_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _runtime.Publish(TestFile("Minimal.xsd"), null!));
    }

    [Fact]
    public void Publish_AllTransformations_DoesNotThrow()
    {
        Directory.CreateDirectory(_outputDir);
        var settings = _runtime.CreateDefaultSettings();
        settings.AddRange(_runtime.TransformationFactories.Select(f => f.GetDefaultSettings()));
        _runtime.Publish(TestFile("BitOfEverything.xsd"), _outputDir, settings);
        Assert.True(Directory.GetFiles(_outputDir, "*.xsd").Length > 0);
    }

    [Fact]
    public void FormattedXmlWriterSettings_DefaultsAreCorrect()
    {
        var s = _runtime.OutputSettings;
        Assert.Contains(XsPub.Library.Xml.Xs.Schema, s.WrappedElements);
        Assert.False(s.WriterSettings.Indent);
        Assert.Equal("utf-8", s.WriterSettings.Encoding.WebName);
    }
}
