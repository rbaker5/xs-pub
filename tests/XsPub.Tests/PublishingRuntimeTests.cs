using Microsoft.Extensions.DependencyInjection;
using XsPub.Runtime;
using XsPub.Runtime.Transformations;
using Xunit;

namespace XsPub.Tests;

public class PublishingRuntimeTests : IDisposable
{
    private readonly string _outputDir;
    private readonly PublishingRuntime _runtime;
    private readonly PublishingRuntime _runtimeNoFactories;

    public PublishingRuntimeTests()
    {
        _outputDir = Path.Join(Path.GetTempPath(), "xspub-runtime-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_outputDir);

        var services = new ServiceCollection();
        services.AddSingleton<ITransformationFactory, AllToSequenceFactory>();
        services.AddSingleton<ITransformationFactory, InlineGroupsFactory>();
        services.AddSingleton<ITransformationFactory, ExplicitFormsFactory>();
        services.AddSingleton<PublishingRuntime>();
        _runtime = services.BuildServiceProvider().GetRequiredService<PublishingRuntime>();

        _runtimeNoFactories = new PublishingRuntime(Enumerable.Empty<ITransformationFactory>());
    }

    public void Dispose() => Directory.Delete(_outputDir, recursive: true);

    // ── Null / empty guards ────────────────────────────────────────────────

    [Fact]
    public void Publish_ThrowsOnNullOutputPath()
    {
        Assert.Throws<ArgumentNullException>(() => _runtimeNoFactories.Publish(TestFile("Minimal.xsd"), null!));
    }

    [Fact]
    public void Publish_ThrowsOnEmptyInputPath()
    {
        Assert.Throws<ArgumentException>(() => _runtimeNoFactories.Publish("", _outputDir));
    }

    [Fact]
    public void Publish_NonSchemaXml_Throws()
    {
        var inputPath = Path.Join(_outputDir, "NotASchema.xml");
        File.WriteAllText(inputPath, "<root />");

        Assert.Throws<InvalidOperationException>(() => _runtimeNoFactories.Publish(inputPath, _outputDir));
    }

    // ── Output directory ───────────────────────────────────────────────────

    [Fact]
    public void Publish_CreatesOutputDirectory_WhenMissing()
    {
        var subDir = Path.Join(_outputDir, "new-subdir");
        Assert.False(Directory.Exists(subDir));

        _runtimeNoFactories.Publish(TestFile("Minimal.xsd"), subDir);

        Assert.True(Directory.Exists(subDir));
    }

    // ── Output file naming ─────────────────────────────────────────────────

    [Fact]
    public void Publish_WritesXsd_WithInputFileName()
    {
        _runtimeNoFactories.Publish(TestFile("Minimal.xsd"), _outputDir);

        Assert.True(File.Exists(Path.Join(_outputDir, "Minimal.xsd")));
    }

    // ── Import / include writes all referenced files ───────────────────────

    [Fact]
    public void Publish_ImporterXsd_WritesBothImporterAndImported()
    {
        _runtimeNoFactories.Publish(TestFile("Importer.xsd"), _outputDir);

        Assert.Equal(2, Directory.GetFiles(_outputDir, "*.xsd").Length);
    }

    [Fact]
    public void Publish_IncluderXsd_WritesBothIncluderAndIncluded()
    {
        _runtimeNoFactories.Publish(TestFile("Includer.xsd"), _outputDir);

        Assert.Equal(2, Directory.GetFiles(_outputDir, "*.xsd").Length);
    }

    // ── All transformations combined ───────────────────────────────────────

    [Fact]
    public void Publish_AllTransformationsCombined_ReplacesAllAddsFormDefault()
    {
        var settings = _runtime.CreateDefaultSettings();
        settings.AddRange(_runtime.TransformationFactories.Select(f => f.GetDefaultSettings()));

        _runtime.Publish(TestFile("WithAll.xsd"), _outputDir, settings);

        var content = File.ReadAllText(Directory.GetFiles(_outputDir, "*.xsd").Single());
        Assert.DoesNotContain("<xs:all>", content);
        Assert.Contains("xs:sequence", content);
        Assert.Contains("attributeFormDefault", content);
    }

    // ── CreateDefaultSettings ──────────────────────────────────────────────

    [Fact]
    public void CreateDefaultSettings_HasOnlyGlobalSettings()
    {
        var settings = _runtimeNoFactories.CreateDefaultSettings();

        Assert.NotNull(settings.GlobalSettings);
        Assert.Single(settings.AllSettings);
        Assert.Same(settings.GlobalSettings, settings.AllSettings.Single());
    }

    // ── WSDL prefix copying ────────────────────────────────────────────────

    [Fact]
    public void Publish_WsdlWithInheritedPrefix_CopiesPrefixExplicitlyToSchema()
    {
        // PrefixCopy.wsdl: definitions root declares xmlns:tns; embedded schema
        // does not redeclare it, but uses it in type="tns:Foo".  copyPrefixes
        // should add an explicit xmlns:tns to the schema element.
        _runtimeNoFactories.Publish(TestFile("PrefixCopy.wsdl"), _outputDir);

        var schemaElement = LoadOutputSchemaElement("PrefixCopy.wsdl");
        Assert.NotNull(schemaElement.Attribute(System.Xml.Linq.XNamespace.Xmlns + "tns"));
    }

    [Fact]
    public void Publish_WsdlWithUnusedPrefix_DoesNotCopyPrefixToSchema()
    {
        // PrefixCopy.wsdl: definitions root also declares xmlns:unused, but the
        // embedded schema never references it.  copyPrefixes should not pollute
        // the schema element with prefixes it doesn't use.
        _runtimeNoFactories.Publish(TestFile("PrefixCopy.wsdl"), _outputDir);

        var schemaElement = LoadOutputSchemaElement("PrefixCopy.wsdl");
        Assert.Null(schemaElement.Attribute(System.Xml.Linq.XNamespace.Xmlns + "unused"));
    }

    [Fact]
    public void Publish_WsdlWithPrefixSubstringOfUsedPrefix_DoesNotCopyToSchema()
    {
        // PrefixCopy.wsdl: definitions root declares xmlns:tn whose string value
        // is a prefix of the used prefix "tns".  The schema contains type="tns:Foo"
        // which starts with "tn" but not "tn:" — so xmlns:tn must not be copied.
        // This guards against a StartsWith(prefix) check without the colon delimiter.
        _runtimeNoFactories.Publish(TestFile("PrefixCopy.wsdl"), _outputDir);

        var schemaElement = LoadOutputSchemaElement("PrefixCopy.wsdl");
        Assert.Null(schemaElement.Attribute(System.Xml.Linq.XNamespace.Xmlns + "tn"));
    }

    private System.Xml.Linq.XElement LoadOutputSchemaElement(string wsdlFileName)
    {
        var content = File.ReadAllText(Path.Join(_outputDir, wsdlFileName));
        return System.Xml.Linq.XDocument.Parse(content)
            .Descendants(XsPub.Library.Xml.Xs.Schema).Single();
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private static string TestFile(string name) =>
        Path.Join(AppContext.BaseDirectory, "TestData", name);
}
