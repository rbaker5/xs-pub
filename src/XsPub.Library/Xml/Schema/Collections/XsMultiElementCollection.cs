using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema.Collections;

internal class XsMultiElementCollection<T> : XsCollectionBase<T>
    where T : XsObject
{
    private readonly Func<XElement, bool> _filter;
    private static readonly Func<XElement, bool> _defaultFilter = (element => element.Name.Namespace == Namespaces.Xs);

    /// <summary>
    /// Creates a wrapper of a set of schema objects, using a filter for all queries.
    /// </summary>
    /// <param name="parent">The schema object from which the child objects are read.
    /// </param>
    /// <param name="filter">
    /// A filter statement that will only allow the intended items to be iterated.
    /// </param>
    internal XsMultiElementCollection(XsObject parent, Func<XElement, bool> filter)
        : base(parent, () => parent.Element.Elements().Where(filter))
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(filter);

        _filter = filter;
    }

    /// <summary>
    /// Creates a wrapper of a set of schema objects, using discreet filters and select functions.
    /// </summary>
    /// <param name="parent">The schema object from which the child objects are read.</param>
    /// <param name="valueSelector">
    /// By providing a value selector you may be able to improve performance.  This
    /// function must respect in order to produce the intended results.
    /// </param>
    /// <param name="filter">A filter, which should match <paramref name="valueSelector"/>.</param>
    internal XsMultiElementCollection(XsObject parent, Func<IEnumerable<XElement>> valueSelector, Func<XElement, bool> filter)
        : base(parent, valueSelector)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(filter);

        _filter = filter;
    }

    /// <summary>
    /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
    /// </summary>
    /// <returns>
    /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
    /// </returns>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
    public override int IndexOf(T item)
    {
        return item.Element.ElementsBeforeSelf().Where(_filter).Count();
    }
}