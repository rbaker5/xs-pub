using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema;

public class XsSchema : XsObject
{
    public XsSchema() : base(new XElement(Xs.Schema))
    {
        resetInits();
    }

    protected XsSchema(XElement element) : base(element)
    {
        if (element.Name != Xs.Schema) throw new ArgumentException($"Expected {nameof(Xs.Schema)}, got {element.Name}.", nameof(element));
        resetInits();
    }

    private void resetInits()
    {
        _includes =
            new Lazy<ICollection<XsExternal>>(() => XsCollection.Create<XsExternal>(this, _includesElementNames));
        _attributeGroupCollectionSet = SchemaCollectionSet.Create(this, Xs.AttributeGroup,
                                                                  schema => schema.AttributeGroups,
                                                                  redefine => redefine.AttributeGroups);
        _attributeCollectionSet = SchemaCollectionSet.Create(this, Xs.Attribute, schema => schema.Attributes, null);
        _elementCollectionSet = SchemaCollectionSet.Create(this, Xs.Element, schema => schema.Elements, null);
        _groupCollectionSet = SchemaCollectionSet.Create(this, Xs.Group, schema => schema.Groups, redefine => redefine.Groups);
        _notationCollectionSet = SchemaCollectionSet.Create(this, Xs.Notation, schema => schema.Notations, null);
        _typeCollectionSet = SchemaCollectionSet.Create(this, _schemaTypeElementNames, schema => schema.SchemaTypes, redefine => redefine.SchemaTypes);
    }

    #region Load/Save
    public static XsSchema Load(XElement schemaElement)
    {
//            if (schemaElement.Name != Xs.Schema) 
//                throw new ArgumentException("Must be <xs:schema> element.", "schemaElement");)
        return XsObjectFactory.Create<XsSchema>(schemaElement, element => new XsSchema(element));
    }

    public static XsSchema Load(string schemaPath, XmlResolver? resolver = null)
    {
        var schema = Load(XDocument.Load(schemaPath, LoadOptions.SetBaseUri | LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace).Root);
        schema.XmlResolver = resolver
            ?? new LocalOnlyXmlResolver(Path.GetDirectoryName(Path.GetFullPath(schemaPath))!);
        return schema;
    }

    public static XsSchema Load(XmlReader reader)
    {
        var document = XDocument.Load(reader, LoadOptions.SetBaseUri | LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace);
        return Load(document.Root);
    }

    public void Save(string schemaPath)
    {
        Element.Save(schemaPath, SaveOptions.DisableFormatting);
    }

    public void Save(XmlWriter writer)
    {
        Element.Save(writer);
    }
    #endregion

    private XmlResolver _xmlResolver;
    public XmlResolver XmlResolver
    {
        set { _xmlResolver = value; }
    }

    internal XmlResolver GetXmlResolver()
    {
        return _xmlResolver;
    }

    #region Simple Accessors
    public XmlSchemaForm AttributeFormDefault
    {
        get { return GetAttributeValueInternal(XsA.AttributeFormDefault, XmlSchemaForm.None); }
        set { SetAttributeValueInternal(XsA.AttributeFormDefault, value, XmlSchemaForm.None); }
    }

    public XmlSchemaForm ElementFormDefault
    {
        get { return GetAttributeValueInternal(XsA.ElementFormDefault, XmlSchemaForm.None); }
        set { SetAttributeValueInternal(XsA.ElementFormDefault, value, XmlSchemaForm.None); }
    }

    public XmlSchemaDerivationMethod BlockDefault
    {
        get { return GetAttributeValueInternal(XsA.BlockDefault, XmlSchemaDerivationMethod.None); }
        set { SetAttributeValueInternal(XsA.BlockDefault, value, XmlSchemaDerivationMethod.None); }
    }

    public XmlSchemaDerivationMethod FinalDefault
    {
        get { return GetAttributeValueInternal(XsA.FinalDefault, XmlSchemaDerivationMethod.None); }
        set { SetAttributeValueInternal(XsA.FinalDefault, value, XmlSchemaDerivationMethod.None); }
    }

    public string Id
    {
        get { return GetAttributeValueInternal(XsA.Id); }
        set { SetAttributeValueInternal(XsA.Id, value); }
    }

    public string TargetNamespace
    {
        get { return GetAttributeValueInternal(XsA.TargetNamespace); }
        set { SetAttributeValueInternal(XsA.TargetNamespace, value); }
    }

