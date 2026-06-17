using System.Xml.Linq;
using System.Xml.Schema;

namespace XsPub.Library.Xml.Schema
{
    public abstract class XsContent : XsAnnotated
    {
        protected XsContent(XElement element) : base(element)
        {
        }

        protected XsContent(XElement element, XsObject parent) : base(element, parent)
        {
        }
    }
}