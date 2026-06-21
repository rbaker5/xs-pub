using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema;

public class XsSimpleContentRestriction : XsContent
{
    public XsSimpleContentRestriction(XElement element)
        : base(element)
    {
    }

    public XsSimpleContentRestriction(XElement element, XsObject parent)
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

    public XsSimpleType BaseType
    {
        get { return GetElement<XsSimpleType>(Xs.SimpleType); }
        set { SetElement(Xs.SimpleType, value); }
    }


    public XName BaseTypeName
    {
        get { return GetQualifiedName(XsA.Base); }
        set { SetQualifiedName(XsA.Base, value); }
    }

    private ICollection<XsFacet> _facets;

    private static readonly IEnumerable<XName> _facetElementNames =
        new[]
            {
                Xs.TotalDigits, Xs.FractionDigits,
                Xs.MaxExclusive, Xs.MaxInclusive,
                Xs.MinExclusive, Xs.MinInclusive,
                Xs.Length, Xs.MaxLength, Xs.MinLength,
                Xs.Enumeration, Xs.Pattern,
                Xs.WhiteSpace
            };

    public ICollection<XsFacet> Facets
    {
        get
        {
            if (_facets == null) _facets = XsCollection.Create<XsFacet>(this, _facetElementNames);
            return _facets;
        }
    }

    protected override bool IsValidAttribute(XName attributeName)
    {
        if (attributeName == XsA.Base) return true;
        return base.IsValidAttribute(attributeName);
    }
}