    public string Version
    {
        get { return GetAttributeValueInternal(XsA.Version); }
        set { SetAttributeValueInternal(XsA.Version, value); }
    }
    #endregion

    public IEnumerable<XAttribute> UnhandledAttributes { get { return GetUnhandledAttributes(); } }

    private SchemaCollectionSet<XsAttributeGroup> _attributeGroupCollectionSet;
    internal INamedCollection<XsAttributeGroup> LocalAttributeGroups { get { return _attributeGroupCollectionSet.Local; } }
    public IGlobalNamedCollection<XsAttributeGroup> GlobalAttributeGroups { get { return _attributeGroupCollectionSet.Global; } }
    public IGlobalNamedCollection<XsAttributeGroup> AttributeGroups { get { return _attributeGroupCollectionSet.General; } }

    private SchemaCollectionSet<XsAttribute> _attributeCollectionSet;
    internal INamedCollection<XsAttribute> LocalAttributes { get { return _attributeCollectionSet.Local; } }
    public IGlobalNamedCollection<XsAttribute> GlobalAttributes { get { return _attributeCollectionSet.Global; } }
    public IGlobalNamedCollection<XsAttribute> Attributes { get { return _attributeCollectionSet.General; } }

    private SchemaCollectionSet<XsElement> _elementCollectionSet;
    internal INamedCollection<XsElement> LocalElements { get { return _elementCollectionSet.Local; } }
    public IGlobalNamedCollection<XsElement> GlobalElements { get { return _elementCollectionSet.Global; } }
    public IGlobalNamedCollection<XsElement> Elements { get { return _elementCollectionSet.General; } }

    private SchemaCollectionSet<XsGroup> _groupCollectionSet;
    internal INamedCollection<XsGroup> LocalGroups { get { return _groupCollectionSet.Local; } }
    public IGlobalNamedCollection<XsGroup> GlobalGroups { get { return _groupCollectionSet.Global; } }
    public IGlobalNamedCollection<XsGroup> Groups { get { return _groupCollectionSet.General; } }

    private SchemaCollectionSet<XsNotation> _notationCollectionSet;
    internal INamedCollection<XsNotation> LocalNotations { get { return _notationCollectionSet.Local; } }
    public IGlobalNamedCollection<XsNotation> GlobalNotations { get { return _notationCollectionSet.Global; } }
    public IGlobalNamedCollection<XsNotation> Notations { get { return _notationCollectionSet.General; } }

    private static readonly IEnumerable<XName> _schemaTypeElementNames =
        new[] { Xs.ComplexType, Xs.SimpleType };
    private SchemaCollectionSet<XsType> _typeCollectionSet;
    internal INamedCollection<XsType> LocalSchemaTypes { get { return _typeCollectionSet.Local; } }
    public IGlobalNamedCollection<XsType> GlobalSchemaTypes { get { return _typeCollectionSet.Global; } }
    public IGlobalNamedCollection<XsType> SchemaTypes { get { return _typeCollectionSet.General; } }

    private Lazy<ICollection<XsExternal>> _includes;
    private static readonly IEnumerable<XName> _includesElementNames = new[] { Xs.Import, Xs.Include, Xs.Redefine};
    public ICollection<XsExternal> Includes { get { return _includes.Value; }}

    private static readonly IEnumerable<XName> _itemsElementNames =
        new[] { Xs.Notation, Xs.Group, Xs.Annotation, Xs.AttributeGroup, Xs.Attribute, Xs.ComplexType, Xs.SimpleType, Xs.Element };
    private ICollection<XsObject> _items;
    public ICollection<XsObject> Items
    {
        get
        {
            if (_items == null) _items = XsCollection.Create(this, _itemsElementNames);
            return _items;
        }
    }

    public XmlSchema CreateXmlSchema()
    {
        using (var reader = Element.CreateReader())
        {
            return XmlSchema.Read(reader, validationFailure);
        }
    }

    private void validationFailure(object sender, ValidationEventArgs e)
    {
        throw new NotImplementedException();
    }

    private static readonly HashSet<XName> _validAttributes = new HashSet<XName> { XsA.AttributeFormDefault, XsA.BlockDefault, XsA.ElementFormDefault, XsA.FinalDefault, XsA.Id, XsA.TargetNamespace, XsA.Version };
    protected override bool IsValidAttribute(XName attributeName)
    {
        if (_validAttributes.Contains(attributeName)) return true;
        return base.IsValidAttribute(attributeName);
    }

    public override string ToString()
    {
        return String.Format("{0} {{{1}}}", TargetNamespace, SourceUri);
    }
}
