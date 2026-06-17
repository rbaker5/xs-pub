using System.Collections.Generic;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema
{
    public class XsSimpleContent : XsContentModel
    {
        public XsSimpleContent(XElement element)
            : base(element)
        {
        }

        public XsSimpleContent(XElement element, XsObject parent)
            : base(element, parent)
        {
        }

        private static readonly IEnumerable<XName> _contentElementNames = new[] { Xs.Extension, Xs.Restriction };
        public override XsContent Content
        {
            get { return GetExclusiveOrElement<XsContent>(_contentElementNames); }
            set { SetExclusiveOrElement(_contentElementNames, value); }
        }
    }
}