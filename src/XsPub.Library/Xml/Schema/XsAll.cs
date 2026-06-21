using System.Collections.Generic;
using System.Xml.Linq;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema;

public class XsAll : XsGroupBase
{
    public XsAll(XElement element) : base(element)
    {
    }

    public XsAll(XElement element, XsObject parent) : base(element, parent)
    {
    }

    private ICollection<XsObject> _items;
    public override ICollection<XsObject> Items
    {
        get
        {
            if (_items == null) _items = XsCollection.Create<XsObject>(this, Xs.Element);
            return _items;
        }
    }
}