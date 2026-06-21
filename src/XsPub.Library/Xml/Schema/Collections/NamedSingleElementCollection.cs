using System.Linq;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema.Collections;

internal class NamedSingleElementCollection<TOutput> : XsSingleElementCollection<TOutput>, INamedCollection<TOutput>
    where TOutput : XsObject
{
    internal NamedSingleElementCollection(XsObject parent, XName schemaElementName) : base(parent, schemaElementName)
    {
    }

    #region Implementation of INamedCollection<XsObject>

    public TOutput this[string name]
    {
        get
        {
            var objectElement = ValueSelector().Where(element => XAttributeExtension.ValueOrDefault(element.Attribute(XsA.Name)) == name).SingleOrDefault();
            return objectElement.IfNotNull<XElement, TOutput>(CreateSchemaObject, null);
        }
    }

    #endregion
}