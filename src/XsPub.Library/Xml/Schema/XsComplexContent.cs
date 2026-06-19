using System.Collections.Generic;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema
{
    public class XsComplexContent : XsContentModel
    {
        public XsComplexContent(XElement element)
            : base(element)
        {
        }

        public XsComplexContent(XElement element, XsObject parent)
            : base(element, parent)
        {
        }

        private static readonly IEnumerable<XName> _contentElementNames = new[] { Xs.Extension, Xs.Restriction };
        public override XsContent Content
        {
            get { return GetExclusiveOrElement<XsContent>(_contentElementNames); }
            set { SetExclusiveOrElement(_contentElementNames, value); }
        }

        public bool IsMixed
        {
            get { return GetAttributeValueInternal(XsA.Mixed, false); }
            set { SetAttributeValueInternal(XsA.Mixed, value, false); }
        }

        protected override bool IsValidAttribute(XName attributeName)
        {
            if (attributeName == XsA.Mixed) return true;
            return base.IsValidAttribute(attributeName);
        }
    }
}