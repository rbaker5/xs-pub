using System.Xml.Linq;
using System.Xml.Schema;

namespace XsPub.Library.Xml.Schema;

public class XsAny : XsParticle
{
    public XsAny(XElement element) : base(element)
    {
    }

    public XsAny(XElement element, XsObject parent) : base(element, parent)
    {
    }


    /// <summary>
    /// Gets or sets the namespaces containing the elements that can be used.
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
    /// handle the validation of XML documents for the elements specified by the
    /// any element.
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

}