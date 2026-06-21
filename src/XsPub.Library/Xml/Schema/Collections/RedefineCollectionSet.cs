using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema.Collections;

internal class RedefineCollectionSet
{
    internal static RedefineCollectionSet<T> Create<T>(XsRedefine redefine, XName elementName, Func<XsSchema, IGlobalNamedCollection<T>> subValueSelector)
        where T : XsObject, IGlobalNamedObject
    {
        return create(redefine, subValueSelector, new(() => NamedCollection.Create<T>(redefine, elementName)));
    }

    internal static RedefineCollectionSet<T> Create<T>(XsRedefine redefine, IEnumerable<XName> elementNames, Func<XsSchema, IGlobalNamedCollection<T>> subValueSelector)
        where T : XsObject, IGlobalNamedObject
    {
        return create(redefine, subValueSelector, new(() => NamedCollection.Create<T>(redefine, elementNames)));
    }

    private static RedefineCollectionSet<T> create<T>(XsRedefine redefine, Func<XsSchema, IGlobalNamedCollection<T>> subValueSelector,
                                                      Lazy<INamedCollection<T>> local)
        where T : XsObject, IGlobalNamedObject
    {
        var set = new RedefineCollectionSet<T>();
        set.Init(local, new(() => RedefineGlobalCollection.Create(redefine, set.Local, subValueSelector)));
        return set;
    }
}

internal class RedefineCollectionSet<T>
    where T : XsObject, IGlobalNamedObject
{
    private Lazy<INamedCollection<T>> _local;
    private Lazy<IGlobalNamedCollection<T>> _general;

    internal INamedCollection<T> Local { get { return _local.Value; }}
    internal IGlobalNamedCollection<T> General { get { return _general.Value; } }

    internal void Init(Lazy<INamedCollection<T>> local, Lazy<IGlobalNamedCollection<T>> general)
    {
        _local = local;
        _general = general;
    }
}