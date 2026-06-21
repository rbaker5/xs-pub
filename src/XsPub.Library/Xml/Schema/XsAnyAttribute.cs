using System.Xml.Linq;
using System.Xml.Schema;

namespace XsPub.Library.Xml.Schema;

public class XsAnyAttribute : XsAnnotated
{
    public XsAnyAttribute(XElement element) : base(element)
    {
    }

    public XsAnyAttribute(XElement element, XsObject parent) : base(element, parent)
    {
    }

    /// <summary>
    /// Gets or sets the namespaces containing the attributes that can be used.
    /// </summary>
    /// <value>
    /// Namespaces for attributes that are available for use. The default is ##any.
    /// Optional.
    /// </value>
    public string Namespace
    {
        get { return GetAttributeValueInternal(XsA.Namespace, "##any"); }
        set { SetAttributeValueInternal(XsA.Namespace, value, "##any"); }
    }

    /// <summary>
    /// Gets or sets information about how an application or XML processor should
    /// handle the validation of XML documents for the attributes specified by the
    /// anyAttribute element.
    /// </summary>
    /// <value>
    /// One of the XmlSchemaContentProcessing values. If no processContents attribute
    /// is specified, the default is Strict.
    /// </value>
    public XmlSchemaContentProcessing ProcessContents
    {
        get { return GetAttributeValueInternal(XsA.ProcessContents, XmlSchemaContentProcessing.Strict); }
        set { SetAttributeValueInternal(XsA.ProcessContents, value, XmlSchemaContentProcessing.Strict); }
    }

    protected override bool IsValidAttribute(XName attributeName)
    {
        if (attributeName == XsA.Namespace || attributeName == XsA.ProcessContents) return true;
        return base.IsValidAttribute(attributeName);
    }
}