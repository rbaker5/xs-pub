using System.Xml;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema
{
    public class XsSimpleTypeList : XsSimpleTypeContent
    {
        public XsSimpleTypeList(XElement element)
            : base(element)
        {
        }

        public XsSimpleTypeList(XElement element, XsObject parent)
            : base(element, parent)
        {
        }

        //public XsSimpleType BaseItemType
        //{
        //    get
        //    {
        //    }
        //    set
        //    {
        //    }
        //}

        public XsSimpleType ItemType
        {
            get { return GetElement<XsSimpleType>(Xs.SimpleType); }
            set { SetElement(Xs.SimpleType, value); }
        }

        public XName ItemTypeName
        {
            get { return GetQualifiedName(XsA.ItemType); }
            set { SetQualifiedName(XsA.ItemType, value); }
        }

        protected override bool IsValidAttribute(XName attributeName)
        {
            if (attributeName == XsA.ItemType) return true;
            return base.IsValidAttribute(attributeName);
        }
    }
}