using System.Linq;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema.Collections;

internal class XsSingleElementCollection<TOutput> : XsCollectionBase<TOutput>
    where TOutput : XsObject
{
    public XName SchemaElementName { get; private set; }

    internal XsSingleElementCollection(XsObject parent, XName schemaElementName)
        : base(parent, () => parent.Element.Elements(schemaElementName))
    {
        SchemaElementName = schemaElementName;
    }

    /// <summary>
    /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
    /// </summary>
    /// <returns>
    /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
    /// </returns>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
    public override int IndexOf(TOutput item)
    {
        return item.Element.ElementsBeforeSelf(SchemaElementName).Count();
    }
}