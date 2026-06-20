using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XsPub.Library.Xml.Schema.Collections
{
    internal class XsCollection : XsMultiElementCollection<XsObject>
    {
        protected static readonly Func<XElement, bool> _defaultFilter = (element => element.Name.Namespace == Namespaces.Xs);

        public static ICollection<XsObject> Create(XsObject parent)
        {
            ArgumentNullException.ThrowIfNull(parent);
            return new XsCollection(parent, _defaultFilter);
        }

        public static ICollection<XsObject> Create(XsObject parent, IEnumerable<XName> elementNames)
        {
            return Create<XsObject>(parent, elementNames);
        }

        public static ICollection<T> Create<T>(XsObject parent, IEnumerable<XName> elementNames)
            where T : XsObject
        {
            if (elementNames.Count() == 1) return Create<T>(parent, elementNames.Single());
            return Create<T>(parent, element => elementNames.Contains(element.Name));
        }

        public static ICollection<T> Create<T>(XsObject parent, Func<XElement, bool> filter)
            where T : XsObject
        {
            ArgumentNullException.ThrowIfNull(parent);
            if (typeof(T) == typeof(XsObject)) return new XsCollection(parent, filter) as ICollection<T>;
            return new XsMultiElementCollection<T>(parent, filter);
        }

        public static ICollection<T> Create<T>(XsObject parent, XName schemaElementName) 
            where T : XsObject
        {
            return new XsSingleElementCollection<T>(parent, schemaElementName);
        }

        /// <summary>
        /// Creates a wrapper of a set of schema objects, using a filter for all queries.
        /// </summary>
        /// <param name="parent">The schema object from which the child objects are read.
        /// </param>
        /// <param name="filter">
        /// A filter statement that will only allow the intended items to be iterated.
        /// </param>
        protected XsCollection(XsObject parent, Func<XElement, bool> filter)
            : base(parent, filter)
        {
            ArgumentNullException.ThrowIfNull(parent);
            ArgumentNullException.ThrowIfNull(filter);
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
        protected XsCollection(XsObject parent, Func<IEnumerable<XElement>> valueSelector, Func<XElement, bool> filter)
            : base(parent, valueSelector, filter)
        {
            ArgumentNullException.ThrowIfNull(parent);
            ArgumentNullException.ThrowIfNull(filter);
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public override IEnumerator<XsObject> GetEnumerator()
        {
            return ValueSelector().Select<XElement, XsObject>(CreateSchemaObject).GetEnumerator();
        }
    }
}