using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema;

public abstract class XsContentModel : XsAnnotated
{
    protected XsContentModel(XElement element) : base(element)
    {
    }

    protected XsContentModel(XElement element, XsObject parent) : base(element, parent)
    {
    }

    public abstract XsContent Content { get; set; }
}