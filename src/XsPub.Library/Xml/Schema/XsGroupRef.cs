using System.Xml;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema
{
    public class XsGroupRef : XsParticle
    {
        public XsGroupRef(XElement element) : base(element)
        {
        }

        public XsGroupRef(XElement element, XsObject parent) : base(element, parent)
        {
        }

        //public XsGroupBase Particle { get; }
        public XName RefName
        {
            get { return GetQualifiedName(XsA.Ref); }
            set { SetQualifiedName(XsA.Ref, value); }
        }


    }
}