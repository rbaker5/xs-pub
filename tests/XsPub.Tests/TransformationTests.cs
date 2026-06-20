using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
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
        _outputDir = Path.Join(Path.GetTempPath(), "xspub-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_outputDir);

        var services = new ServiceCollection();
        services.AddSingleton<ITransformationFactory, AllToSequenceFactory>();
        services.AddSingleton<ITransformationFactory, InlineGroupsFactory>();
        services.AddSingleton<ITransformationFactory, ExplicitFormsFactory>();
        services.AddSingleton<PublishingRuntime>();
        _runtime = services.BuildServiceProvider().GetRequiredService<PublishingRuntime>();
    }

    public void Dispose() => Directory.Delete(_outputDir, recursive: true);

    // ── Factory registration ───────────────────────────────────────────────

    [Fact]
    public void AllToSequence_FactoryRegistered()
    {
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

    // ── AllToSequence transformation ───────────────────────────────────────

    [Fact]
    public void Publish_WithAllToSequence_ReplacesAllWithSequence()
    {
        var settings = _runtime.CreateDefaultSettings();
        settings.AddRange(_runtime.TransformationFactories
            .Where(f => f.Name == "AllToSequence")
            .Select(f => f.GetDefaultSettings()));

        _runtime.Publish(TestFile("WithAll.xsd"), _outputDir, settings);

        var content = File.ReadAllText(Directory.GetFiles(_outputDir, "*.xsd").Single());
        Assert.DoesNotContain("<xs:all>", content);
        Assert.Contains("xs:sequence", content);
    }

    // ── Null-guard ────────────────────────────────────────────────────────

    [Fact]
    public void PublishingRuntime_ThrowsOnNullInput()
    {
        Assert.Throws<ArgumentNullException>(() => _runtime.Publish(null!, _outputDir));
    }

    // ── Golden-file comparison: XSD ───────────────────────────────────────

    [Fact]
    public void Publish_BitOfEverythingXsd_DefaultSettings_MatchesGolden()
    {
        _runtime.Publish(TestFile("BitOfEverything.xsd"), _outputDir);
        AssertMatchesGolden("Expected", "BitOfEverything.xsd");
    }

    [Fact]
    public void Publish_BitOfEverythingXsd_WithInlineGroups_MatchesGolden()
    {
        var settings = _runtime.CreateDefaultSettings();
        settings.AddRange(_runtime.TransformationFactories
            .Where(f => f.Name == "InlineGroups")
            .Select(f => f.GetDefaultSettings()));

        _runtime.Publish(TestFile("BitOfEverything.xsd"), _outputDir, settings);
        AssertMatchesGolden("ExpectedInlineGroups", "BitOfEverything.xsd");
    }

    // ── Golden-file comparison: WSDL ──────────────────────────────────────

    [Fact]
    public void Publish_AwsMturkWsdl_DefaultSettings_MatchesGolden()
    {
        _runtime.Publish(TestFile("AWSMechanicalTurkRequester.wsdl"), _outputDir);
        AssertMatchesGolden("Expected", "AWSMechanicalTurkRequester.wsdl");
    }

    [Fact]
    public void Publish_AwsMturkWsdl_WithInlineGroups_MatchesGolden()
    {
        var settings = _runtime.CreateDefaultSettings();
        settings.AddRange(_runtime.TransformationFactories
            .Where(f => f.Name == "InlineGroups")
            .Select(f => f.GetDefaultSettings()));

        _runtime.Publish(TestFile("AWSMechanicalTurkRequester.wsdl"), _outputDir, settings);
        AssertMatchesGolden("ExpectedInlineGroups", "AWSMechanicalTurkRequester.wsdl");
    }

    // ── Content-assertion tests ───────────────────────────────────────────

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

    // ── xs:import / xs:include integration ───────────────────────────────

    [Fact]
    public void Publish_ImporterXsd_ResolvesLocalImport()
    {
        _runtime.Publish(TestFile("Importer.xsd"), _outputDir);
        Assert.True(Directory.GetFiles(_outputDir, "*.xsd").Length > 0);
    }

    [Fact]
    public void Publish_IncluderXsd_ResolvesLocalInclude()
    {
        _runtime.Publish(TestFile("Includer.xsd"), _outputDir);
        Assert.True(Directory.GetFiles(_outputDir, "*.xsd").Length > 0);
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private void AssertMatchesGolden(string goldenSubDir, string fileName)
    {
        var goldenPath = TestFile(Path.Join(goldenSubDir, fileName));
        var actualPath = Path.Join(_outputDir, fileName);

        Assert.True(File.Exists(actualPath), $"Publish did not write output file '{fileName}'.");

        var expected = NormalizeForComparison(File.ReadAllText(goldenPath));
        var actual = NormalizeForComparison(File.ReadAllText(actualPath));

        Assert.Equal(expected, actual);
    }

    // Normalizations that reconcile .NET 4.0 golden files with .NET 10 XmlWriter output.
    private static string NormalizeForComparison(string text)
    {
        // Strip BOM (golden files from Visual Studio on Windows may have it; XmlWriter on Linux won't)
        if (text.StartsWith("﻿", StringComparison.Ordinal))
            text = text[1..];
        // Normalize line endings so cross-platform comparisons are stable
        text = text.Replace("\r\n", "\n").Replace("\r", "\n");
        // XmlWriter always emits ` />` (space before slash); golden files may have `"/>`
        text = text.Replace("\"/>", "\" />");
        // XmlWriter never writes trailing space before a wrapped-attribute newline
        text = Regex.Replace(text, @"(?<=<[^>]*)[ ]*\n(?=[^<]*>)", "\n");
        // .NET writes the encoding declaration in lower case
        text = text.Replace("\"UTF-8\"", "\"utf-8\"");
        return text;
    }

    private static string TestFile(string name) =>
        Path.Join(AppContext.BaseDirectory, "TestData", name);
}
