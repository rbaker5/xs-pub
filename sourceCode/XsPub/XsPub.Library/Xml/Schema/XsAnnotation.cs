using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Xml.Linq;
using System.Linq;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema
{
    public class XsAnnotation : XsObject
    {
        public XsAnnotation(XElement element)
            : base(element)
        {
            Contract.Requires<ArgumentException>(element.Name == Xs.Annotation);
        }

        public XsAnnotation(XElement element, XsObject parent)
            : base(element, parent)
        {
            Contract.Requires<ArgumentException>(element.Name == Xs.Annotation);
        }

        public string Id
        {
            get { return GetAttributeValueInternal(XsA.Id); }
            set { SetAttributeValueInternal(XsA.Id, value); }
        }

        public IEnumerable<XAttribute> UnhandledAttributes { get { return GetUnhandledAttributes(); } }

        private static readonly IEnumerable<XName> _itemElementNames = new[] {Xs.AppInfo, Xs.Documentation};
        public ICollection<XsObject> Items
        {
            get { return XsCollection.Create(this, _itemElementNames); }
        }

        protected override bool IsValidAttribute(XName attributeName)
        {
            if (attributeName == XsA.Id) return true;
            return base.IsValidAttribute(attributeName);
        }
    }
}