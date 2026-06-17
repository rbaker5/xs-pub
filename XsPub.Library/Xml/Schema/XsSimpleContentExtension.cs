using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema
{
    public class XsSimpleContentExtension : XsContent
    {
        public XsSimpleContentExtension(XElement element)
            : base(element)
        {
        }

        public XsSimpleContentExtension(XElement element, XsObject parent)
            : base(element, parent)
        {
        }

        public XsAnyAttribute AnyAttribute
        {
            get { return GetElement<XsAnyAttribute>(Xs.AnyAttribute); }
            set { SetElement(Xs.AnyAttribute, value); }
        }

        private ICollection<XsObject> _attributes;
        private static readonly IEnumerable<XName> _attributeElementNames = new[] { Xs.Attribute, Xs.AttributeGroup };
        public ICollection<XsObject> Attributes
        {
            get
            {
                if (_attributes == null) _attributes = XsCollection.Create<XsObject>(this, _attributeElementNames);
                return _attributes;
            }
        }

        public XName BaseTypeName
        {
            get { return GetQualifiedName(XsA.Base); }
            set { SetQualifiedName(XsA.Base, value); }
        }

        protected override bool IsValidAttribute(XName attributeName)
        {
            if (attributeName == XsA.Base) return true;
            return base.IsValidAttribute(attributeName);
        }
    }
}