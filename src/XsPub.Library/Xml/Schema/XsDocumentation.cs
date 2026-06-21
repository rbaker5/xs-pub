using System;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema;

public class XsDocumentation : XsObject
{
    public XsDocumentation(XElement element) : base(element)
    {
    }

    public XsDocumentation(XElement element, XsObject parent)
        : base(element, parent)
    {
    }

    public string Source
    {
        get { return GetAttributeValueInternal(XsA.Source); }
        set { SetAttributeValueInternal(XsA.Source, value); }
    }

    public string Language
    {
        get { return GetAttributeValueInternal(XNamespace.Xml.GetName("lang")); }
        set { SetAttributeValueInternal(XNamespace.Xml.GetName("lang"), value); }
    }

    protected override bool IsValidAttribute(XName attributeName)
    {
        if (attributeName == XsA.Source || attributeName == XNamespace.Xml.GetName("lang")) return true;
        return base.IsValidAttribute(attributeName);
    }
}