using System.Xml;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema;

/// <summary>
/// Represents the attributeGroup element with the ref attribute from the XML
/// Schema as specified by the World Wide Web Consortium. AttributesGroupRef is the
/// reference for an attributeGroup, name property contains the attribute group
/// being referenced.
/// </summary>
public class XsAttributeGroupRef : XsAnnotated
{
    /// <summary>
    /// Initializes an attributeGroupRef schema object outside or at the root of a tree structure.
    /// </summary>
    /// <param name="element">An element containing the XML representation of a attributeGroupRef schema object.</param>
    public XsAttributeGroupRef(XElement element) : base(element)
    {
    }

    /// <summary>
    /// Initializes a attributeGroupRef schema object inside a tree structure.
    /// </summary>
    /// <param name="element">An element containing the XML representation of a attributeGroupRef schema object.</param>
    /// <param name="parent">An appropriate wrapper around <paramref name="element"/>.<see cref="XElement.Parent">Parent</see>.</param>
    public XsAttributeGroupRef(XElement element, XsObject parent) : base(element, parent)
    {
    }

    /// <summary>
    /// Gets or sets the name of the referenced attributeGroup element.
    /// </summary>
    /// <value>
    /// The name of the referenced attribute group. The value must be a QName.
    /// </value>
    public XName RefName
    {
        get { return GetQualifiedName(XsA.Ref); }
        set { SetQualifiedName(XsA.Ref, value); }
    }

    protected override bool IsValidAttribute(XName attributeName)
    {
        if (attributeName == XsA.Ref) return true;
        return base.IsValidAttribute(attributeName);
    }
}