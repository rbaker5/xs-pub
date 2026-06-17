using System.Net;
using System.Xml;

namespace XsPub.Library.Xml;

/// <summary>
/// Restricts xs:import / xs:include resolution to local files within
/// the same directory tree as the root input file.  Blocks http/https
/// and absolute file:// paths that escape the base directory.
/// </summary>
public sealed class LocalOnlyXmlResolver : XmlResolver
{
    private readonly string _baseDir;

    private static readonly StringComparison PathComparison =
        OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

    public LocalOnlyXmlResolver(string baseDir)
    {
        _baseDir = Path.GetFullPath(baseDir).TrimEnd(Path.DirectorySeparatorChar)
                   + Path.DirectorySeparatorChar;
    }

    public override ICredentials? Credentials
    {
        set { /* not used for local-only resolution */ }
    }

    public override Uri ResolveUri(Uri? baseUri, string? relativeUri)
    {
        var resolved = new XmlUrlResolver().ResolveUri(baseUri, relativeUri);

        if (resolved.Scheme != Uri.UriSchemeFile)
            throw new InvalidOperationException(
                $"External resource access denied: only local file references are permitted. " +
                $"Attempted: {resolved}  — pass --allow-external (CLI) or allowExternalResources: true (API) to enable.");

        var fullPath = Path.GetFullPath(resolved.LocalPath);
        if (!fullPath.StartsWith(_baseDir, PathComparison))
            throw new InvalidOperationException(
                $"Schema reference outside input directory denied: '{fullPath}' is not under '{_baseDir}'. " +
                $"Use --allow-external (CLI) or allowExternalResources: true (API) to enable.");

        return resolved;
    }

    public override object? GetEntity(Uri absoluteUri, string? role, Type? ofObjectToReturn)
        => new XmlUrlResolver().GetEntity(absoluteUri, role, ofObjectToReturn);
}
