using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema
{
    public class XsAppInfo : XsObject
    {
        public XsAppInfo(XElement element) : base(element)
        {
        }

        public XsAppInfo(XElement element, XsObject parent) : base(element, parent)
        {
        }

        public string Source
        {
            get { return GetAttributeValueInternal(XsA.Source); }
            set { SetAttributeValueInternal(XsA.Source, value); }
        }

        protected override bool IsValidAttribute(XName attributeName)
        {
            if (attributeName == XsA.Source) return true;
            return base.IsValidAttribute(attributeName);
        }

        
    }
}