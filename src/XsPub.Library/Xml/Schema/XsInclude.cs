using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema
{
    public class XsInclude : XsExternal
    {
        public XsInclude(XElement element)
            : base(element)
        {
        }

        public XsInclude(XElement element, XsObject parent)
            : base(element, parent)
        {
        }

        public XsAnnotation Annotation
        {
            get { return GetElement<XsAnnotation>(Xs.Annotation); }
            set { SetElement(Xs.Annotation, value); }
        }
    }
}