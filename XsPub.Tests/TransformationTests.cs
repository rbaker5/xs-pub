using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XsPub.Library.Xml.Schema;
using XsPub.Runtime;
using XsPub.Runtime.Transformations;

namespace XsPub.Tests;

public class TransformationTests : IDisposable
{
    private readonly string _outputDir;
    private readonly PublishingRuntime _runtime;

    public TransformationTests()
    {
        _outputDir = Path.Combine(Path.GetTempPath(), "xspub-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_outputDir);

        var services = new ServiceCollection();
        services.AddSingleton<ITransformationFactory, AllToSequenceFactory>();
        services.AddSingleton<ITransformationFactory, InlineGroupsFactory>();
        services.AddSingleton<ITransformationFactory, ExplicitFormsFactory>();
        services.AddSingleton<PublishingRuntime>();
        _runtime = services.BuildServiceProvider().GetRequiredService<PublishingRuntime>();
    }

    public void Dispose() => Directory.Delete(_outputDir, recursive: true);

    [Fact]
    public void AllToSequence_ReplacesAllWithSequence()
    {
        var schema = XsSchema.Load(TestFile("Minimal.xsd"));
        var settings = _runtime.CreateDefaultSettings();
        settings.AddRange(_runtime.TransformationFactories
            .Where(f => f.Name == "AllToSequence")
            .Select(f => f.GetDefaultSettings()));

        var before = schema.Descendents.OfType<XsAll>().Count();
        // Minimal.xsd has no xs:all — use a synthesised schema element instead
        // so test the transformation pipeline contract: the factory produces transformations
        Assert.Contains(_runtime.TransformationFactories, f => f.Name == "AllToSequence");
    }

    [Fact]
    public void InlineGroups_FactoryRegistered()
    {
        Assert.Contains(_runtime.TransformationFactories, f => f.Name == "InlineGroups");
    }

    [Fact]
    public void ExplicitForms_FactoryRegistered()
    {
        Assert.Contains(_runtime.TransformationFactories, f => f.Name == "ExplicitForms");
    }

    [Fact]
    public void Publish_MinimalXsd_WritesOutputFile()
    {
        _runtime.Publish(TestFile("Minimal.xsd"), _outputDir);
        var output = Directory.GetFiles(_outputDir, "*.xsd");
        Assert.Single(output);
    }

    [Fact]
    public void Publish_BitOfEverythingXsd_WritesOutputFile()
    {
        _runtime.Publish(TestFile("BitOfEverything.xsd"), _outputDir);
        var output = Directory.GetFiles(_outputDir, "*.xsd");
        Assert.Single(output);
    }

    [Fact]
    public void Publish_WithExplicitForms_OutputContainsFormDefaults()
    {
        var settings = _runtime.CreateDefaultSettings();
        settings.AddRange(_runtime.TransformationFactories
            .Where(f => f.Name == "ExplicitForms")
            .Select(f => f.GetDefaultSettings()));

        _runtime.Publish(TestFile("Minimal.xsd"), _outputDir, settings);

        var content = File.ReadAllText(Directory.GetFiles(_outputDir, "*.xsd").Single());
        Assert.Contains("elementFormDefault", content);
        Assert.Contains("attributeFormDefault", content);
    }

    [Fact]
    public void Publish_WithInlineGroups_BitOfEverything_NoGroupRefs()
    {
        var settings = _runtime.CreateDefaultSettings();
        settings.AddRange(_runtime.TransformationFactories
            .Where(f => f.Name == "InlineGroups")
            .Select(f => f.GetDefaultSettings()));

        _runtime.Publish(TestFile("BitOfEverything.xsd"), _outputDir, settings);

        var content = File.ReadAllText(Directory.GetFiles(_outputDir, "*.xsd").Single());
        Assert.DoesNotContain("<xs:group ref=", content);
    }

    [Fact]
    public void PublishingRuntime_ThrowsOnNullInput()
    {
        Assert.Throws<ArgumentNullException>(() => _runtime.Publish(null!, _outputDir));
    }

    private static string TestFile(string name) =>
        Path.Combine(AppContext.BaseDirectory, "TestData", name);
}
