using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema.Collections
{
    internal class NamedMultiElementCollection<TOutput> : XsMultiElementCollection<TOutput>, INamedCollection<TOutput> 
        where TOutput : XsObject
    {

        #region Implementation of INamedCollection<XsObject>

        internal NamedMultiElementCollection(XsObject parent, Func<XElement, bool> filter) : base(parent, filter)
        {
        }

        internal NamedMultiElementCollection(XsObject parent, Func<IEnumerable<XElement>> valueSelector, Func<XElement, bool> filter)
            : base(parent, valueSelector, filter)
        {
        }

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
}