using System.Collections.Generic;
using System.Xml.Linq;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema
{
    public class XsChoice : XsGroupBase
    {
        public XsChoice(XElement element) : base(element)
        {
        }

        public XsChoice(XElement element, XsObject parent) : base(element, parent)
        {
        }

        private ICollection<XsObject> _items;
        private static readonly IEnumerable<XName> _itemElementNames = new[] { Xs.Any, Xs.Choice, Xs.Sequence, Xs.Element, Xs.Group };
        public override ICollection<XsObject> Items
        {
            get
            {
                if (_items == null) _items = XsCollection.Create<XsObject>(this, _itemElementNames);
                return _items;
            }
        }
    }
}