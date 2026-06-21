using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema;

public class XsAnnotation : XsObject
{
    public XsAnnotation(XElement element)
        : base(element)
    {
        if (element.Name != Xs.Annotation) throw new ArgumentException($"Expected {nameof(Xs.Annotation)}, got {element.Name}.", nameof(element));
    }

    public XsAnnotation(XElement element, XsObject parent)
        : base(element, parent)
    {
        if (element.Name != Xs.Annotation) throw new ArgumentException($"Expected {nameof(Xs.Annotation)}, got {element.Name}.", nameof(element));
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