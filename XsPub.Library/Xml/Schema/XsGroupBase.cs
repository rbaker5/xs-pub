using System.Collections.Generic;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema
{
    public abstract class XsGroupBase : XsParticle
    {
        protected XsGroupBase(XElement element) : base(element)
        {
        }

        protected XsGroupBase(XElement element, XsObject parent) : base(element, parent)
        {
        }

        public abstract ICollection<XsObject> Items { get; }
    }
}