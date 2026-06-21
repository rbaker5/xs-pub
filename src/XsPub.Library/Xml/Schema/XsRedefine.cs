using System.Collections.Generic;
using System.Xml.Linq;
using XsPub.Library.Xml.Schema.Collections;

namespace XsPub.Library.Xml.Schema;

public class XsRedefine : XsExternal
{
    public XsRedefine(XElement element)
        : base(element)
    {
        resetInits();
    }

    public XsRedefine(XElement element, XsObject parent)
        : base(element, parent)
    {
        resetInits();
    }

    private void resetInits()
    {
        _attributeGroupCollectionSet = RedefineCollectionSet.Create(this, Xs.AttributeGroup, schema => schema.AttributeGroups);
        _groupCollectionSet = RedefineCollectionSet.Create(this, Xs.Group, schema => schema.Groups);
        _typeCollectionSet = RedefineCollectionSet.Create(this, _schemaTypeElementNames, schema => schema.SchemaTypes);
    }

    private RedefineCollectionSet<XsAttributeGroup> _attributeGroupCollectionSet;
    internal INamedCollection<XsAttributeGroup> LocalAttributeGroups { get { return _attributeGroupCollectionSet.Local; } }
    public IGlobalNamedCollection<XsAttributeGroup> AttributeGroups { get { return _attributeGroupCollectionSet.General; } }

    private RedefineCollectionSet<XsGroup> _groupCollectionSet;
    internal INamedCollection<XsGroup> LocalGroups { get { return _groupCollectionSet.Local; } }
    public IGlobalNamedCollection<XsGroup> Groups { get { return _groupCollectionSet.General; } }

    private static readonly IEnumerable<XName> _itemsElementNames =
        new[] { Xs.Group, Xs.Annotation, Xs.AttributeGroup, Xs.ComplexType, Xs.SimpleType};
    private INamedCollection<XsObject> _items;
    public INamedCollection<XsObject> Items
    {
        get
        {
            if (_items == null) _items = NamedCollection.Create(this, _itemsElementNames);
            return _items;
        }
    }

    private static readonly IEnumerable<XName> _schemaTypeElementNames =
        new[] { Xs.ComplexType, Xs.SimpleType };
    private RedefineCollectionSet<XsType> _typeCollectionSet;
    internal INamedCollection<XsType> LocalSchemaTypes { get { return _typeCollectionSet.Local; } }
    public IGlobalNamedCollection<XsType> SchemaTypes { get { return _typeCollectionSet.General; } }
}