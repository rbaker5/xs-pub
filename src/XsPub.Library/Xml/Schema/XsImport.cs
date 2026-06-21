using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema;

public class XsImport : XsExternal
{
    public XsImport(XElement element) : base(element)
    {
    }

    public XsImport(XElement element, XsObject parent) : base(element, parent)
    {
    }

    public XsAnnotation Annotation
    {
        get { return GetElement<XsAnnotation>(Xs.Annotation); }
        set { SetElement(Xs.Annotation, value); }
    }

    public string Namespace
    {
        get { return GetAttributeValueInternal(XsA.Namespace); }
        set { SetAttributeValueInternal(XsA.Namespace, value); }
    }

    protected override bool IsValidAttribute(XName attributeName)
    {
        if (attributeName == XsA.Namespace) return true;
        return base.IsValidAttribute(attributeName);
    }
}