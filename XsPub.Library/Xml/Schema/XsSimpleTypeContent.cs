using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema
{
    public abstract class XsSimpleTypeContent : XsAnnotated
    {
        protected XsSimpleTypeContent(XElement element) : base(element)
        {
        }

        protected XsSimpleTypeContent(XElement element, XsObject parent) : base(element, parent)
        {
        }
    }
}