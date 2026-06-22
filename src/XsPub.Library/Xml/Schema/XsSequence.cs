using System.Collections.Generic;
using System.Xml.Linq;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema;

public sealed class XsSequence : XsGroupBase
{
    public XsSequence(XElement element) : base(element)
    {
    }

    public XsSequence(XElement element, XsObject parent) : base(element, parent)
    {
    }

    public XsSequence(IEnumerable<XsObject> items)
        : base(new XElement(Xs.Sequence))
    {
        foreach (var xsObject in items)
            Items.Add(xsObject);
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