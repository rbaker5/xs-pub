using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema.Collections;

internal class NamedCollection : XsCollection, INamedCollection<XsObject>
{
    public new static INamedCollection<XsObject> Create(XsObject parent)
    {
        ArgumentNullException.ThrowIfNull(parent);
        return new NamedCollection(parent, _defaultFilter);
    }

    public new static INamedCollection<XsObject> Create(XsObject parent, IEnumerable<XName> elementNames)
    {
        return Create<XsObject>(parent, elementNames);
    }

    public new static INamedCollection<T> Create<T>(XsObject parent, IEnumerable<XName> elementNames)
        where T : XsObject
    {
        return Create<T>(parent, element => elementNames.Contains(element.Name));
    }

    public new static INamedCollection<T> Create<T>(XsObject parent, Func<XElement, bool> filter)
        where T : XsObject
    {
        ArgumentNullException.ThrowIfNull(parent);
        if (typeof(T) == typeof(XsObject)) return new NamedCollection(parent, filter) as INamedCollection<T>;
        return new NamedMultiElementCollection<T>(parent, filter);
    }

    public new static INamedCollection<T> Create<T>(XsObject parent, XName schemaElementName)
        where T : XsObject
    {
        return new NamedSingleElementCollection<T>(parent, schemaElementName);
    }


    protected NamedCollection(XsObject parent, Func<XElement, bool> filter) : base(parent, filter)
    {
    }

    protected NamedCollection(XsObject parent, Func<IEnumerable<XElement>> valueSelector, Func<XElement, bool> filter) : base(parent, valueSelector, filter)
    {
    }

    #region Implementation of INamedCollection<XsObject>

    public XsObject this[string name]
    {
        get
        {
            var objectElement = ValueSelector().Where(element => element.Attribute(XsA.Name).ValueOrDefault() == name).SingleOrDefault();
            return objectElement.IfNotNull<XElement, XsObject>(CreateSchemaObject, null);
        }
    }

    #endregion
}