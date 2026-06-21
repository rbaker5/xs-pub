using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema;

public class XsComplexType : XsType
{
    private const XmlSchemaDerivationMethod BlockResolvedFilter = XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension;
    internal sealed override XmlSchemaDerivationMethod FinalResolvedFilter { get { return XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension; }}

    public XsComplexType(XElement element)
        : base(element)
    {
    }

    public XsComplexType(XElement element, XsObject parent)
        : base(element, parent)
    {
    }

    public XsAnyAttribute AnyAttribute
    {
        get { return GetElement<XsAnyAttribute>(Xs.AnyAttribute); }
        set { SetElement(Xs.AnyAttribute, value); }
    }

    private ICollection<XsObject> _attributes;
    private static readonly IEnumerable<XName> _attributestElementNames = new[] { Xs.Attribute, Xs.AttributeGroup };
    public ICollection<XsObject> Attributes
    {
        get
        {
            if (_attributes == null) _attributes = XsCollection.Create<XsObject>(this, _attributestElementNames);
            return _attributes;
        }
    }

    //public SchemaObjectTable AttributeUses {get;}
    //public XsAnyAttribute AttributeWildcard {get;}


    public XmlSchemaDerivationMethod Block
    {
        get { return GetAttributeValueInternal(XsA.Block, XmlSchemaDerivationMethod.None); }
        set { SetAttributeValueInternal(XsA.Block, value, XmlSchemaDerivationMethod.None); }
    }

    public XmlSchemaDerivationMethod BlockResolved
    {
        get { return ResolveDerivationMethod(XsA.Block, BlockResolvedFilter, schema => schema.BlockDefault); }
    }

    public override bool IsMixed
    {
        get { return GetAttributeValueInternal(XsA.Mixed, false); }
        set { SetAttributeValueInternal(XsA.Mixed, value, false); }
    }

    public bool IsAbstract
    {
        get { return GetAttributeValueInternal(XsA.Abstract, false); }
        set { SetAttributeValueInternal(XsA.Abstract, value, false); }
    }

    private static readonly IEnumerable<XName> _contentModelElementNames = new[] { Xs.SimpleContent, Xs.ComplexContent };
    public XsContentModel ContentModel
    {
        get { return GetExclusiveOrElement<XsContentModel>(_contentModelElementNames); }
        set { SetExclusiveOrElement(_contentModelElementNames, value); }
    }

    /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
    public override XmlSchemaDerivationMethod DerivedBy
    {
        get
        {
            if (ContentModel == null) return XmlSchemaDerivationMethod.Restriction;
            if (ContentModel is XsSimpleContent)
            {
                if (ContentModel.Content is XsSimpleContentExtension) return XmlSchemaDerivationMethod.Extension;
                if (ContentModel.Content is XsSimpleContentRestriction) return XmlSchemaDerivationMethod.Restriction;
            }
            if (ContentModel is XsComplexContent)
            {
                if (ContentModel.Content is XsComplexContentExtension) return XmlSchemaDerivationMethod.Extension;
                if (ContentModel.Content is XsComplexContentRestriction) return XmlSchemaDerivationMethod.Restriction;
            }

            throw createUnsupportedContentModelException();
        }
    }

    /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
    public XmlSchemaContentType ContentType
    {
        get
        {
            if (ContentModel == null) return getSchemaContentType(this, null, ContentTypeParticle);
            if (ContentModel is XsSimpleContent) return XmlSchemaContentType.TextOnly;
            if (ContentModel is XsComplexContent cc)
            {
                var ext = cc.Content as XsComplexContentExtension;
                var rst = cc.Content as XsComplexContentRestriction;
                var particle = ext?.Particle ?? rst?.Particle;
                return getSchemaContentType(this, cc, particle);
            }

            throw createUnsupportedContentModelException();
        }
    }

    private XmlSchemaContentType getSchemaContentType(XsComplexType complexType, XsComplexContent complexContent, XsParticle particle)
    {
        if (((complexContent != null) && complexContent.IsMixed) || ((complexContent == null) && complexType.IsMixed))
        {
            return XmlSchemaContentType.Mixed;
        }
        if ((particle != null) && !particle.IsEmpty)
        {
            return XmlSchemaContentType.ElementOnly;
        }
        return XmlSchemaContentType.Empty;
    }


    /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
    public XsParticle ContentTypeParticle
    {
        get
        {
            if (ContentModel == null) return Particle ?? XsParticle.Empty;
            if (ContentModel is XsSimpleContent)
            {

            }
            if (ContentModel is XsComplexContent)
            {
                //if (ContentModel.Content is XsComplexContentExtension) return XmlSchemaDerivationMethod.Extension;
                //if (ContentModel.Content is XsComplexContentRestriction) return XmlSchemaDerivationMethod.Restriction;
            }
            throw createUnsupportedContentModelException();
        }
    }

    private InvalidOperationException createUnsupportedContentModelException()
    {
        return new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                                                           "Unsupported content model: {0}", ContentModel == null ? "null" : ContentModel.Element.ToString()));
    }

    private static readonly IEnumerable<XName> _particleElementNames = new[] { Xs.Sequence, Xs.Group, Xs.Choice, Xs.All };
    public XsParticle Particle
    {
        get { return GetExclusiveOrElement<XsParticle>(_particleElementNames); }
        set { SetExclusiveOrElement(_particleElementNames, value); }
    }

    private static readonly HashSet<XName> _validAttributes = new HashSet<XName> { XsA.Abstract, XsA.Block, XsA.Mixed };
    protected override bool IsValidAttribute(XName attributeName)
    {
        if (_validAttributes.Contains(attributeName)) return true;
        return base.IsValidAttribute(attributeName);
    }
}