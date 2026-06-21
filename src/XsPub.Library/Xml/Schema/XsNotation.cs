using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema;

public class XsNotation : XsAnnotated, IGlobalNamedObject
{
    public XsNotation(XElement element)
        : base(element)
    {
    }

    public XsNotation(XElement element, XsObject parent)
        : base(element, parent)
    {
    }

    /// <summary>
    /// Gets or sets the name of the notation.
    /// </summary>
    /// <value>
    /// The name of the notation.
    /// </value>
    public string Name
    {
        get { return GetAttributeValueInternal(XsA.Name); }
        set { SetAttributeValueInternal(XsA.Name, value); }
    }

    public XName QualifiedName { get { return GetGlobalQualifiedName(XsA.Name); } }

    public string Public
    {
        get { return GetAttributeValueInternal(XsA.Public); }
        set { SetAttributeValueInternal(XsA.Public, value); }
    }

    public string System
    {
        get { return GetAttributeValueInternal(XsA.System); }
        set { SetAttributeValueInternal(XsA.System, value); }
    }

    protected override bool IsValidAttribute(XName attributeName)
    {
        if (attributeName == XsA.Name || attributeName == XsA.Public || attributeName == XsA.System) return true;
        return base.IsValidAttribute(attributeName);
    }
}