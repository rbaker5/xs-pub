using System;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema;

public class XsParticle : XsAnnotated
{
    private const string UnboundedString = "unbounded";
    public static XsParticle _empty = new XsParticle(new XElement("EmptyParticle"));
    public static XsParticle Empty { get { return _empty; } }

    protected XsParticle(XElement element) : base(element) { }
    protected XsParticle(XElement element, XsObject parent) : base(element, parent) { }

    public int MaxOccurs
    {
        get { return getOccurs(XsA.MaxOccurs); }
        set { setOccurs(XsA.MaxOccurs, value); }
    }

    public int MinOccurs
    {
        get { return getOccurs(XsA.MinOccurs); }
        set { setOccurs(XsA.MinOccurs, value); }
    }

    internal bool IsEmpty { get { return this == _empty; } }

    private int getOccurs(XName attributeName)
    {
        var attribute = Element.Attribute(attributeName);
        if (attribute == null) return 1;

        if (attribute.Value == UnboundedString) return Int32.MaxValue;
        int returnValue;
        if (!Int32.TryParse(attribute.Value, out returnValue))
            throw new XsException(
                string.Format("Unable to read '{0}' because the format is invalid.", attributeName), this);

        return returnValue;
    }

    private void setOccurs(XName attributeName, int value)
    {
        Element.SetAttributeValue(attributeName, occursToString(value));
    }

    private string occursToString(int value)
    {
        return (value == Int32.MaxValue || value < 0) ? UnboundedString : value.ToString();
    }

    protected override bool IsValidAttribute(XName attributeName)
    {
        if (attributeName == XsA.MaxOccurs || attributeName == XsA.MinOccurs) return true;
        return base.IsValidAttribute(attributeName);
    }
}