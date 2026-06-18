using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema
{
    public abstract class XsIdentityConstraint : XsObject
    {
        public XsIdentityConstraint(XElement element) : base(element)
        {
        }

        public XsIdentityConstraint(XElement element, XsObject parent) : base(element, parent)
        {
        }
    }
}