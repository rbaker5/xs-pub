using Xunit;
using XsPub.Library.Xml;
using XsPub.Library.Xml.Schema;

namespace XsPub.Tests;

public class LocalOnlyXmlResolverTests : IDisposable
{
    private readonly string _baseDir;

    public LocalOnlyXmlResolverTests()
    {
        _baseDir = Path.Combine(Path.GetTempPath(), "xspub-resolver-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_baseDir);
    }

    public void Dispose() => Directory.Delete(_baseDir, recursive: true);

    // ── Unit tests: resolver logic (no I/O) ───────────────────────────────

    [Fact]
    public void ResolveUri_AllowsFileWithinBaseDir()
    {
        var resolver = new LocalOnlyXmlResolver(_baseDir);
        var baseUri = new Uri(Path.Combine(_baseDir, "root.xsd"));
        var result = resolver.ResolveUri(baseUri, "child.xsd");
        Assert.Equal(Uri.UriSchemeFile, result.Scheme);
        Assert.Contains("child.xsd", result.LocalPath, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ResolveUri_BlocksHttp()
    {
        var resolver = new LocalOnlyXmlResolver(_baseDir);
        var ex = Assert.Throws<InvalidOperationException>(
            () => resolver.ResolveUri(null, "http://example.com/schema.xsd"));
        Assert.Contains("--allow-external", ex.Message);
    }

    [Fact]
    public void ResolveUri_BlocksHttps()
    {
        var resolver = new LocalOnlyXmlResolver(_baseDir);
        var ex = Assert.Throws<InvalidOperationException>(
            () => resolver.ResolveUri(null, "https://example.com/schema.xsd"));
        Assert.Contains("--allow-external", ex.Message);
    }

    [Fact]
    public void ResolveUri_BlocksAbsoluteFileOutsideBaseDir()
    {
        var resolver = new LocalOnlyXmlResolver(_baseDir);
        var baseUri = new Uri(Path.Combine(_baseDir, "root.xsd"));
        var outsidePath = Path.Combine(Path.GetTempPath(), "evil.xsd");
        var ex = Assert.Throws<InvalidOperationException>(
            () => resolver.ResolveUri(baseUri, outsidePath));
        Assert.Contains("--allow-external", ex.Message);
    }

    [Fact]
    public void ResolveUri_BlocksPathTraversalOutsideBaseDir()
    {
        var resolver = new LocalOnlyXmlResolver(_baseDir);
        var baseUri = new Uri(Path.Combine(_baseDir, "root.xsd"));
        var ex = Assert.Throws<InvalidOperationException>(
            () => resolver.ResolveUri(baseUri, "../outside.xsd"));
        Assert.Contains("--allow-external", ex.Message);
    }

    [Fact]
    public void ResolveUri_AllowsRelativeReferenceWithinBaseDir()
    {
        var resolver = new LocalOnlyXmlResolver(_baseDir);
        var baseUri = new Uri(Path.Combine(_baseDir, "root.xsd"));
        var result = resolver.ResolveUri(baseUri, "sub/nested.xsd");
        Assert.Equal(Uri.UriSchemeFile, result.Scheme);
        Assert.StartsWith(_baseDir, result.LocalPath, StringComparison.OrdinalIgnoreCase);
    }

    // ── Integration tests: resolver wired into schema loading ─────────────

    [Fact]
    public void XsSchema_Load_DefaultResolver_BlocksHttpImport()
    {
        var schemaPath = Path.Combine(_baseDir, "http_import.xsd");
        File.WriteAllText(schemaPath, """
            <?xml version="1.0" encoding="utf-8"?>
            <xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema"
                       targetNamespace="http://test.example.com/a">
              <xs:import namespace="http://external.example.com/b"
                         schemaLocation="https://external.example.com/b.xsd" />
            </xs:schema>
            """);

        var schema = XsSchema.Load(schemaPath);
        var ex = Assert.Throws<InvalidOperationException>(
            () => { var _ = schema.Includes.First().Schema; });
        Assert.Contains("--allow-external", ex.Message);
    }

    [Fact]
    public void XsSchema_Load_DefaultResolver_BlocksOutOfDirInclude()
    {
        var schemaPath = Path.Combine(_baseDir, "traversal.xsd");
        File.WriteAllText(schemaPath, """
            <?xml version="1.0" encoding="utf-8"?>
            <xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
              <xs:include schemaLocation="../sibling.xsd" />
            </xs:schema>
            """);

        var schema = XsSchema.Load(schemaPath);
        var ex = Assert.Throws<InvalidOperationException>(
            () => { var _ = schema.Includes.First().Schema; });
        Assert.Contains("--allow-external", ex.Message);
    }

    [Fact]
    public void XsSchema_Load_DefaultResolver_AllowsSameDirInclude()
    {
        File.WriteAllText(Path.Combine(_baseDir, "child.xsd"), """
            <?xml version="1.0" encoding="utf-8"?>
            <xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema"
                       targetNamespace="http://tempuri.org/child" />
            """);
        var schemaPath = Path.Combine(_baseDir, "parent.xsd");
        File.WriteAllText(schemaPath, """
            <?xml version="1.0" encoding="utf-8"?>
            <xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema"
                       targetNamespace="http://tempuri.org/child">
              <xs:include schemaLocation="child.xsd" />
            </xs:schema>
            """);

        var schema = XsSchema.Load(schemaPath);
        var child = schema.Includes.First().Schema;
        Assert.NotNull(child);
    }
}
