using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema;

public abstract class XsExternal : XsObject
{
    protected XsExternal(XElement element) : base(element)
    {
        resetSchema();
    }

    protected XsExternal(XElement element, XsObject parent) : base(element, parent)
    {
        resetSchema();
    }

    public string Id
    {
        get { return GetAttributeValueInternal(XsA.Id); }
        set { SetAttributeValueInternal(XsA.Id, value); }
    }

    private Lazy<XsSchema> _schema;
    public XsSchema Schema { get { return _schema.Value; }}

    public string SchemaLocation
    {
        get { return GetAttributeValueInternal(XsA.SchemaLocation); }
        set { SetAttributeValueInternal(XsA.SchemaLocation, value); }
    }

    public IEnumerable<XAttribute> UnhandledAttributes { get { return GetUnhandledAttributes(); } }

    private XsSchema getReferencedSchema()
    {
        var enclosingSchema = GetSchema();
        var resolver = enclosingSchema.GetXmlResolver()
            ?? throw new InvalidOperationException(
                $"No XmlResolver configured on the enclosing schema. Cannot resolve external reference '{SchemaLocation}'.");
        var baseUriString = enclosingSchema.Element.BaseUri;
        var fullUri = resolver.ResolveUri(string.IsNullOrEmpty(baseUriString) ? null : new Uri(baseUriString), SchemaLocation);
        using (var stream = resolver.GetEntity(fullUri, null, null) as Stream)
        {
            if (stream == null) throw new InvalidOperationException($"Could not open stream for external schema '{fullUri}'.");

            var readerSettings = new XmlReaderSettings
            {
                MaxCharactersInDocument = 0x03FFFFFF, // 64 MB
                XmlResolver = resolver
            };

            using (var reader = XmlReader.Create(stream, readerSettings, fullUri.ToString()))
            {
                var childSchema = XsSchema.Load(reader);
                // Propagate the resolver so grandchild xs:import/xs:include are subject to the same policy.
                childSchema.XmlResolver = resolver;
                return childSchema;
            }
        }
    }

    private void resetSchema()
    {
        _schema = new Lazy<XsSchema>(getReferencedSchema);
    }
}